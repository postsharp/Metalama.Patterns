// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using System.ComponentModel;
using System.Windows.Input;

namespace Metalama.Patterns.Xaml.AspectTests.CommandTests.INotifyPropertyChangedIntegration.AnyImplementation;

public class AnyImplementation : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    [Command]
    public ICommand Foo1Command { get; }

    private void ExecuteFoo1() { }

    public bool CanExecuteFoo1 => true;
}

public class ImplementedByBase : AnyImplementation
{
    [Command]
    public ICommand Foo2Command { get; }

    private void ExecuteFoo2() { }

    public bool CanExecuteFoo2 => true;
}