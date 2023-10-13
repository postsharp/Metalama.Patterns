using System.Windows;
namespace Metalama.Patterns.Xaml.AspectTests.Callbacks.StaticOnChangingDependencyProperty;
public partial class StaticOnChangingDependencyProperty : DependencyObject
{
  [DependencyProperty]
  public int Foo
  {
    get
    {
      return ((global::System.Int32)this.GetValue(global::Metalama.Patterns.Xaml.AspectTests.Callbacks.StaticOnChangingDependencyProperty.StaticOnChangingDependencyProperty.FooProperty));
    }
    set
    {
      this.SetValue(global::Metalama.Patterns.Xaml.AspectTests.Callbacks.StaticOnChangingDependencyProperty.StaticOnChangingDependencyProperty.FooProperty, value);
    }
  }
  private static void OnFooChanging(DependencyProperty d)
  {
  }
  public static readonly global::System.Windows.DependencyProperty FooProperty;
  static StaticOnChangingDependencyProperty()
  {
    object CoerceValue_1(global::System.Windows.DependencyObject d_1, object value)
    {
      global::Metalama.Patterns.Xaml.AspectTests.Callbacks.StaticOnChangingDependencyProperty.StaticOnChangingDependencyProperty.OnFooChanging(global::Metalama.Patterns.Xaml.AspectTests.Callbacks.StaticOnChangingDependencyProperty.StaticOnChangingDependencyProperty.FooProperty);
      return (global::System.Object)value;
    }
    var metadata = new global::System.Windows.PropertyMetadata();
    metadata.CoerceValueCallback = (global::System.Windows.CoerceValueCallback)CoerceValue_1;
    global::Metalama.Patterns.Xaml.AspectTests.Callbacks.StaticOnChangingDependencyProperty.StaticOnChangingDependencyProperty.FooProperty = global::System.Windows.DependencyProperty.Register("Foo", typeof(global::System.Int32), typeof(global::Metalama.Patterns.Xaml.AspectTests.Callbacks.StaticOnChangingDependencyProperty.StaticOnChangingDependencyProperty), metadata);
  }
}