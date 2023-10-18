// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Aspects;

namespace Metalama.Patterns.Observability.Implementation;

[CompileTime]
internal enum InpcInstrumentationKind
{
    None,
    Implicit,
    Explicit,

    /// <summary>
    /// Returned at design time for types other than the current type and its ancestors.
    /// </summary>
    Unknown
}