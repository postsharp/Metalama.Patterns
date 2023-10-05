// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Aspects;
using Metalama.Framework.Code;

namespace Metalama.Patterns.NotifyPropertyChanged.Options;

[CompileTime]
public static class NotifyPropertyChangedExtensions
{
    /// <summary>
    /// Configures <see cref="NotifyPropertyChangedAttribute"/> for the current project.
    /// </summary>
    /// <param name="receiver">The <see cref="IAspectReceiver{TDeclaration}"/> for the current compilation.</param>
    /// <param name="configure">A delegate that configures the aspect.</param>
    public static void ConfigureNotifyPropertyChanged(
        this IAspectReceiver<ICompilation> receiver,
        Action<NotifyPropertyChangedOptionsBuilder> configure )
    {
        var builder = new NotifyPropertyChangedOptionsBuilder();
        configure( builder );

        var options = builder.Build();

        receiver.SetOptions( options );
    }

    /// <summary>
    /// Configures <see cref="NotifyPropertyChangedAttribute"/> for the current namespace.
    /// </summary>
    /// <param name="receiver">The <see cref="IAspectReceiver{TDeclaration}"/> for the current namespace.</param>
    /// <param name="configure">A delegate that configures the aspect.</param>
    public static void ConfigureNotifyPropertyChanged(
        this IAspectReceiver<INamespace> receiver,
        Action<NotifyPropertyChangedOptionsBuilder> configure )
    {
        var builder = new NotifyPropertyChangedOptionsBuilder();
        configure( builder );

        var options = builder.Build();

        receiver.SetOptions( options );
    }

    /// <summary>
    /// Configures <see cref="NotifyPropertyChangedAttribute"/> for the current type.
    /// </summary>
    /// <param name="receiver">The <see cref="IAspectReceiver{TDeclaration}"/> for the current type.</param>
    /// <param name="configure">A delegate that configures the aspect.</param>
    public static void ConfigureNotifyPropertyChanged(
        this IAspectReceiver<INamedType> receiver,
        Action<NotifyPropertyChangedOptionsBuilder> configure )
    {
        var builder = new NotifyPropertyChangedOptionsBuilder();
        configure( builder );

        var options = builder.Build();

        receiver.SetOptions( options );
    }
}