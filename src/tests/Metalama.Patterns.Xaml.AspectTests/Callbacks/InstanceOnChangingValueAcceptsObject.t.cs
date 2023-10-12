using System.Windows;
namespace Metalama.Patterns.Xaml.AspectTests.Callbacks.InstanceOnChangingValueAcceptsObject;
public partial class InstanceOnChangingValueAcceptsObject : DependencyObject
{
  [DependencyProperty]
  public int Foo
  {
    get
    {
      return ((global::System.Int32)this.GetValue(global::Metalama.Patterns.Xaml.AspectTests.Callbacks.InstanceOnChangingValueAcceptsObject.InstanceOnChangingValueAcceptsObject.FooProperty));
    }
    set
    {
      this.SetValue(global::Metalama.Patterns.Xaml.AspectTests.Callbacks.InstanceOnChangingValueAcceptsObject.InstanceOnChangingValueAcceptsObject.FooProperty, value);
    }
  }
  private void OnFooChanging(object value)
  {
  }
  public static readonly global::System.Windows.DependencyProperty FooProperty;
  static InstanceOnChangingValueAcceptsObject()
  {
    object CoerceValue_1(global::System.Windows.DependencyObject d, object value_1)
    {
      ((global::Metalama.Patterns.Xaml.AspectTests.Callbacks.InstanceOnChangingValueAcceptsObject.InstanceOnChangingValueAcceptsObject)d).OnFooChanging(value_1);
      return (global::System.Object)value_1;
    }
    void PropertyChanged(global::System.Windows.DependencyObject d_1, global::System.Windows.DependencyPropertyChangedEventArgs e)
    {
    }
    var metadata = new global::System.Windows.PropertyMetadata();
    metadata.CoerceValueCallback = (global::System.Windows.CoerceValueCallback)CoerceValue_1;
    global::Metalama.Patterns.Xaml.AspectTests.Callbacks.InstanceOnChangingValueAcceptsObject.InstanceOnChangingValueAcceptsObject.FooProperty = global::System.Windows.DependencyProperty.Register("Foo", typeof(global::System.Int32), typeof(global::Metalama.Patterns.Xaml.AspectTests.Callbacks.InstanceOnChangingValueAcceptsObject.InstanceOnChangingValueAcceptsObject), metadata);
  }
}