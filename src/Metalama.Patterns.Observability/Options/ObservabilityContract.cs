// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Aspects;
using Metalama.Framework.Serialization;

namespace Metalama.Patterns.Observability.Options;

[CompileTime]
public sealed class ObservabilityContract : ICompileTimeSerializable
{
    private ObservabilityContract() { }

    /// <summary>
    /// Gets an <see cref="ObservabilityContract"/> that guarantees that the member (1) does not depend on any non-constant
    /// inputs or factors (i.e. will always return the same output given the same input) and (2) does not return non-constant outputs
    /// (i.e. even the properties of the outputs are constant).
    /// </summary>
    public static ObservabilityContract Constant { get; } = new();
}