using System.Windows;
namespace Metalama.Patterns.Xaml.AspectTests.Callbacks.InstanceOnChangingValueAcceptsGeneric;
public partial class InstanceOnChangingValueAcceptsGeneric : DependencyObject
{
  [DependencyProperty]
  public int Foo
  {
    get
    {
      return ((global::System.Int32)this.GetValue(global::Metalama.Patterns.Xaml.AspectTests.Callbacks.InstanceOnChangingValueAcceptsGeneric.InstanceOnChangingValueAcceptsGeneric.FooProperty));
    }
    set
    {
      this.SetValue(global::Metalama.Patterns.Xaml.AspectTests.Callbacks.InstanceOnChangingValueAcceptsGeneric.InstanceOnChangingValueAcceptsGeneric.FooProperty, value);
    }
  }
  private void OnFooChanging<T>(T value)
  {
  }
  public static readonly global::System.Windows.DependencyProperty FooProperty;
  static InstanceOnChangingValueAcceptsGeneric()
  {
    object CoerceValue_1(global::System.Windows.DependencyObject d, object value_1)
    {
      ((global::Metalama.Patterns.Xaml.AspectTests.Callbacks.InstanceOnChangingValueAcceptsGeneric.InstanceOnChangingValueAcceptsGeneric)d).OnFooChanging<global::System.Int32>((global::System.Int32)value_1);
      return (global::System.Object)value_1;
    }
    void PropertyChanged(global::System.Windows.DependencyObject d_1, global::System.Windows.DependencyPropertyChangedEventArgs e)
    {
    }
    var metadata = new global::System.Windows.PropertyMetadata();
    metadata.CoerceValueCallback = (global::System.Windows.CoerceValueCallback)CoerceValue_1;
    global::Metalama.Patterns.Xaml.AspectTests.Callbacks.InstanceOnChangingValueAcceptsGeneric.InstanceOnChangingValueAcceptsGeneric.FooProperty = global::System.Windows.DependencyProperty.Register("Foo", typeof(global::System.Int32), typeof(global::Metalama.Patterns.Xaml.AspectTests.Callbacks.InstanceOnChangingValueAcceptsGeneric.InstanceOnChangingValueAcceptsGeneric), metadata);
  }
}