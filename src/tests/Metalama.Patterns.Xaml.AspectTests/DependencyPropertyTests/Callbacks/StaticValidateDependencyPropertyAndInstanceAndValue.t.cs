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
  public static readonly DependencyProperty AcceptsAssignableForValueProperty = StaticValidateDependencyPropertyAndInstanceAndValue.CreateAcceptsAssignableForValueProperty();
  public static readonly DependencyProperty AcceptsDependencyObjectForInstanceProperty = StaticValidateDependencyPropertyAndInstanceAndValue.CreateAcceptsDependencyObjectForInstanceProperty();
  public static readonly DependencyProperty AcceptsGenericForValueProperty = StaticValidateDependencyPropertyAndInstanceAndValue.CreateAcceptsGenericForValueProperty();
  public static readonly DependencyProperty AcceptsObjectForInstanceProperty = StaticValidateDependencyPropertyAndInstanceAndValue.CreateAcceptsObjectForInstanceProperty();
  public static readonly DependencyProperty AcceptsObjectForValueProperty = StaticValidateDependencyPropertyAndInstanceAndValue.CreateAcceptsObjectForValueProperty();
  public static readonly DependencyProperty FooProperty = StaticValidateDependencyPropertyAndInstanceAndValue.CreateFooProperty();
  private static DependencyProperty CreateAcceptsAssignableForValueProperty()
  {
    object CoerceValue_1(DependencyObject d, object value)
    {
      if (!StaticValidateDependencyPropertyAndInstanceAndValue.ValidateAcceptsAssignableForValue(StaticValidateDependencyPropertyAndInstanceAndValue.AcceptsAssignableForValueProperty, (StaticValidateDependencyPropertyAndInstanceAndValue)d, (List<int>)value))
      {
        throw new ArgumentException("Invalid property value.", "value");
      }
      return value;
    }
    var metadata = new PropertyMetadata();
    metadata.CoerceValueCallback = CoerceValue_1;
    return DependencyProperty.Register("AcceptsAssignableForValue", typeof(List<int>), typeof(StaticValidateDependencyPropertyAndInstanceAndValue), metadata);
  }
  private static DependencyProperty CreateAcceptsDependencyObjectForInstanceProperty()
  {
    object CoerceValue_1(DependencyObject d, object value)
    {
      if (!StaticValidateDependencyPropertyAndInstanceAndValue.ValidateAcceptsDependencyObjectForInstance(StaticValidateDependencyPropertyAndInstanceAndValue.AcceptsDependencyObjectForInstanceProperty, d, (int)value))
      {
        throw new ArgumentException("Invalid property value.", "value");
      }
      return value;
    }
    var metadata = new PropertyMetadata();
    metadata.CoerceValueCallback = CoerceValue_1;
    return DependencyProperty.Register("AcceptsDependencyObjectForInstance", typeof(int), typeof(StaticValidateDependencyPropertyAndInstanceAndValue), metadata);
  }
  private static DependencyProperty CreateAcceptsGenericForValueProperty()
  {
    object CoerceValue_1(DependencyObject d, object value)
    {
      if (!StaticValidateDependencyPropertyAndInstanceAndValue.ValidateAcceptsGenericForValue<int>(StaticValidateDependencyPropertyAndInstanceAndValue.AcceptsGenericForValueProperty, (StaticValidateDependencyPropertyAndInstanceAndValue)d, (int)value))
      {
        throw new ArgumentException("Invalid property value.", "value");
      }
      return value;
    }
    var metadata = new PropertyMetadata();
    metadata.CoerceValueCallback = CoerceValue_1;
    return DependencyProperty.Register("AcceptsGenericForValue", typeof(int), typeof(StaticValidateDependencyPropertyAndInstanceAndValue), metadata);
  }
  private static DependencyProperty CreateAcceptsObjectForInstanceProperty()
  {
    object CoerceValue_1(DependencyObject d, object value)
    {
      if (!StaticValidateDependencyPropertyAndInstanceAndValue.ValidateAcceptsObjectForInstance(StaticValidateDependencyPropertyAndInstanceAndValue.AcceptsObjectForInstanceProperty, d, (int)value))
      {
        throw new ArgumentException("Invalid property value.", "value");
      }
      return value;
    }
    var metadata = new PropertyMetadata();
    metadata.CoerceValueCallback = CoerceValue_1;
    return DependencyProperty.Register("AcceptsObjectForInstance", typeof(int), typeof(StaticValidateDependencyPropertyAndInstanceAndValue), metadata);
  }
  private static DependencyProperty CreateAcceptsObjectForValueProperty()
  {
    object CoerceValue_1(DependencyObject d, object value)
    {
      if (!StaticValidateDependencyPropertyAndInstanceAndValue.ValidateAcceptsObjectForValue(StaticValidateDependencyPropertyAndInstanceAndValue.AcceptsObjectForValueProperty, (StaticValidateDependencyPropertyAndInstanceAndValue)d, value))
      {
        throw new ArgumentException("Invalid property value.", "value");
      }
      return value;
    }
    var metadata = new PropertyMetadata();
    metadata.CoerceValueCallback = CoerceValue_1;
    return DependencyProperty.Register("AcceptsObjectForValue", typeof(int), typeof(StaticValidateDependencyPropertyAndInstanceAndValue), metadata);
  }
  private static DependencyProperty CreateFooProperty()
  {
    object CoerceValue_1(DependencyObject d, object value)
    {
      if (!StaticValidateDependencyPropertyAndInstanceAndValue.ValidateFoo(StaticValidateDependencyPropertyAndInstanceAndValue.FooProperty, (StaticValidateDependencyPropertyAndInstanceAndValue)d, (int)value))
      {
        throw new ArgumentException("Invalid property value.", "value");
      }
      return value;
    }
    var metadata = new PropertyMetadata();
    metadata.CoerceValueCallback = CoerceValue_1;
    return DependencyProperty.Register("Foo", typeof(int), typeof(StaticValidateDependencyPropertyAndInstanceAndValue), metadata);
  }
}