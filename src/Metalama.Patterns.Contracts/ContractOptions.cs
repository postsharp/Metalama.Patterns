// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Project;

namespace Metalama.Patterns.Contracts;

public sealed class ContractOptions : ProjectExtension
{
    public ContractThrowTemplates ThrowTemplates { get; set; } = new();
}