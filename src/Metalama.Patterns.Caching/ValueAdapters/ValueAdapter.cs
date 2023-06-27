// Copyright (c) SharpCrafters s.r.o. This file is not open source. It is released under a commercial
// source-available license. Please see the LICENSE.md file in the repository root for details.

using System.Threading;
using System.Threading.Tasks;

namespace PostSharp.Patterns.Caching.ValueAdapters
{
    /// <summary>
    /// An abstract implementation of <see cref="IValueAdapter{T}"/>.
    /// </summary>
    /// <typeparam name="T">Type of the exposed value, i.e. typically return type of the cached method.</typeparam>
    public abstract class ValueAdapter<T> : IValueAdapter<T>
    {
        /// <inheritdoc />
        public virtual bool IsAsyncSupported => false;


        /// <inheritdoc />
        object IValueAdapter.GetStoredValue( object value )
        {
            return this.GetStoredValue( (T) value );
        }

        /// <inheritdoc />
        Task<object> IValueAdapter.GetStoredValueAsync( object value, CancellationToken cancellationToken )
        {
            return this.GetStoredValueAsync( (T) value, cancellationToken);
        }

        /// <inheritdoc />
        public abstract T GetExposedValue( object storedValue );


        /// <inheritdoc />
        public abstract object GetStoredValue( T value );


        /// <inheritdoc />
        public virtual Task<object> GetStoredValueAsync( T value, CancellationToken cancellationToken)
        {
            return Task.FromResult( this.GetStoredValue( value ) );
        }

        /// <inheritdoc />
        object IValueAdapter.GetExposedValue( object storedValue )
        {
            return this.GetExposedValue( storedValue );
        }

    }
}
