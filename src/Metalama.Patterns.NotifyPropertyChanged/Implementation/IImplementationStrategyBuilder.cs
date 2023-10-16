// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Aspects;
using Metalama.Framework.Code;

namespace Metalama.Patterns.NotifyPropertyChanged.Implementation;

[CompileTime]
internal interface IImplementationStrategyBuilder
{
    /// <summary>
    /// Build the aspect. This method will be called at most once for any <see cref="IImplementationStrategyBuilder"/> returned by <see cref="IImplementationStrategyFactory.GetBuilder(IAspectBuilder{INamedType})"/>.
    /// </summary>
    void BuildAspect();
}