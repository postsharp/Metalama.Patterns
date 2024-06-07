// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Aspects;
using Metalama.Framework.Fabrics;
using System.Linq;

namespace Metalama.Patterns.Immutability.AspectTests.AttributeAndWarnings.ClassificationTests;

[Immutable( ImmutabilityKind.Deep )]
internal class ClassMarkedDeeplyImmutable;

[Immutable( ImmutabilityKind.Shallow )]
internal class ClassMarkedShallowlyImmutable;

[Immutable( ImmutabilityKind.None )]
internal class ClassMarkedNotImmutable;

internal class ClassNotMarked;

internal readonly struct ReadOnlyStruct;

// <target>
public class C
{
    private class Fabric : TypeFabric
    {
        [Introduce]
        public void PrintImmutability()
        {
            foreach ( var type in meta.Target.Compilation.Types.OrderBy( t => t.Name ) )
            {
                meta.InsertComment( $"{type}: {type.GetImmutabilityKind()}" );
            }
        }
    }
}