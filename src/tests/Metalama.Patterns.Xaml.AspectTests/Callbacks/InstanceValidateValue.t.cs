using System.Windows;
namespace Metalama.Patterns.Xaml.AspectTests.Callbacks.InstanceValidateValue;
public partial class InstanceValidateValue : DependencyObject
{
  [DependencyProperty]
  public int Foo
  {
    get
    {
      return ((global::System.Int32)this.GetValue(global::Metalama.Patterns.Xaml.AspectTests.Callbacks.InstanceValidateValue.InstanceValidateValue.FooProperty));
    }
    set
    {
      this.SetValue(global::Metalama.Patterns.Xaml.AspectTests.Callbacks.InstanceValidateValue.InstanceValidateValue.FooProperty, value);
    }
  }
  private bool ValidateFoo(int value) => true;
  [DependencyProperty]
  public List<int> AcceptAssignable
  {
    get
    {
      return ((global::System.Collections.Generic.List<global::System.Int32>)this.GetValue(global::Metalama.Patterns.Xaml.AspectTests.Callbacks.InstanceValidateValue.InstanceValidateValue.AcceptAssignableProperty));
    }
    set
    {
      this.SetValue(global::Metalama.Patterns.Xaml.AspectTests.Callbacks.InstanceValidateValue.InstanceValidateValue.AcceptAssignableProperty, value);
    }
  }
  private bool ValidateAcceptAssignable(IEnumerable<int> value) => true;
  [DependencyProperty]
  public int AcceptGeneric
  {
    get
    {
      return ((global::System.Int32)this.GetValue(global::Metalama.Patterns.Xaml.AspectTests.Callbacks.InstanceValidateValue.InstanceValidateValue.AcceptGenericProperty));
    }
    set
    {
      this.SetValue(global::Metalama.Patterns.Xaml.AspectTests.Callbacks.InstanceValidateValue.InstanceValidateValue.AcceptGenericProperty, value);
    }
  }
  private bool ValidateAcceptGeneric<T>(T value) => true;
  [DependencyProperty]
  public int AcceptObject
  {
    get
    {
      return ((global::System.Int32)this.GetValue(global::Metalama.Patterns.Xaml.AspectTests.Callbacks.InstanceValidateValue.InstanceValidateValue.AcceptObjectProperty));
    }
    set
    {
      this.SetValue(global::Metalama.Patterns.Xaml.AspectTests.Callbacks.InstanceValidateValue.InstanceValidateValue.AcceptObjectProperty, value);
    }
  }
  private bool ValidateAcceptObject(object value) => true;
  public static readonly global::System.Windows.DependencyProperty AcceptAssignableProperty;
  public static readonly global::System.Windows.DependencyProperty AcceptGenericProperty;
  public static readonly global::System.Windows.DependencyProperty AcceptObjectProperty;
  public static readonly global::System.Windows.DependencyProperty FooProperty;
  static InstanceValidateValue()
  {
    object CoerceValue_4(global::System.Windows.DependencyObject d_3, object value_4)
    {
      ((global::Metalama.Patterns.Xaml.AspectTests.Callbacks.InstanceValidateValue.InstanceValidateValue)d_3).ValidateAcceptObject(value_4);
      return (global::System.Object)value_4;
    }
    var metadata_3 = new global::System.Windows.PropertyMetadata();
    metadata_3.CoerceValueCallback = (global::System.Windows.CoerceValueCallback)CoerceValue_4;
    global::Metalama.Patterns.Xaml.AspectTests.Callbacks.InstanceValidateValue.InstanceValidateValue.AcceptObjectProperty = global::System.Windows.DependencyProperty.Register("AcceptObject", typeof(global::System.Int32), typeof(global::Metalama.Patterns.Xaml.AspectTests.Callbacks.InstanceValidateValue.InstanceValidateValue), metadata_3);
    object CoerceValue_3(global::System.Windows.DependencyObject d_2, object value_3)
    {
      ((global::Metalama.Patterns.Xaml.AspectTests.Callbacks.InstanceValidateValue.InstanceValidateValue)d_2).ValidateAcceptGeneric<global::System.Int32>((global::System.Int32)value_3);
      return (global::System.Object)value_3;
    }
    var metadata_2 = new global::System.Windows.PropertyMetadata();
    metadata_2.CoerceValueCallback = (global::System.Windows.CoerceValueCallback)CoerceValue_3;
    global::Metalama.Patterns.Xaml.AspectTests.Callbacks.InstanceValidateValue.InstanceValidateValue.AcceptGenericProperty = global::System.Windows.DependencyProperty.Register("AcceptGeneric", typeof(global::System.Int32), typeof(global::Metalama.Patterns.Xaml.AspectTests.Callbacks.InstanceValidateValue.InstanceValidateValue), metadata_2);
    object CoerceValue_2(global::System.Windows.DependencyObject d_1, object value_2)
    {
      ((global::Metalama.Patterns.Xaml.AspectTests.Callbacks.InstanceValidateValue.InstanceValidateValue)d_1).ValidateAcceptAssignable((global::System.Collections.Generic.List<global::System.Int32>)value_2);
      return (global::System.Object)value_2;
    }
    var metadata_1 = new global::System.Windows.PropertyMetadata();
    metadata_1.CoerceValueCallback = (global::System.Windows.CoerceValueCallback)CoerceValue_2;
    global::Metalama.Patterns.Xaml.AspectTests.Callbacks.InstanceValidateValue.InstanceValidateValue.AcceptAssignableProperty = global::System.Windows.DependencyProperty.Register("AcceptAssignable", typeof(global::System.Collections.Generic.List<global::System.Int32>), typeof(global::Metalama.Patterns.Xaml.AspectTests.Callbacks.InstanceValidateValue.InstanceValidateValue), metadata_1);
    object CoerceValue_1(global::System.Windows.DependencyObject d, object value_1)
    {
      ((global::Metalama.Patterns.Xaml.AspectTests.Callbacks.InstanceValidateValue.InstanceValidateValue)d).ValidateFoo((global::System.Int32)value_1);
      return (global::System.Object)value_1;
    }
    var metadata = new global::System.Windows.PropertyMetadata();
    metadata.CoerceValueCallback = (global::System.Windows.CoerceValueCallback)CoerceValue_1;
    global::Metalama.Patterns.Xaml.AspectTests.Callbacks.InstanceValidateValue.InstanceValidateValue.FooProperty = global::System.Windows.DependencyProperty.Register("Foo", typeof(global::System.Int32), typeof(global::Metalama.Patterns.Xaml.AspectTests.Callbacks.InstanceValidateValue.InstanceValidateValue), metadata);
  }
}