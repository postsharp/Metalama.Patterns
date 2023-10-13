using System.Windows;
namespace Metalama.Patterns.Xaml.AspectTests.Callbacks.StaticOnChangingInstance;
public partial class StaticOnChangingInstance : DependencyObject
{
  [DependencyProperty]
  public int Foo
  {
    get
    {
      return ((global::System.Int32)this.GetValue(global::Metalama.Patterns.Xaml.AspectTests.Callbacks.StaticOnChangingInstance.StaticOnChangingInstance.FooProperty));
    }
    set
    {
      this.SetValue(global::Metalama.Patterns.Xaml.AspectTests.Callbacks.StaticOnChangingInstance.StaticOnChangingInstance.FooProperty, value);
    }
  }
  private static void OnFooChanging(StaticOnChangingInstance instance)
  {
  }
  [DependencyProperty]
  public int AcceptsDependencyObject
  {
    get
    {
      return ((global::System.Int32)this.GetValue(global::Metalama.Patterns.Xaml.AspectTests.Callbacks.StaticOnChangingInstance.StaticOnChangingInstance.AcceptsDependencyObjectProperty));
    }
    set
    {
      this.SetValue(global::Metalama.Patterns.Xaml.AspectTests.Callbacks.StaticOnChangingInstance.StaticOnChangingInstance.AcceptsDependencyObjectProperty, value);
    }
  }
  private static void OnAcceptsDependencyObjectChanging(DependencyObject instance)
  {
  }
  [DependencyProperty]
  public int AcceptsObject
  {
    get
    {
      return ((global::System.Int32)this.GetValue(global::Metalama.Patterns.Xaml.AspectTests.Callbacks.StaticOnChangingInstance.StaticOnChangingInstance.AcceptsObjectProperty));
    }
    set
    {
      this.SetValue(global::Metalama.Patterns.Xaml.AspectTests.Callbacks.StaticOnChangingInstance.StaticOnChangingInstance.AcceptsObjectProperty, value);
    }
  }
  private static void OnAcceptsObjectChanging(object instance)
  {
  }
  public static readonly global::System.Windows.DependencyProperty AcceptsDependencyObjectProperty;
  public static readonly global::System.Windows.DependencyProperty AcceptsObjectProperty;
  public static readonly global::System.Windows.DependencyProperty FooProperty;
  static StaticOnChangingInstance()
  {
    object CoerceValue_3(global::System.Windows.DependencyObject d_2, object value_2)
    {
      global::Metalama.Patterns.Xaml.AspectTests.Callbacks.StaticOnChangingInstance.StaticOnChangingInstance.OnAcceptsObjectChanging(d_2);
      return (global::System.Object)value_2;
    }
    var metadata_2 = new global::System.Windows.PropertyMetadata();
    metadata_2.CoerceValueCallback = (global::System.Windows.CoerceValueCallback)CoerceValue_3;
    global::Metalama.Patterns.Xaml.AspectTests.Callbacks.StaticOnChangingInstance.StaticOnChangingInstance.AcceptsObjectProperty = global::System.Windows.DependencyProperty.Register("AcceptsObject", typeof(global::System.Int32), typeof(global::Metalama.Patterns.Xaml.AspectTests.Callbacks.StaticOnChangingInstance.StaticOnChangingInstance), metadata_2);
    object CoerceValue_2(global::System.Windows.DependencyObject d_1, object value_1)
    {
      global::Metalama.Patterns.Xaml.AspectTests.Callbacks.StaticOnChangingInstance.StaticOnChangingInstance.OnAcceptsDependencyObjectChanging(d_1);
      return (global::System.Object)value_1;
    }
    var metadata_1 = new global::System.Windows.PropertyMetadata();
    metadata_1.CoerceValueCallback = (global::System.Windows.CoerceValueCallback)CoerceValue_2;
    global::Metalama.Patterns.Xaml.AspectTests.Callbacks.StaticOnChangingInstance.StaticOnChangingInstance.AcceptsDependencyObjectProperty = global::System.Windows.DependencyProperty.Register("AcceptsDependencyObject", typeof(global::System.Int32), typeof(global::Metalama.Patterns.Xaml.AspectTests.Callbacks.StaticOnChangingInstance.StaticOnChangingInstance), metadata_1);
    object CoerceValue_1(global::System.Windows.DependencyObject d, object value)
    {
      global::Metalama.Patterns.Xaml.AspectTests.Callbacks.StaticOnChangingInstance.StaticOnChangingInstance.OnFooChanging((global::Metalama.Patterns.Xaml.AspectTests.Callbacks.StaticOnChangingInstance.StaticOnChangingInstance)d);
      return (global::System.Object)value;
    }
    var metadata = new global::System.Windows.PropertyMetadata();
    metadata.CoerceValueCallback = (global::System.Windows.CoerceValueCallback)CoerceValue_1;
    global::Metalama.Patterns.Xaml.AspectTests.Callbacks.StaticOnChangingInstance.StaticOnChangingInstance.FooProperty = global::System.Windows.DependencyProperty.Register("Foo", typeof(global::System.Int32), typeof(global::Metalama.Patterns.Xaml.AspectTests.Callbacks.StaticOnChangingInstance.StaticOnChangingInstance), metadata);
  }
}