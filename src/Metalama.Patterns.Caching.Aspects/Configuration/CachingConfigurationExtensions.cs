// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using JetBrains.Annotations;
using Metalama.Framework.Aspects;
using Metalama.Framework.Code;

namespace Metalama.Patterns.Caching.Aspects.Configuration;

[PublicAPI]
[CompileTime]
public static class CachingConfigurationExtensions
{
    public static void ConfigureCaching( this IAspectReceiver<IMethod> method, Action<CachingOptionsBuilder> build )
    {
        var builder = new CachingOptionsBuilder();
        build( builder );
        method.SetOptions( builder.Build() );
    }

    public static void ConfigureCaching( this IAspectReceiver<ICompilation> method, Action<CachingOptionsBuilder> build )
    {
        var builder = new CachingOptionsBuilder();
        build( builder );
        method.SetOptions( builder.Build() );
    }

    public static void ConfigureCaching( this IAspectReceiver<INamespace> method, Action<CachingOptionsBuilder> build )
    {
        var builder = new CachingOptionsBuilder();
        build( builder );
        method.SetOptions( builder.Build() );
    }

    public static void ConfigureCaching( this IAspectReceiver<INamedType> method, Action<CachingOptionsBuilder> build )
    {
        var builder = new CachingOptionsBuilder();
        build( builder );
        method.SetOptions( builder.Build() );
    }
}