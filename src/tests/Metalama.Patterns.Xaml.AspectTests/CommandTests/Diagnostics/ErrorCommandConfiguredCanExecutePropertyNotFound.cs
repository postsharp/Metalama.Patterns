// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using System.Windows.Input;

namespace Metalama.Patterns.Xaml.AspectTests.CommandTests.Diagnostics;

public class ErrorCommandConfiguredCanExecutePropertyNotFound
{
    [Command( CanExecuteMethod = "DoesNotExist" )]
    public ICommand FooCommand { get; }
}