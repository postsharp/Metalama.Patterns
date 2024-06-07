// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using System.Collections.Immutable;
using System.Runtime.CompilerServices;
#if TEST_OPTIONS
// @RemoveOutputCode
#endif

namespace Metalama.Patterns.Immutability.AspectTestsDeepAttributeAndWarnings;

[Immutable]
public class ShallowlyImmutableClass;

[Immutable( ImmutabilityKind.Deep )]
public class DeeplyImmutableClass
{
    // The following definitions should have a warning.
    public readonly ShallowlyImmutableClass ClassMarkedShallowlyImmutable;

    public readonly StrongBox<int> UnmarkedClass;

    public readonly ImmutableArray<ShallowlyImmutableClass> ImmutableArrayOfShallowImmutable;

    // The following definitions should NOT have a warning.
    public readonly string ReadOnlyStringField;

    public readonly ImmutableArray<string> ImmutableArray;

    public DeeplyImmutableClass ClassMarkedDeeplyImmutable { get; init; }
}