// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using System.Runtime.Serialization;

namespace Metalama.Patterns.Caching.Backends.Redis;

[PSerializable]
[Serializable]
[DataContract]
internal class RedisCacheValue
{
    public RedisCacheValue( object value, TimeSpan slidingExpiration )
    {
        this.Value = value;
        this.SlidingExpiration = slidingExpiration;
    }

    public RedisCacheValue() { }

    [DataMember]
    public object Value { get; set; }

    [DataMember]
    public TimeSpan SlidingExpiration { get; set; }
}