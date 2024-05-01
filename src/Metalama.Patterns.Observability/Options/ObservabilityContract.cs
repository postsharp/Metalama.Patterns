// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Aspects;
using Metalama.Framework.Serialization;

namespace Metalama.Patterns.Observability.Options;

[CompileTime]
public sealed class ObservabilityContract : ICompileTimeSerializable
{
    private ObservabilityContract() { }

    public static ObservabilityContract ShallNotDependOnMutableState { get; } = new();
}