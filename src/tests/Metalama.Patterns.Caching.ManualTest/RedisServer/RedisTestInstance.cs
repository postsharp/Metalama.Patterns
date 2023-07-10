// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Patterns.Caching.ManualTest.Executables;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Diagnostics;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;

namespace Metalama.Patterns.Caching.ManualTest;

#region Taken from github.com/poulfoged/redis-inside (no strong name in the distributed package).

/// <summary>
/// Run integration-tests against Redis.
/// </summary>
public class RedisTestInstance : IDisposable
{
    public static readonly ConcurrentBag<WeakReference<RedisTestInstance>> Instances = new();
    private readonly Process process;
    private readonly TemporaryFile executable;
    private readonly Config config;

    public string Name { get; }

    public bool IsDisposed { get; private set; }

    public RedisTestInstance( string name = null )
    {
        this.Name = name;

        Instances.Add( new WeakReference<RedisTestInstance>( this ) );

        Stream executableStream;
        string extension;

        if ( RuntimeInformation.IsOSPlatform( OSPlatform.Windows ) )
        {
            executableStream =
                typeof(ExecutablesResourceTarget).Assembly.GetManifestResourceStream( typeof(ExecutablesResourceTarget), "redis-server.exe" );

            extension = "exe";
        }
        else
        {
            executableStream = File.OpenRead( "/usr/bin/redis-server" );
            extension = "";
        }

        this.executable = new TemporaryFile( executableStream, "redis-test-", extension );

    restart:
        this.config = new Config();

        var processStartInfo = new ProcessStartInfo( this.executable.Info.FullName )
        {
            UseShellExecute = false,
            Arguments =
                $"--port {this.config.port} --bind 127.0.0.1 --appendonly no --save \"\" --maxmemory-policy volatile-lru --notify-keyspace-events AKE --dbfilename {this.executable.Info.Name}.rdb",
            WindowStyle = ProcessWindowStyle.Hidden,
            CreateNoWindow = true,
            RedirectStandardError = true,
            RedirectStandardOutput = true,
            RedirectStandardInput = true,
            StandardOutputEncoding = Encoding.ASCII
        };

        var serverStartupEvent = new ManualResetEventSlim( false );
        var exited = false;
        var portError = false;
        string lastLine = null;

        this.process = new Process() { StartInfo = processStartInfo };

        this.process.ErrorDataReceived +=
            ( sender, eventArgs ) => this.config.logger.Invoke( eventArgs.Data );

        this.process.OutputDataReceived +=
            ( sender, eventArgs ) =>
            {
                if ( eventArgs.Data == null )
                {
                    return;
                }

                // This not the best way, but it's a quick way to know it's properly started.
                if ( eventArgs.Data.Contains( $"The server is now ready to accept connections on port {this.config.port}" ) )
                {
                    serverStartupEvent.Set();
                }

                // This is to recognize port conflicts and restart quickly.
                if ( eventArgs.Data.Contains( $":{this.config.port}: bind: No such file or directory" )
                   )
                {
                    Volatile.Write( ref portError, true );
                    serverStartupEvent.Set();
                }

                Volatile.Write( ref lastLine, eventArgs.Data );
                this.config.logger.Invoke( eventArgs.Data );
            };

        this.process.Exited +=
            ( s, ea ) =>
            {
                Volatile.Write( ref exited, true );
                serverStartupEvent.Set();
            };

        try
        {
            this.process.Start();
        }
        catch ( Win32Exception e )
        {
            throw new Exception(
                $"Unable to start Redis test instance process (path={processStartInfo.FileName},exists={File.Exists( processStartInfo.FileName )},exception={e.Message},code={e.NativeErrorCode})." );
        }

        this.process.BeginOutputReadLine();

        for ( var i = 0; i < 30; i++ )
        {
            if ( serverStartupEvent.Wait( TimeSpan.FromSeconds( 1 ) ) )
            {
                goto waitingDone;
            }

            if ( this.process.HasExited )
            {
                Volatile.Write( ref exited, true );

                goto waitingDone;
            }
        }

        throw new Exception( $"Redis process did not start in time." );

    waitingDone:

        if ( Volatile.Read( ref portError ) )
        {
            this.process.Kill();

            goto restart;
        }

        if ( Volatile.Read( ref exited ) )
        {
            throw new Exception( $"Redis process exited without being initialized properly. Last received line: {Volatile.Read( ref lastLine )}" );
        }
    }

    [Obsolete( "Use Endpoint Instead" )]
    public string Node => this.Endpoint.ToString();

    public EndPoint Endpoint => new IPEndPoint( IPAddress.Loopback, this.config.port );

    protected virtual void Dispose( bool disposing )
    {
        if ( this.IsDisposed )
        {
            return;
        }

        try
        {
            this.process.CancelOutputRead();
            this.process.StandardInput.Close();
            this.process.Kill();

            if ( disposing )
            {
                this.process.WaitForExit( 2000 );
                this.process.Dispose();
                this.executable.Dispose();
            }
        }
        catch ( Exception ex )
        {
            this.config.logger.Invoke( ex.ToString() );
        }

        this.IsDisposed = true;
    }

    ~RedisTestInstance()
    {
        this.Dispose( false );
    }

    public void Dispose()
    {
        this.Dispose( true );
        GC.SuppressFinalize( this );
    }

    private class Config
    {
        private static readonly ConcurrentDictionary<int, byte> usedPorts = new();
        private static readonly Random random = new();
        internal Action<string> logger;
        internal int port;

        public Config()
        {
            do
            {
                this.port = random.Next( 49152, 65535 + 1 );
            }
            while ( usedPorts.ContainsKey( this.port ) );

            usedPorts.AddOrUpdate( this.port, i => byte.MinValue, ( i, b ) => byte.MinValue );
            this.logger = this.TraceMessage;
        }

        private void TraceMessage( string message )
        {
            try
            {
                Trace.WriteLine( message );
            }
            catch { }
        }
    }
}

public class TemporaryFile : IDisposable
{
    [DllImport( "libc", SetLastError = true )]
    private static extern int chmod( string pathname, int mode );

    private readonly FileInfo _fileInfo;
    private bool _disposed;

    public TemporaryFile( string prefix = "", string extension = "tmp" )
    {
        this._fileInfo = new FileInfo(
            Path.Combine(
                Path.GetTempPath(),
                string.IsNullOrEmpty( extension )
                    ? prefix + Guid.NewGuid().ToString( "N" )
                    : prefix + Guid.NewGuid().ToString( "N" ) + "." + extension ) );
    }

    public TemporaryFile( Stream stream, string prefix = "", string extension = "tmp" ) : this( prefix, extension )
    {
        using ( stream )
        using ( var destination = this._fileInfo.OpenWrite() )
        {
            stream.CopyTo( destination );
        }

        if ( RuntimeInformation.IsOSPlatform( OSPlatform.Linux ) )
        {
            chmod( this._fileInfo.FullName, Convert.ToInt32( "777", 8 ) );
        }
    }

    public FileInfo Info
    {
        get { return this._fileInfo; }
    }

    protected virtual void Dispose( bool disposing )
    {
        if ( this._disposed )
        {
            return;
        }

        try
        {
            this._fileInfo.Delete();
        }
        catch ( Exception ex )
        {
            Trace.WriteLine( ex );
        }

        this._disposed = true;
    }

    ~TemporaryFile()
    {
        this.Dispose( false );
    }

    public void Dispose()
    {
        this.Dispose( true );
        GC.SuppressFinalize( this );
    }

    public void CopyTo( Stream result )
    {
        using ( var stream = this._fileInfo.OpenRead() )
        {
            stream.CopyTo( result );
        }

        if ( result.CanSeek )
        {
            result.Seek( 0, SeekOrigin.Begin );
        }
    }
}

#endregion