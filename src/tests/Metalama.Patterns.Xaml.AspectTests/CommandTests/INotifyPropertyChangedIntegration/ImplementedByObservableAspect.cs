// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Patterns.NotifyPropertyChanged;
using System.Windows.Input;

namespace Metalama.Patterns.Xaml.AspectTests.CommandTests.INotifyPropertyChangedIntegration.ImplementedByObservableAspect;

[NotifyPropertyChanged]
public class ImplementedByObservableAspect
{
    [Command]
    public ICommand Foo1Command { get; }

    private void ExecuteFoo1() { }

    public bool CanExecuteFoo1 { get; set; }
}

public class ImplementedByBase : ImplementedByObservableAspect
{
    [Command]
    public ICommand Foo2Command { get; }

    private void ExecuteFoo2() { }

    public bool CanExecuteFoo2 { get; set; }
}