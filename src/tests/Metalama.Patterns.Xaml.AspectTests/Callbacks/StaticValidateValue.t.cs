using System.Windows;
namespace Metalama.Patterns.Xaml.AspectTests.Callbacks.StaticValidateValue;
public partial class StaticValidateValue : DependencyObject
{
  [DependencyProperty]
  public int Foo
  {
    get
    {
      return ((global::System.Int32)this.GetValue(global::Metalama.Patterns.Xaml.AspectTests.Callbacks.StaticValidateValue.StaticValidateValue.FooProperty));
    }
    set
    {
      this.SetValue(global::Metalama.Patterns.Xaml.AspectTests.Callbacks.StaticValidateValue.StaticValidateValue.FooProperty, value);
    }
  }
  private static bool ValidateFoo(int value) => true;
  [DependencyProperty]
  public List<int> AcceptsAssignable
  {
    get
    {
      return ((global::System.Collections.Generic.List<global::System.Int32>)this.GetValue(global::Metalama.Patterns.Xaml.AspectTests.Callbacks.StaticValidateValue.StaticValidateValue.AcceptsAssignableProperty));
    }
    set
    {
      this.SetValue(global::Metalama.Patterns.Xaml.AspectTests.Callbacks.StaticValidateValue.StaticValidateValue.AcceptsAssignableProperty, value);
    }
  }
  private static bool ValidateAcceptsAssignable(IEnumerable<int> value) => true;
  [DependencyProperty]
  public int AcceptsGeneric
  {
    get
    {
      return ((global::System.Int32)this.GetValue(global::Metalama.Patterns.Xaml.AspectTests.Callbacks.StaticValidateValue.StaticValidateValue.AcceptsGenericProperty));
    }
    set
    {
      this.SetValue(global::Metalama.Patterns.Xaml.AspectTests.Callbacks.StaticValidateValue.StaticValidateValue.AcceptsGenericProperty, value);
    }
  }
  private static bool ValidateAcceptsGeneric<T>(T value) => true;
  [DependencyProperty]
  public int AcceptsObject
  {
    get
    {
      return ((global::System.Int32)this.GetValue(global::Metalama.Patterns.Xaml.AspectTests.Callbacks.StaticValidateValue.StaticValidateValue.AcceptsObjectProperty));
    }
    set
    {
      this.SetValue(global::Metalama.Patterns.Xaml.AspectTests.Callbacks.StaticValidateValue.StaticValidateValue.AcceptsObjectProperty, value);
    }
  }
  private static bool ValidateAcceptsObject(object value) => true;
  public static readonly global::System.Windows.DependencyProperty AcceptsAssignableProperty;
  public static readonly global::System.Windows.DependencyProperty AcceptsGenericProperty;
  public static readonly global::System.Windows.DependencyProperty AcceptsObjectProperty;
  public static readonly global::System.Windows.DependencyProperty FooProperty;
  static StaticValidateValue()
  {
    object CoerceValue_4(global::System.Windows.DependencyObject d_3, object value_4)
    {
      global::Metalama.Patterns.Xaml.AspectTests.Callbacks.StaticValidateValue.StaticValidateValue.ValidateAcceptsObject(value_4);
      return (global::System.Object)value_4;
    }
    var metadata_3 = new global::System.Windows.PropertyMetadata();
    metadata_3.CoerceValueCallback = (global::System.Windows.CoerceValueCallback)CoerceValue_4;
    global::Metalama.Patterns.Xaml.AspectTests.Callbacks.StaticValidateValue.StaticValidateValue.AcceptsObjectProperty = global::System.Windows.DependencyProperty.Register("AcceptsObject", typeof(global::System.Int32), typeof(global::Metalama.Patterns.Xaml.AspectTests.Callbacks.StaticValidateValue.StaticValidateValue), metadata_3);
    object CoerceValue_3(global::System.Windows.DependencyObject d_2, object value_3)
    {
      global::Metalama.Patterns.Xaml.AspectTests.Callbacks.StaticValidateValue.StaticValidateValue.ValidateAcceptsGeneric<global::System.Int32>((global::System.Int32)value_3);
      return (global::System.Object)value_3;
    }
    var metadata_2 = new global::System.Windows.PropertyMetadata();
    metadata_2.CoerceValueCallback = (global::System.Windows.CoerceValueCallback)CoerceValue_3;
    global::Metalama.Patterns.Xaml.AspectTests.Callbacks.StaticValidateValue.StaticValidateValue.AcceptsGenericProperty = global::System.Windows.DependencyProperty.Register("AcceptsGeneric", typeof(global::System.Int32), typeof(global::Metalama.Patterns.Xaml.AspectTests.Callbacks.StaticValidateValue.StaticValidateValue), metadata_2);
    object CoerceValue_2(global::System.Windows.DependencyObject d_1, object value_2)
    {
      global::Metalama.Patterns.Xaml.AspectTests.Callbacks.StaticValidateValue.StaticValidateValue.ValidateAcceptsAssignable((global::System.Collections.Generic.List<global::System.Int32>)value_2);
      return (global::System.Object)value_2;
    }
    var metadata_1 = new global::System.Windows.PropertyMetadata();
    metadata_1.CoerceValueCallback = (global::System.Windows.CoerceValueCallback)CoerceValue_2;
    global::Metalama.Patterns.Xaml.AspectTests.Callbacks.StaticValidateValue.StaticValidateValue.AcceptsAssignableProperty = global::System.Windows.DependencyProperty.Register("AcceptsAssignable", typeof(global::System.Collections.Generic.List<global::System.Int32>), typeof(global::Metalama.Patterns.Xaml.AspectTests.Callbacks.StaticValidateValue.StaticValidateValue), metadata_1);
    object CoerceValue_1(global::System.Windows.DependencyObject d, object value_1)
    {
      global::Metalama.Patterns.Xaml.AspectTests.Callbacks.StaticValidateValue.StaticValidateValue.ValidateFoo((global::System.Int32)value_1);
      return (global::System.Object)value_1;
    }
    var metadata = new global::System.Windows.PropertyMetadata();
    metadata.CoerceValueCallback = (global::System.Windows.CoerceValueCallback)CoerceValue_1;
    global::Metalama.Patterns.Xaml.AspectTests.Callbacks.StaticValidateValue.StaticValidateValue.FooProperty = global::System.Windows.DependencyProperty.Register("Foo", typeof(global::System.Int32), typeof(global::Metalama.Patterns.Xaml.AspectTests.Callbacks.StaticValidateValue.StaticValidateValue), metadata);
  }
}