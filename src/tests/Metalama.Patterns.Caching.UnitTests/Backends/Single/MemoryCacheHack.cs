// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using System.Reflection;
using System.Runtime;
using System.Runtime.Caching;

namespace Metalama.Patterns.Caching.Tests.Backends.Single
{
    internal static class MemoryCacheHack
    {
        private static bool _hackMade;

        private static Version? GetNetCoreVersion()
        {
            // https://github.com/dotnet/BenchmarkDotNet/issues/448
            var assembly = typeof(GCSettings).GetTypeInfo().Assembly;

            // TODO: This will be needed to be solved for NET6.0+.
#pragma warning disable SYSLIB0012 // Type or member is obsolete
#pragma warning disable CS8602
            var assemblyPath = assembly.CodeBase.Split( new[] { '/', '\\' }, StringSplitOptions.RemoveEmptyEntries );
#pragma warning restore CS8602
#pragma warning restore SYSLIB0012 // Type or member is obsolete
            var netCoreAppIndex = Array.IndexOf( assemblyPath, "Microsoft.NETCore.App" );

            if ( netCoreAppIndex > 0 && netCoreAppIndex < assemblyPath.Length - 2 )
            {
                Version.TryParse( assemblyPath[netCoreAppIndex + 1].Split( '-' )[0], out var version );

                return version;
            }

            return null;
        }

        /// <summary>
        /// Reduces the waiting period between the times Microsoft's <see cref="MemoryCache"/> checks for whether items expired from the cache.
        /// Normally, the cache checks once per 20 seconds. After this, it will check once per 100 milliseconds. See https://stackoverflow.com/a/12645397/1580088
        /// for details.
        /// </summary>
        public static void MakeExpirationChecksMoreFrequently()
        {
            if ( _hackMade )
            {
                return;
            }

            if ( GetNetCoreVersion() >= new Version( 3, 0 ) )
            {
                // The hack does not work in .NET Core 3.0 and above:
                // System.FieldAccessException: Cannot set init-only static field '_tsPerBucket' after type 'System.Runtime.Caching.CacheExpires' is initialized.
                _hackMade = true;

                return;
            }

            // https://stackoverflow.com/a/12645397/1580088

            const string assembly = "System.Runtime.Caching, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a";
            var type = Type.GetType( "System.Runtime.Caching.CacheExpires, " + assembly, true, true )!;
            var field = type.GetField( "_tsPerBucket", BindingFlags.Static | BindingFlags.NonPublic )!;
            field.SetValue( null, TimeSpan.FromSeconds( 0.1 ) );

            type = typeof(MemoryCache);
            field = type.GetField( "s_defaultCache", BindingFlags.Static | BindingFlags.NonPublic )!;
            field.SetValue( null, null );

            _hackMade = true;
        }
    }
}