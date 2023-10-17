// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Aspects;
using Metalama.Framework.Code;
using Metalama.Framework.Project;
using Metalama.Patterns.Xaml.Options;
using System.Diagnostics;

namespace Metalama.Patterns.Xaml.Implementation;

[CompileTime]
internal sealed partial class CommandAspectBuilder
{
    private readonly IAspectBuilder<IProperty> _builder;
    private readonly Assets _assets;
    private readonly INamedType _declaringType;
    private readonly string _propertyName;
    private readonly CommandOptions _options;

    public CommandAspectBuilder( IAspectBuilder<IProperty> builder )
    {
        this._builder = builder;
        this._assets = builder.Target.Compilation.Cache.GetOrAdd( _ => new Assets() );
        this._declaringType = builder.Target.DeclaringType;
        this._propertyName = builder.Target.Name;
        this._options = builder.Target.Enhancements().GetOptions<CommandOptions>();
    }

    public void Build()
    {
        Debugger.Break();

        var target = this._builder.Target;
        var propertyName = target.Name;

        var commandName = propertyName.EndsWith( "Command", StringComparison.Ordinal )
            ? propertyName.Substring( 0, propertyName.Length - 7 )
            : propertyName;

        if ( this._options.CanExecuteMethod != null && this._options.CanExecuteProperty != null )
        {
            this._builder.Diagnostics.Report( Diagnostics.ErrorCannotSpecifyBothCanExecuteMethodAndCanExecuteProperty );
            
            // Further diagnostics would be confusing and transformation is not possible.
            
            return;
        }

        // NB: Alternative names are implemented in PostSharp but not documented.

        var defaultCanExecuteName = $"CanExecute{commandName}";
        var defaultAltCanExecuteName = $"Can{commandName}";

        var executeMethodName = this._options.ExecuteMethod ?? $"Execute{commandName}";
        var altExecuteMethodName = this._options.ExecuteMethod != null ? null : commandName;
        
        var canExecuteMethodName = this._options.CanExecuteMethod ?? defaultCanExecuteName;
        var altCanExecuteMethodName = this._options.CanExecuteMethod != null ? null : defaultAltCanExecuteName;
        
        var canExecutePropertyName = this._options.CanExecuteProperty ?? defaultCanExecuteName;
        var altCanExecutePropertyName = this._options.CanExecuteProperty != null ? null : defaultAltCanExecuteName;

        if ( executeMethodName == canExecuteMethodName )
        {
            this._builder.Diagnostics.Report( Diagnostics.ErrorCommandExecuteAndCanExecuteCannotBeTheSame.WithArguments(
                (this._options.ExecuteMethod == null ? "default" : "configured", this._options.CanExecuteMethod == null ? "default" : "configured", executeMethodName) ) );

            // Further diagnostics would be confusing and transformation is not possible.
            
            return;
        }

        var candidateExecuteMethods = new List<IMethod>( 2 );
        var candidateCanExecuteMethods = new List<IMethod>( 2 );

        foreach ( var method in this._declaringType.Methods )
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

        if ( this._options.CanExecuteMethod == null )
        {
            canExecuteProperty = this._declaringType.Properties.FirstOrDefault( p => p.Name == canExecutePropertyName || p.Name == altCanExecutePropertyName );

            if ( canExecuteProperty != null )
            {
                if ( !IsValidCanExecuteProperty( canExecuteProperty ) )
                {
                    this._builder.Diagnostics.Report( Diagnostics.ErrorCommandCanExecutePropertyIsNotValid.WithArguments( target ), canExecuteProperty );
                    canTransform = false;
                }
            }
            else if ( this._options.CanExecuteProperty != null )
            {
                this._builder.Diagnostics.Report( Diagnostics.ErrorCommandConfiguredCanExecutePropertyNotFound.WithArguments( canExecutePropertyName ) );
                canTransform = false;
            }
        }

        IMethod? canExecuteMethod = null;

        // Don't look for the CanExecuteMethod if an explicit CanExecuteProperty is configured.

        if ( this._options.CanExecuteProperty == null )
        {
            switch ( candidateCanExecuteMethods.Count )
            {
                case 0:
                    if ( this._options.CanExecuteMethod != null )
                    {
                        this._builder.Diagnostics.Report( Diagnostics.ErrorCommandConfiguredCanExecuteMethodNotFound.WithArguments( canExecuteMethodName ) );
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
                        this._builder.Diagnostics.Report( Diagnostics.ErrorCommandCanExecuteMethodIsNotValid.WithArguments( target ), candidateCanExecuteMethods[0] );
                        canTransform = false;
                    }
                    break;

                case > 1:

                    this._builder.Diagnostics.Report( Diagnostics.ErrorCommandCanExecuteMethodIsAmbiguous.WithArguments( (this._declaringType, canExecuteMethodName, altCanExecuteMethodName) ) );
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
                this._builder.Diagnostics.Report( Diagnostics.ErrorCommandExecuteMethodNotFound.WithArguments( (executeMethodName, altExecuteMethodName) ) );
                canTransform = false;
                break;

            case 1:
                if ( IsValidMethod( candidateExecuteMethods[0], SpecialType.Void ) )
                {
                    executeMethod = candidateExecuteMethods[0];
                }
                else
                {
                    this._builder.Diagnostics.Report( Diagnostics.ErrorCommandExecuteMethodIsNotValid.WithArguments( target ), candidateExecuteMethods[0] );
                    canTransform = false;
                }
                break;

            case > 1:

                this._builder.Diagnostics.Report( Diagnostics.ErrorCommandExecuteMethodIsAmbiguous.WithArguments( (this._declaringType, executeMethodName, altExecuteMethodName) ) );
                canTransform = false;
                break;
        }

        if ( !canTransform || !MetalamaExecutionContext.Current.ExecutionScenario.CapturesNonObservableTransformations )
        {
            return;
        }

        // When reaching this point:
        // - executeMethod is defined and valid.
        // - zero or one of canExecuteMethod and canExecuteProperty is defined and valid.

    }

    private static bool IsValidMethod( IMethod method, SpecialType requiredReturnType )
        => method.ReturnType.SpecialType == requiredReturnType
        && (method.Parameters is [] or [{ RefKind: RefKind.None or RefKind.In }]);

    private static bool IsValidCanExecuteProperty( IProperty property )
        => property.Type.SpecialType == SpecialType.Boolean;
}