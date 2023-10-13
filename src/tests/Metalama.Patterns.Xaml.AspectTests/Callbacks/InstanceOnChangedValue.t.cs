using System.Windows;
namespace Metalama.Patterns.Xaml.AspectTests.Callbacks.InstanceOnChangedValue;
public partial class InstanceOnChangedValue : DependencyObject
{
  [DependencyProperty]
  public int Foo
  {
    get
    {
      return ((global::System.Int32)this.GetValue(global::Metalama.Patterns.Xaml.AspectTests.Callbacks.InstanceOnChangedValue.InstanceOnChangedValue.FooProperty));
    }
    set
    {
      this.SetValue(global::Metalama.Patterns.Xaml.AspectTests.Callbacks.InstanceOnChangedValue.InstanceOnChangedValue.FooProperty, value);
    }
  }
  private void OnFooChanged(int value)
  {
  }
  [DependencyProperty]
  public List<int> AcceptAssignable
  {
    get
    {
      return ((global::System.Collections.Generic.List<global::System.Int32>)this.GetValue(global::Metalama.Patterns.Xaml.AspectTests.Callbacks.InstanceOnChangedValue.InstanceOnChangedValue.AcceptAssignableProperty));
    }
    set
    {
      this.SetValue(global::Metalama.Patterns.Xaml.AspectTests.Callbacks.InstanceOnChangedValue.InstanceOnChangedValue.AcceptAssignableProperty, value);
    }
  }
  private void OnAcceptAssignableChanged(IEnumerable<int> value)
  {
  }
  [DependencyProperty]
  public int AcceptGeneric
  {
    get
    {
      return ((global::System.Int32)this.GetValue(global::Metalama.Patterns.Xaml.AspectTests.Callbacks.InstanceOnChangedValue.InstanceOnChangedValue.AcceptGenericProperty));
    }
    set
    {
      this.SetValue(global::Metalama.Patterns.Xaml.AspectTests.Callbacks.InstanceOnChangedValue.InstanceOnChangedValue.AcceptGenericProperty, value);
    }
  }
  private void OnAcceptGenericChanged<T>(T value)
  {
  }
  [DependencyProperty]
  public int AcceptObject
  {
    get
    {
      return ((global::System.Int32)this.GetValue(global::Metalama.Patterns.Xaml.AspectTests.Callbacks.InstanceOnChangedValue.InstanceOnChangedValue.AcceptObjectProperty));
    }
    set
    {
      this.SetValue(global::Metalama.Patterns.Xaml.AspectTests.Callbacks.InstanceOnChangedValue.InstanceOnChangedValue.AcceptObjectProperty, value);
    }
  }
  private void OnAcceptObjectChanged(object value)
  {
  }
  public static readonly global::System.Windows.DependencyProperty AcceptAssignableProperty;
  public static readonly global::System.Windows.DependencyProperty AcceptGenericProperty;
  public static readonly global::System.Windows.DependencyProperty AcceptObjectProperty;
  public static readonly global::System.Windows.DependencyProperty FooProperty;
  static InstanceOnChangedValue()
  {
    void PropertyChanged_3(global::System.Windows.DependencyObject d_3, global::System.Windows.DependencyPropertyChangedEventArgs e_3)
    {
      ((global::Metalama.Patterns.Xaml.AspectTests.Callbacks.InstanceOnChangedValue.InstanceOnChangedValue)d_3).OnAcceptObjectChanged(e_3.NewValue);
    }
    global::Metalama.Patterns.Xaml.AspectTests.Callbacks.InstanceOnChangedValue.InstanceOnChangedValue.AcceptObjectProperty = global::System.Windows.DependencyProperty.Register("AcceptObject", typeof(global::System.Int32), typeof(global::Metalama.Patterns.Xaml.AspectTests.Callbacks.InstanceOnChangedValue.InstanceOnChangedValue), new global::System.Windows.PropertyMetadata((global::System.Windows.PropertyChangedCallback)PropertyChanged_3));
    void PropertyChanged_2(global::System.Windows.DependencyObject d_2, global::System.Windows.DependencyPropertyChangedEventArgs e_2)
    {
      ((global::Metalama.Patterns.Xaml.AspectTests.Callbacks.InstanceOnChangedValue.InstanceOnChangedValue)d_2).OnAcceptGenericChanged<global::System.Int32>((global::System.Int32)e_2.NewValue);
    }
    global::Metalama.Patterns.Xaml.AspectTests.Callbacks.InstanceOnChangedValue.InstanceOnChangedValue.AcceptGenericProperty = global::System.Windows.DependencyProperty.Register("AcceptGeneric", typeof(global::System.Int32), typeof(global::Metalama.Patterns.Xaml.AspectTests.Callbacks.InstanceOnChangedValue.InstanceOnChangedValue), new global::System.Windows.PropertyMetadata((global::System.Windows.PropertyChangedCallback)PropertyChanged_2));
    void PropertyChanged_1(global::System.Windows.DependencyObject d_1, global::System.Windows.DependencyPropertyChangedEventArgs e_1)
    {
      ((global::Metalama.Patterns.Xaml.AspectTests.Callbacks.InstanceOnChangedValue.InstanceOnChangedValue)d_1).OnAcceptAssignableChanged((global::System.Collections.Generic.List<global::System.Int32>)e_1.NewValue);
    }
    global::Metalama.Patterns.Xaml.AspectTests.Callbacks.InstanceOnChangedValue.InstanceOnChangedValue.AcceptAssignableProperty = global::System.Windows.DependencyProperty.Register("AcceptAssignable", typeof(global::System.Collections.Generic.List<global::System.Int32>), typeof(global::Metalama.Patterns.Xaml.AspectTests.Callbacks.InstanceOnChangedValue.InstanceOnChangedValue), new global::System.Windows.PropertyMetadata((global::System.Windows.PropertyChangedCallback)PropertyChanged_1));
    void PropertyChanged(global::System.Windows.DependencyObject d, global::System.Windows.DependencyPropertyChangedEventArgs e)
    {
      ((global::Metalama.Patterns.Xaml.AspectTests.Callbacks.InstanceOnChangedValue.InstanceOnChangedValue)d).OnFooChanged((global::System.Int32)e.NewValue);
    }
    global::Metalama.Patterns.Xaml.AspectTests.Callbacks.InstanceOnChangedValue.InstanceOnChangedValue.FooProperty = global::System.Windows.DependencyProperty.Register("Foo", typeof(global::System.Int32), typeof(global::Metalama.Patterns.Xaml.AspectTests.Callbacks.InstanceOnChangedValue.InstanceOnChangedValue), new global::System.Windows.PropertyMetadata((global::System.Windows.PropertyChangedCallback)PropertyChanged));
  }
}