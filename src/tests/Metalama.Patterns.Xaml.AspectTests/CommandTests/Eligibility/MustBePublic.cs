// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using System.Windows.Input;

namespace Metalama.Patterns.Xaml.AspectTests.CommandTests.Eligibility;

public class MustBePublic
{
    [Command]
    private ICommand PrivateCommand { get; }

    [Command]
    protected ICommand ProtectedCommand { get; }

    [Command]
    internal ICommand InternalCommand { get; }
}