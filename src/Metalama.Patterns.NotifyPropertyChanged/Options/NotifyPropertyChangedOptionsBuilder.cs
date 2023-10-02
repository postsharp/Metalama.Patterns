// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Aspects;
using Metalama.Patterns.NotifyPropertyChanged.Implementation;

namespace Metalama.Patterns.NotifyPropertyChanged.Options;

[CompileTime]
public class NotifyPropertyChangedOptionsBuilder
{
    private NotifyPropertyChangedOptions _options = new();

    /// <summary>
    /// Sets the <see cref="IImplementationStrategyFactory"/> used to provide <see cref="IImplementationStrategyBuilder"/> instances.
    /// </summary>
    public IImplementationStrategyFactory ImplementationStrategyFactory
    {
        set => this._options = this._options with { ImplementationStrategyFactory = value };
    }

    /// <summary>
    /// Sets a value indicating the verbosity of diagnostic comments inserted into genereated code. Must be a value
    /// between 0 and 3 (inclusive). 0 (default) inserts no comments, 3 is the most verbose.
    /// </summary>
    public int? DiagnosticCommentVerbosity
    {
        set => this._options = this._options with { DiagnosticCommentVerbosity = value };
    }

    public NotifyPropertyChangedOptions Build() => this._options;
}