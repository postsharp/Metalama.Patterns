// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Aspects;
using Metalama.Framework.Code;

namespace Metalama.Patterns.Observability.Options;

[CompileTime]
public static class DependencyAnalysisExtensions
{
    public static void ConfigureDependencyAnalysis(
        this IAspectReceiver<ICompilation> receiver,
        Action<DependencyAnalysisOptionsBuilder> configure )
    {
        var builder = new DependencyAnalysisOptionsBuilder();
        configure( builder );
        receiver.SetOptions( builder.Build() );
    }

    public static void ConfigureDependencyAnalysis(
        this IAspectReceiver<INamespace> receiver,
        Action<DependencyAnalysisOptionsBuilder> configure )
    {
        var builder = new DependencyAnalysisOptionsBuilder();
        configure( builder );
        receiver.SetOptions( builder.Build() );
    }

    public static void ConfigureDependencyAnalysis(
        this IAspectReceiver<INamedType> receiver,
        Action<DependencyAnalysisOptionsBuilder> configure )
    {
        var builder = new DependencyAnalysisOptionsBuilder();
        configure( builder );
        receiver.SetOptions( builder.Build() );
    }

    public static void ConfigureDependencyAnalysis(
        this IAspectReceiver<IMember> receiver,
        Action<DependencyAnalysisOptionsBuilder> configure )
    {
        var builder = new DependencyAnalysisOptionsBuilder();
        configure( builder );
        receiver.SetOptions( builder.Build() );
    }
}