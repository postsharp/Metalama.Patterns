using System.Windows;
namespace Metalama.Patterns.Xaml.AspectTests.Callbacks.StaticOnChangedDependencyPropertyAndInstance;
public partial class StaticOnChangedDependencyPropertyAndInstance : DependencyObject
{
  [DependencyProperty]
  public int Foo
  {
    get
    {
      return ((global::System.Int32)this.GetValue(global::Metalama.Patterns.Xaml.AspectTests.Callbacks.StaticOnChangedDependencyPropertyAndInstance.StaticOnChangedDependencyPropertyAndInstance.FooProperty));
    }
    set
    {
      this.SetValue(global::Metalama.Patterns.Xaml.AspectTests.Callbacks.StaticOnChangedDependencyPropertyAndInstance.StaticOnChangedDependencyPropertyAndInstance.FooProperty, value);
    }
  }
  private static void OnFooChanged(DependencyProperty d, StaticOnChangedDependencyPropertyAndInstance instance)
  {
  }
  public static readonly global::System.Windows.DependencyProperty FooProperty;
  static StaticOnChangedDependencyPropertyAndInstance()
  {
    object CoerceValue_1(global::System.Windows.DependencyObject d_1, object value)
    {
      return (global::System.Object)value;
    }
    void PropertyChanged(global::System.Windows.DependencyObject d_2, global::System.Windows.DependencyPropertyChangedEventArgs e)
    {
      global::Metalama.Patterns.Xaml.AspectTests.Callbacks.StaticOnChangedDependencyPropertyAndInstance.StaticOnChangedDependencyPropertyAndInstance.OnFooChanged(global::Metalama.Patterns.Xaml.AspectTests.Callbacks.StaticOnChangedDependencyPropertyAndInstance.StaticOnChangedDependencyPropertyAndInstance.FooProperty, (global::Metalama.Patterns.Xaml.AspectTests.Callbacks.StaticOnChangedDependencyPropertyAndInstance.StaticOnChangedDependencyPropertyAndInstance)d_2);
    }
    global::Metalama.Patterns.Xaml.AspectTests.Callbacks.StaticOnChangedDependencyPropertyAndInstance.StaticOnChangedDependencyPropertyAndInstance.FooProperty = global::System.Windows.DependencyProperty.Register("Foo", typeof(global::System.Int32), typeof(global::Metalama.Patterns.Xaml.AspectTests.Callbacks.StaticOnChangedDependencyPropertyAndInstance.StaticOnChangedDependencyPropertyAndInstance), new global::System.Windows.PropertyMetadata((global::System.Windows.PropertyChangedCallback)PropertyChanged));
  }
}