using System.Windows;
namespace Metalama.Patterns.Xaml.AspectTests.Callbacks.InstanceOnChangedOldValueAndNewValue;
public partial class InstanceOnChangedOldValueAndNewValue : DependencyObject
{
  [DependencyProperty]
  public int Foo
  {
    get
    {
      return ((global::System.Int32)this.GetValue(global::Metalama.Patterns.Xaml.AspectTests.Callbacks.InstanceOnChangedOldValueAndNewValue.InstanceOnChangedOldValueAndNewValue.FooProperty));
    }
    set
    {
      this.SetValue(global::Metalama.Patterns.Xaml.AspectTests.Callbacks.InstanceOnChangedOldValueAndNewValue.InstanceOnChangedOldValueAndNewValue.FooProperty, value);
    }
  }
  private void OnFooChanged(int oldValue, int newValue)
  {
  }
  [DependencyProperty]
  public List<int> AcceptAssignable
  {
    get
    {
      return ((global::System.Collections.Generic.List<global::System.Int32>)this.GetValue(global::Metalama.Patterns.Xaml.AspectTests.Callbacks.InstanceOnChangedOldValueAndNewValue.InstanceOnChangedOldValueAndNewValue.AcceptAssignableProperty));
    }
    set
    {
      this.SetValue(global::Metalama.Patterns.Xaml.AspectTests.Callbacks.InstanceOnChangedOldValueAndNewValue.InstanceOnChangedOldValueAndNewValue.AcceptAssignableProperty, value);
    }
  }
  private void OnAcceptAssignableChanged(IEnumerable<int> oldValue, IEnumerable<int> newValue)
  {
  }
  [DependencyProperty]
  public int AcceptGeneric
  {
    get
    {
      return ((global::System.Int32)this.GetValue(global::Metalama.Patterns.Xaml.AspectTests.Callbacks.InstanceOnChangedOldValueAndNewValue.InstanceOnChangedOldValueAndNewValue.AcceptGenericProperty));
    }
    set
    {
      this.SetValue(global::Metalama.Patterns.Xaml.AspectTests.Callbacks.InstanceOnChangedOldValueAndNewValue.InstanceOnChangedOldValueAndNewValue.AcceptGenericProperty, value);
    }
  }
  private void OnAcceptGenericChanged<T>(T oldValue, T newValue)
  {
  }
  [DependencyProperty]
  public int AcceptObject
  {
    get
    {
      return ((global::System.Int32)this.GetValue(global::Metalama.Patterns.Xaml.AspectTests.Callbacks.InstanceOnChangedOldValueAndNewValue.InstanceOnChangedOldValueAndNewValue.AcceptObjectProperty));
    }
    set
    {
      this.SetValue(global::Metalama.Patterns.Xaml.AspectTests.Callbacks.InstanceOnChangedOldValueAndNewValue.InstanceOnChangedOldValueAndNewValue.AcceptObjectProperty, value);
    }
  }
  private void OnAcceptObjectChanged(object oldValue, object newValue)
  {
  }
  public static readonly global::System.Windows.DependencyProperty AcceptAssignableProperty;
  public static readonly global::System.Windows.DependencyProperty AcceptGenericProperty;
  public static readonly global::System.Windows.DependencyProperty AcceptObjectProperty;
  public static readonly global::System.Windows.DependencyProperty FooProperty;
  static InstanceOnChangedOldValueAndNewValue()
  {
    void PropertyChanged_3(global::System.Windows.DependencyObject d_3, global::System.Windows.DependencyPropertyChangedEventArgs e_3)
    {
      ((global::Metalama.Patterns.Xaml.AspectTests.Callbacks.InstanceOnChangedOldValueAndNewValue.InstanceOnChangedOldValueAndNewValue)d_3).OnAcceptObjectChanged(e_3.OldValue, e_3.NewValue);
    }
    global::Metalama.Patterns.Xaml.AspectTests.Callbacks.InstanceOnChangedOldValueAndNewValue.InstanceOnChangedOldValueAndNewValue.AcceptObjectProperty = global::System.Windows.DependencyProperty.Register("AcceptObject", typeof(global::System.Int32), typeof(global::Metalama.Patterns.Xaml.AspectTests.Callbacks.InstanceOnChangedOldValueAndNewValue.InstanceOnChangedOldValueAndNewValue), new global::System.Windows.PropertyMetadata((global::System.Windows.PropertyChangedCallback)PropertyChanged_3));
    void PropertyChanged_2(global::System.Windows.DependencyObject d_2, global::System.Windows.DependencyPropertyChangedEventArgs e_2)
    {
      ((global::Metalama.Patterns.Xaml.AspectTests.Callbacks.InstanceOnChangedOldValueAndNewValue.InstanceOnChangedOldValueAndNewValue)d_2).OnAcceptGenericChanged<global::System.Int32>((global::System.Int32)e_2.OldValue, (global::System.Int32)e_2.NewValue);
    }
    global::Metalama.Patterns.Xaml.AspectTests.Callbacks.InstanceOnChangedOldValueAndNewValue.InstanceOnChangedOldValueAndNewValue.AcceptGenericProperty = global::System.Windows.DependencyProperty.Register("AcceptGeneric", typeof(global::System.Int32), typeof(global::Metalama.Patterns.Xaml.AspectTests.Callbacks.InstanceOnChangedOldValueAndNewValue.InstanceOnChangedOldValueAndNewValue), new global::System.Windows.PropertyMetadata((global::System.Windows.PropertyChangedCallback)PropertyChanged_2));
    void PropertyChanged_1(global::System.Windows.DependencyObject d_1, global::System.Windows.DependencyPropertyChangedEventArgs e_1)
    {
      ((global::Metalama.Patterns.Xaml.AspectTests.Callbacks.InstanceOnChangedOldValueAndNewValue.InstanceOnChangedOldValueAndNewValue)d_1).OnAcceptAssignableChanged((global::System.Collections.Generic.List<global::System.Int32>)e_1.OldValue, (global::System.Collections.Generic.List<global::System.Int32>)e_1.NewValue);
    }
    global::Metalama.Patterns.Xaml.AspectTests.Callbacks.InstanceOnChangedOldValueAndNewValue.InstanceOnChangedOldValueAndNewValue.AcceptAssignableProperty = global::System.Windows.DependencyProperty.Register("AcceptAssignable", typeof(global::System.Collections.Generic.List<global::System.Int32>), typeof(global::Metalama.Patterns.Xaml.AspectTests.Callbacks.InstanceOnChangedOldValueAndNewValue.InstanceOnChangedOldValueAndNewValue), new global::System.Windows.PropertyMetadata((global::System.Windows.PropertyChangedCallback)PropertyChanged_1));
    void PropertyChanged(global::System.Windows.DependencyObject d, global::System.Windows.DependencyPropertyChangedEventArgs e)
    {
      ((global::Metalama.Patterns.Xaml.AspectTests.Callbacks.InstanceOnChangedOldValueAndNewValue.InstanceOnChangedOldValueAndNewValue)d).OnFooChanged((global::System.Int32)e.OldValue, (global::System.Int32)e.NewValue);
    }
    global::Metalama.Patterns.Xaml.AspectTests.Callbacks.InstanceOnChangedOldValueAndNewValue.InstanceOnChangedOldValueAndNewValue.FooProperty = global::System.Windows.DependencyProperty.Register("Foo", typeof(global::System.Int32), typeof(global::Metalama.Patterns.Xaml.AspectTests.Callbacks.InstanceOnChangedOldValueAndNewValue.InstanceOnChangedOldValueAndNewValue), new global::System.Windows.PropertyMetadata((global::System.Windows.PropertyChangedCallback)PropertyChanged));
  }
}