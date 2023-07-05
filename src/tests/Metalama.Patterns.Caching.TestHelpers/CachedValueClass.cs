// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using System;

namespace Metalama.Patterns.Caching.TestHelpers
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

        public CachedValueClass() { }

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
            var value = obj as CachedValueClass;

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