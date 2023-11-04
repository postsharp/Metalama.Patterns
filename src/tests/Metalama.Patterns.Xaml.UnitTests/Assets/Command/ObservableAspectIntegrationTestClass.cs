// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Patterns.Observability;

namespace Metalama.Patterns.Xaml.UnitTests.Assets.Command;

[Observable]
public partial class ObservableAspectIntegrationTestClass : CommandTestBase
{
    [Command]
    private void ExecuteFoo( int v )
    {
        LogCall( $"{v}" );
    }

    public bool CanExecuteFoo { get; set; }
}