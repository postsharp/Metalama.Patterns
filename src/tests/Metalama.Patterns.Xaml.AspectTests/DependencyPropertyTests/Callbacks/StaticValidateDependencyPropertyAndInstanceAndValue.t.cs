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
      this.SetValue(FooProperty, value);
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
      this.SetValue(AcceptsAssignableForValueProperty, value);
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
      this.SetValue(AcceptsGenericForValueProperty, value);
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
      this.SetValue(AcceptsObjectForValueProperty, value);
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
      this.SetValue(AcceptsDependencyObjectForInstanceProperty, value);
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
      this.SetValue(AcceptsObjectForInstanceProperty, value);
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
    var metadata = new PropertyMetadata();
    metadata.CoerceValueCallback = new CoerceValueCallback((d_1, value_1) =>
    {
      if (!ValidateFoo(FooProperty, (StaticValidateDependencyPropertyAndInstanceAndValue)d_1, (int)value_1))
      {
        throw new ArgumentException("Invalid property value.", "value");
      }
      return value_1;
    });
    FooProperty = DependencyProperty.Register("Foo", typeof(int), typeof(StaticValidateDependencyPropertyAndInstanceAndValue), metadata);
    var metadata_1 = new PropertyMetadata();
    metadata_1.CoerceValueCallback = new CoerceValueCallback((d_2, value_2) =>
    {
      if (!ValidateAcceptsAssignableForValue(AcceptsAssignableForValueProperty, (StaticValidateDependencyPropertyAndInstanceAndValue)d_2, (List<int>)value_2))
      {
        throw new ArgumentException("Invalid property value.", "value");
      }
      return value_2;
    });
    AcceptsAssignableForValueProperty = DependencyProperty.Register("AcceptsAssignableForValue", typeof(List<int>), typeof(StaticValidateDependencyPropertyAndInstanceAndValue), metadata_1);
    var metadata_2 = new PropertyMetadata();
    metadata_2.CoerceValueCallback = new CoerceValueCallback((d_3, value_3) =>
    {
      if (!ValidateAcceptsGenericForValue(AcceptsGenericForValueProperty, (StaticValidateDependencyPropertyAndInstanceAndValue)d_3, (int)value_3))
      {
        throw new ArgumentException("Invalid property value.", "value");
      }
      return value_3;
    });
    AcceptsGenericForValueProperty = DependencyProperty.Register("AcceptsGenericForValue", typeof(int), typeof(StaticValidateDependencyPropertyAndInstanceAndValue), metadata_2);
    var metadata_3 = new PropertyMetadata();
    metadata_3.CoerceValueCallback = new CoerceValueCallback((d_4, value_4) =>
    {
      if (!ValidateAcceptsObjectForValue(AcceptsObjectForValueProperty, (StaticValidateDependencyPropertyAndInstanceAndValue)d_4, value_4))
      {
        throw new ArgumentException("Invalid property value.", "value");
      }
      return value_4;
    });
    AcceptsObjectForValueProperty = DependencyProperty.Register("AcceptsObjectForValue", typeof(int), typeof(StaticValidateDependencyPropertyAndInstanceAndValue), metadata_3);
    var metadata_4 = new PropertyMetadata();
    metadata_4.CoerceValueCallback = new CoerceValueCallback((d_5, value_5) =>
    {
      if (!ValidateAcceptsDependencyObjectForInstance(AcceptsDependencyObjectForInstanceProperty, d_5, (int)value_5))
      {
        throw new ArgumentException("Invalid property value.", "value");
      }
      return value_5;
    });
    AcceptsDependencyObjectForInstanceProperty = DependencyProperty.Register("AcceptsDependencyObjectForInstance", typeof(int), typeof(StaticValidateDependencyPropertyAndInstanceAndValue), metadata_4);
    var metadata_5 = new PropertyMetadata();
    metadata_5.CoerceValueCallback = new CoerceValueCallback((d_6, value_6) =>
    {
      if (!ValidateAcceptsObjectForInstance(AcceptsObjectForInstanceProperty, d_6, (int)value_6))
      {
        throw new ArgumentException("Invalid property value.", "value");
      }
      return value_6;
    });
    AcceptsObjectForInstanceProperty = DependencyProperty.Register("AcceptsObjectForInstance", typeof(int), typeof(StaticValidateDependencyPropertyAndInstanceAndValue), metadata_5);
  }
}