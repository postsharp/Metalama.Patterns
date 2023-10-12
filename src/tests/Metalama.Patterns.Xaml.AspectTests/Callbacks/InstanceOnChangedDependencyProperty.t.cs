using System.Windows;
namespace Metalama.Patterns.Xaml.AspectTests.Callbacks.InstanceOnChangedDependencyProperty;
public partial class InstanceOnChangedDependencyProperty : DependencyObject
{
  [DependencyProperty]
  public int Foo
  {
    get
    {
      return ((global::System.Int32)this.GetValue(global::Metalama.Patterns.Xaml.AspectTests.Callbacks.InstanceOnChangedDependencyProperty.InstanceOnChangedDependencyProperty.FooProperty));
    }
    set
    {
      this.SetValue(global::Metalama.Patterns.Xaml.AspectTests.Callbacks.InstanceOnChangedDependencyProperty.InstanceOnChangedDependencyProperty.FooProperty, value);
    }
  }
  private void OnFooChanged(DependencyProperty d)
  {
  }
  public static readonly global::System.Windows.DependencyProperty FooProperty;
  static InstanceOnChangedDependencyProperty()
  {
    object CoerceValue_1(global::System.Windows.DependencyObject d_1, object value)
    {
      return (global::System.Object)value;
    }
    void PropertyChanged(global::System.Windows.DependencyObject d_2, global::System.Windows.DependencyPropertyChangedEventArgs e)
    {
      ((global::Metalama.Patterns.Xaml.AspectTests.Callbacks.InstanceOnChangedDependencyProperty.InstanceOnChangedDependencyProperty)d_2).OnFooChanged(global::Metalama.Patterns.Xaml.AspectTests.Callbacks.InstanceOnChangedDependencyProperty.InstanceOnChangedDependencyProperty.FooProperty);
    }
    global::Metalama.Patterns.Xaml.AspectTests.Callbacks.InstanceOnChangedDependencyProperty.InstanceOnChangedDependencyProperty.FooProperty = global::System.Windows.DependencyProperty.Register("Foo", typeof(global::System.Int32), typeof(global::Metalama.Patterns.Xaml.AspectTests.Callbacks.InstanceOnChangedDependencyProperty.InstanceOnChangedDependencyProperty), new global::System.Windows.PropertyMetadata((global::System.Windows.PropertyChangedCallback)PropertyChanged));
  }
}