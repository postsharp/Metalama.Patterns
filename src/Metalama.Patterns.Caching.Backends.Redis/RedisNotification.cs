// Copyright (c) SharpCrafters s.r.o. This file is not open source. It is released under a commercial
// source-available license. Please see the LICENSE.md file in the repository root for details.

using StackExchange.Redis;

namespace PostSharp.Patterns.Caching.Backends.Redis
{
    internal struct RedisNotification
    {
        public RedisChannel Channel;
        public RedisValue Value;
    }
}
