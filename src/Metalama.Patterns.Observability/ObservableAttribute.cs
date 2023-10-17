// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Aspects;
using Metalama.Framework.Code;
using Metalama.Framework.Eligibility;
using Metalama.Framework.Options;
using Metalama.Patterns.Observability.Implementation;
using Metalama.Patterns.Observability.Options;

namespace Metalama.Patterns.Observability;

[AttributeUsage( AttributeTargets.Class )]
[Inheritable]
public sealed class ObservableAttribute : Attribute, IAspect<INamedType>, IHierarchicalOptionsProvider
{
    private int? _diagnosticCommentVerbosity;

    /// <summary>
    /// Gets or sets a value indicating the verbosity of diagnostic comments inserted into generated code. Must be a value
    /// between 0 and 3 (inclusive). 0 (default) inserts no comments, 3 is the most verbose.
    /// </summary>
    public int DiagnosticCommentVerbosity
    {
        get => this._diagnosticCommentVerbosity ?? 0;

        set
        {
            if ( value is < 0 or > 3 )
            {
                throw new ArgumentOutOfRangeException( nameof(value) );
            }

            this._diagnosticCommentVerbosity = value;
        }
    }
    
    IEnumerable<IHierarchicalOptions> IHierarchicalOptionsProvider.GetOptions( in OptionsProviderContext context )
    {
        IImplementationStrategyFactory? factory = null;

        return new[]
        {
            new ObservabilityOptions() { ImplementationStrategyFactory = factory, DiagnosticCommentVerbosity = this._diagnosticCommentVerbosity }
        };
    }
    
    void IEligible<INamedType>.BuildEligibility( IEligibilityBuilder<INamedType> builder )
    {
        builder.MustNotBeStatic();
    }

    void IAspect<INamedType>.BuildAspect( IAspectBuilder<INamedType> builder )
    {
        var options = builder.Target.Enhancements().GetOptions<ObservabilityOptions>();
        var strategyBuilder = options.ImplementationStrategyFactory!.GetBuilder( builder );

        strategyBuilder.BuildAspect();
    }
}