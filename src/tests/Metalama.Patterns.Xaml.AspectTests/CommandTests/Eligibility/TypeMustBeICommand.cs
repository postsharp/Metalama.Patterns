// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

namespace Metalama.Patterns.Xaml.AspectTests.CommandTests.Eligibility;

public class TypeMustBeICommand
{
    [Command]
    public object ObjectCommand { get; }

    [Command]
    public int IntCommand { get; }
}