// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using System.Windows.Input;

namespace Metalama.Patterns.Xaml.AspectTests.CommandTests.Diagnostics;

public class ErrorCommandCanExecuteMethodIsAmbiguous
{
    [Command]
    public ICommand FooCommand { get; }

    private void ExecuteFoo() { }

    private bool CanExecuteFoo() => true;

    private bool CanExecuteFoo( int v ) => true;
}