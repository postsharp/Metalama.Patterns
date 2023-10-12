using System.Windows;
namespace Metalama.Patterns.Xaml.AspectTests.Callbacks.StaticOnChangedDependencyProperty;
public partial class StaticOnChangedDependencyProperty : DependencyObject
{
  [DependencyProperty]
  public int Foo
  {
    get
    {
      return ((global::System.Int32)this.GetValue(global::Metalama.Patterns.Xaml.AspectTests.Callbacks.StaticOnChangedDependencyProperty.StaticOnChangedDependencyProperty.FooProperty));
    }
    set
    {
      this.SetValue(global::Metalama.Patterns.Xaml.AspectTests.Callbacks.StaticOnChangedDependencyProperty.StaticOnChangedDependencyProperty.FooProperty, value);
    }
  }
  private static void OnFooChanged(DependencyProperty d)
  {
  }
  public static readonly global::System.Windows.DependencyProperty FooProperty;
  static StaticOnChangedDependencyProperty()
  {
    object CoerceValue_1(global::System.Windows.DependencyObject d_1, object value)
    {
      return (global::System.Object)value;
    }
    void PropertyChanged(global::System.Windows.DependencyObject d_2, global::System.Windows.DependencyPropertyChangedEventArgs e)
    {
      global::Metalama.Patterns.Xaml.AspectTests.Callbacks.StaticOnChangedDependencyProperty.StaticOnChangedDependencyProperty.OnFooChanged(global::Metalama.Patterns.Xaml.AspectTests.Callbacks.StaticOnChangedDependencyProperty.StaticOnChangedDependencyProperty.FooProperty);
    }
    global::Metalama.Patterns.Xaml.AspectTests.Callbacks.StaticOnChangedDependencyProperty.StaticOnChangedDependencyProperty.FooProperty = global::System.Windows.DependencyProperty.Register("Foo", typeof(global::System.Int32), typeof(global::Metalama.Patterns.Xaml.AspectTests.Callbacks.StaticOnChangedDependencyProperty.StaticOnChangedDependencyProperty), new global::System.Windows.PropertyMetadata((global::System.Windows.PropertyChangedCallback)PropertyChanged));
  }
}