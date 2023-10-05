// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using System.ComponentModel;

namespace Metalama.Patterns.NotifyPropertyChanged.AspectTests.Diagnostics;

[NotifyPropertyChanged]
public class PropertyOfGenericTypeThatIsStructAndInpc<T>
    where T : struct, INotifyPropertyChanged
{
    public T C1 { get; set; }
}