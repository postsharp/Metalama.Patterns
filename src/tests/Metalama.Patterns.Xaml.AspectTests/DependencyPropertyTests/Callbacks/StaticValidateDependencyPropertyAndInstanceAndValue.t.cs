    // Warning LAMA5206 on `Foo`: `No property-changed method was found using the default naming convention, with candidate member name 'OnFooChanged'.`
// Warning LAMA5206 on `Foo`: `No property-changing method was found using the default naming convention, with candidate member name 'OnFooChanging'.`
// Warning LAMA5206 on `AcceptsAssignableForValue`: `No property-changed method was found using the default naming convention, with candidate member name 'OnAcceptsAssignableForValueChanged'.`
// Warning LAMA5206 on `AcceptsAssignableForValue`: `No property-changing method was found using the default naming convention, with candidate member name 'OnAcceptsAssignableForValueChanging'.`
// Warning LAMA5206 on `AcceptsGenericForValue`: `No property-changed method was found using the default naming convention, with candidate member name 'OnAcceptsGenericForValueChanged'.`
// Warning LAMA5206 on `AcceptsGenericForValue`: `No property-changing method was found using the default naming convention, with candidate member name 'OnAcceptsGenericForValueChanging'.`
// Warning LAMA5206 on `AcceptsObjectForValue`: `No property-changed method was found using the default naming convention, with candidate member name 'OnAcceptsObjectForValueChanged'.`
// Warning LAMA5206 on `AcceptsObjectForValue`: `No property-changing method was found using the default naming convention, with candidate member name 'OnAcceptsObjectForValueChanging'.`
// Warning LAMA5206 on `AcceptsDependencyObjectForInstance`: `No property-changed method was found using the default naming convention, with candidate member name 'OnAcceptsDependencyObjectForInstanceChanged'.`
// Warning LAMA5206 on `AcceptsDependencyObjectForInstance`: `No property-changing method was found using the default naming convention, with candidate member name 'OnAcceptsDependencyObjectForInstanceChanging'.`
// Warning LAMA5206 on `AcceptsObjectForInstance`: `No property-changed method was found using the default naming convention, with candidate member name 'OnAcceptsObjectForInstanceChanged'.`
// Warning LAMA5206 on `AcceptsObjectForInstance`: `No property-changing method was found using the default naming convention, with candidate member name 'OnAcceptsObjectForInstanceChanging'.`
using System.Windows;
namespace Metalama.Patterns.Xaml.AspectTests.DependencyPropertyTests.Callbacks.StaticValidateDependencyPropertyAndInstanceAndValue;
public partial class StaticValidateDependencyPropertyAndInstanceAndValue : DependencyObject
{
  [DependencyProperty]
  public int Foo
  {
    get
    {
      return (int)GetValue(FooProperty);
    }
    set
    {
      this.SetValue(StaticValidateDependencyPropertyAndInstanceAndValue.FooProperty, value);
    }
  }
  private static bool ValidateFoo(DependencyProperty d, StaticValidateDependencyPropertyAndInstanceAndValue instance, int value) => true;
  [DependencyProperty]
  public List<int> AcceptsAssignableForValue
  {
    get
    {
      return (List<int>)GetValue(AcceptsAssignableForValueProperty);
    }
    set
    {
      this.SetValue(StaticValidateDependencyPropertyAndInstanceAndValue.AcceptsAssignableForValueProperty, value);
    }
  }
  private static bool ValidateAcceptsAssignableForValue(DependencyProperty d, StaticValidateDependencyPropertyAndInstanceAndValue instance, IEnumerable<int> value) => true;
  [DependencyProperty]
  public int AcceptsGenericForValue
  {
    get
    {
      return (int)GetValue(AcceptsGenericForValueProperty);
    }
    set
    {
      this.SetValue(StaticValidateDependencyPropertyAndInstanceAndValue.AcceptsGenericForValueProperty, value);
    }
  }
  private static bool ValidateAcceptsGenericForValue<T>(DependencyProperty d, StaticValidateDependencyPropertyAndInstanceAndValue instance, T value) => true;
  [DependencyProperty]
  public int AcceptsObjectForValue
  {
    get
    {
      return (int)GetValue(AcceptsObjectForValueProperty);
    }
    set
    {
      this.SetValue(StaticValidateDependencyPropertyAndInstanceAndValue.AcceptsObjectForValueProperty, value);
    }
  }
  private static bool ValidateAcceptsObjectForValue(DependencyProperty d, StaticValidateDependencyPropertyAndInstanceAndValue instance, object value) => true;
  [DependencyProperty]
  public int AcceptsDependencyObjectForInstance
  {
    get
    {
      return (int)GetValue(AcceptsDependencyObjectForInstanceProperty);
    }
    set
    {
      this.SetValue(StaticValidateDependencyPropertyAndInstanceAndValue.AcceptsDependencyObjectForInstanceProperty, value);
    }
  }
  private static bool ValidateAcceptsDependencyObjectForInstance(DependencyProperty d, DependencyObject instance, int value) => true;
  [DependencyProperty]
  public int AcceptsObjectForInstance
  {
    get
    {
      return (int)GetValue(AcceptsObjectForInstanceProperty);
    }
    set
    {
      this.SetValue(StaticValidateDependencyPropertyAndInstanceAndValue.AcceptsObjectForInstanceProperty, value);
    }
  }
  private static bool ValidateAcceptsObjectForInstance(DependencyProperty d, object instance, int value) => true;
  public static readonly DependencyProperty AcceptsAssignableForValueProperty;
  public static readonly DependencyProperty AcceptsDependencyObjectForInstanceProperty;
  public static readonly DependencyProperty AcceptsGenericForValueProperty;
  public static readonly DependencyProperty AcceptsObjectForInstanceProperty;
  public static readonly DependencyProperty AcceptsObjectForValueProperty;
  public static readonly DependencyProperty FooProperty;
  static StaticValidateDependencyPropertyAndInstanceAndValue()
  {
    object CoerceValue_1(DependencyObject d_1, object value_1)
    {
      if (!StaticValidateDependencyPropertyAndInstanceAndValue.ValidateFoo(StaticValidateDependencyPropertyAndInstanceAndValue.FooProperty, (StaticValidateDependencyPropertyAndInstanceAndValue)d_1, (int)value_1))
      {
        throw new ArgumentException("Invalid property value.", "value");
      }
      return value_1;
    }
    var metadata = new PropertyMetadata();
    metadata.CoerceValueCallback = CoerceValue_1;
    StaticValidateDependencyPropertyAndInstanceAndValue.FooProperty = DependencyProperty.Register("Foo", typeof(int), typeof(StaticValidateDependencyPropertyAndInstanceAndValue), metadata);
    object CoerceValue_2(DependencyObject d_2, object value_2)
    {
      if (!StaticValidateDependencyPropertyAndInstanceAndValue.ValidateAcceptsAssignableForValue(StaticValidateDependencyPropertyAndInstanceAndValue.AcceptsAssignableForValueProperty, (StaticValidateDependencyPropertyAndInstanceAndValue)d_2, (List<int>)value_2))
      {
        throw new ArgumentException("Invalid property value.", "value");
      }
      return value_2;
    }
    var metadata_1 = new PropertyMetadata();
    metadata_1.CoerceValueCallback = CoerceValue_2;
    StaticValidateDependencyPropertyAndInstanceAndValue.AcceptsAssignableForValueProperty = DependencyProperty.Register("AcceptsAssignableForValue", typeof(List<int>), typeof(StaticValidateDependencyPropertyAndInstanceAndValue), metadata_1);
    object CoerceValue_3(DependencyObject d_3, object value_3)
    {
      if (!StaticValidateDependencyPropertyAndInstanceAndValue.ValidateAcceptsGenericForValue<int>(StaticValidateDependencyPropertyAndInstanceAndValue.AcceptsGenericForValueProperty, (StaticValidateDependencyPropertyAndInstanceAndValue)d_3, (int)value_3))
      {
        throw new ArgumentException("Invalid property value.", "value");
      }
      return value_3;
    }
    var metadata_2 = new PropertyMetadata();
    metadata_2.CoerceValueCallback = CoerceValue_3;
    StaticValidateDependencyPropertyAndInstanceAndValue.AcceptsGenericForValueProperty = DependencyProperty.Register("AcceptsGenericForValue", typeof(int), typeof(StaticValidateDependencyPropertyAndInstanceAndValue), metadata_2);
    object CoerceValue_4(DependencyObject d_4, object value_4)
    {
      if (!StaticValidateDependencyPropertyAndInstanceAndValue.ValidateAcceptsObjectForValue(StaticValidateDependencyPropertyAndInstanceAndValue.AcceptsObjectForValueProperty, (StaticValidateDependencyPropertyAndInstanceAndValue)d_4, value_4))
      {
        throw new ArgumentException("Invalid property value.", "value");
      }
      return value_4;
    }
    var metadata_3 = new PropertyMetadata();
    metadata_3.CoerceValueCallback = CoerceValue_4;
    StaticValidateDependencyPropertyAndInstanceAndValue.AcceptsObjectForValueProperty = DependencyProperty.Register("AcceptsObjectForValue", typeof(int), typeof(StaticValidateDependencyPropertyAndInstanceAndValue), metadata_3);
    object CoerceValue_5(DependencyObject d_5, object value_5)
    {
      if (!StaticValidateDependencyPropertyAndInstanceAndValue.ValidateAcceptsDependencyObjectForInstance(StaticValidateDependencyPropertyAndInstanceAndValue.AcceptsDependencyObjectForInstanceProperty, d_5, (int)value_5))
      {
        throw new ArgumentException("Invalid property value.", "value");
      }
      return value_5;
    }
    var metadata_4 = new PropertyMetadata();
    metadata_4.CoerceValueCallback = CoerceValue_5;
    StaticValidateDependencyPropertyAndInstanceAndValue.AcceptsDependencyObjectForInstanceProperty = DependencyProperty.Register("AcceptsDependencyObjectForInstance", typeof(int), typeof(StaticValidateDependencyPropertyAndInstanceAndValue), metadata_4);
    object CoerceValue_6(DependencyObject d_6, object value_6)
    {
      if (!StaticValidateDependencyPropertyAndInstanceAndValue.ValidateAcceptsObjectForInstance(StaticValidateDependencyPropertyAndInstanceAndValue.AcceptsObjectForInstanceProperty, d_6, (int)value_6))
      {
        throw new ArgumentException("Invalid property value.", "value");
      }
      return value_6;
    }
    var metadata_5 = new PropertyMetadata();
    metadata_5.CoerceValueCallback = CoerceValue_6;
    StaticValidateDependencyPropertyAndInstanceAndValue.AcceptsObjectForInstanceProperty = DependencyProperty.Register("AcceptsObjectForInstance", typeof(int), typeof(StaticValidateDependencyPropertyAndInstanceAndValue), metadata_5);
  }
}