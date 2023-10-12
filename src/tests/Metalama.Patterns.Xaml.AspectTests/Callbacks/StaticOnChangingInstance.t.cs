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
  public static readonly global::System.Windows.DependencyProperty FooProperty;
  static StaticOnChangingInstance()
  {
    object CoerceValue_1(global::System.Windows.DependencyObject d, object value)
    {
      global::Metalama.Patterns.Xaml.AspectTests.Callbacks.StaticOnChangingInstance.StaticOnChangingInstance.OnFooChanging((global::Metalama.Patterns.Xaml.AspectTests.Callbacks.StaticOnChangingInstance.StaticOnChangingInstance)d);
      return (global::System.Object)value;
    }
    void PropertyChanged(global::System.Windows.DependencyObject d_1, global::System.Windows.DependencyPropertyChangedEventArgs e)
    {
    }
    var metadata = new global::System.Windows.PropertyMetadata();
    metadata.CoerceValueCallback = (global::System.Windows.CoerceValueCallback)CoerceValue_1;
    global::Metalama.Patterns.Xaml.AspectTests.Callbacks.StaticOnChangingInstance.StaticOnChangingInstance.FooProperty = global::System.Windows.DependencyProperty.Register("Foo", typeof(global::System.Int32), typeof(global::Metalama.Patterns.Xaml.AspectTests.Callbacks.StaticOnChangingInstance.StaticOnChangingInstance), metadata);
  }
}