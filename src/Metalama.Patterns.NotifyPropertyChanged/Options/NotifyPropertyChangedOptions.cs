// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using JetBrains.Annotations;
using Metalama.Framework.Aspects;
using Metalama.Framework.Code;
using Metalama.Framework.Options;
using Metalama.Patterns.NotifyPropertyChanged.Implementation;
using Metalama.Patterns.NotifyPropertyChanged.Implementation.ClassicStrategy;

namespace Metalama.Patterns.NotifyPropertyChanged.Options;

[PublicAPI]
[RunTimeOrCompileTime]
public sealed record NotifyPropertyChangedOptions : IHierarchicalOptions<ICompilation>, IHierarchicalOptions<INamespace>, IHierarchicalOptions<INamedType>
{
    /// <summary>
    /// Gets the <see cref="IImplementationStrategyFactory"/> used to provide <see cref="IImplementationStrategyBuilder"/> instances.
    /// </summary>
    public IImplementationStrategyFactory? ImplementationStrategyFactory { get; init; }

    private int? _diagnosticCommentVerbosity;

    /// <summary>
    /// Gets a value indicating the verbosity of diagnostic comments inserted into generated code. Must be a value
    /// between 0 and 3 (inclusive). 0 (default) inserts no comments, 3 is the most verbose.
    /// </summary>
    public int? DiagnosticCommentVerbosity
    {
        get => this._diagnosticCommentVerbosity;
        init
        {
            if ( value is < 0 or > 3 )
            {
                throw new ArgumentOutOfRangeException( nameof(value) );
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
        const string inpcDiagnosticCommentVerbosity = "InpcDiagnosticCommentVerbosity";

        var diagnosticCommentVerbosity = 0;

        if ( context.Project.TryGetProperty( inpcDiagnosticCommentVerbosity, out var verbosityStr ) )
        {
            if ( !int.TryParse( verbosityStr, out diagnosticCommentVerbosity ) || diagnosticCommentVerbosity < 0 || diagnosticCommentVerbosity > 3 )
            {
                context.Diagnostics.Report(
                    DiagnosticDescriptors.WarningInvalidProjectPropertyValueWillBeIgnored.WithArguments(
                        (inpcDiagnosticCommentVerbosity,
                         verbosityStr,
                         "be an integer between 0 and 3 inclusive.") ) );
            }
        }

        return new NotifyPropertyChangedOptions()
        {
            ImplementationStrategyFactory = ClassicImplementationStrategyFactory.Instance, DiagnosticCommentVerbosity = diagnosticCommentVerbosity
        };
    }
}