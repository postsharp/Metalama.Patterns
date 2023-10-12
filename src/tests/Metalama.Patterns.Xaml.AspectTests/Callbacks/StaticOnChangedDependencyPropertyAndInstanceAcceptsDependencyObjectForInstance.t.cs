using System.Windows;
namespace Metalama.Patterns.Xaml.AspectTests.Callbacks.StaticOnChangedDependencyPropertyAndInstanceAcceptsDependencyObjectForInstance;
public partial class StaticOnChangedDependencyPropertyAndInstanceAcceptsDependencyObjectForInstance : DependencyObject
{
  [DependencyProperty]
  public int Foo
  {
    get
    {
      return ((global::System.Int32)this.GetValue(global::Metalama.Patterns.Xaml.AspectTests.Callbacks.StaticOnChangedDependencyPropertyAndInstanceAcceptsDependencyObjectForInstance.StaticOnChangedDependencyPropertyAndInstanceAcceptsDependencyObjectForInstance.FooProperty));
    }
    set
    {
      this.SetValue(global::Metalama.Patterns.Xaml.AspectTests.Callbacks.StaticOnChangedDependencyPropertyAndInstanceAcceptsDependencyObjectForInstance.StaticOnChangedDependencyPropertyAndInstanceAcceptsDependencyObjectForInstance.FooProperty, value);
    }
  }
  private static void OnFooChanged(DependencyProperty d, DependencyObject instance)
  {
  }
  public static readonly global::System.Windows.DependencyProperty FooProperty;
  static StaticOnChangedDependencyPropertyAndInstanceAcceptsDependencyObjectForInstance()
  {
    object CoerceValue_1(global::System.Windows.DependencyObject d_1, object value)
    {
      return (global::System.Object)value;
    }
    void PropertyChanged(global::System.Windows.DependencyObject d_2, global::System.Windows.DependencyPropertyChangedEventArgs e)
    {
      global::Metalama.Patterns.Xaml.AspectTests.Callbacks.StaticOnChangedDependencyPropertyAndInstanceAcceptsDependencyObjectForInstance.StaticOnChangedDependencyPropertyAndInstanceAcceptsDependencyObjectForInstance.OnFooChanged(global::Metalama.Patterns.Xaml.AspectTests.Callbacks.StaticOnChangedDependencyPropertyAndInstanceAcceptsDependencyObjectForInstance.StaticOnChangedDependencyPropertyAndInstanceAcceptsDependencyObjectForInstance.FooProperty, (global::Metalama.Patterns.Xaml.AspectTests.Callbacks.StaticOnChangedDependencyPropertyAndInstanceAcceptsDependencyObjectForInstance.StaticOnChangedDependencyPropertyAndInstanceAcceptsDependencyObjectForInstance)d_2);
    }
    global::Metalama.Patterns.Xaml.AspectTests.Callbacks.StaticOnChangedDependencyPropertyAndInstanceAcceptsDependencyObjectForInstance.StaticOnChangedDependencyPropertyAndInstanceAcceptsDependencyObjectForInstance.FooProperty = global::System.Windows.DependencyProperty.Register("Foo", typeof(global::System.Int32), typeof(global::Metalama.Patterns.Xaml.AspectTests.Callbacks.StaticOnChangedDependencyPropertyAndInstanceAcceptsDependencyObjectForInstance.StaticOnChangedDependencyPropertyAndInstanceAcceptsDependencyObjectForInstance), new global::System.Windows.PropertyMetadata((global::System.Windows.PropertyChangedCallback)PropertyChanged));
  }
}