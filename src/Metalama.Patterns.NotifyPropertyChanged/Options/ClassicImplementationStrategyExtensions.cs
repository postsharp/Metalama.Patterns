// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Aspects;
using Metalama.Framework.Code;

namespace Metalama.Patterns.NotifyPropertyChanged.Options;

[CompileTime]
public static class ClassicImplementationStrategyExtensions
{
    /// <summary>
    /// Configures <c>Metalama.Patterns.NotifyPropertyChanged.Implementation.ClassicStrategy</c> for the current project.
    /// </summary>
    /// <param name="receiver">The <see cref="IAspectReceiver{TDeclaration}"/> for the current compilation.</param>
    /// <param name="configure">A delegate that configures the strategy.</param>
    public static void ConfigureClassicImplementationStrategy(
        this IAspectReceiver<ICompilation> receiver,
        Action<ClassicImplementationStrategyOptionsBuilder> configure )
    {
        var builder = new ClassicImplementationStrategyOptionsBuilder();
        configure( builder );

        var options = builder.Build();

        receiver.SetOptions( options );
    }

    /// <summary>
    /// Configures <c>Metalama.Patterns.NotifyPropertyChanged.Implementation.ClassicStrategy</c> for the current namespace.
    /// </summary>
    /// <param name="receiver">The <see cref="IAspectReceiver{TDeclaration}"/> for the current namespace.</param>
    /// <param name="configure">A delegate that configures the strategy.</param>
    public static void ConfigureClassicImplementationStrategy(
        this IAspectReceiver<INamespace> receiver,
        Action<ClassicImplementationStrategyOptionsBuilder> configure )
    {
        var builder = new ClassicImplementationStrategyOptionsBuilder();
        configure( builder );

        var options = builder.Build();

        receiver.SetOptions( options );
    }

    /// <summary>
    /// Configures <c>Metalama.Patterns.NotifyPropertyChanged.Implementation.ClassicStrategy</c> for the current type.
    /// </summary>
    /// <param name="receiver">The <see cref="IAspectReceiver{TDeclaration}"/> for the current type.</param>
    /// <param name="configure">A delegate that configures the strategy.</param>
    public static void ConfigureClassicImplementationStrategy(
        this IAspectReceiver<INamedType> receiver,
        Action<ClassicImplementationStrategyOptionsBuilder> configure )
    {
        var builder = new ClassicImplementationStrategyOptionsBuilder();
        configure( builder );

        var options = builder.Build();

        receiver.SetOptions( options );
    }
}