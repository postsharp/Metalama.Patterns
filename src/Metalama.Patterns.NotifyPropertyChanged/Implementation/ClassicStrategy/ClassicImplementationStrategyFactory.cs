// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Aspects;
using Metalama.Framework.Code;

namespace Metalama.Patterns.NotifyPropertyChanged.Implementation.ClassicStrategy;

[CompileTime]
public sealed class ClassicImplementationStrategyFactory : IImplementationStrategyFactory
{
    private ClassicImplementationStrategyFactory() { }

    public static ClassicImplementationStrategyFactory Instance { get; } = new();

    public IImplementationStrategyBuilder GetBuilder( IAspectBuilder<INamedType> aspectBuilder )
    {
        return new ClassicImplementationStrategyBuilder( aspectBuilder );
    }
}