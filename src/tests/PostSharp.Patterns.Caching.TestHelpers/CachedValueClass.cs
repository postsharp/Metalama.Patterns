// Copyright (c) SharpCrafters s.r.o. This file is not open source. It is released under a commercial
// source-available license. Please see the LICENSE.md file in the repository root for details.

using System;

namespace PostSharp.Patterns.Caching.TestHelpers
{
    [Serializable]
    public class CachedValueClass
    {
        private int? id;

        public int Id
        {
            get { return this.id.Value; }

            set
            {
                if ( this.id.HasValue )
                {
                    throw new InvalidOperationException( "The id can (and has to be) set exactly once." );
                }

                this.id = value;
            }
        }

        public CachedValueClass()
        {
        }

        public CachedValueClass( int id )
        {
            this.id = id;
        }

        public override int GetHashCode()
        {
            return this.Id.GetHashCode();
        }

        public override bool Equals( object obj )
        {
            CachedValueClass value = obj as CachedValueClass;
            return value != null && this.Equals( value );
        }

        public bool Equals( CachedValueClass other )
        {
            return other != null && this.Id.Equals( other.Id );
        }

        public override string ToString()
        {
            return $"Value #{this.Id}";
        }
    }
}