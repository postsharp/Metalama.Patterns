using System.Windows;
namespace Metalama.Patterns.Xaml.AspectTests.Callbacks.InstanceOnChangingDependencyProperty;
public partial class InstanceOnChangingDependencyProperty : DependencyObject
{
  [DependencyProperty]
  public int Foo
  {
    get
    {
      return ((global::System.Int32)this.GetValue(global::Metalama.Patterns.Xaml.AspectTests.Callbacks.InstanceOnChangingDependencyProperty.InstanceOnChangingDependencyProperty.FooProperty));
    }
    set
    {
      this.SetValue(global::Metalama.Patterns.Xaml.AspectTests.Callbacks.InstanceOnChangingDependencyProperty.InstanceOnChangingDependencyProperty.FooProperty, value);
    }
  }
  private void OnFooChanging(DependencyProperty d)
  {
  }
  public static readonly global::System.Windows.DependencyProperty FooProperty;
  static InstanceOnChangingDependencyProperty()
  {
    object CoerceValue_1(global::System.Windows.DependencyObject d_1, object value)
    {
      ((global::Metalama.Patterns.Xaml.AspectTests.Callbacks.InstanceOnChangingDependencyProperty.InstanceOnChangingDependencyProperty)d_1).OnFooChanging(global::Metalama.Patterns.Xaml.AspectTests.Callbacks.InstanceOnChangingDependencyProperty.InstanceOnChangingDependencyProperty.FooProperty);
      return (global::System.Object)value;
    }
    var metadata = new global::System.Windows.PropertyMetadata();
    metadata.CoerceValueCallback = (global::System.Windows.CoerceValueCallback)CoerceValue_1;
    global::Metalama.Patterns.Xaml.AspectTests.Callbacks.InstanceOnChangingDependencyProperty.InstanceOnChangingDependencyProperty.FooProperty = global::System.Windows.DependencyProperty.Register("Foo", typeof(global::System.Int32), typeof(global::Metalama.Patterns.Xaml.AspectTests.Callbacks.InstanceOnChangingDependencyProperty.InstanceOnChangingDependencyProperty), metadata);
  }
}