using System.Windows;
namespace Metalama.Patterns.Xaml.AspectTests.Callbacks.InstanceOnChangedOldValueAndNewValueAcceptsGeneric;
public partial class InstanceOnChangedOldValueAndNewValueAcceptsGeneric : DependencyObject
{
  [DependencyProperty]
  public int Foo
  {
    get
    {
      return ((global::System.Int32)this.GetValue(global::Metalama.Patterns.Xaml.AspectTests.Callbacks.InstanceOnChangedOldValueAndNewValueAcceptsGeneric.InstanceOnChangedOldValueAndNewValueAcceptsGeneric.FooProperty));
    }
    set
    {
      this.SetValue(global::Metalama.Patterns.Xaml.AspectTests.Callbacks.InstanceOnChangedOldValueAndNewValueAcceptsGeneric.InstanceOnChangedOldValueAndNewValueAcceptsGeneric.FooProperty, value);
    }
  }
  private void OnFooChanged<T>(T oldValue, T newValue)
  {
  }
  public static readonly global::System.Windows.DependencyProperty FooProperty;
  static InstanceOnChangedOldValueAndNewValueAcceptsGeneric()
  {
    object CoerceValue_1(global::System.Windows.DependencyObject d, object value)
    {
      return (global::System.Object)value;
    }
    void PropertyChanged(global::System.Windows.DependencyObject d_1, global::System.Windows.DependencyPropertyChangedEventArgs e)
    {
      ((global::Metalama.Patterns.Xaml.AspectTests.Callbacks.InstanceOnChangedOldValueAndNewValueAcceptsGeneric.InstanceOnChangedOldValueAndNewValueAcceptsGeneric)d_1).OnFooChanged<global::System.Int32>((global::System.Int32)e.OldValue, (global::System.Int32)e.NewValue);
    }
    global::Metalama.Patterns.Xaml.AspectTests.Callbacks.InstanceOnChangedOldValueAndNewValueAcceptsGeneric.InstanceOnChangedOldValueAndNewValueAcceptsGeneric.FooProperty = global::System.Windows.DependencyProperty.Register("Foo", typeof(global::System.Int32), typeof(global::Metalama.Patterns.Xaml.AspectTests.Callbacks.InstanceOnChangedOldValueAndNewValueAcceptsGeneric.InstanceOnChangedOldValueAndNewValueAcceptsGeneric), new global::System.Windows.PropertyMetadata((global::System.Windows.PropertyChangedCallback)PropertyChanged));
  }
}