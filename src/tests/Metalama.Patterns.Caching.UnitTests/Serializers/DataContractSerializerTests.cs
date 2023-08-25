// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

#if NETFRAMEWORK
using Metalama.Patterns.Caching.Serializers;
using System.Runtime.Serialization;
using Xunit;

namespace Metalama.Patterns.Caching.Tests.Serializers;

public sealed class DataContractSerializerTests : SerializerBaseTests
{
    public DataContractSerializerTests() : base( new DataContractCachingSerializer() ) { }
}

#endif