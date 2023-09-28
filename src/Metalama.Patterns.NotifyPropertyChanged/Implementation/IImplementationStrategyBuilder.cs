// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Aspects;

namespace Metalama.Patterns.NotifyPropertyChanged.Implementation;

[CompileTime]
internal interface IImplementationStrategyBuilder
{
    /// <summary>
    /// Build the aspect. This method must be called at most once for a given instance of <see cref="IImplementationStrategyBuilder"/>.
    /// </summary>
    public void BuildAspect();
}