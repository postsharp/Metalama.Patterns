using System.Windows;
namespace Metalama.Patterns.Xaml.AspectTests.Callbacks.InstanceOnChangedValueAcceptsObject;
public partial class InstanceOnChangedValueAcceptsObject : DependencyObject
{
  [DependencyProperty]
  public int Foo
  {
    get
    {
      return ((global::System.Int32)this.GetValue(global::Metalama.Patterns.Xaml.AspectTests.Callbacks.InstanceOnChangedValueAcceptsObject.InstanceOnChangedValueAcceptsObject.FooProperty));
    }
    set
    {
      this.SetValue(global::Metalama.Patterns.Xaml.AspectTests.Callbacks.InstanceOnChangedValueAcceptsObject.InstanceOnChangedValueAcceptsObject.FooProperty, value);
    }
  }
  private void OnFooChanged(object value)
  {
  }
  public static readonly global::System.Windows.DependencyProperty FooProperty;
  static InstanceOnChangedValueAcceptsObject()
  {
    object CoerceValue_1(global::System.Windows.DependencyObject d, object value_1)
    {
      return (global::System.Object)value_1;
    }
    void PropertyChanged(global::System.Windows.DependencyObject d_1, global::System.Windows.DependencyPropertyChangedEventArgs e)
    {
      ((global::Metalama.Patterns.Xaml.AspectTests.Callbacks.InstanceOnChangedValueAcceptsObject.InstanceOnChangedValueAcceptsObject)d_1).OnFooChanged(e.NewValue);
    }
    global::Metalama.Patterns.Xaml.AspectTests.Callbacks.InstanceOnChangedValueAcceptsObject.InstanceOnChangedValueAcceptsObject.FooProperty = global::System.Windows.DependencyProperty.Register("Foo", typeof(global::System.Int32), typeof(global::Metalama.Patterns.Xaml.AspectTests.Callbacks.InstanceOnChangedValueAcceptsObject.InstanceOnChangedValueAcceptsObject), new global::System.Windows.PropertyMetadata((global::System.Windows.PropertyChangedCallback)PropertyChanged));
  }
}