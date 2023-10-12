using System.Windows;
namespace Metalama.Patterns.Xaml.AspectTests.Callbacks.InstanceOnChangedValue;
public partial class InstanceOnChangedValue : DependencyObject
{
  [DependencyProperty]
  public int Foo
  {
    get
    {
      return ((global::System.Int32)this.GetValue(global::Metalama.Patterns.Xaml.AspectTests.Callbacks.InstanceOnChangedValue.InstanceOnChangedValue.FooProperty));
    }
    set
    {
      this.SetValue(global::Metalama.Patterns.Xaml.AspectTests.Callbacks.InstanceOnChangedValue.InstanceOnChangedValue.FooProperty, value);
    }
  }
  private void OnFooChanged(int value)
  {
  }
  public static readonly global::System.Windows.DependencyProperty FooProperty;
  static InstanceOnChangedValue()
  {
    object CoerceValue_1(global::System.Windows.DependencyObject d, object value_1)
    {
      return (global::System.Object)value_1;
    }
    void PropertyChanged(global::System.Windows.DependencyObject d_1, global::System.Windows.DependencyPropertyChangedEventArgs e)
    {
      ((global::Metalama.Patterns.Xaml.AspectTests.Callbacks.InstanceOnChangedValue.InstanceOnChangedValue)d_1).OnFooChanged((global::System.Int32)e.NewValue);
    }
    global::Metalama.Patterns.Xaml.AspectTests.Callbacks.InstanceOnChangedValue.InstanceOnChangedValue.FooProperty = global::System.Windows.DependencyProperty.Register("Foo", typeof(global::System.Int32), typeof(global::Metalama.Patterns.Xaml.AspectTests.Callbacks.InstanceOnChangedValue.InstanceOnChangedValue), new global::System.Windows.PropertyMetadata((global::System.Windows.PropertyChangedCallback)PropertyChanged));
  }
}