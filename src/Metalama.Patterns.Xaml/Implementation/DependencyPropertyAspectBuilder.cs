// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Advising;
using Metalama.Framework.Aspects;
using Metalama.Framework.Code;
using Metalama.Framework.Project;
using Metalama.Patterns.Xaml.Options;
using System.Diagnostics;
using System.Windows;

namespace Metalama.Patterns.Xaml.Implementation;

[CompileTime]
internal sealed partial class DependencyPropertyAspectBuilder
{
    private readonly IAspectBuilder<IProperty> _builder;
    private readonly Assets _assets;
    private readonly IType _propertyType;
    private readonly INamedType _declaringType;
    private readonly string _propertyName;
    private readonly DependencyPropertyOptions _options;

    public DependencyPropertyAspectBuilder( IAspectBuilder<IProperty> builder )
    {
        this._builder = builder;
        this._assets = builder.Target.Compilation.Cache.GetOrAdd( _ => new Assets() );
        this._propertyType = builder.Target.Type;
        this._declaringType = builder.Target.DeclaringType;
        this._propertyName = builder.Target.Name;
        this._options = builder.Target.Enhancements().GetOptions<DependencyPropertyOptions>();
    }

    public void Build()
    {
        // NB: WPF convention requires a specific field name.

        var introduceFieldResult = this._builder.Advice.IntroduceField(
            this._declaringType,
            $"{this._propertyName}Property",
            typeof( DependencyProperty ),
            IntroductionScope.Static,
            OverrideStrategy.Fail,
            b =>
            {
                b.Accessibility = Framework.Code.Accessibility.Public;
                b.Writeability = Writeability.ConstructorOnly;
            } );

        // TODO: CapturesNonObservableTransformations handling: stopping here may lead to warnings in user code that handler methods are not used, because we don't generate calls to them.

        if ( introduceFieldResult.Outcome != AdviceOutcome.Default || !MetalamaExecutionContext.Current.ExecutionScenario.CapturesNonObservableTransformations )
        {
            return;
        }

        IField? initialValueField = null;

        if ( this._builder.Target.InitializerExpression != null && this._options.SetInitialValueFromInitializer == true )
        {
            var exprToAssign = this._builder.Target.InitializerExpression;

            if ( this._builder.Target.InitializerExpression is not ISourceExpression { AsTypedConstant: not null } )
            {
                // The initializer is not a simple constant, create a static readonly field to hold the value.

                var initialValueFieldName = this.GetAndReserveUnusedMemberName( $"_initialValueOf{this._propertyName}" );

                var result = this._builder.Advice.IntroduceField(
                    this._declaringType,
                    initialValueFieldName,
                    this._propertyType,
                    IntroductionScope.Static,
                    OverrideStrategy.Fail,
                    b =>
                    {
                        b.InitializerExpression = this._builder.Target.InitializerExpression;
                        b.Writeability = Writeability.ConstructorOnly;
                    } );

                if ( result.Outcome == AdviceOutcome.Default )
                {
                    initialValueField = result.Declaration;

                    exprToAssign = (IExpression) initialValueField.Value!;
                }
            }

            this._builder.Advice.WithTemplateProvider( Templates.Provider ).AddInitializer(
                    this._declaringType,
                    nameof( Templates.Assign ),
                    InitializerKind.BeforeInstanceConstructor,
                    args: new
                    {
                        lhs = (IExpression) this._builder.Target.Value!,
                        rhs = exprToAssign
                    } );
        }

        // TODO: Cache methodsByName?
        var methodsByName = this._declaringType.Methods.ToLookup( m => m.Name );

        var onChangingHandlerName = $"On{this._propertyName}Changing";
        var onChangedHandlerName = $"On{this._propertyName}Changed";

        var (onChangingHandlerMethod, onChangingHandlerParametersKind) = this.GetHandlerMethod( methodsByName, onChangingHandlerName, allowOldValue: false );
        var (onChangedHandlerMethod, onChangedHandlerParametersKind) = this.GetHandlerMethod( methodsByName, onChangedHandlerName, allowOldValue: true );

        this._builder.Advice.WithTemplateProvider( Templates.Provider ).AddInitializer(
            this._declaringType,
            nameof( Templates.InitializeDependencyProperty ),
            InitializerKind.BeforeTypeConstructor,
            args: new
            {
                dependencyPropertyField = introduceFieldResult.Declaration,
                registerAsReadOnly = this._options.IsReadOnly == true,
                propertyName = this._propertyName,
                propertyType = this._propertyType,
                declaringType = this._declaringType,
                defaultValueExpr = initialValueField == null ? this._builder.Target.InitializerExpression : (IExpression?) initialValueField.Value,
                onChangingHandlerMethod,
                onChangingHandlerParametersKind,
                onChangedHandlerMethod,
                onChangedHandlerParametersKind
            } );

        this._builder.Advice.WithTemplateProvider( Templates.Provider ).OverrideAccessors(
            this._builder.Target,
            new GetterTemplateSelector( nameof( Templates.OverrideGetter ) ),
            args: new
            {
                propertyType = this._propertyType,
                dependencyPropertyField = introduceFieldResult.Declaration
            } );

        if ( this._builder.Target.Writeability != Writeability.None )
        {
            this._builder.Advice.WithTemplateProvider( Templates.Provider ).OverrideAccessors(
                this._builder.Target,
                setTemplate: nameof( Templates.OverrideSetter ),
                args: new
                {
                    dependencyPropertyField = introduceFieldResult.Declaration
                } );
        }
    }

    private (IMethod? ChangeHanlderMethod, ChangeHandlerParametersKind ParametersKind) GetHandlerMethod(
        ILookup<string, IMethod> methodsByName,
        string methodName,
        bool allowOldValue )
    {
        IMethod? method = null;
        var parametersKind = ChangeHandlerParametersKind.Invalid;

        var onChangingGroup = methodsByName[methodName];

        switch ( onChangingGroup.Count() )
        {
            case 0:
                break;

            case 1:
                method = onChangingGroup.First();
                break;

            default:
                // TODO: Ambiguous method diagnostic
                throw new NotSupportedException( $"Ambiguous handler method for {methodName}" );
        }

        if ( method != null )
        {
            parametersKind = GetChangeHandlerParametersKind( method, this._declaringType, this._propertyType, allowOldValue, this._assets );

            if ( parametersKind == ChangeHandlerParametersKind.Invalid )
            {
                Debugger.Break();
                // TODO: Invalid method signature diagnostic
                throw new NotSupportedException( $"Invalid handler method signature for {methodName}" );
            }
        }

        return (method, parametersKind);
    }

    private static ChangeHandlerParametersKind GetChangeHandlerParametersKind(
        IMethod method,
        INamedType declaringType,
        IType propertyType,
        bool allowOldValue,
        Assets assets )
    {
        var p = method.Parameters;

        switch ( p.Count )
        {
            case 0:
                return method.IsStatic ? ChangeHandlerParametersKind.StaticNone : ChangeHandlerParametersKind.None;

            case 1:

                if ( p[0].RefKind is RefKind.None or RefKind.In )
                {
                    if ( p[0].Type.Equals( assets.DependencyProperty ) )
                    {
                        return method.IsStatic ? ChangeHandlerParametersKind.StaticDependencyProperty : ChangeHandlerParametersKind.DependencyProperty;
                    }
                    else if ( method.IsStatic
                             && (p[0].Type.SpecialType == SpecialType.Object
                                  || p[0].Type.Equals( declaringType )
                                  || p[0].Type.Equals( assets.DependencyObject )) )
                    {
                        return ChangeHandlerParametersKind.StaticInstance;
                    }
                    else if ( !method.IsStatic
                              && (p[0].Type.SpecialType == SpecialType.Object
                                   || propertyType.Is( p[0].Type )
                                   || (p[0].Type.TypeKind == TypeKind.TypeParameter && method.TypeParameters.Count == 1)) )
                    {
                        return ChangeHandlerParametersKind.Value;
                    }
                }
                break;

            case 2:

                if ( p[0].RefKind is RefKind.None or RefKind.In && p[1].RefKind is RefKind.None or RefKind.In )
                {
                    if ( method.IsStatic
                         && p[0].Type.Equals( assets.DependencyProperty )
                         && (p[1].Type.SpecialType == SpecialType.Object
                              || p[1].Type.Equals( declaringType )
                              || p[1].Type.Equals( assets.DependencyObject )) )
                    {
                        return ChangeHandlerParametersKind.StaticDependencyPropertyAndInstance;
                    }
                    else if ( allowOldValue
                              && !method.IsStatic
                              && (p[0].Type.SpecialType == SpecialType.Object
                                   || propertyType.Is( p[0].Type )
                                   || (p[0].Type.TypeKind == TypeKind.TypeParameter && method.TypeParameters.Count == 1))
                              && (p[1].Type.SpecialType == SpecialType.Object
                                   || propertyType.Is( p[1].Type )
                                   || (p[1].Type.TypeKind == TypeKind.TypeParameter && method.TypeParameters.Count == 1)) )
                    {
                        return ChangeHandlerParametersKind.OldValueAndNewValue;
                    }
                }
                break;
        }

        return ChangeHandlerParametersKind.Invalid;
    }

    private HashSet<string>? _existingMemberNames;

    /// <summary>
    /// Gets an unused member name for the declaring type of the target property by adding an numeric suffix until an unused name is found.
    /// </summary>
    /// <param name="desiredName"></param>
    /// <returns></returns>
    private string GetAndReserveUnusedMemberName( string desiredName )
    {
        this._existingMemberNames ??= new HashSet<string>(
            ((IEnumerable<INamedDeclaration>) this._builder.Target.DeclaringType.AllMembers()).Concat( this._builder.Target.DeclaringType.NestedTypes ).Select( m => m.Name ) );

        if ( this._existingMemberNames.Add( desiredName ) )
        {
            return desiredName;
        }
        else
        {
            // ReSharper disable once BadSemicolonSpaces
            for ( var i = 1; /* Intentionally empty */; i++ )
            {
                var adjustedName = $"{desiredName}_{i}";

                if ( this._existingMemberNames.Add( adjustedName ) )
                {
                    return adjustedName;
                }
            }
        }
    }
}