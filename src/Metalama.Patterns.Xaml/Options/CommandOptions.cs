    // Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Code;
using Metalama.Framework.Options;
using System.ComponentModel;
using System.Windows.Input;

namespace Metalama.Patterns.Xaml.Options;

internal sealed record CommandOptions : IHierarchicalOptions<ICompilation>, IHierarchicalOptions<INamespace>, IHierarchicalOptions<INamedType>,
                                                   IHierarchicalOptions<IProperty>
{
    /// <summary>
    /// Gets the name of the method that implements the command logic. This method corresponds to the
    /// to the <see cref="ICommand.Execute"/> method. It is called every time the command is invoked.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The <c>Execute</c> method must be declared in the same class as the command property and can have zero parameter or one parameter of any type, which becomes the command parameter.
    /// </para>
    /// <para>
    /// If this property is not set, then the aspect will look for a method named <c>Foo</c> or <c>ExecuteFoo</c>, where <c>Foo</c> is the name of the command without the <c>Command</c> suffix.
    /// </para>
    /// </remarks>
    public string? ExecuteMethod { get; init; }

    /// <summary>
    /// Gets the name of the method that is called to determine whether the command can be executed.
    /// This method corresponds to the <see cref="ICommand.CanExecute"/> method.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The <c>CanExecute</c> method must be declared in the same class as the command property, return a <c>bool</c> value and can have zero or one parameter.
    /// </para>
    /// <para>
    /// If this property is not set, then the aspect will look for a method named <c>CanFoo</c> or <c>CanExecuteFoo</c>, where <c>Foo</c> is the name of the command without the <c>Command</c> suffix.
    /// </para>
    /// <para>
    /// At most one of the <see cref="CanExecuteMethod"/> and <see cref="CanExecuteProperty"/> properties may be set.
    /// </para>
    /// </remarks>
    public string? CanExecuteMethod { get; init; }

    /// <summary>
    /// Gets the name of the property that is evaluated to determine whether the command can be executed.
    /// This property corresponds to the <see cref="ICommand.CanExecute"/> method.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The <c>CanExecute</c> property must be declared in the same class as the command property and return a <c>bool</c> value.
    /// </para>
    /// <para>
    /// If this property is not set, then the aspect will look for a property named <c>CanFoo</c> or <c>CanExecuteFoo</c>, where <c>Foo</c> is the name of the command without the <c>Command</c> suffix.
    /// </para>
    /// <para>
    /// At most one of the <see cref="CanExecuteMethod"/> and <see cref="CanExecuteProperty"/> properties may be set.
    /// </para>
    /// </remarks>
    public string? CanExecuteProperty { get; init; }

    /// <summary>
    /// Gets a value indicating whether integration with <see cref="INotifyPropertyChanged"/> is enabled. The default is <see langword="true"/>.
    /// </summary>
    /// <remarks>
    /// <para>
    /// When <see cref="EnableINotifyPropertyChangedIntegration"/> is <see langword="true"/> (the default), and when a can-execute property (not a method) is used,
    /// and when the containing type of the target property implements <see cref="INotifyPropertyChanged"/>,then the <see cref="ICommand.CanExecuteChanged"/> event of 
    /// the command will be raised when the can-execute property changes. A warning is reported if the can-execute property is not public because <see cref="INotifyPropertyChanged"/>
    /// implementations typically only notify changes to public properties.
    /// </para>
    /// </remarks>
    public bool? EnableINotifyPropertyChangedIntegration { get; init; }

    IHierarchicalOptions? IHierarchicalOptions.GetDefaultOptions( OptionsInitializationContext context )
        => new CommandOptions { EnableINotifyPropertyChangedIntegration = true };

    object IIncrementalObject.ApplyChanges( object changes, in ApplyChangesContext context )
    {
        var other = (CommandOptions) changes;

        return new CommandOptions
        {
            ExecuteMethod = other.ExecuteMethod ?? this.ExecuteMethod,
            CanExecuteMethod = other.CanExecuteMethod ?? this.CanExecuteMethod,
            CanExecuteProperty = other.CanExecuteProperty ?? this.CanExecuteProperty,
            EnableINotifyPropertyChangedIntegration = other.EnableINotifyPropertyChangedIntegration ?? this.EnableINotifyPropertyChangedIntegration
        };
    }
}