using System.Windows;
namespace Metalama.Patterns.Xaml.AspectTests.Callbacks.StaticOnChangedNoParameters;
public partial class StaticOnChangedNoParameters : DependencyObject
{
  [DependencyProperty]
  public int Foo
  {
    get
    {
      return ((global::System.Int32)this.GetValue(global::Metalama.Patterns.Xaml.AspectTests.Callbacks.StaticOnChangedNoParameters.StaticOnChangedNoParameters.FooProperty));
    }
    set
    {
      this.SetValue(global::Metalama.Patterns.Xaml.AspectTests.Callbacks.StaticOnChangedNoParameters.StaticOnChangedNoParameters.FooProperty, value);
    }
  }
  private static void OnFooChanged()
  {
  }
  public static readonly global::System.Windows.DependencyProperty FooProperty;
  static StaticOnChangedNoParameters()
  {
    object CoerceValue_1(global::System.Windows.DependencyObject d, object value)
    {
      return (global::System.Object)value;
    }
    void PropertyChanged(global::System.Windows.DependencyObject d_1, global::System.Windows.DependencyPropertyChangedEventArgs e)
    {
      global::Metalama.Patterns.Xaml.AspectTests.Callbacks.StaticOnChangedNoParameters.StaticOnChangedNoParameters.OnFooChanged();
    }
    global::Metalama.Patterns.Xaml.AspectTests.Callbacks.StaticOnChangedNoParameters.StaticOnChangedNoParameters.FooProperty = global::System.Windows.DependencyProperty.Register("Foo", typeof(global::System.Int32), typeof(global::Metalama.Patterns.Xaml.AspectTests.Callbacks.StaticOnChangedNoParameters.StaticOnChangedNoParameters), new global::System.Windows.PropertyMetadata((global::System.Windows.PropertyChangedCallback)PropertyChanged));
  }
}