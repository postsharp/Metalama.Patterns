﻿// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

namespace Metalama.Patterns.Xaml.AspectTests.CommandTests.Diagnostics;

public class ErrorCannotSpecifyBothCanExecuteMethodAndCanExecuteProperty
{
    [Command( CanExecuteMethod = "A", CanExecuteProperty = "B" )]
    private void Foo() { }
}