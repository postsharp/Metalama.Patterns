// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using System.Windows.Input;

namespace Metalama.Patterns.Xaml.AspectTests.CommandTests.Callbacks;

public class ExecuteMethod
{
    [Command]
    public ICommand InstanceNoParametersCommand { get; }

    private void ExecuteInstanceNoParameters() { }

    [Command]
    public ICommand StaticNoParametersCommand { get; }

    private static void ExecuteStaticNoParameters() { }

    [Command]
    public ICommand InstanceWithParameterCommand { get; }

    private void ExecuteInstanceWithParameter( int v ) { }

    [Command]
    public ICommand StaticWithParameterCommand { get; }

    private static void ExecuteStaticWithParameter( int v ) { }
}