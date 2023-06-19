// Copyright (c) SharpCrafters s.r.o. This file is not open source. It is released under a commercial
// source-available license. Please see the LICENSE.md file in the repository root for details.

#if APP_DOMAIN

using System;
using System.Security.Policy;
using PostSharp.Patterns.Formatters;

namespace PostSharp.Patterns.Common.Tests.Formatters
{
    // TODO: Refactor the tests so that running in separate app domains is not needed anymore.
    public abstract class AppDomainTestsBaseClass<T>
        where T : AppDomainTestsBaseClass<T>, new()
    {
        /// <summary>
        /// Executes test in a separate AppDomain, to make sure all static state is reset between tests.
        /// </summary>
        protected static void ExecuteTest( Action<T> testMethod )
        {
            AppDomain domain = AppDomain.CreateDomain(
                "Test",
                new Evidence( AppDomain.CurrentDomain.Evidence ),
                new AppDomainSetup
                {
                    ApplicationBase = AppDomain.CurrentDomain.SetupInformation.ApplicationBase,
                } );

            // can't use compiler-generated closure here, because that wouldn't be [Serializable]
            domain.DoCallBack( new Closure( testMethod ).Execute );
        }

        [Serializable]
        private class Closure
        {
            private readonly Action<T> callback;

            public Closure( Action<T> callback )
            {
                this.callback = callback;
            }

            public void Execute()
            {
                FormatterRepository<TestRole>.Reset();

                T testClass = new T();
                this.callback( testClass );
            }
        }
    }
}

#endif