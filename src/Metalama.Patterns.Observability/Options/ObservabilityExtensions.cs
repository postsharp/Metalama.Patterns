// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using JetBrains.Annotations;
using Metalama.Framework.Aspects;
using Metalama.Framework.Code;

namespace Metalama.Patterns.Observability.Options;

/// <summary>
/// Extension methods that configure the <see cref="ObservableAttribute"/> aspect.
/// </summary>
[PublicAPI]
[CompileTime]
public static class ObservabilityExtensions
{
    /// <summary>
    /// Configures <see cref="ObservableAttribute"/> for the current project.
    /// </summary>
    /// <param name="receiver">The <see cref="IAspectReceiver{TDeclaration}"/> for the current compilation.</param>
    /// <param name="configure">A delegate that configures the aspect.</param>
    public static void ConfigureObservability(
        this IAspectReceiver<ICompilation> receiver,
        Action<ObservabilityTypeOptionsBuilder> configure )
    {
        var builder = new ObservabilityTypeOptionsBuilder();
        configure( builder );

        if ( builder.ObservabilityOptions != null )
        {
            receiver.SetOptions( builder.ObservabilityOptions );
        }

        if ( builder.ClassicStrategyOptions != null )
        {
            receiver.SetOptions( builder.ClassicStrategyOptions );
        }

        if ( builder.DependencyAnalysisOptions != null )
        {
            receiver.SetOptions( builder.DependencyAnalysisOptions );
        }
    }

    /// <summary>
    /// Configures <see cref="ObservableAttribute"/> for the current namespace.
    /// </summary>
    /// <param name="receiver">The <see cref="IAspectReceiver{TDeclaration}"/> for the current namespace.</param>
    /// <param name="configure">A delegate that configures the aspect.</param>
    public static void ConfigureObservability(
        this IAspectReceiver<INamespace> receiver,
        Action<ObservabilityTypeOptionsBuilder> configure )
    {
        var builder = new ObservabilityTypeOptionsBuilder();
        configure( builder );

        if ( builder.ObservabilityOptions != null )
        {
            receiver.SetOptions( builder.ObservabilityOptions );
        }

        if ( builder.ClassicStrategyOptions != null )
        {
            receiver.SetOptions( builder.ClassicStrategyOptions );
        }

        if ( builder.DependencyAnalysisOptions != null )
        {
            receiver.SetOptions( builder.DependencyAnalysisOptions );
        }
    }

    /// <summary>
    /// Configures <see cref="ObservableAttribute"/> for the current type.
    /// </summary>
    /// <param name="receiver">The <see cref="IAspectReceiver{TDeclaration}"/> for the current type.</param>
    /// <param name="configure">A delegate that configures the aspect.</param>
    public static void ConfigureObservability(
        this IAspectReceiver<INamedType> receiver,
        Action<ObservabilityTypeOptionsBuilder> configure )
    {
        var builder = new ObservabilityTypeOptionsBuilder();
        configure( builder );

        if ( builder.ObservabilityOptions != null )
        {
            receiver.SetOptions( builder.ObservabilityOptions );
        }

        if ( builder.ClassicStrategyOptions != null )
        {
            receiver.SetOptions( builder.ClassicStrategyOptions );
        }

        if ( builder.DependencyAnalysisOptions != null )
        {
            receiver.SetOptions( builder.DependencyAnalysisOptions );
        }
    }

    /// <summary>
    /// Configures <see cref="ObservableAttribute"/> for the current member.
    /// </summary>
    /// <param name="receiver">The <see cref="IAspectReceiver{TDeclaration}"/> for the current type.</param>
    /// <param name="configure">A delegate that configures the aspect.</param>
    public static void ConfigureObservability(
        this IAspectReceiver<IMember> receiver,
        Action<ObservabilityMemberOptionsBuilder> configure )
    {
        var builder = new ObservabilityMemberOptionsBuilder();
        configure( builder );

        if ( builder.DependencyAnalysisOptions != null )
        {
            receiver.SetOptions( builder.DependencyAnalysisOptions );
        }
    }
}