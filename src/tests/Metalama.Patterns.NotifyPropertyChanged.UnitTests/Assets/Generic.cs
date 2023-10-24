﻿// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Patterns.NotifyPropertyChanged.UnitTests.Assets.Core;
using System.ComponentModel;

namespace Metalama.Patterns.NotifyPropertyChanged.UnitTests.Assets.Generic;

/* Currently supported generic property types:
 * 
 * where class
 * where class, INotifyPropertyChanged
 * where struct
 * 
 * Not supported, probably never will(?):
 * 
 * where struct, INotifyPropertyChanged
 * unconstrained
 */

[NotifyPropertyChanged]
public partial class A<T>
    where T : class, INotifyPropertyChanged
{
    public T A1 { get; set; }
}

public partial class AOfSimple : A<Simple>
{
    public int RefA1S1 => this.A1.S1;
}

[NotifyPropertyChanged]
public partial class B<T>
    where T : class
{
    public T B1 { get; set; }
}

[NotifyPropertyChanged]
public partial class C<T>
    where T : struct
{
    public T C1 { get; set; }
}

public interface IFoo
{
    int X { get; }

    int Y { get; }
}

[NotifyPropertyChanged]
public partial class D<T>
    where T : class, INotifyPropertyChanged, IFoo
{
    public T D1 { get; set; }

    public int FooX => this.D1.X;
}

public partial class DD<T> : D<T> 
    where T : class, INotifyPropertyChanged, IFoo
{
    public int FooY => this.D1.Y;
}

[NotifyPropertyChanged]
public partial class MyFoo : IFoo
{
    public int X { get; set; }

    public int Y { get; set; }
}