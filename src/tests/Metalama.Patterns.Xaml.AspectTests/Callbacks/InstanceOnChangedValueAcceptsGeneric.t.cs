using System.Windows;
namespace Metalama.Patterns.Xaml.AspectTests.Callbacks.InstanceOnChangedValueAcceptsGeneric;
public partial class InstanceOnChangedValueAcceptsGeneric : DependencyObject
{
  [DependencyProperty]
  public int Foo
  {
    get
    {
      return ((global::System.Int32)this.GetValue(global::Metalama.Patterns.Xaml.AspectTests.Callbacks.InstanceOnChangedValueAcceptsGeneric.InstanceOnChangedValueAcceptsGeneric.FooProperty));
    }
    set
    {
      this.SetValue(global::Metalama.Patterns.Xaml.AspectTests.Callbacks.InstanceOnChangedValueAcceptsGeneric.InstanceOnChangedValueAcceptsGeneric.FooProperty, value);
    }
  }
  private void OnFooChanged<T>(T value)
  {
  }
  public static readonly global::System.Windows.DependencyProperty FooProperty;
  static InstanceOnChangedValueAcceptsGeneric()
  {
    object CoerceValue_1(global::System.Windows.DependencyObject d, object value_1)
    {
      return (global::System.Object)value_1;
    }
    void PropertyChanged(global::System.Windows.DependencyObject d_1, global::System.Windows.DependencyPropertyChangedEventArgs e)
    {
      ((global::Metalama.Patterns.Xaml.AspectTests.Callbacks.InstanceOnChangedValueAcceptsGeneric.InstanceOnChangedValueAcceptsGeneric)d_1).OnFooChanged<global::System.Int32>((global::System.Int32)e.NewValue);
    }
    global::Metalama.Patterns.Xaml.AspectTests.Callbacks.InstanceOnChangedValueAcceptsGeneric.InstanceOnChangedValueAcceptsGeneric.FooProperty = global::System.Windows.DependencyProperty.Register("Foo", typeof(global::System.Int32), typeof(global::Metalama.Patterns.Xaml.AspectTests.Callbacks.InstanceOnChangedValueAcceptsGeneric.InstanceOnChangedValueAcceptsGeneric), new global::System.Windows.PropertyMetadata((global::System.Windows.PropertyChangedCallback)PropertyChanged));
  }
}