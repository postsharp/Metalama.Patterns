using System.Windows;
namespace Metalama.Patterns.Xaml.AspectTests.Callbacks.StaticOnChangingDependencyPropertyAndInstance;
public partial class StaticOnChangingDependencyPropertyAndInstance : DependencyObject
{
  [DependencyProperty]
  public int Foo
  {
    get
    {
      return ((global::System.Int32)this.GetValue(global::Metalama.Patterns.Xaml.AspectTests.Callbacks.StaticOnChangingDependencyPropertyAndInstance.StaticOnChangingDependencyPropertyAndInstance.FooProperty));
    }
    set
    {
      this.SetValue(global::Metalama.Patterns.Xaml.AspectTests.Callbacks.StaticOnChangingDependencyPropertyAndInstance.StaticOnChangingDependencyPropertyAndInstance.FooProperty, value);
    }
  }
  private static void OnFooChanging(DependencyProperty d, StaticOnChangingDependencyPropertyAndInstance instance)
  {
  }
  [DependencyProperty]
  public int AcceptsDependencyObjectForInstance
  {
    get
    {
      return ((global::System.Int32)this.GetValue(global::Metalama.Patterns.Xaml.AspectTests.Callbacks.StaticOnChangingDependencyPropertyAndInstance.StaticOnChangingDependencyPropertyAndInstance.AcceptsDependencyObjectForInstanceProperty));
    }
    set
    {
      this.SetValue(global::Metalama.Patterns.Xaml.AspectTests.Callbacks.StaticOnChangingDependencyPropertyAndInstance.StaticOnChangingDependencyPropertyAndInstance.AcceptsDependencyObjectForInstanceProperty, value);
    }
  }
  private static void OnAcceptsDependencyObjectForInstanceChanging(DependencyProperty d, DependencyObject instance)
  {
  }
  [DependencyProperty]
  public int AcceptsObjectForInstance
  {
    get
    {
      return ((global::System.Int32)this.GetValue(global::Metalama.Patterns.Xaml.AspectTests.Callbacks.StaticOnChangingDependencyPropertyAndInstance.StaticOnChangingDependencyPropertyAndInstance.AcceptsObjectForInstanceProperty));
    }
    set
    {
      this.SetValue(global::Metalama.Patterns.Xaml.AspectTests.Callbacks.StaticOnChangingDependencyPropertyAndInstance.StaticOnChangingDependencyPropertyAndInstance.AcceptsObjectForInstanceProperty, value);
    }
  }
  private static void OnAcceptsObjectForInstanceChanging(DependencyProperty d, object instance)
  {
  }
  public static readonly global::System.Windows.DependencyProperty AcceptsDependencyObjectForInstanceProperty;
  public static readonly global::System.Windows.DependencyProperty AcceptsObjectForInstanceProperty;
  public static readonly global::System.Windows.DependencyProperty FooProperty;
  static StaticOnChangingDependencyPropertyAndInstance()
  {
    object CoerceValue_3(global::System.Windows.DependencyObject d_3, object value_2)
    {
      global::Metalama.Patterns.Xaml.AspectTests.Callbacks.StaticOnChangingDependencyPropertyAndInstance.StaticOnChangingDependencyPropertyAndInstance.OnAcceptsObjectForInstanceChanging(global::Metalama.Patterns.Xaml.AspectTests.Callbacks.StaticOnChangingDependencyPropertyAndInstance.StaticOnChangingDependencyPropertyAndInstance.AcceptsObjectForInstanceProperty, d_3);
      return (global::System.Object)value_2;
    }
    var metadata_2 = new global::System.Windows.PropertyMetadata();
    metadata_2.CoerceValueCallback = (global::System.Windows.CoerceValueCallback)CoerceValue_3;
    global::Metalama.Patterns.Xaml.AspectTests.Callbacks.StaticOnChangingDependencyPropertyAndInstance.StaticOnChangingDependencyPropertyAndInstance.AcceptsObjectForInstanceProperty = global::System.Windows.DependencyProperty.Register("AcceptsObjectForInstance", typeof(global::System.Int32), typeof(global::Metalama.Patterns.Xaml.AspectTests.Callbacks.StaticOnChangingDependencyPropertyAndInstance.StaticOnChangingDependencyPropertyAndInstance), metadata_2);
    object CoerceValue_2(global::System.Windows.DependencyObject d_2, object value_1)
    {
      global::Metalama.Patterns.Xaml.AspectTests.Callbacks.StaticOnChangingDependencyPropertyAndInstance.StaticOnChangingDependencyPropertyAndInstance.OnAcceptsDependencyObjectForInstanceChanging(global::Metalama.Patterns.Xaml.AspectTests.Callbacks.StaticOnChangingDependencyPropertyAndInstance.StaticOnChangingDependencyPropertyAndInstance.AcceptsDependencyObjectForInstanceProperty, d_2);
      return (global::System.Object)value_1;
    }
    var metadata_1 = new global::System.Windows.PropertyMetadata();
    metadata_1.CoerceValueCallback = (global::System.Windows.CoerceValueCallback)CoerceValue_2;
    global::Metalama.Patterns.Xaml.AspectTests.Callbacks.StaticOnChangingDependencyPropertyAndInstance.StaticOnChangingDependencyPropertyAndInstance.AcceptsDependencyObjectForInstanceProperty = global::System.Windows.DependencyProperty.Register("AcceptsDependencyObjectForInstance", typeof(global::System.Int32), typeof(global::Metalama.Patterns.Xaml.AspectTests.Callbacks.StaticOnChangingDependencyPropertyAndInstance.StaticOnChangingDependencyPropertyAndInstance), metadata_1);
    object CoerceValue_1(global::System.Windows.DependencyObject d_1, object value)
    {
      global::Metalama.Patterns.Xaml.AspectTests.Callbacks.StaticOnChangingDependencyPropertyAndInstance.StaticOnChangingDependencyPropertyAndInstance.OnFooChanging(global::Metalama.Patterns.Xaml.AspectTests.Callbacks.StaticOnChangingDependencyPropertyAndInstance.StaticOnChangingDependencyPropertyAndInstance.FooProperty, (global::Metalama.Patterns.Xaml.AspectTests.Callbacks.StaticOnChangingDependencyPropertyAndInstance.StaticOnChangingDependencyPropertyAndInstance)d_1);
      return (global::System.Object)value;
    }
    var metadata = new global::System.Windows.PropertyMetadata();
    metadata.CoerceValueCallback = (global::System.Windows.CoerceValueCallback)CoerceValue_1;
    global::Metalama.Patterns.Xaml.AspectTests.Callbacks.StaticOnChangingDependencyPropertyAndInstance.StaticOnChangingDependencyPropertyAndInstance.FooProperty = global::System.Windows.DependencyProperty.Register("Foo", typeof(global::System.Int32), typeof(global::Metalama.Patterns.Xaml.AspectTests.Callbacks.StaticOnChangingDependencyPropertyAndInstance.StaticOnChangingDependencyPropertyAndInstance), metadata);
  }
}