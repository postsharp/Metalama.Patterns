using System.Windows;
namespace Metalama.Patterns.Xaml.AspectTests.Callbacks.StaticValidateDependencyPropertyAndInstanceAndValue;
public partial class StaticValidateDependencyPropertyAndInstanceAndValue : DependencyObject
{
  [DependencyProperty]
  public int Foo
  {
    get
    {
      return ((global::System.Int32)this.GetValue(global::Metalama.Patterns.Xaml.AspectTests.Callbacks.StaticValidateDependencyPropertyAndInstanceAndValue.StaticValidateDependencyPropertyAndInstanceAndValue.FooProperty));
    }
    set
    {
      this.SetValue(global::Metalama.Patterns.Xaml.AspectTests.Callbacks.StaticValidateDependencyPropertyAndInstanceAndValue.StaticValidateDependencyPropertyAndInstanceAndValue.FooProperty, value);
    }
  }
  private static bool ValidateFoo(DependencyProperty d, StaticValidateDependencyPropertyAndInstanceAndValue instance, int value) => true;
  [DependencyProperty]
  public List<int> AcceptsAssignableForValue
  {
    get
    {
      return ((global::System.Collections.Generic.List<global::System.Int32>)this.GetValue(global::Metalama.Patterns.Xaml.AspectTests.Callbacks.StaticValidateDependencyPropertyAndInstanceAndValue.StaticValidateDependencyPropertyAndInstanceAndValue.AcceptsAssignableForValueProperty));
    }
    set
    {
      this.SetValue(global::Metalama.Patterns.Xaml.AspectTests.Callbacks.StaticValidateDependencyPropertyAndInstanceAndValue.StaticValidateDependencyPropertyAndInstanceAndValue.AcceptsAssignableForValueProperty, value);
    }
  }
  private static bool ValidateAcceptsAssignableForValue(DependencyProperty d, StaticValidateDependencyPropertyAndInstanceAndValue instance, IEnumerable<int> value) => true;
  [DependencyProperty]
  public int AcceptsGenericForValue
  {
    get
    {
      return ((global::System.Int32)this.GetValue(global::Metalama.Patterns.Xaml.AspectTests.Callbacks.StaticValidateDependencyPropertyAndInstanceAndValue.StaticValidateDependencyPropertyAndInstanceAndValue.AcceptsGenericForValueProperty));
    }
    set
    {
      this.SetValue(global::Metalama.Patterns.Xaml.AspectTests.Callbacks.StaticValidateDependencyPropertyAndInstanceAndValue.StaticValidateDependencyPropertyAndInstanceAndValue.AcceptsGenericForValueProperty, value);
    }
  }
  private static bool ValidateAcceptsGenericForValue<T>(DependencyProperty d, StaticValidateDependencyPropertyAndInstanceAndValue instance, T value) => true;
  [DependencyProperty]
  public int AcceptsObjectForValue
  {
    get
    {
      return ((global::System.Int32)this.GetValue(global::Metalama.Patterns.Xaml.AspectTests.Callbacks.StaticValidateDependencyPropertyAndInstanceAndValue.StaticValidateDependencyPropertyAndInstanceAndValue.AcceptsObjectForValueProperty));
    }
    set
    {
      this.SetValue(global::Metalama.Patterns.Xaml.AspectTests.Callbacks.StaticValidateDependencyPropertyAndInstanceAndValue.StaticValidateDependencyPropertyAndInstanceAndValue.AcceptsObjectForValueProperty, value);
    }
  }
  private static bool ValidateAcceptsObjectForValue(DependencyProperty d, StaticValidateDependencyPropertyAndInstanceAndValue instance, object value) => true;
  [DependencyProperty]
  public int AcceptsDependencyObjectForInstance
  {
    get
    {
      return ((global::System.Int32)this.GetValue(global::Metalama.Patterns.Xaml.AspectTests.Callbacks.StaticValidateDependencyPropertyAndInstanceAndValue.StaticValidateDependencyPropertyAndInstanceAndValue.AcceptsDependencyObjectForInstanceProperty));
    }
    set
    {
      this.SetValue(global::Metalama.Patterns.Xaml.AspectTests.Callbacks.StaticValidateDependencyPropertyAndInstanceAndValue.StaticValidateDependencyPropertyAndInstanceAndValue.AcceptsDependencyObjectForInstanceProperty, value);
    }
  }
  private static bool ValidateAcceptsDependencyObjectForInstance(DependencyProperty d, DependencyObject instance, int value) => true;
  [DependencyProperty]
  public int AcceptsObjectForInstance
  {
    get
    {
      return ((global::System.Int32)this.GetValue(global::Metalama.Patterns.Xaml.AspectTests.Callbacks.StaticValidateDependencyPropertyAndInstanceAndValue.StaticValidateDependencyPropertyAndInstanceAndValue.AcceptsObjectForInstanceProperty));
    }
    set
    {
      this.SetValue(global::Metalama.Patterns.Xaml.AspectTests.Callbacks.StaticValidateDependencyPropertyAndInstanceAndValue.StaticValidateDependencyPropertyAndInstanceAndValue.AcceptsObjectForInstanceProperty, value);
    }
  }
  private static bool ValidateAcceptsObjectForInstance(DependencyProperty d, object instance, int value) => true;
  public static readonly global::System.Windows.DependencyProperty AcceptsAssignableForValueProperty;
  public static readonly global::System.Windows.DependencyProperty AcceptsDependencyObjectForInstanceProperty;
  public static readonly global::System.Windows.DependencyProperty AcceptsGenericForValueProperty;
  public static readonly global::System.Windows.DependencyProperty AcceptsObjectForInstanceProperty;
  public static readonly global::System.Windows.DependencyProperty AcceptsObjectForValueProperty;
  public static readonly global::System.Windows.DependencyProperty FooProperty;
  static StaticValidateDependencyPropertyAndInstanceAndValue()
  {
    object CoerceValue_6(global::System.Windows.DependencyObject d_6, object value_6)
    {
      global::Metalama.Patterns.Xaml.AspectTests.Callbacks.StaticValidateDependencyPropertyAndInstanceAndValue.StaticValidateDependencyPropertyAndInstanceAndValue.ValidateAcceptsObjectForInstance(global::Metalama.Patterns.Xaml.AspectTests.Callbacks.StaticValidateDependencyPropertyAndInstanceAndValue.StaticValidateDependencyPropertyAndInstanceAndValue.AcceptsObjectForInstanceProperty, d_6, (global::System.Int32)value_6);
      return (global::System.Object)value_6;
    }
    var metadata_5 = new global::System.Windows.PropertyMetadata();
    metadata_5.CoerceValueCallback = (global::System.Windows.CoerceValueCallback)CoerceValue_6;
    global::Metalama.Patterns.Xaml.AspectTests.Callbacks.StaticValidateDependencyPropertyAndInstanceAndValue.StaticValidateDependencyPropertyAndInstanceAndValue.AcceptsObjectForInstanceProperty = global::System.Windows.DependencyProperty.Register("AcceptsObjectForInstance", typeof(global::System.Int32), typeof(global::Metalama.Patterns.Xaml.AspectTests.Callbacks.StaticValidateDependencyPropertyAndInstanceAndValue.StaticValidateDependencyPropertyAndInstanceAndValue), metadata_5);
    object CoerceValue_5(global::System.Windows.DependencyObject d_5, object value_5)
    {
      global::Metalama.Patterns.Xaml.AspectTests.Callbacks.StaticValidateDependencyPropertyAndInstanceAndValue.StaticValidateDependencyPropertyAndInstanceAndValue.ValidateAcceptsDependencyObjectForInstance(global::Metalama.Patterns.Xaml.AspectTests.Callbacks.StaticValidateDependencyPropertyAndInstanceAndValue.StaticValidateDependencyPropertyAndInstanceAndValue.AcceptsDependencyObjectForInstanceProperty, d_5, (global::System.Int32)value_5);
      return (global::System.Object)value_5;
    }
    var metadata_4 = new global::System.Windows.PropertyMetadata();
    metadata_4.CoerceValueCallback = (global::System.Windows.CoerceValueCallback)CoerceValue_5;
    global::Metalama.Patterns.Xaml.AspectTests.Callbacks.StaticValidateDependencyPropertyAndInstanceAndValue.StaticValidateDependencyPropertyAndInstanceAndValue.AcceptsDependencyObjectForInstanceProperty = global::System.Windows.DependencyProperty.Register("AcceptsDependencyObjectForInstance", typeof(global::System.Int32), typeof(global::Metalama.Patterns.Xaml.AspectTests.Callbacks.StaticValidateDependencyPropertyAndInstanceAndValue.StaticValidateDependencyPropertyAndInstanceAndValue), metadata_4);
    object CoerceValue_4(global::System.Windows.DependencyObject d_4, object value_4)
    {
      global::Metalama.Patterns.Xaml.AspectTests.Callbacks.StaticValidateDependencyPropertyAndInstanceAndValue.StaticValidateDependencyPropertyAndInstanceAndValue.ValidateAcceptsObjectForValue(global::Metalama.Patterns.Xaml.AspectTests.Callbacks.StaticValidateDependencyPropertyAndInstanceAndValue.StaticValidateDependencyPropertyAndInstanceAndValue.AcceptsObjectForValueProperty, (global::Metalama.Patterns.Xaml.AspectTests.Callbacks.StaticValidateDependencyPropertyAndInstanceAndValue.StaticValidateDependencyPropertyAndInstanceAndValue)d_4, value_4);
      return (global::System.Object)value_4;
    }
    var metadata_3 = new global::System.Windows.PropertyMetadata();
    metadata_3.CoerceValueCallback = (global::System.Windows.CoerceValueCallback)CoerceValue_4;
    global::Metalama.Patterns.Xaml.AspectTests.Callbacks.StaticValidateDependencyPropertyAndInstanceAndValue.StaticValidateDependencyPropertyAndInstanceAndValue.AcceptsObjectForValueProperty = global::System.Windows.DependencyProperty.Register("AcceptsObjectForValue", typeof(global::System.Int32), typeof(global::Metalama.Patterns.Xaml.AspectTests.Callbacks.StaticValidateDependencyPropertyAndInstanceAndValue.StaticValidateDependencyPropertyAndInstanceAndValue), metadata_3);
    object CoerceValue_3(global::System.Windows.DependencyObject d_3, object value_3)
    {
      global::Metalama.Patterns.Xaml.AspectTests.Callbacks.StaticValidateDependencyPropertyAndInstanceAndValue.StaticValidateDependencyPropertyAndInstanceAndValue.ValidateAcceptsGenericForValue<global::System.Int32>(global::Metalama.Patterns.Xaml.AspectTests.Callbacks.StaticValidateDependencyPropertyAndInstanceAndValue.StaticValidateDependencyPropertyAndInstanceAndValue.AcceptsGenericForValueProperty, (global::Metalama.Patterns.Xaml.AspectTests.Callbacks.StaticValidateDependencyPropertyAndInstanceAndValue.StaticValidateDependencyPropertyAndInstanceAndValue)d_3, (global::System.Int32)value_3);
      return (global::System.Object)value_3;
    }
    var metadata_2 = new global::System.Windows.PropertyMetadata();
    metadata_2.CoerceValueCallback = (global::System.Windows.CoerceValueCallback)CoerceValue_3;
    global::Metalama.Patterns.Xaml.AspectTests.Callbacks.StaticValidateDependencyPropertyAndInstanceAndValue.StaticValidateDependencyPropertyAndInstanceAndValue.AcceptsGenericForValueProperty = global::System.Windows.DependencyProperty.Register("AcceptsGenericForValue", typeof(global::System.Int32), typeof(global::Metalama.Patterns.Xaml.AspectTests.Callbacks.StaticValidateDependencyPropertyAndInstanceAndValue.StaticValidateDependencyPropertyAndInstanceAndValue), metadata_2);
    object CoerceValue_2(global::System.Windows.DependencyObject d_2, object value_2)
    {
      global::Metalama.Patterns.Xaml.AspectTests.Callbacks.StaticValidateDependencyPropertyAndInstanceAndValue.StaticValidateDependencyPropertyAndInstanceAndValue.ValidateAcceptsAssignableForValue(global::Metalama.Patterns.Xaml.AspectTests.Callbacks.StaticValidateDependencyPropertyAndInstanceAndValue.StaticValidateDependencyPropertyAndInstanceAndValue.AcceptsAssignableForValueProperty, (global::Metalama.Patterns.Xaml.AspectTests.Callbacks.StaticValidateDependencyPropertyAndInstanceAndValue.StaticValidateDependencyPropertyAndInstanceAndValue)d_2, (global::System.Collections.Generic.List<global::System.Int32>)value_2);
      return (global::System.Object)value_2;
    }
    var metadata_1 = new global::System.Windows.PropertyMetadata();
    metadata_1.CoerceValueCallback = (global::System.Windows.CoerceValueCallback)CoerceValue_2;
    global::Metalama.Patterns.Xaml.AspectTests.Callbacks.StaticValidateDependencyPropertyAndInstanceAndValue.StaticValidateDependencyPropertyAndInstanceAndValue.AcceptsAssignableForValueProperty = global::System.Windows.DependencyProperty.Register("AcceptsAssignableForValue", typeof(global::System.Collections.Generic.List<global::System.Int32>), typeof(global::Metalama.Patterns.Xaml.AspectTests.Callbacks.StaticValidateDependencyPropertyAndInstanceAndValue.StaticValidateDependencyPropertyAndInstanceAndValue), metadata_1);
    object CoerceValue_1(global::System.Windows.DependencyObject d_1, object value_1)
    {
      global::Metalama.Patterns.Xaml.AspectTests.Callbacks.StaticValidateDependencyPropertyAndInstanceAndValue.StaticValidateDependencyPropertyAndInstanceAndValue.ValidateFoo(global::Metalama.Patterns.Xaml.AspectTests.Callbacks.StaticValidateDependencyPropertyAndInstanceAndValue.StaticValidateDependencyPropertyAndInstanceAndValue.FooProperty, (global::Metalama.Patterns.Xaml.AspectTests.Callbacks.StaticValidateDependencyPropertyAndInstanceAndValue.StaticValidateDependencyPropertyAndInstanceAndValue)d_1, (global::System.Int32)value_1);
      return (global::System.Object)value_1;
    }
    var metadata = new global::System.Windows.PropertyMetadata();
    metadata.CoerceValueCallback = (global::System.Windows.CoerceValueCallback)CoerceValue_1;
    global::Metalama.Patterns.Xaml.AspectTests.Callbacks.StaticValidateDependencyPropertyAndInstanceAndValue.StaticValidateDependencyPropertyAndInstanceAndValue.FooProperty = global::System.Windows.DependencyProperty.Register("Foo", typeof(global::System.Int32), typeof(global::Metalama.Patterns.Xaml.AspectTests.Callbacks.StaticValidateDependencyPropertyAndInstanceAndValue.StaticValidateDependencyPropertyAndInstanceAndValue), metadata);
  }
}