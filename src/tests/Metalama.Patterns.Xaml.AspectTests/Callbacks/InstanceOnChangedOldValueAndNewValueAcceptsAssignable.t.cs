using System.Windows;
namespace Metalama.Patterns.Xaml.AspectTests.Callbacks.InstanceOnChangedOldValueAndNewValueAcceptsAssignable;
public partial class InstanceOnChangedOldValueAndNewValueAcceptsAssignable : DependencyObject
{
  [DependencyProperty]
  public List<int> Foo
  {
    get
    {
      return ((global::System.Collections.Generic.List<global::System.Int32>)this.GetValue(global::Metalama.Patterns.Xaml.AspectTests.Callbacks.InstanceOnChangedOldValueAndNewValueAcceptsAssignable.InstanceOnChangedOldValueAndNewValueAcceptsAssignable.FooProperty));
    }
    set
    {
      this.SetValue(global::Metalama.Patterns.Xaml.AspectTests.Callbacks.InstanceOnChangedOldValueAndNewValueAcceptsAssignable.InstanceOnChangedOldValueAndNewValueAcceptsAssignable.FooProperty, value);
    }
  }
  private void OnFooChanged(IEnumerable<int> oldValue, IEnumerable<int> newValue)
  {
  }
  public static readonly global::System.Windows.DependencyProperty FooProperty;
  static InstanceOnChangedOldValueAndNewValueAcceptsAssignable()
  {
    object CoerceValue_1(global::System.Windows.DependencyObject d, object value)
    {
      return (global::System.Object)value;
    }
    void PropertyChanged(global::System.Windows.DependencyObject d_1, global::System.Windows.DependencyPropertyChangedEventArgs e)
    {
      ((global::Metalama.Patterns.Xaml.AspectTests.Callbacks.InstanceOnChangedOldValueAndNewValueAcceptsAssignable.InstanceOnChangedOldValueAndNewValueAcceptsAssignable)d_1).OnFooChanged((global::System.Collections.Generic.List<global::System.Int32>)e.OldValue, (global::System.Collections.Generic.List<global::System.Int32>)e.NewValue);
    }
    global::Metalama.Patterns.Xaml.AspectTests.Callbacks.InstanceOnChangedOldValueAndNewValueAcceptsAssignable.InstanceOnChangedOldValueAndNewValueAcceptsAssignable.FooProperty = global::System.Windows.DependencyProperty.Register("Foo", typeof(global::System.Collections.Generic.List<global::System.Int32>), typeof(global::Metalama.Patterns.Xaml.AspectTests.Callbacks.InstanceOnChangedOldValueAndNewValueAcceptsAssignable.InstanceOnChangedOldValueAndNewValueAcceptsAssignable), new global::System.Windows.PropertyMetadata((global::System.Windows.PropertyChangedCallback)PropertyChanged));
  }
}