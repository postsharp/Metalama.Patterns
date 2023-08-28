// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using System.ComponentModel;

namespace Metalama.Patterns.NotifyPropertyChanged.Implementation;

public interface IPropertyChangedEventArgs<out T> : INotifyPropertyChanged
{
    T? OldValue { get; }

    T? NewValue { get; }
}