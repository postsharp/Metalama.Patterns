// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

// TODO: [Porting] Discuss with gael: C:\src\PostSharp.Patterns\src\PostSharp\Tests\PostSharp.Patterns.Caching.Tests.BuildTests\CacheConfigurationAttribute
// Check PS git history, maybe messages explain intent. Also need to understand "build test" framework.

// TODO: [Porting] Temporary attributes defined during porting - resolve usages and remove.
// ReSharper disable All
#pragma warning disable

namespace Metalama.Patterns.Caching;

public sealed class PSerializableAttribute : Attribute { }

public sealed class PNonSerializedAttribute : Attribute { }

public sealed class ExplicitCrossPackageInternalAttribute : Attribute { }

public sealed class ImportSerializerAttribute : Attribute
{
    public ImportSerializerAttribute( Type objectType, Type serializerType ) { }
}

public sealed class ProtectedAttribute : Attribute { }