﻿using System;

namespace PostSharp.Patterns.Caching.Implementation
{
    public sealed class CacheItemSetEventArgs : EventArgs
    {
        public string Key { get; }

        public CacheItem Item { get; }

        public string SourceId { get; }

        public CacheItemSetEventArgs( string key, CacheItem item, string sourceId )
        {
            if ( key == null )
                throw new ArgumentNullException( nameof( key ) );

            if ( item == null )
                throw new ArgumentNullException( nameof( item ) );

            this.Key = key;
            this.Item = item;
            this.SourceId = sourceId;
        }
    }

    public delegate void CacheItemSetEventHandler( object sender, CacheItemSetEventArgs args );
}
