using System.Windows;
namespace Metalama.Patterns.Xaml.AspectTests.Callbacks.InstanceOnChangedOldValueAndNewValueAcceptsObject;
public partial class InstanceOnChangedOldValueAndNewValueAcceptsObject : DependencyObject
{
  [DependencyProperty]
  public int Foo
  {
    get
    {
      return ((global::System.Int32)this.GetValue(global::Metalama.Patterns.Xaml.AspectTests.Callbacks.InstanceOnChangedOldValueAndNewValueAcceptsObject.InstanceOnChangedOldValueAndNewValueAcceptsObject.FooProperty));
    }
    set
    {
      this.SetValue(global::Metalama.Patterns.Xaml.AspectTests.Callbacks.InstanceOnChangedOldValueAndNewValueAcceptsObject.InstanceOnChangedOldValueAndNewValueAcceptsObject.FooProperty, value);
    }
  }
  private void OnFooChanged(object oldValue, object newValue)
  {
  }
  public static readonly global::System.Windows.DependencyProperty FooProperty;
  static InstanceOnChangedOldValueAndNewValueAcceptsObject()
  {
    object CoerceValue_1(global::System.Windows.DependencyObject d, object value)
    {
      return (global::System.Object)value;
    }
    void PropertyChanged(global::System.Windows.DependencyObject d_1, global::System.Windows.DependencyPropertyChangedEventArgs e)
    {
      ((global::Metalama.Patterns.Xaml.AspectTests.Callbacks.InstanceOnChangedOldValueAndNewValueAcceptsObject.InstanceOnChangedOldValueAndNewValueAcceptsObject)d_1).OnFooChanged(e.OldValue, e.NewValue);
    }
    global::Metalama.Patterns.Xaml.AspectTests.Callbacks.InstanceOnChangedOldValueAndNewValueAcceptsObject.InstanceOnChangedOldValueAndNewValueAcceptsObject.FooProperty = global::System.Windows.DependencyProperty.Register("Foo", typeof(global::System.Int32), typeof(global::Metalama.Patterns.Xaml.AspectTests.Callbacks.InstanceOnChangedOldValueAndNewValueAcceptsObject.InstanceOnChangedOldValueAndNewValueAcceptsObject), new global::System.Windows.PropertyMetadata((global::System.Windows.PropertyChangedCallback)PropertyChanged));
  }
}