using System.Windows;
namespace Metalama.Patterns.Xaml.AspectTests.Callbacks.InstanceOnChangingValueAcceptsAssignable;
public partial class InstanceOnChangingValueAcceptsAssignable : DependencyObject
{
  [DependencyProperty]
  public List<int> Foo
  {
    get
    {
      return ((global::System.Collections.Generic.List<global::System.Int32>)this.GetValue(global::Metalama.Patterns.Xaml.AspectTests.Callbacks.InstanceOnChangingValueAcceptsAssignable.InstanceOnChangingValueAcceptsAssignable.FooProperty));
    }
    set
    {
      this.SetValue(global::Metalama.Patterns.Xaml.AspectTests.Callbacks.InstanceOnChangingValueAcceptsAssignable.InstanceOnChangingValueAcceptsAssignable.FooProperty, value);
    }
  }
  private void OnFooChanging(IEnumerable<int> value)
  {
  }
  public static readonly global::System.Windows.DependencyProperty FooProperty;
  static InstanceOnChangingValueAcceptsAssignable()
  {
    object CoerceValue_1(global::System.Windows.DependencyObject d, object value_1)
    {
      ((global::Metalama.Patterns.Xaml.AspectTests.Callbacks.InstanceOnChangingValueAcceptsAssignable.InstanceOnChangingValueAcceptsAssignable)d).OnFooChanging((global::System.Collections.Generic.List<global::System.Int32>)value_1);
      return (global::System.Object)value_1;
    }
    var metadata = new global::System.Windows.PropertyMetadata();
    metadata.CoerceValueCallback = (global::System.Windows.CoerceValueCallback)CoerceValue_1;
    global::Metalama.Patterns.Xaml.AspectTests.Callbacks.InstanceOnChangingValueAcceptsAssignable.InstanceOnChangingValueAcceptsAssignable.FooProperty = global::System.Windows.DependencyProperty.Register("Foo", typeof(global::System.Collections.Generic.List<global::System.Int32>), typeof(global::Metalama.Patterns.Xaml.AspectTests.Callbacks.InstanceOnChangingValueAcceptsAssignable.InstanceOnChangingValueAcceptsAssignable), metadata);
  }
}