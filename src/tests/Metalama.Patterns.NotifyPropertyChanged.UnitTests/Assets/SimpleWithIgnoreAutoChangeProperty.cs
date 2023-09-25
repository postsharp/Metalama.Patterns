// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Patterns.NotifyPropertyChanged.UnitTests.Assets.Core;

namespace Metalama.Patterns.NotifyPropertyChanged.UnitTests.Assets.IgnoreAutoChangePropertyAttribute;

[NotifyPropertyChanged]
public class SimpleWithIgnoreAutoChangeProperty
{
    public int P1 { get; set; }

    [IgnoreAutoChangeNotification]
    public int P2 { get; set; }

    [IgnoreAutoChangeNotification]
    public Simple P3 { get; set; }
}