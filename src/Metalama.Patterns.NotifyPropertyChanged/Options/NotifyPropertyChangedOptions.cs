// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Aspects;
using Metalama.Framework.Code;
using Metalama.Framework.Options;
using Metalama.Patterns.NotifyPropertyChanged.Implementation;

namespace Metalama.Patterns.NotifyPropertyChanged.Options;

[RunTimeOrCompileTime]
public sealed record NotifyPropertyChangedOptions : IHierarchicalOptions<ICompilation>, IHierarchicalOptions<INamespace>, IHierarchicalOptions<INamedType>
{
    public IImplementationStrategyFactory? ImplementationStrategyFactory { get; set; }

    private int? _diagnosticCommentVerbosity;

    /// <summary>
    /// Gets a value indicating the verbosity of diagnostic comments inserted into genereated code. Must be a value
    /// between 0 and 3 (inclusive). 0 (default) inserts no comments, 3 is the most verbose.
    /// </summary>
    public int? DiagnosticCommentVerbosity 
    {
        get => this._diagnosticCommentVerbosity;
        init
        {
            if ( value < 0 || value > 3 )
            {
                throw new ArgumentOutOfRangeException( nameof( value ) );
            }

            this._diagnosticCommentVerbosity = value;
        }
    }
    
    object IIncrementalObject.ApplyChanges( object changes, in ApplyChangesContext context )
    {
        var other = (NotifyPropertyChangedOptions) changes;

        return new NotifyPropertyChangedOptions
        {
            ImplementationStrategyFactory = other.ImplementationStrategyFactory ?? this.ImplementationStrategyFactory,
            DiagnosticCommentVerbosity = other.DiagnosticCommentVerbosity ?? this.DiagnosticCommentVerbosity
        };
    }

    IHierarchicalOptions IHierarchicalOptions.GetDefaultOptions( OptionsInitializationContext context )
    {
        const string InpcDiagnosticCommentVerbosity = "InpcDiagnosticCommentVerbosity";

        var diagnosticCommentVerbosity = 0;
        
        if ( context.Project.TryGetProperty( InpcDiagnosticCommentVerbosity, out var verbosityStr ) )
        {
            if ( !int.TryParse( verbosityStr, out diagnosticCommentVerbosity ) || diagnosticCommentVerbosity < 0 || diagnosticCommentVerbosity > 3 )
            {
                context.Diagnostics.Report(
                    DiagnosticDescriptors.WarningInvalidProjectPropertyValueWillBeIgnored.WithArguments(
                        (InpcDiagnosticCommentVerbosity,
                        verbosityStr,
                        "be an integer between 0 and 3 inclusive.") ) );
            }
        }

        return new NotifyPropertyChangedOptions()
        {
            ImplementationStrategyFactory = Implementation.ClassicStrategy.ClassicImplementationStrategyFactory.Instance,
            DiagnosticCommentVerbosity = diagnosticCommentVerbosity
        };
    }
}