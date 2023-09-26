// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Aspects;
using Metalama.Framework.Code;
using Metalama.Framework.Options;
using Metalama.Framework.Project;

namespace Metalama.Patterns.NotifyPropertyChanged.Options;

[RunTimeOrCompileTime]
public sealed record NotifyPropertyChangedOptions : IHierarchicalOptions<ICompilation>, IHierarchicalOptions<INamespace>, IHierarchicalOptions<INamedType>
{
    private const int _defaultDiagnosticCommentVerbosity = 0;

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

    internal int DiagnosticCommentVerbosityOrDefault => this.DiagnosticCommentVerbosity ?? _defaultDiagnosticCommentVerbosity;

    // TODO: Add property to select the desired implementation once there is more than one.

    object IIncrementalObject.ApplyChanges( object changes, in ApplyChangesContext context )
    {
        var other = (NotifyPropertyChangedOptions) changes;

        return new NotifyPropertyChangedOptions
        {
            DiagnosticCommentVerbosity = other.DiagnosticCommentVerbosity ?? this.DiagnosticCommentVerbosity
        };
    }

    IHierarchicalOptions IHierarchicalOptions.GetDefaultOptions( IProject project )
    {
        // TODO: Report diagnostic if out of range if/when a diagnostic adder is available here.

        var diagnosticCommentVerbosity = _defaultDiagnosticCommentVerbosity;

        if ( project.TryGetProperty( "InpcDiagnosticCommentVerbosity", out var verbosityStr ) )
        {
            if ( int.TryParse( verbosityStr, out diagnosticCommentVerbosity ) )
            {
                diagnosticCommentVerbosity = Math.Max( 0, Math.Min( diagnosticCommentVerbosity, 3 ) );
            }
        }

        return new NotifyPropertyChangedOptions()
        {
            DiagnosticCommentVerbosity = diagnosticCommentVerbosity
        };
    }
}