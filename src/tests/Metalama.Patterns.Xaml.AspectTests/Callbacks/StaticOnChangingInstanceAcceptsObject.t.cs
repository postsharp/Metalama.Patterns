using System.Windows;
namespace Metalama.Patterns.Xaml.AspectTests.Callbacks.StaticOnChangingInstanceAcceptsObject;
public partial class StaticOnChangingInstanceAcceptsObject : DependencyObject
{
  [DependencyProperty]
  public int Foo
  {
    get
    {
      return ((global::System.Int32)this.GetValue(global::Metalama.Patterns.Xaml.AspectTests.Callbacks.StaticOnChangingInstanceAcceptsObject.StaticOnChangingInstanceAcceptsObject.FooProperty));
    }
    set
    {
      this.SetValue(global::Metalama.Patterns.Xaml.AspectTests.Callbacks.StaticOnChangingInstanceAcceptsObject.StaticOnChangingInstanceAcceptsObject.FooProperty, value);
    }
  }
  private static void OnFooChanging(object instance)
  {
  }
  public static readonly global::System.Windows.DependencyProperty FooProperty;
  static StaticOnChangingInstanceAcceptsObject()
  {
    object CoerceValue_1(global::System.Windows.DependencyObject d, object value)
    {
      global::Metalama.Patterns.Xaml.AspectTests.Callbacks.StaticOnChangingInstanceAcceptsObject.StaticOnChangingInstanceAcceptsObject.OnFooChanging((global::Metalama.Patterns.Xaml.AspectTests.Callbacks.StaticOnChangingInstanceAcceptsObject.StaticOnChangingInstanceAcceptsObject)d);
      return (global::System.Object)value;
    }
    void PropertyChanged(global::System.Windows.DependencyObject d_1, global::System.Windows.DependencyPropertyChangedEventArgs e)
    {
    }
    var metadata = new global::System.Windows.PropertyMetadata();
    metadata.CoerceValueCallback = (global::System.Windows.CoerceValueCallback)CoerceValue_1;
    global::Metalama.Patterns.Xaml.AspectTests.Callbacks.StaticOnChangingInstanceAcceptsObject.StaticOnChangingInstanceAcceptsObject.FooProperty = global::System.Windows.DependencyProperty.Register("Foo", typeof(global::System.Int32), typeof(global::Metalama.Patterns.Xaml.AspectTests.Callbacks.StaticOnChangingInstanceAcceptsObject.StaticOnChangingInstanceAcceptsObject), metadata);
  }
}