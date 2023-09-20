// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Patterns.NotifyPropertyChanged;
using System.ComponentModel;

#pragma warning disable

namespace NpcExperiments.Exp4;

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
class A<T> where T : class, INotifyPropertyChanged
{
    public T A1 { get; set; }
}

[NotifyPropertyChanged]
class B<T> where T : class
{
    public T B1 { get; set; }
}

[NotifyPropertyChanged]
class C<T> where T : struct
{
    public T C1 { get; set; }
}

interface IFoo
{
    int X { get; }

    int Y { get; }
}

[NotifyPropertyChanged]
class D<T> where T : class, INotifyPropertyChanged, IFoo
{
    public T A1 { get; set; }

    public int FooX => A1.X;
}

// TODO: Pending #33805
#if false
class DD<T> : D<T> where T : class, INotifyPropertyChanged, IFoo
{
    public int FooY => A1.Y;
}
#endif