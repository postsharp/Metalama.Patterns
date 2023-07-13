// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using System;

namespace Metalama.Patterns.Caching.TestHelpers
{
    [Serializable]
    public class CachedValueClass
    {
        private int? _id;

        public int Id
        {
            get { return this._id.Value; }

            set
            {
                if ( this._id.HasValue )
                {
                    throw new InvalidOperationException( "The id can (and has to be) set exactly once." );
                }

                this._id = value;
            }
        }

        public CachedValueClass() { }

        public CachedValueClass( int id )
        {
            this._id = id;
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