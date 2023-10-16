using System.Windows;
namespace Metalama.Patterns.Xaml.AspectTests.Callbacks.InstanceValidateDependencyPropertyAndValue;
public partial class InstanceValidateDependencyPropertyAndValue : DependencyObject
{
  [DependencyProperty]
  public int Foo
  {
    get
    {
      return ((global::System.Int32)this.GetValue(global::Metalama.Patterns.Xaml.AspectTests.Callbacks.InstanceValidateDependencyPropertyAndValue.InstanceValidateDependencyPropertyAndValue.FooProperty));
    }
    set
    {
      this.SetValue(global::Metalama.Patterns.Xaml.AspectTests.Callbacks.InstanceValidateDependencyPropertyAndValue.InstanceValidateDependencyPropertyAndValue.FooProperty, value);
    }
  }
  private bool ValidateFoo(DependencyProperty d, int value) => true;
  [DependencyProperty]
  public List<int> AcceptAssignable
  {
    get
    {
      return ((global::System.Collections.Generic.List<global::System.Int32>)this.GetValue(global::Metalama.Patterns.Xaml.AspectTests.Callbacks.InstanceValidateDependencyPropertyAndValue.InstanceValidateDependencyPropertyAndValue.AcceptAssignableProperty));
    }
    set
    {
      this.SetValue(global::Metalama.Patterns.Xaml.AspectTests.Callbacks.InstanceValidateDependencyPropertyAndValue.InstanceValidateDependencyPropertyAndValue.AcceptAssignableProperty, value);
    }
  }
  private bool ValidateAcceptAssignable(DependencyProperty d, IEnumerable<int> value) => true;
  [DependencyProperty]
  public int AcceptGeneric
  {
    get
    {
      return ((global::System.Int32)this.GetValue(global::Metalama.Patterns.Xaml.AspectTests.Callbacks.InstanceValidateDependencyPropertyAndValue.InstanceValidateDependencyPropertyAndValue.AcceptGenericProperty));
    }
    set
    {
      this.SetValue(global::Metalama.Patterns.Xaml.AspectTests.Callbacks.InstanceValidateDependencyPropertyAndValue.InstanceValidateDependencyPropertyAndValue.AcceptGenericProperty, value);
    }
  }
  private bool ValidateAcceptGeneric<T>(DependencyProperty d, T value) => true;
  [DependencyProperty]
  public int AcceptObject
  {
    get
    {
      return ((global::System.Int32)this.GetValue(global::Metalama.Patterns.Xaml.AspectTests.Callbacks.InstanceValidateDependencyPropertyAndValue.InstanceValidateDependencyPropertyAndValue.AcceptObjectProperty));
    }
    set
    {
      this.SetValue(global::Metalama.Patterns.Xaml.AspectTests.Callbacks.InstanceValidateDependencyPropertyAndValue.InstanceValidateDependencyPropertyAndValue.AcceptObjectProperty, value);
    }
  }
  private bool ValidateAcceptObject(DependencyProperty d, object value) => true;
  public static readonly global::System.Windows.DependencyProperty AcceptAssignableProperty;
  public static readonly global::System.Windows.DependencyProperty AcceptGenericProperty;
  public static readonly global::System.Windows.DependencyProperty AcceptObjectProperty;
  public static readonly global::System.Windows.DependencyProperty FooProperty;
  static InstanceValidateDependencyPropertyAndValue()
  {
    object CoerceValue_4(global::System.Windows.DependencyObject d_4, object value_4)
    {
      if (!((global::Metalama.Patterns.Xaml.AspectTests.Callbacks.InstanceValidateDependencyPropertyAndValue.InstanceValidateDependencyPropertyAndValue)d_4).ValidateAcceptObject(global::Metalama.Patterns.Xaml.AspectTests.Callbacks.InstanceValidateDependencyPropertyAndValue.InstanceValidateDependencyPropertyAndValue.AcceptObjectProperty, value_4))
      {
        throw new global::System.ArgumentException("Invalid property value.", "value");
      }
      return (global::System.Object)value_4;
    }
    var metadata_3 = new global::System.Windows.PropertyMetadata();
    metadata_3.CoerceValueCallback = (global::System.Windows.CoerceValueCallback)CoerceValue_4;
    global::Metalama.Patterns.Xaml.AspectTests.Callbacks.InstanceValidateDependencyPropertyAndValue.InstanceValidateDependencyPropertyAndValue.AcceptObjectProperty = global::System.Windows.DependencyProperty.Register("AcceptObject", typeof(global::System.Int32), typeof(global::Metalama.Patterns.Xaml.AspectTests.Callbacks.InstanceValidateDependencyPropertyAndValue.InstanceValidateDependencyPropertyAndValue), metadata_3);
    object CoerceValue_3(global::System.Windows.DependencyObject d_3, object value_3)
    {
      if (!((global::Metalama.Patterns.Xaml.AspectTests.Callbacks.InstanceValidateDependencyPropertyAndValue.InstanceValidateDependencyPropertyAndValue)d_3).ValidateAcceptGeneric<global::System.Int32>(global::Metalama.Patterns.Xaml.AspectTests.Callbacks.InstanceValidateDependencyPropertyAndValue.InstanceValidateDependencyPropertyAndValue.AcceptGenericProperty, (global::System.Int32)value_3))
      {
        throw new global::System.ArgumentException("Invalid property value.", "value");
      }
      return (global::System.Object)value_3;
    }
    var metadata_2 = new global::System.Windows.PropertyMetadata();
    metadata_2.CoerceValueCallback = (global::System.Windows.CoerceValueCallback)CoerceValue_3;
    global::Metalama.Patterns.Xaml.AspectTests.Callbacks.InstanceValidateDependencyPropertyAndValue.InstanceValidateDependencyPropertyAndValue.AcceptGenericProperty = global::System.Windows.DependencyProperty.Register("AcceptGeneric", typeof(global::System.Int32), typeof(global::Metalama.Patterns.Xaml.AspectTests.Callbacks.InstanceValidateDependencyPropertyAndValue.InstanceValidateDependencyPropertyAndValue), metadata_2);
    object CoerceValue_2(global::System.Windows.DependencyObject d_2, object value_2)
    {
      if (!((global::Metalama.Patterns.Xaml.AspectTests.Callbacks.InstanceValidateDependencyPropertyAndValue.InstanceValidateDependencyPropertyAndValue)d_2).ValidateAcceptAssignable(global::Metalama.Patterns.Xaml.AspectTests.Callbacks.InstanceValidateDependencyPropertyAndValue.InstanceValidateDependencyPropertyAndValue.AcceptAssignableProperty, (global::System.Collections.Generic.List<global::System.Int32>)value_2))
      {
        throw new global::System.ArgumentException("Invalid property value.", "value");
      }
      return (global::System.Object)value_2;
    }
    var metadata_1 = new global::System.Windows.PropertyMetadata();
    metadata_1.CoerceValueCallback = (global::System.Windows.CoerceValueCallback)CoerceValue_2;
    global::Metalama.Patterns.Xaml.AspectTests.Callbacks.InstanceValidateDependencyPropertyAndValue.InstanceValidateDependencyPropertyAndValue.AcceptAssignableProperty = global::System.Windows.DependencyProperty.Register("AcceptAssignable", typeof(global::System.Collections.Generic.List<global::System.Int32>), typeof(global::Metalama.Patterns.Xaml.AspectTests.Callbacks.InstanceValidateDependencyPropertyAndValue.InstanceValidateDependencyPropertyAndValue), metadata_1);
    object CoerceValue_1(global::System.Windows.DependencyObject d_1, object value_1)
    {
      if (!((global::Metalama.Patterns.Xaml.AspectTests.Callbacks.InstanceValidateDependencyPropertyAndValue.InstanceValidateDependencyPropertyAndValue)d_1).ValidateFoo(global::Metalama.Patterns.Xaml.AspectTests.Callbacks.InstanceValidateDependencyPropertyAndValue.InstanceValidateDependencyPropertyAndValue.FooProperty, (global::System.Int32)value_1))
      {
        throw new global::System.ArgumentException("Invalid property value.", "value");
      }
      return (global::System.Object)value_1;
    }
    var metadata = new global::System.Windows.PropertyMetadata();
    metadata.CoerceValueCallback = (global::System.Windows.CoerceValueCallback)CoerceValue_1;
    global::Metalama.Patterns.Xaml.AspectTests.Callbacks.InstanceValidateDependencyPropertyAndValue.InstanceValidateDependencyPropertyAndValue.FooProperty = global::System.Windows.DependencyProperty.Register("Foo", typeof(global::System.Int32), typeof(global::Metalama.Patterns.Xaml.AspectTests.Callbacks.InstanceValidateDependencyPropertyAndValue.InstanceValidateDependencyPropertyAndValue), metadata);
  }
}