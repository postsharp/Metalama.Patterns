using System.Windows;
namespace Metalama.Patterns.Xaml.AspectTests.Callbacks.StaticOnChangingDependencyPropertyAndInstanceAcceptsObjectForInstance;
public partial class StaticOnChangingDependencyPropertyAndInstanceAcceptsObjectForInstance : DependencyObject
{
  [DependencyProperty]
  public int Foo
  {
    get
    {
      return ((global::System.Int32)this.GetValue(global::Metalama.Patterns.Xaml.AspectTests.Callbacks.StaticOnChangingDependencyPropertyAndInstanceAcceptsObjectForInstance.StaticOnChangingDependencyPropertyAndInstanceAcceptsObjectForInstance.FooProperty));
    }
    set
    {
      this.SetValue(global::Metalama.Patterns.Xaml.AspectTests.Callbacks.StaticOnChangingDependencyPropertyAndInstanceAcceptsObjectForInstance.StaticOnChangingDependencyPropertyAndInstanceAcceptsObjectForInstance.FooProperty, value);
    }
  }
  private static void OnFooChanging(DependencyProperty d, object instance)
  {
  }
  public static readonly global::System.Windows.DependencyProperty FooProperty;
  static StaticOnChangingDependencyPropertyAndInstanceAcceptsObjectForInstance()
  {
    object CoerceValue_1(global::System.Windows.DependencyObject d_1, object value)
    {
      global::Metalama.Patterns.Xaml.AspectTests.Callbacks.StaticOnChangingDependencyPropertyAndInstanceAcceptsObjectForInstance.StaticOnChangingDependencyPropertyAndInstanceAcceptsObjectForInstance.OnFooChanging(global::Metalama.Patterns.Xaml.AspectTests.Callbacks.StaticOnChangingDependencyPropertyAndInstanceAcceptsObjectForInstance.StaticOnChangingDependencyPropertyAndInstanceAcceptsObjectForInstance.FooProperty, (global::Metalama.Patterns.Xaml.AspectTests.Callbacks.StaticOnChangingDependencyPropertyAndInstanceAcceptsObjectForInstance.StaticOnChangingDependencyPropertyAndInstanceAcceptsObjectForInstance)d_1);
      return (global::System.Object)value;
    }
    var metadata = new global::System.Windows.PropertyMetadata();
    metadata.CoerceValueCallback = (global::System.Windows.CoerceValueCallback)CoerceValue_1;
    global::Metalama.Patterns.Xaml.AspectTests.Callbacks.StaticOnChangingDependencyPropertyAndInstanceAcceptsObjectForInstance.StaticOnChangingDependencyPropertyAndInstanceAcceptsObjectForInstance.FooProperty = global::System.Windows.DependencyProperty.Register("Foo", typeof(global::System.Int32), typeof(global::Metalama.Patterns.Xaml.AspectTests.Callbacks.StaticOnChangingDependencyPropertyAndInstanceAcceptsObjectForInstance.StaticOnChangingDependencyPropertyAndInstanceAcceptsObjectForInstance), metadata);
  }
}