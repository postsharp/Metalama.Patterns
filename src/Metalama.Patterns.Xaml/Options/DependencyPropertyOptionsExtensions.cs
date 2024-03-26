// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using JetBrains.Annotations;
using Metalama.Framework.Aspects;
using Metalama.Framework.Code;

namespace Metalama.Patterns.Xaml.Options;

[PublicAPI]
[CompileTime]
public static class DependencyPropertyOptionsExtensions
{
    /// <summary>
    /// Configures <see cref="DependencyPropertyAttribute"/> for the current project.
    /// </summary>
    /// <param name="receiver">The <see cref="IAspectReceiver{TDeclaration}"/> for the current compilation.</param>
    /// <param name="configure">A delegate that configures the aspect.</param>
    public static void ConfigureDependencyProperty(
        this IAspectReceiver<ICompilation> receiver,
        Action<DependencyPropertyOptionsBuilder> configure )
    {
        var builder = new DependencyPropertyOptionsBuilder();
        configure( builder );

        var options = builder.Build();
        receiver.SetOptions( options );
    }

    /// <summary>
    /// Configures <see cref="DependencyPropertyAttribute"/> for the current namespace.
    /// </summary>
    /// <param name="receiver">The <see cref="IAspectReceiver{TDeclaration}"/> for the current namespace.</param>
    /// <param name="configure">A delegate that configures the aspect.</param>
    public static void ConfigureDependencyProperty(
        this IAspectReceiver<INamespace> receiver,
        Action<DependencyPropertyOptionsBuilder> configure )
    {
        var builder = new DependencyPropertyOptionsBuilder();
        configure( builder );

        var options = builder.Build();
        receiver.SetOptions( options );
    }

    /// <summary>
    /// Configures <see cref="DependencyPropertyAttribute"/> for the current type.
    /// </summary>
    /// <param name="receiver">The <see cref="IAspectReceiver{TDeclaration}"/> for the current type.</param>
    /// <param name="configure">A delegate that configures the aspect.</param>
    public static void ConfigureDependencyProperty(
        this IAspectReceiver<INamedType> receiver,
        Action<DependencyPropertyOptionsBuilder> configure )
    {
        var builder = new DependencyPropertyOptionsBuilder();
        configure( builder );

        var options = builder.Build();
        receiver.SetOptions( options );
    }

    /// <summary>
    /// Configures <see cref="DependencyPropertyAttribute"/> for the current property.
    /// </summary>
    /// <param name="receiver">The <see cref="IAspectReceiver{TDeclaration}"/> for the current property.</param>
    /// <param name="configure">A delegate that configures the aspect.</param>
    public static void ConfigureDependencyProperty(
        this IAspectReceiver<IProperty> receiver,
        Action<DependencyPropertyOptionsBuilder> configure )
    {
        var builder = new DependencyPropertyOptionsBuilder();
        configure( builder );

        var options = builder.Build();
        receiver.SetOptions( options );
    }
}