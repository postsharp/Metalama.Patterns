// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Aspects;
using Metalama.Framework.Options;

namespace Metalama.Patterns.NotifyPropertyChanged.Options;

[RunTimeOrCompileTime]
[AttributeUsage( AttributeTargets.Class | AttributeTargets.Assembly )]
public class NotifyPropertyChangedOptionsAttribute : Attribute, IHierarchicalOptionsProvider<NotifyPropertyChangedOptions>
{
    private int? _diagnosticCommentVerbosity;

    /// <summary>
    /// Gets or sets a value indicating the verbosity of diagnostic comments inserted into genereated code. Must be a value
    /// between 0 and 3 (inclusive). 0 (default) inserts no comments, 3 is the most verbose.
    /// </summary>
    public int DiagnosticCommentVerbosity
    {
        get => this._diagnosticCommentVerbosity ?? 0;

        set
        {
            if ( value < 0 || value > 3 )
            {
                throw new ArgumentOutOfRangeException( nameof( value ) );
            }

            this._diagnosticCommentVerbosity = value;
        }
    }

    NotifyPropertyChangedOptions IHierarchicalOptionsProvider<NotifyPropertyChangedOptions>.GetOptions()
    {
        return new()
        {
            DiagnosticCommentVerbosity = this._diagnosticCommentVerbosity
        };
    }
}