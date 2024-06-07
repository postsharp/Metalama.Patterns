// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Aspects;
using Metalama.Framework.Code;
using Metalama.Framework.Options;
using Metalama.Framework.Project;
using Metalama.Framework.Validation;
using Metalama.Patterns.Immutability.Configuration;

namespace Metalama.Patterns.Immutability;

[Inheritable]
public class ImmutableAttribute : TypeAspect, IHierarchicalOptionsProvider
{
    private readonly ImmutabilityKind _kind;

    public ImmutableAttribute( ImmutabilityKind kind = ImmutabilityKind.Shallow )
    {
        this._kind = kind;
    }

    public IEnumerable<IHierarchicalOptions> GetOptions( in OptionsProviderContext context ) => new[] { new ImmutabilityOptions() { Kind = this._kind } };

    public override void BuildAspect( IAspectBuilder<INamedType> builder )
    {
        foreach ( var field in builder.Target.Fields )
        {
            if ( !field.IsImplicitlyDeclared )
            {
                if ( field is { Writeability: > Writeability.InitOnly } )
                {
                    builder.Diagnostics.Report( ImmutabilityDiagnostics.FieldMustBeReadOnly.WithArguments( field ) );
                }
                else
                {
                    AddValidator( field );
                }
            }
        }

        foreach ( var property in builder.Target.Properties )
        {
            if ( property.IsAutoPropertyOrField == true )
            {
                switch ( property.Writeability )
                {
                    case Writeability.All:
                        builder.Diagnostics.Report( ImmutabilityDiagnostics.PropertyMustHaveNoSetter.WithArguments( property ) );

                        break;

                    case Writeability.None:
                        // Read-only properties are ignored.
                        continue;

                    default:
                        AddValidator( property );

                        break;
                }
            }
        }

        void AddValidator( IFieldOrProperty field )
        {
            if ( this._kind == ImmutabilityKind.Deep && !MetalamaExecutionContext.Current.ExecutionScenario.IsDesignTime )
            {
                builder.Outbound.Select( c => field.ForCompilation( c.Compilation ) ).Validate( ValidateDeepImmutability );
            }
        }
    }

    private static void ValidateDeepImmutability( in DeclarationValidationContext context )
    {
        var field = (IFieldOrProperty) context.Declaration;

        if ( field.Type.GetImmutabilityKind() != ImmutabilityKind.Deep )
        {
            context.Diagnostics.Report( ImmutabilityDiagnostics.FieldOrPropertyMustBeOfDeeplyImmutableType.WithArguments( (field, field.DeclarationKind) ) );
        }
    }
}