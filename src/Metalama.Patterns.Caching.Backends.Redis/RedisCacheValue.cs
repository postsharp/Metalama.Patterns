// Copyright (c) SharpCrafters s.r.o. This file is not open source. It is released under a commercial
// source-available license. Please see the LICENSE.md file in the repository root for details.

using System;
using PostSharp.Serialization;
using System.Runtime.Serialization;

namespace PostSharp.Patterns.Caching.Backends.Redis
{
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

        public RedisCacheValue()
        {
        }

        [DataMember]
        public object Value { get; set; }

        [DataMember]
        public TimeSpan SlidingExpiration { get; set; }
    }
}
