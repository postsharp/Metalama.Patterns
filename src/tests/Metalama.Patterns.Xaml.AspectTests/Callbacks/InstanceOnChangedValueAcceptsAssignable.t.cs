using System.Windows;
namespace Metalama.Patterns.Xaml.AspectTests.Callbacks.InstanceOnChangedValueAcceptsAssignable;
public partial class InstanceOnChangedValueAcceptsAssignable : DependencyObject
{
  [DependencyProperty]
  public List<int> Foo
  {
    get
    {
      return ((global::System.Collections.Generic.List<global::System.Int32>)this.GetValue(global::Metalama.Patterns.Xaml.AspectTests.Callbacks.InstanceOnChangedValueAcceptsAssignable.InstanceOnChangedValueAcceptsAssignable.FooProperty));
    }
    set
    {
      this.SetValue(global::Metalama.Patterns.Xaml.AspectTests.Callbacks.InstanceOnChangedValueAcceptsAssignable.InstanceOnChangedValueAcceptsAssignable.FooProperty, value);
    }
  }
  private void OnFooChanged(IEnumerable<int> value)
  {
  }
  public static readonly global::System.Windows.DependencyProperty FooProperty;
  static InstanceOnChangedValueAcceptsAssignable()
  {
    void PropertyChanged(global::System.Windows.DependencyObject d, global::System.Windows.DependencyPropertyChangedEventArgs e)
    {
      ((global::Metalama.Patterns.Xaml.AspectTests.Callbacks.InstanceOnChangedValueAcceptsAssignable.InstanceOnChangedValueAcceptsAssignable)d).OnFooChanged((global::System.Collections.Generic.List<global::System.Int32>)e.NewValue);
    }
    global::Metalama.Patterns.Xaml.AspectTests.Callbacks.InstanceOnChangedValueAcceptsAssignable.InstanceOnChangedValueAcceptsAssignable.FooProperty = global::System.Windows.DependencyProperty.Register("Foo", typeof(global::System.Collections.Generic.List<global::System.Int32>), typeof(global::Metalama.Patterns.Xaml.AspectTests.Callbacks.InstanceOnChangedValueAcceptsAssignable.InstanceOnChangedValueAcceptsAssignable), new global::System.Windows.PropertyMetadata((global::System.Windows.PropertyChangedCallback)PropertyChanged));
  }
}