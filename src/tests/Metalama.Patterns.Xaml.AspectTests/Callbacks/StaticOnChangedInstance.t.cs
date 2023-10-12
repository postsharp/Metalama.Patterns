using System.Windows;
namespace Metalama.Patterns.Xaml.AspectTests.Callbacks.StaticOnChangedInstance;
public partial class StaticOnChangedInstance : DependencyObject
{
  [DependencyProperty]
  public int Foo
  {
    get
    {
      return ((global::System.Int32)this.GetValue(global::Metalama.Patterns.Xaml.AspectTests.Callbacks.StaticOnChangedInstance.StaticOnChangedInstance.FooProperty));
    }
    set
    {
      this.SetValue(global::Metalama.Patterns.Xaml.AspectTests.Callbacks.StaticOnChangedInstance.StaticOnChangedInstance.FooProperty, value);
    }
  }
  private static void OnFooChanged(StaticOnChangedInstance instance)
  {
  }
  public static readonly global::System.Windows.DependencyProperty FooProperty;
  static StaticOnChangedInstance()
  {
    object CoerceValue_1(global::System.Windows.DependencyObject d, object value)
    {
      return (global::System.Object)value;
    }
    void PropertyChanged(global::System.Windows.DependencyObject d_1, global::System.Windows.DependencyPropertyChangedEventArgs e)
    {
      global::Metalama.Patterns.Xaml.AspectTests.Callbacks.StaticOnChangedInstance.StaticOnChangedInstance.OnFooChanged((global::Metalama.Patterns.Xaml.AspectTests.Callbacks.StaticOnChangedInstance.StaticOnChangedInstance)d_1);
    }
    global::Metalama.Patterns.Xaml.AspectTests.Callbacks.StaticOnChangedInstance.StaticOnChangedInstance.FooProperty = global::System.Windows.DependencyProperty.Register("Foo", typeof(global::System.Int32), typeof(global::Metalama.Patterns.Xaml.AspectTests.Callbacks.StaticOnChangedInstance.StaticOnChangedInstance), new global::System.Windows.PropertyMetadata((global::System.Windows.PropertyChangedCallback)PropertyChanged));
  }
}