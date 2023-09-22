// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

// TODO: Enable when INPC-type properties with initializers are supported.

#if false
using Metalama.Patterns.NotifyPropertyChanged.UnitTests.Assets.Core;

namespace Metalama.Patterns.NotifyPropertyChanged.UnitTests.Assets.Initializers;


[NotifyPropertyChanged]
public class A
{
    /// <summary>
    /// Auto property with initializer 'new()'.
    /// </summary>
    public Simple A1 { get; set; } = new();

    /// <summary>
    /// Ref to A1.S1.
    /// </summary>
    public int? A1S1 => this.A1?.S1;
}

#endif