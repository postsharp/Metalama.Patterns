using System;
using System.Collections.Generic;
using System.Text;

namespace Metalama.Patterns.Tests.Helpers
{
    public static class TestHelpers
    {
        public static TException RecordException<TException>( Action action )
            where TException : Exception
        {
            try
            {
                action();
            }
            catch ( TException exception )
            {
                return exception;
            }

            return null;
        }
    }
}
