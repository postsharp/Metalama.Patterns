using System.Windows;
namespace Metalama.Patterns.Xaml.AspectTests.Callbacks.StaticOnChangingDependencyPropertyAndInstanceAcceptsDependencyObjectForInstance;
public partial class StaticOnChangingDependencyPropertyAndInstanceAcceptsDependencyObjectForInstance : DependencyObject
{
  [DependencyProperty]
  public int Foo
  {
    get
    {
      return ((global::System.Int32)this.GetValue(global::Metalama.Patterns.Xaml.AspectTests.Callbacks.StaticOnChangingDependencyPropertyAndInstanceAcceptsDependencyObjectForInstance.StaticOnChangingDependencyPropertyAndInstanceAcceptsDependencyObjectForInstance.FooProperty));
    }
    set
    {
      this.SetValue(global::Metalama.Patterns.Xaml.AspectTests.Callbacks.StaticOnChangingDependencyPropertyAndInstanceAcceptsDependencyObjectForInstance.StaticOnChangingDependencyPropertyAndInstanceAcceptsDependencyObjectForInstance.FooProperty, value);
    }
  }
  private static void OnFooChanging(DependencyProperty d, DependencyObject instance)
  {
  }
  public static readonly global::System.Windows.DependencyProperty FooProperty;
  static StaticOnChangingDependencyPropertyAndInstanceAcceptsDependencyObjectForInstance()
  {
    object CoerceValue_1(global::System.Windows.DependencyObject d_1, object value)
    {
      global::Metalama.Patterns.Xaml.AspectTests.Callbacks.StaticOnChangingDependencyPropertyAndInstanceAcceptsDependencyObjectForInstance.StaticOnChangingDependencyPropertyAndInstanceAcceptsDependencyObjectForInstance.OnFooChanging(global::Metalama.Patterns.Xaml.AspectTests.Callbacks.StaticOnChangingDependencyPropertyAndInstanceAcceptsDependencyObjectForInstance.StaticOnChangingDependencyPropertyAndInstanceAcceptsDependencyObjectForInstance.FooProperty, (global::Metalama.Patterns.Xaml.AspectTests.Callbacks.StaticOnChangingDependencyPropertyAndInstanceAcceptsDependencyObjectForInstance.StaticOnChangingDependencyPropertyAndInstanceAcceptsDependencyObjectForInstance)d_1);
      return (global::System.Object)value;
    }
    var metadata = new global::System.Windows.PropertyMetadata();
    metadata.CoerceValueCallback = (global::System.Windows.CoerceValueCallback)CoerceValue_1;
    global::Metalama.Patterns.Xaml.AspectTests.Callbacks.StaticOnChangingDependencyPropertyAndInstanceAcceptsDependencyObjectForInstance.StaticOnChangingDependencyPropertyAndInstanceAcceptsDependencyObjectForInstance.FooProperty = global::System.Windows.DependencyProperty.Register("Foo", typeof(global::System.Int32), typeof(global::Metalama.Patterns.Xaml.AspectTests.Callbacks.StaticOnChangingDependencyPropertyAndInstanceAcceptsDependencyObjectForInstance.StaticOnChangingDependencyPropertyAndInstanceAcceptsDependencyObjectForInstance), metadata);
  }
}