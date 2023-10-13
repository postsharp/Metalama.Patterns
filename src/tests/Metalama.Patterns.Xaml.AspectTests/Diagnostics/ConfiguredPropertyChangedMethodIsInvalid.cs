// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

// @RemoveOutputCode

using System.Windows;

namespace Metalama.Patterns.Xaml.AspectTests.Diagnostics.ConfiguredPropertyChangedMethodIsInvalid;

public class ConfiguredPropertyChangedMethodIsInvalid : DependencyObject
{
    [DependencyProperty( PropertyChangedMethod = nameof( Changed ) )]
    public int Foo { get; set; }

    private void Changed( DBNull blah ) { }
}