// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using JetBrains.Annotations;
using Metalama.Framework.Aspects;
using Metalama.Framework.Code;

namespace Metalama.Patterns.Xaml.Options;

[PublicAPI]
[CompileTime]
public static class CommandOptionsExtensions
{
    /// <summary>
    /// Configures <see cref="CommandAttribute"/> for the current project.
    /// </summary>
    /// <param name="receiver">The <see cref="IAspectReceiver{TDeclaration}"/> for the current compilation.</param>
    /// <param name="configure">A delegate that configures the aspect.</param>
    public static void ConfigureCommand(
        this IAspectReceiver<ICompilation> receiver,
        Action<CommandOptionsBuilder> configure )
    {
        var builder = new CommandOptionsBuilder();
        configure( builder );

        var options = builder.Build();
        receiver.SetOptions( options );
    }

    /// <summary>
    /// Configures <see cref="CommandAttribute"/> for the current namespace.
    /// </summary>
    /// <param name="receiver">The <see cref="IAspectReceiver{TDeclaration}"/> for the current namespace.</param>
    /// <param name="configure">A delegate that configures the aspect.</param>
    public static void ConfigureCommand(
        this IAspectReceiver<INamespace> receiver,
        Action<CommandOptionsBuilder> configure )
    {
        var builder = new CommandOptionsBuilder();
        configure( builder );

        var options = builder.Build();
        receiver.SetOptions( options );
    }

    /// <summary>
    /// Configures <see cref="CommandAttribute"/> for the current type.
    /// </summary>
    /// <param name="receiver">The <see cref="IAspectReceiver{TDeclaration}"/> for the current type.</param>
    /// <param name="configure">A delegate that configures the aspect.</param>
    public static void ConfigureCommand(
        this IAspectReceiver<INamedType> receiver,
        Action<CommandOptionsBuilder> configure )
    {
        var builder = new CommandOptionsBuilder();
        configure( builder );

        var options = builder.Build();
        receiver.SetOptions( options );
    }

    /// <summary>
    /// Configures <see cref="CommandAttribute"/> for the current method.
    /// </summary>
    /// <param name="receiver">The <see cref="IAspectReceiver{TDeclaration}"/> for the current property.</param>
    /// <param name="configure">A delegate that configures the aspect.</param>
    public static void ConfigureCommand(
        this IAspectReceiver<IMethod> receiver,
        Action<CommandOptionsBuilder> configure )
    {
        var builder = new CommandOptionsBuilder();
        configure( builder );

        var options = builder.Build();
        receiver.SetOptions( options );
    }
}