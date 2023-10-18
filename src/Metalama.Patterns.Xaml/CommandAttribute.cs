// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Advising;
using Metalama.Framework.Aspects;
using Metalama.Framework.Code;
using Metalama.Framework.Code.SyntaxBuilders;
using Metalama.Framework.Eligibility;
using Metalama.Framework.Project;
using Metalama.Patterns.Xaml.Implementation;
using Metalama.Patterns.Xaml.Options;
using System.ComponentModel;
using System.Windows.Input;

// TODO: Skip [Observable] on [Command]-targetted auto properties. No functional impact, would just avoid unnecessary generated code.
// TODO: Remove reference to Metalama.Patterns.NotifyPropertyChanged once the rename has been merged.

[assembly: AspectOrder( "Metalama.Patterns.Xaml.CommandAttribute:*", "Metalama.Patterns.NotifyPropertyChanged.NotifyPropertyChangedAttribute:*" )]
[assembly: AspectOrder( "Metalama.Patterns.Xaml.CommandAttribute:*", "Metalama.Patterns.Observability.ObservableAttribute:*" )]

namespace Metalama.Patterns.Xaml;

[AttributeUsage( AttributeTargets.Property )]
public sealed class CommandAttribute : CommandOptionsAttribute, IAspect<IProperty>
{
    void IEligible<IProperty>.BuildEligibility( IEligibilityBuilder<IProperty> builder )
    {
        builder.MustNotBeStatic();
        builder.MustHaveAccessibility( Framework.Code.Accessibility.Public );
        builder.MustSatisfy( p => p.GetMethod != null, p => $"{p} must have a getter" );
        builder.MustSatisfy(
            p => p is { IsAutoPropertyOrField: true, Writeability: Writeability.ConstructorOnly } or { IsAutoPropertyOrField: not true, Writeability: Writeability.All or Writeability.InitOnly },
            p => $"{p} must be an auto-property with only a getter, or a non-auto property with both a getter and setter or init" );
        builder.Type().MustBe( typeof( ICommand ) );
        builder.MustSatisfy( p => p.InitializerExpression == null, p => $"{p} must not have an initializer expression" );
    }

    void IAspect<IProperty>.BuildAspect( IAspectBuilder<IProperty> builder )
    {
        var target = builder.Target;
        var declaringType = target.DeclaringType;
        var options = target.Enhancements().GetOptions<CommandOptions>();
        var propertyName = target.Name;

        var commandName = propertyName.EndsWith( "Command", StringComparison.Ordinal )
            ? propertyName.Substring( 0, propertyName.Length - 7 )
            : propertyName;

        if ( options.CanExecuteMethod != null && options.CanExecuteProperty != null )
        {
            builder.Diagnostics.Report( Diagnostics.ErrorCannotSpecifyBothCanExecuteMethodAndCanExecuteProperty );

            // Further diagnostics would be confusing and transformation is not possible.

            return;
        }

        // NB: Alternative names are implemented in PostSharp but not documented.

        var defaultCanExecuteName = $"CanExecute{commandName}";
        var defaultAltCanExecuteName = $"Can{commandName}";

        var executeMethodName = options.ExecuteMethod ?? $"Execute{commandName}";
        var altExecuteMethodName = options.ExecuteMethod != null ? null : commandName;

        var canExecuteMethodName = options.CanExecuteMethod ?? defaultCanExecuteName;
        var altCanExecuteMethodName = options.CanExecuteMethod != null ? null : defaultAltCanExecuteName;

        var canExecutePropertyName = options.CanExecuteProperty ?? defaultCanExecuteName;
        var altCanExecutePropertyName = options.CanExecuteProperty != null ? null : defaultAltCanExecuteName;

        if ( executeMethodName == canExecuteMethodName )
        {
            builder.Diagnostics.Report( Diagnostics.ErrorCommandExecuteAndCanExecuteCannotBeTheSame.WithArguments(
                (options.ExecuteMethod == null ? "default" : "configured", options.CanExecuteMethod == null ? "default" : "configured", executeMethodName) ) );

            // Further diagnostics would be confusing and transformation is not possible.

            return;
        }

        var candidateExecuteMethods = new List<IMethod>( 2 );
        var candidateCanExecuteMethods = new List<IMethod>( 2 );

        foreach ( var method in declaringType.Methods )
        {
            if ( (method.Name == executeMethodName || method.Name == altExecuteMethodName) && candidateExecuteMethods.Count < 2 )
            {
                candidateExecuteMethods.Add( method );
            }
            else if ( (method.Name == canExecuteMethodName || method.Name == altCanExecuteMethodName) && candidateCanExecuteMethods.Count < 2 )
            {
                candidateCanExecuteMethods.Add( method );
            }
        }

        var canTransform = true;

        IProperty? canExecuteProperty = null;

        // Don't look for the CanExecuteProperty if an explicit CanExecuteMethod is configured.

        if ( options.CanExecuteMethod == null )
        {
            canExecuteProperty = declaringType.Properties.FirstOrDefault( p => p.Name == canExecutePropertyName || p.Name == altCanExecutePropertyName );

            if ( canExecuteProperty != null )
            {
                if ( !IsValidCanExecuteProperty( canExecuteProperty ) )
                {
                    builder.Diagnostics.Report( Diagnostics.ErrorCommandCanExecutePropertyIsNotValid.WithArguments( target ), canExecuteProperty );
                    canTransform = false;
                }
            }
            else if ( options.CanExecuteProperty != null )
            {
                builder.Diagnostics.Report( Diagnostics.ErrorCommandConfiguredCanExecutePropertyNotFound.WithArguments( canExecutePropertyName ) );
                canTransform = false;
            }
        }

        IMethod? canExecuteMethod = null;

        // Don't look for the CanExecuteMethod if an explicit CanExecuteProperty is configured.

        if ( options.CanExecuteProperty == null )
        {
            switch ( candidateCanExecuteMethods.Count )
            {
                case 0:
                    if ( options.CanExecuteMethod != null )
                    {
                        builder.Diagnostics.Report( Diagnostics.ErrorCommandConfiguredCanExecuteMethodNotFound.WithArguments( canExecuteMethodName ) );
                        canTransform = false;
                    }
                    break;

                case 1:
                    if ( IsValidMethod( candidateCanExecuteMethods[0], SpecialType.Boolean ) )
                    {
                        canExecuteMethod = candidateCanExecuteMethods[0];
                    }
                    else
                    {
                        builder.Diagnostics.Report( Diagnostics.ErrorCommandCanExecuteMethodIsNotValid.WithArguments( target ), candidateCanExecuteMethods[0] );
                        canTransform = false;
                    }
                    break;

                case > 1:

                    builder.Diagnostics.Report( Diagnostics.ErrorCommandCanExecuteMethodIsAmbiguous.WithArguments(
                        (declaringType, altCanExecuteMethodName == null ? $"'{canExecuteMethodName}'" : $"'{canExecuteMethodName}' or '{altCanExecuteMethodName}'") ) );
                            
                    canTransform = false;
                    break;
            }
        }

        // If neither CanExecuteMethod and CanExecuteProperty are explicitly configured, we prefer any discovered method over
        // any discovered property, as per the PostSharp behaviour.

        if ( canExecuteMethod != null && canExecuteProperty != null )
        {
            canExecuteProperty = null;
        }

        IMethod? executeMethod = null;

        switch ( candidateExecuteMethods.Count )
        {
            case 0:
                builder.Diagnostics.Report( Diagnostics.ErrorCommandExecuteMethodNotFound.WithArguments(
                    altExecuteMethodName == null ? $"'{executeMethodName}'" : $"'{executeMethodName}' or '{altExecuteMethodName}'" ) );
                
                canTransform = false;
                break;

            case 1:
                if ( IsValidMethod( candidateExecuteMethods[0], SpecialType.Void ) )
                {
                    executeMethod = candidateExecuteMethods[0];
                }
                else
                {
                    builder.Diagnostics.Report( Diagnostics.ErrorCommandExecuteMethodIsNotValid.WithArguments( target ), candidateExecuteMethods[0] );
                    canTransform = false;
                }
                break;

            case > 1:

                builder.Diagnostics.Report( Diagnostics.ErrorCommandExecuteMethodIsAmbiguous.WithArguments(
                    (declaringType, altExecuteMethodName == null ? $"'{executeMethodName}'" : $"'{executeMethodName}' or '{altExecuteMethodName}'") ) );
                        
                canTransform = false;
                break;
        }

        var useInpcIntegration = false;

        if ( canTransform && canExecuteProperty != null && options.EnableINotifyPropertyChangedIntegration == true )
        {
            if ( declaringType.AllImplementedInterfaces.Contains( typeof( INotifyPropertyChanged ) ) )
            {
                if ( canExecuteProperty.Accessibility != Framework.Code.Accessibility.Public )
                {
                    builder.Diagnostics.Report( Diagnostics.WarniningCommandNotifiableCanExecutePropertyIsNotPublic.WithArguments( target ), canExecuteProperty );
                }

                useInpcIntegration = true;
            }
        }

        if ( !canTransform || !MetalamaExecutionContext.Current.ExecutionScenario.CapturesNonObservableTransformations )
        {
            if ( canTransform )
            {
                if ( executeMethod != null )
                {
                    builder.Diagnostics.Suppress( Suppressions.SuppressRemoveUnusedPrivateMembersIDE0051, executeMethod );
                }

                if ( canExecuteMethod != null )
                {
                    builder.Diagnostics.Suppress( Suppressions.SuppressRemoveUnusedPrivateMembersIDE0051, canExecuteMethod );
                }

                if ( canExecuteProperty != null )
                {
                    builder.Diagnostics.Suppress( Suppressions.SuppressRemoveUnusedPrivateMembersIDE0051, canExecuteProperty );
                }

                builder.Diagnostics.Suppress( Suppressions.SuppressNonNullableFieldMustContainNonNullValueWhenExitingConstructorCS8618, target );
            }

            return;
        }

        // When reaching this point:
        // - executeMethod is defined and valid.
        // - zero or one of canExecuteMethod and canExecuteProperty is defined and valid.

        builder.Advice.AddInitializer(
            declaringType,
            nameof( InitializeCommand ),
            InitializerKind.BeforeInstanceConstructor,
            args: new
            {
                commandProperty = target,
                executeMethod,
                canExecuteMethod,
                canExecuteProperty,
                useInpcIntegration
            } );
    }

    private static bool IsValidMethod( IMethod method, SpecialType requiredReturnType )
        => method.ReturnType.SpecialType == requiredReturnType
        && (method.Parameters is [] or [{ RefKind: RefKind.None or RefKind.In }])
        && method.TypeParameters.Count == 0;

    private static bool IsValidCanExecuteProperty( IProperty property )
        => property.Type.SpecialType == SpecialType.Boolean
           && property.GetMethod != null;

    [Template]
    private static void InitializeCommand(
        [CompileTime] IProperty commandProperty,
        [CompileTime] IMethod executeMethod,
        [CompileTime] IMethod? canExecuteMethod,
        [CompileTime] IProperty? canExecuteProperty,
        [CompileTime] bool useInpcIntegration )
    {
        IExpression? canExecuteExpression = null;

        if ( canExecuteMethod != null || canExecuteProperty != null )
        {
            bool CanExecute( object parameter )
            {
                if ( canExecuteMethod != null )
                {
                    if ( canExecuteMethod.Parameters.Count == 0 )
                    {
                        return canExecuteMethod.Invoke();
                    }
                    else
                    {
                        return canExecuteMethod.Invoke( meta.Cast( canExecuteMethod.Parameters[0].Type, parameter ) );
                    }
                }
                else
                {
                    return canExecuteProperty!.Value;
                }
            }

            canExecuteExpression = ExpressionFactory.Capture( (Func<object, bool>) CanExecute );
        }

        void Execute( object parameter )
        {
            if ( executeMethod.Parameters.Count == 0 )
            {
                executeMethod.Invoke();
            }
            else
            {
                executeMethod.Invoke( meta.Cast( executeMethod.Parameters[0].Type, parameter ) );
            }
        }

        if ( useInpcIntegration )
        {
            commandProperty.Value = new DelegateCommand( (Action<object>) Execute, canExecuteExpression!.Value, meta.This, canExecuteProperty!.Name );
        }
        else
        {
#pragma warning disable IDE0031 // Use null propagation
            commandProperty.Value = new DelegateCommand( (Action<object>) Execute, canExecuteExpression == null ? null : canExecuteExpression.Value );
#pragma warning restore IDE0031 // Use null propagation
        }
    }
}