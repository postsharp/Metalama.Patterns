// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

// TODO: Test disabled due to #34010 - [Observable] overrides the setter, framework generates unsupported `init` keyword in net471.

#if NETCOREAPP
using Metalama.Patterns.Observability;
using System.Windows.Input;

namespace Metalama.Patterns.Xaml.UnitTests.Assets.Command;

[Observable]
public partial class ObservableAspectIntegrationTestClass : CommandTestBase
{
    [Command]
    public ICommand FooCommand { get; }

    private void ExecuteFoo( int v )
    {
        LogCall( $"{v}" );
    }

    public bool CanExecuteFoo { get; set; }
}
#endif