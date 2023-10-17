// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using System.Windows.Input;

namespace Metalama.Patterns.Xaml.AspectTests.CommandTests.Callbacks;

public class CanExecuteMethod
{
    [Command]
    public ICommand InstanceNoParametersCommand { get; }

    private void ExecuteInstanceNoParameters() { }

    private bool CanExecuteInstanceNoParameters() => true;

    [Command]
    public ICommand StaticNoParametersCommand { get; }

    private static void ExecuteStaticNoParameters() { }

    private static bool CanExecuteStaticNoParameters() => true;

    [Command]
    public ICommand InstanceWithParameterCommand { get; }

    private void ExecuteInstanceWithParameter( int v ) { }

    private bool CanExecuteInstanceWithParameter( int v ) => true;

    [Command]
    public ICommand StaticWithParameterCommand { get; }

    private static void ExecuteStaticWithParameter( int v ) { }

    private static bool CanExecuteStaticWithParameter( int v ) => true;
}