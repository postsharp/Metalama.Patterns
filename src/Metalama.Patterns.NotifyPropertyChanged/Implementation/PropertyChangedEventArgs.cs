// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using System.ComponentModel;

namespace Metalama.Patterns.NotifyPropertyChanged.Implementation;

/* Providing old/new value via derived PropertyChangedEventArgs
 * ------------------------------------------------------------
 * 
 * A notable downside of this approach is that a new instance of the derived type must be used for 
 * each event raised derived type must be used for each event raised. Underived PropertyChangedEventArgs 
 * can be cached per property name (eg, static field, global dictionary).
 * 
 * The naive model benefits most from using the extended PropertyChangedEventArgs when notifying that
 * a ref type property has changed (that is, the instance has been changed to a different instance). So
 * for now try using it only for that.
 * 
 */

public class PropertyChangedEventArgs<T> : PropertyChangedEventArgs, IPropertyChangedEventArgs<T>
{
    public PropertyChangedEventArgs( T? oldValue, T? newValue, string? propertyName ) : base( propertyName )
    {
        this.OldValue = oldValue;
        this.NewValue = newValue;
    }

    public T? OldValue { get; }

    public T? NewValue { get; }
}