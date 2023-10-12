using System.Windows;
namespace Metalama.Patterns.Xaml.AspectTests.Callbacks.StaticOnChangedDependencyPropertyAndInstanceAcceptsObjectForInstance;
public partial class StaticOnChangedDependencyPropertyAndInstanceAcceptsObjectForInstance : DependencyObject
{
  [DependencyProperty]
  public int Foo
  {
    get
    {
      return ((global::System.Int32)this.GetValue(global::Metalama.Patterns.Xaml.AspectTests.Callbacks.StaticOnChangedDependencyPropertyAndInstanceAcceptsObjectForInstance.StaticOnChangedDependencyPropertyAndInstanceAcceptsObjectForInstance.FooProperty));
    }
    set
    {
      this.SetValue(global::Metalama.Patterns.Xaml.AspectTests.Callbacks.StaticOnChangedDependencyPropertyAndInstanceAcceptsObjectForInstance.StaticOnChangedDependencyPropertyAndInstanceAcceptsObjectForInstance.FooProperty, value);
    }
  }
  private static void OnFooChanged(DependencyProperty d, object instance)
  {
  }
  public static readonly global::System.Windows.DependencyProperty FooProperty;
  static StaticOnChangedDependencyPropertyAndInstanceAcceptsObjectForInstance()
  {
    object CoerceValue_1(global::System.Windows.DependencyObject d_1, object value)
    {
      return (global::System.Object)value;
    }
    void PropertyChanged(global::System.Windows.DependencyObject d_2, global::System.Windows.DependencyPropertyChangedEventArgs e)
    {
      global::Metalama.Patterns.Xaml.AspectTests.Callbacks.StaticOnChangedDependencyPropertyAndInstanceAcceptsObjectForInstance.StaticOnChangedDependencyPropertyAndInstanceAcceptsObjectForInstance.OnFooChanged(global::Metalama.Patterns.Xaml.AspectTests.Callbacks.StaticOnChangedDependencyPropertyAndInstanceAcceptsObjectForInstance.StaticOnChangedDependencyPropertyAndInstanceAcceptsObjectForInstance.FooProperty, (global::Metalama.Patterns.Xaml.AspectTests.Callbacks.StaticOnChangedDependencyPropertyAndInstanceAcceptsObjectForInstance.StaticOnChangedDependencyPropertyAndInstanceAcceptsObjectForInstance)d_2);
    }
    global::Metalama.Patterns.Xaml.AspectTests.Callbacks.StaticOnChangedDependencyPropertyAndInstanceAcceptsObjectForInstance.StaticOnChangedDependencyPropertyAndInstanceAcceptsObjectForInstance.FooProperty = global::System.Windows.DependencyProperty.Register("Foo", typeof(global::System.Int32), typeof(global::Metalama.Patterns.Xaml.AspectTests.Callbacks.StaticOnChangedDependencyPropertyAndInstanceAcceptsObjectForInstance.StaticOnChangedDependencyPropertyAndInstanceAcceptsObjectForInstance), new global::System.Windows.PropertyMetadata((global::System.Windows.PropertyChangedCallback)PropertyChanged));
  }
}