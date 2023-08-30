// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Aspects;
using Metalama.Framework.Code;
using Metalama.Framework.Diagnostics;

namespace Metalama.Patterns.Contracts;

[CompileTime]
internal static class ContractDiagnostics
{
    public static DiagnosticDefinition<(IDeclaration Declaration, string TargetBasicType, string AspectType)> RangeCannotBeApplied
        = new(
            "LAMA5001",
            Severity.Error,
            "The [{2}] contract cannot be applied to '{0}' because the value range cannot be satisfied by the type {1}.",
            $"The Range contract cannot be applied because the range cannot be satisfied by the type of the target declaration.",
            "Metalama.Patterns.Contracts" );
}