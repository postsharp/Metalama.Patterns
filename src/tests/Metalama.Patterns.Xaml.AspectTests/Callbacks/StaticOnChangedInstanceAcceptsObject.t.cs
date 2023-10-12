using System.Windows;
namespace Metalama.Patterns.Xaml.AspectTests.Callbacks.StaticOnChangedInstanceAcceptsObject;
public partial class StaticOnChangedInstanceAcceptsObject : DependencyObject
{
  [DependencyProperty]
  public int Foo
  {
    get
    {
      return ((global::System.Int32)this.GetValue(global::Metalama.Patterns.Xaml.AspectTests.Callbacks.StaticOnChangedInstanceAcceptsObject.StaticOnChangedInstanceAcceptsObject.FooProperty));
    }
    set
    {
      this.SetValue(global::Metalama.Patterns.Xaml.AspectTests.Callbacks.StaticOnChangedInstanceAcceptsObject.StaticOnChangedInstanceAcceptsObject.FooProperty, value);
    }
  }
  private static void OnFooChanged(object instance)
  {
  }
  public static readonly global::System.Windows.DependencyProperty FooProperty;
  static StaticOnChangedInstanceAcceptsObject()
  {
    object CoerceValue_1(global::System.Windows.DependencyObject d, object value)
    {
      return (global::System.Object)value;
    }
    void PropertyChanged(global::System.Windows.DependencyObject d_1, global::System.Windows.DependencyPropertyChangedEventArgs e)
    {
      global::Metalama.Patterns.Xaml.AspectTests.Callbacks.StaticOnChangedInstanceAcceptsObject.StaticOnChangedInstanceAcceptsObject.OnFooChanged((global::Metalama.Patterns.Xaml.AspectTests.Callbacks.StaticOnChangedInstanceAcceptsObject.StaticOnChangedInstanceAcceptsObject)d_1);
    }
    global::Metalama.Patterns.Xaml.AspectTests.Callbacks.StaticOnChangedInstanceAcceptsObject.StaticOnChangedInstanceAcceptsObject.FooProperty = global::System.Windows.DependencyProperty.Register("Foo", typeof(global::System.Int32), typeof(global::Metalama.Patterns.Xaml.AspectTests.Callbacks.StaticOnChangedInstanceAcceptsObject.StaticOnChangedInstanceAcceptsObject), new global::System.Windows.PropertyMetadata((global::System.Windows.PropertyChangedCallback)PropertyChanged));
  }
}