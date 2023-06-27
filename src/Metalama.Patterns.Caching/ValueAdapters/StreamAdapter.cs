// Copyright (c) SharpCrafters s.r.o. This file is not open source. It is released under a commercial
// source-available license. Please see the LICENSE.md file in the repository root for details.

using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace PostSharp.Patterns.Caching.ValueAdapters
{
    internal sealed class StreamAdapter : ValueAdapter<Stream>
    {
        public override bool IsAsyncSupported => true;

        public override object GetStoredValue( Stream value )
        {
            if ( value == null )
            {
                return null;
            }

            byte[] buffer = new byte[8192];

            using ( MemoryStream memoryStream = new MemoryStream() )
            {

                int bytes;
                while ( (bytes = value.Read( buffer, 0, buffer.Length )) > 0 )
                {
                    memoryStream.Write( buffer, 0, bytes );
                }

                return memoryStream.ToArray();
            }
        }

        public override async Task<object> GetStoredValueAsync( Stream value, CancellationToken cancellationToken )
        {
            if (value == null)
            {
                return null;
            }

            byte[] buffer = new byte[8192];

            using ( MemoryStream memoryStream = new MemoryStream() )
            {
                int bytes;
#pragma warning disable CA1835 // Use Memory<T> instead of T[]
                while ( (bytes = await value.ReadAsync( buffer, 0, buffer.Length, cancellationToken )) > 0 )
#pragma warning disable CA1835 // Use Memory<T> instead of T[]
                {
                    memoryStream.Write(buffer, 0, bytes);
                }

                return memoryStream.ToArray();
            }
        }

        public override Stream GetExposedValue(object storedValue)
        {
            if (storedValue == null)
            {
                return null;
            }

            byte[] buffer = (byte[])storedValue;

            return new MemoryStream(buffer, false);
        }

       
    }
}
