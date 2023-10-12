using System.Windows;
namespace Metalama.Patterns.Xaml.AspectTests.Callbacks.StaticOnChangedInstanceAcceptsDependencyObject;
public partial class StaticOnChangedInstanceAcceptsDependencyObject : DependencyObject
{
  [DependencyProperty]
  public int Foo
  {
    get
    {
      return ((global::System.Int32)this.GetValue(global::Metalama.Patterns.Xaml.AspectTests.Callbacks.StaticOnChangedInstanceAcceptsDependencyObject.StaticOnChangedInstanceAcceptsDependencyObject.FooProperty));
    }
    set
    {
      this.SetValue(global::Metalama.Patterns.Xaml.AspectTests.Callbacks.StaticOnChangedInstanceAcceptsDependencyObject.StaticOnChangedInstanceAcceptsDependencyObject.FooProperty, value);
    }
  }
  private static void OnFooChanged(DependencyObject instance)
  {
  }
  public static readonly global::System.Windows.DependencyProperty FooProperty;
  static StaticOnChangedInstanceAcceptsDependencyObject()
  {
    object CoerceValue_1(global::System.Windows.DependencyObject d, object value)
    {
      return (global::System.Object)value;
    }
    void PropertyChanged(global::System.Windows.DependencyObject d_1, global::System.Windows.DependencyPropertyChangedEventArgs e)
    {
      global::Metalama.Patterns.Xaml.AspectTests.Callbacks.StaticOnChangedInstanceAcceptsDependencyObject.StaticOnChangedInstanceAcceptsDependencyObject.OnFooChanged((global::Metalama.Patterns.Xaml.AspectTests.Callbacks.StaticOnChangedInstanceAcceptsDependencyObject.StaticOnChangedInstanceAcceptsDependencyObject)d_1);
    }
    global::Metalama.Patterns.Xaml.AspectTests.Callbacks.StaticOnChangedInstanceAcceptsDependencyObject.StaticOnChangedInstanceAcceptsDependencyObject.FooProperty = global::System.Windows.DependencyProperty.Register("Foo", typeof(global::System.Int32), typeof(global::Metalama.Patterns.Xaml.AspectTests.Callbacks.StaticOnChangedInstanceAcceptsDependencyObject.StaticOnChangedInstanceAcceptsDependencyObject), new global::System.Windows.PropertyMetadata((global::System.Windows.PropertyChangedCallback)PropertyChanged));
  }
}