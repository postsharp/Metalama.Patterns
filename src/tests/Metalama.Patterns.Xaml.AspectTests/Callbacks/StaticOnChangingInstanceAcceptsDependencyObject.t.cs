using System.Windows;
namespace Metalama.Patterns.Xaml.AspectTests.Callbacks.StaticOnChangingInstanceAcceptsDependencyObject;
public partial class StaticOnChangingInstanceAcceptsDependencyObject : DependencyObject
{
  [DependencyProperty]
  public int Foo
  {
    get
    {
      return ((global::System.Int32)this.GetValue(global::Metalama.Patterns.Xaml.AspectTests.Callbacks.StaticOnChangingInstanceAcceptsDependencyObject.StaticOnChangingInstanceAcceptsDependencyObject.FooProperty));
    }
    set
    {
      this.SetValue(global::Metalama.Patterns.Xaml.AspectTests.Callbacks.StaticOnChangingInstanceAcceptsDependencyObject.StaticOnChangingInstanceAcceptsDependencyObject.FooProperty, value);
    }
  }
  private static void OnFooChanging(DependencyObject instance)
  {
  }
  public static readonly global::System.Windows.DependencyProperty FooProperty;
  static StaticOnChangingInstanceAcceptsDependencyObject()
  {
    object CoerceValue_1(global::System.Windows.DependencyObject d, object value)
    {
      global::Metalama.Patterns.Xaml.AspectTests.Callbacks.StaticOnChangingInstanceAcceptsDependencyObject.StaticOnChangingInstanceAcceptsDependencyObject.OnFooChanging((global::Metalama.Patterns.Xaml.AspectTests.Callbacks.StaticOnChangingInstanceAcceptsDependencyObject.StaticOnChangingInstanceAcceptsDependencyObject)d);
      return (global::System.Object)value;
    }
    var metadata = new global::System.Windows.PropertyMetadata();
    metadata.CoerceValueCallback = (global::System.Windows.CoerceValueCallback)CoerceValue_1;
    global::Metalama.Patterns.Xaml.AspectTests.Callbacks.StaticOnChangingInstanceAcceptsDependencyObject.StaticOnChangingInstanceAcceptsDependencyObject.FooProperty = global::System.Windows.DependencyProperty.Register("Foo", typeof(global::System.Int32), typeof(global::Metalama.Patterns.Xaml.AspectTests.Callbacks.StaticOnChangingInstanceAcceptsDependencyObject.StaticOnChangingInstanceAcceptsDependencyObject), metadata);
  }
}