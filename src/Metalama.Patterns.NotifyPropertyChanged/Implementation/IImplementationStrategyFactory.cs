// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Aspects;
using Metalama.Framework.Code;
using Metalama.Framework.Project;

namespace Metalama.Patterns.NotifyPropertyChanged.Implementation;

[CompileTime]
public interface IImplementationStrategyFactory
{
    /// <summary>
    /// Gets an <see cref="IImplementationStrategyBuilder"/> instance initialized with the given <see cref="IAspectBuilder{TAspectTarget}"/>.
    /// </summary>
    /// <remarks>
    /// Notably, implementations must take account of <see cref="IExecutionContext.ExecutionScenario"/>.
    /// </remarks>
    /// <param name="aspectBuilder"></param>
    /// <returns></returns>
    IImplementationStrategyBuilder GetBuilder( IAspectBuilder<INamedType> aspectBuilder );
}