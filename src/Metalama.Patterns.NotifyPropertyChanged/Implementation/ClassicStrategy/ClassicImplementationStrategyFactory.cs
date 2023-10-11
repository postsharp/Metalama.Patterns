// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Aspects;
using Metalama.Framework.Code;
using Metalama.Framework.Project;

namespace Metalama.Patterns.NotifyPropertyChanged.Implementation.ClassicStrategy;

[CompileTime]
internal sealed class ClassicImplementationStrategyFactory : IImplementationStrategyFactory
{
    private ClassicImplementationStrategyFactory() { }

    public static ClassicImplementationStrategyFactory Instance { get; } = new();

    public IImplementationStrategyBuilder GetBuilder( IAspectBuilder<INamedType> aspectBuilder )
    {
        var executionScenario = MetalamaExecutionContext.Current.ExecutionScenario;

        return executionScenario.CapturesNonObservableTransformations
            ? new ClassicImplementationStrategyBuilder( aspectBuilder )
            : new ClassicDesignTimeImplementationStrategyBuilder( aspectBuilder );
    }
}