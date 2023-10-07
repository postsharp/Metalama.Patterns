// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

namespace Flashtrace;

public sealed class FlashtraceServiceBuilder
{
    public IServiceProvider ServiceProvider { get; }

    public HashSet<string> EnabledRoles { get; } = new();

    internal FlashtraceServiceBuilder( IServiceProvider serviceProvider )
    {
        this.ServiceProvider = serviceProvider;
        this.EnabledRoles.Add( FlashtraceRoles.Default );
    }
}