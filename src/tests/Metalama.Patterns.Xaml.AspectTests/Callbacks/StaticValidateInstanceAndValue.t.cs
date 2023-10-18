using System.Windows;
namespace Metalama.Patterns.Xaml.AspectTests.Callbacks.StaticValidateInstanceAndValue;
public partial class StaticValidateInstanceAndValue : DependencyObject
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
      this.SetValue(StaticValidateInstanceAndValue.FooProperty, value);
    }
  }
  private static bool ValidateFoo(StaticValidateInstanceAndValue instance, int value) => true;
  [DependencyProperty]
  public List<int> AcceptsAssignableForValue
  {
    get
    {
      return (List<int>)GetValue(AcceptsAssignableForValueProperty);
    }
    set
    {
      this.SetValue(StaticValidateInstanceAndValue.AcceptsAssignableForValueProperty, value);
    }
  }
  private static bool ValidateAcceptsAssignableForValue(StaticValidateInstanceAndValue instance, IEnumerable<int> value) => true;
  [DependencyProperty]
  public int AcceptsGenericForValue
  {
    get
    {
      return (int)GetValue(AcceptsGenericForValueProperty);
    }
    set
    {
      this.SetValue(StaticValidateInstanceAndValue.AcceptsGenericForValueProperty, value);
    }
  }
  private static bool ValidateAcceptsGenericForValue<T>(StaticValidateInstanceAndValue instance, T value) => true;
  [DependencyProperty]
  public int AcceptsObjectForValue
  {
    get
    {
      return (int)GetValue(AcceptsObjectForValueProperty);
    }
    set
    {
      this.SetValue(StaticValidateInstanceAndValue.AcceptsObjectForValueProperty, value);
    }
  }
  private static bool ValidateAcceptsObjectForValue(StaticValidateInstanceAndValue instance, object value) => true;
  [DependencyProperty]
  public int AcceptsDependencyObjectForInstance
  {
    get
    {
      return (int)GetValue(AcceptsDependencyObjectForInstanceProperty);
    }
    set
    {
      this.SetValue(StaticValidateInstanceAndValue.AcceptsDependencyObjectForInstanceProperty, value);
    }
  }
  private static bool ValidateAcceptsDependencyObjectForInstance(DependencyObject instance, int value) => true;
  [DependencyProperty]
  public int AcceptsObjectForInstance
  {
    get
    {
      return (int)GetValue(AcceptsObjectForInstanceProperty);
    }
    set
    {
      this.SetValue(StaticValidateInstanceAndValue.AcceptsObjectForInstanceProperty, value);
    }
  }
  private static bool ValidateAcceptsObjectForInstance(object instance, int value) => true;
  public static readonly DependencyProperty AcceptsAssignableForValueProperty;
  public static readonly DependencyProperty AcceptsDependencyObjectForInstanceProperty;
  public static readonly DependencyProperty AcceptsGenericForValueProperty;
  public static readonly DependencyProperty AcceptsObjectForInstanceProperty;
  public static readonly DependencyProperty AcceptsObjectForValueProperty;
  public static readonly DependencyProperty FooProperty;
  static StaticValidateInstanceAndValue()
  {
    object CoerceValue_6(DependencyObject d_5, object value_6)
    {
      if (!StaticValidateInstanceAndValue.ValidateAcceptsObjectForInstance(d_5, (int)value_6))
      {
        throw new ArgumentException("Invalid property value.", "value");
      }
      return value_6;
    }
    var metadata_5 = new PropertyMetadata();
    metadata_5.CoerceValueCallback = CoerceValue_6;
    StaticValidateInstanceAndValue.AcceptsObjectForInstanceProperty = DependencyProperty.Register("AcceptsObjectForInstance", typeof(int), typeof(StaticValidateInstanceAndValue), metadata_5);
    object CoerceValue_5(DependencyObject d_4, object value_5)
    {
      if (!StaticValidateInstanceAndValue.ValidateAcceptsDependencyObjectForInstance(d_4, (int)value_5))
      {
        throw new ArgumentException("Invalid property value.", "value");
      }
      return value_5;
    }
    var metadata_4 = new PropertyMetadata();
    metadata_4.CoerceValueCallback = CoerceValue_5;
    StaticValidateInstanceAndValue.AcceptsDependencyObjectForInstanceProperty = DependencyProperty.Register("AcceptsDependencyObjectForInstance", typeof(int), typeof(StaticValidateInstanceAndValue), metadata_4);
    object CoerceValue_4(DependencyObject d_3, object value_4)
    {
      if (!StaticValidateInstanceAndValue.ValidateAcceptsObjectForValue((StaticValidateInstanceAndValue)d_3, value_4))
      {
        throw new ArgumentException("Invalid property value.", "value");
      }
      return value_4;
    }
    var metadata_3 = new PropertyMetadata();
    metadata_3.CoerceValueCallback = CoerceValue_4;
    StaticValidateInstanceAndValue.AcceptsObjectForValueProperty = DependencyProperty.Register("AcceptsObjectForValue", typeof(int), typeof(StaticValidateInstanceAndValue), metadata_3);
    object CoerceValue_3(DependencyObject d_2, object value_3)
    {
      if (!StaticValidateInstanceAndValue.ValidateAcceptsGenericForValue<int>((StaticValidateInstanceAndValue)d_2, (int)value_3))
      {
        throw new ArgumentException("Invalid property value.", "value");
      }
      return value_3;
    }
    var metadata_2 = new PropertyMetadata();
    metadata_2.CoerceValueCallback = CoerceValue_3;
    StaticValidateInstanceAndValue.AcceptsGenericForValueProperty = DependencyProperty.Register("AcceptsGenericForValue", typeof(int), typeof(StaticValidateInstanceAndValue), metadata_2);
    object CoerceValue_2(DependencyObject d_1, object value_2)
    {
      if (!StaticValidateInstanceAndValue.ValidateAcceptsAssignableForValue((StaticValidateInstanceAndValue)d_1, (List<int>)value_2))
      {
        throw new ArgumentException("Invalid property value.", "value");
      }
      return value_2;
    }
    var metadata_1 = new PropertyMetadata();
    metadata_1.CoerceValueCallback = CoerceValue_2;
    StaticValidateInstanceAndValue.AcceptsAssignableForValueProperty = DependencyProperty.Register("AcceptsAssignableForValue", typeof(List<int>), typeof(StaticValidateInstanceAndValue), metadata_1);
    object CoerceValue_1(DependencyObject d, object value_1)
    {
      if (!StaticValidateInstanceAndValue.ValidateFoo((StaticValidateInstanceAndValue)d, (int)value_1))
      {
        throw new ArgumentException("Invalid property value.", "value");
      }
      return value_1;
    }
    var metadata = new PropertyMetadata();
    metadata.CoerceValueCallback = CoerceValue_1;
    StaticValidateInstanceAndValue.FooProperty = DependencyProperty.Register("Foo", typeof(int), typeof(StaticValidateInstanceAndValue), metadata);
  }
}