using System.Windows;
namespace Metalama.Patterns.Xaml.AspectTests.Callbacks.InstanceOnChangingValue;
public partial class InstanceOnChangingValue : DependencyObject
{
  [DependencyProperty]
  public int Foo
  {
    get
    {
      return ((global::System.Int32)this.GetValue(global::Metalama.Patterns.Xaml.AspectTests.Callbacks.InstanceOnChangingValue.InstanceOnChangingValue.FooProperty));
    }
    set
    {
      this.SetValue(global::Metalama.Patterns.Xaml.AspectTests.Callbacks.InstanceOnChangingValue.InstanceOnChangingValue.FooProperty, value);
    }
  }
  private void OnFooChanging(int value)
  {
  }
  [DependencyProperty]
  public List<int> AcceptAssignable
  {
    get
    {
      return ((global::System.Collections.Generic.List<global::System.Int32>)this.GetValue(global::Metalama.Patterns.Xaml.AspectTests.Callbacks.InstanceOnChangingValue.InstanceOnChangingValue.AcceptAssignableProperty));
    }
    set
    {
      this.SetValue(global::Metalama.Patterns.Xaml.AspectTests.Callbacks.InstanceOnChangingValue.InstanceOnChangingValue.AcceptAssignableProperty, value);
    }
  }
  private void OnAcceptAssignableChanging(IEnumerable<int> value)
  {
  }
  [DependencyProperty]
  public int AcceptGeneric
  {
    get
    {
      return ((global::System.Int32)this.GetValue(global::Metalama.Patterns.Xaml.AspectTests.Callbacks.InstanceOnChangingValue.InstanceOnChangingValue.AcceptGenericProperty));
    }
    set
    {
      this.SetValue(global::Metalama.Patterns.Xaml.AspectTests.Callbacks.InstanceOnChangingValue.InstanceOnChangingValue.AcceptGenericProperty, value);
    }
  }
  private void OnAcceptGenericChanging<T>(T value)
  {
  }
  [DependencyProperty]
  public int AcceptObject
  {
    get
    {
      return ((global::System.Int32)this.GetValue(global::Metalama.Patterns.Xaml.AspectTests.Callbacks.InstanceOnChangingValue.InstanceOnChangingValue.AcceptObjectProperty));
    }
    set
    {
      this.SetValue(global::Metalama.Patterns.Xaml.AspectTests.Callbacks.InstanceOnChangingValue.InstanceOnChangingValue.AcceptObjectProperty, value);
    }
  }
  private void OnAcceptObjectChanging(object value)
  {
  }
  public static readonly global::System.Windows.DependencyProperty AcceptAssignableProperty;
  public static readonly global::System.Windows.DependencyProperty AcceptGenericProperty;
  public static readonly global::System.Windows.DependencyProperty AcceptObjectProperty;
  public static readonly global::System.Windows.DependencyProperty FooProperty;
  static InstanceOnChangingValue()
  {
    object CoerceValue_4(global::System.Windows.DependencyObject d_3, object value_4)
    {
      ((global::Metalama.Patterns.Xaml.AspectTests.Callbacks.InstanceOnChangingValue.InstanceOnChangingValue)d_3).OnAcceptObjectChanging(value_4);
      return (global::System.Object)value_4;
    }
    var metadata_3 = new global::System.Windows.PropertyMetadata();
    metadata_3.CoerceValueCallback = (global::System.Windows.CoerceValueCallback)CoerceValue_4;
    global::Metalama.Patterns.Xaml.AspectTests.Callbacks.InstanceOnChangingValue.InstanceOnChangingValue.AcceptObjectProperty = global::System.Windows.DependencyProperty.Register("AcceptObject", typeof(global::System.Int32), typeof(global::Metalama.Patterns.Xaml.AspectTests.Callbacks.InstanceOnChangingValue.InstanceOnChangingValue), metadata_3);
    object CoerceValue_3(global::System.Windows.DependencyObject d_2, object value_3)
    {
      ((global::Metalama.Patterns.Xaml.AspectTests.Callbacks.InstanceOnChangingValue.InstanceOnChangingValue)d_2).OnAcceptGenericChanging<global::System.Int32>((global::System.Int32)value_3);
      return (global::System.Object)value_3;
    }
    var metadata_2 = new global::System.Windows.PropertyMetadata();
    metadata_2.CoerceValueCallback = (global::System.Windows.CoerceValueCallback)CoerceValue_3;
    global::Metalama.Patterns.Xaml.AspectTests.Callbacks.InstanceOnChangingValue.InstanceOnChangingValue.AcceptGenericProperty = global::System.Windows.DependencyProperty.Register("AcceptGeneric", typeof(global::System.Int32), typeof(global::Metalama.Patterns.Xaml.AspectTests.Callbacks.InstanceOnChangingValue.InstanceOnChangingValue), metadata_2);
    object CoerceValue_2(global::System.Windows.DependencyObject d_1, object value_2)
    {
      ((global::Metalama.Patterns.Xaml.AspectTests.Callbacks.InstanceOnChangingValue.InstanceOnChangingValue)d_1).OnAcceptAssignableChanging((global::System.Collections.Generic.List<global::System.Int32>)value_2);
      return (global::System.Object)value_2;
    }
    var metadata_1 = new global::System.Windows.PropertyMetadata();
    metadata_1.CoerceValueCallback = (global::System.Windows.CoerceValueCallback)CoerceValue_2;
    global::Metalama.Patterns.Xaml.AspectTests.Callbacks.InstanceOnChangingValue.InstanceOnChangingValue.AcceptAssignableProperty = global::System.Windows.DependencyProperty.Register("AcceptAssignable", typeof(global::System.Collections.Generic.List<global::System.Int32>), typeof(global::Metalama.Patterns.Xaml.AspectTests.Callbacks.InstanceOnChangingValue.InstanceOnChangingValue), metadata_1);
    object CoerceValue_1(global::System.Windows.DependencyObject d, object value_1)
    {
      ((global::Metalama.Patterns.Xaml.AspectTests.Callbacks.InstanceOnChangingValue.InstanceOnChangingValue)d).OnFooChanging((global::System.Int32)value_1);
      return (global::System.Object)value_1;
    }
    var metadata = new global::System.Windows.PropertyMetadata();
    metadata.CoerceValueCallback = (global::System.Windows.CoerceValueCallback)CoerceValue_1;
    global::Metalama.Patterns.Xaml.AspectTests.Callbacks.InstanceOnChangingValue.InstanceOnChangingValue.FooProperty = global::System.Windows.DependencyProperty.Register("Foo", typeof(global::System.Int32), typeof(global::Metalama.Patterns.Xaml.AspectTests.Callbacks.InstanceOnChangingValue.InstanceOnChangingValue), metadata);
  }
}