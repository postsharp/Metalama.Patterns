// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Metalama.Patterns.Caching.TestHelpers
{
    public partial class CachingClass<T>
        where T : CachedValueClass, new()
    {
        private int counter = 0;

        private bool methodCalled = false;

        public event EventHandler<T> MethodCalled;

        public bool Reset()
        {
            var result = this.methodCalled;
            this.methodCalled = false;

            return result;
        }

        private T CreateNextValue()
        {
            if ( this.methodCalled )
            {
                throw new InvalidOperationException(
                    "Cached method called twice unexpectedly. If this is the expected behavior, call reset before the second call of the method." );
            }

            this.methodCalled = true;

            var value = new T() { Id = this.counter++ };
            this.MethodCalled?.Invoke( this, value );

            return value;
        }

        private async Task<T> CreateNextValueAsync()
        {
            await Task.Delay( 1 );

            return this.CreateNextValue();
        }

        private T CreateNextValueAsDependency()
        {
            var value = this.CreateNextValue();
            CachingContext.Current.AddDependency( value );

            return value;
        }

        private async Task<T> CreateNextValueAsDependencyAsync()
        {
            var value = await this.CreateNextValueAsync();
            CachingContext.Current.AddDependency( value );

            return value;
        }

        public virtual T GetValue()
        {
            return this.CreateNextValue();
        }

        public virtual async Task<T> GetValueAsync()
        {
            return await this.CreateNextValueAsync();
        }

        public virtual T GetValueAsDependency()
        {
            return this.CreateNextValueAsDependency();
        }

        public virtual async Task<T> GetValueAsDependencyAsync()
        {
            return await this.CreateNextValueAsDependencyAsync();
        }

        public virtual IEnumerable<T> GetValues()
        {
            yield return this.CreateNextValue();
        }
    }

    public class CachingClass : CachingClass<CachedValueClass> { }
}