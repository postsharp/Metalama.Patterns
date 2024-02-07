// Warning LAMA5206 on `Foo`: `No property-changed method was found using the default naming convention, with candidate member name 'OnFooChanged'.`
// Warning LAMA5206 on `Foo`: `No property-changing method was found using the default naming convention, with candidate member name 'OnFooChanging'.`
// Warning LAMA5206 on `AcceptAssignable`: `No property-changed method was found using the default naming convention, with candidate member name 'OnAcceptAssignableChanged'.`
// Warning LAMA5206 on `AcceptAssignable`: `No property-changing method was found using the default naming convention, with candidate member name 'OnAcceptAssignableChanging'.`
// Warning LAMA5206 on `AcceptGeneric`: `No property-changed method was found using the default naming convention, with candidate member name 'OnAcceptGenericChanged'.`
// Warning LAMA5206 on `AcceptGeneric`: `No property-changing method was found using the default naming convention, with candidate member name 'OnAcceptGenericChanging'.`
// Warning LAMA5206 on `AcceptObject`: `No property-changed method was found using the default naming convention, with candidate member name 'OnAcceptObjectChanged'.`
// Warning LAMA5206 on `AcceptObject`: `No property-changing method was found using the default naming convention, with candidate member name 'OnAcceptObjectChanging'.`
using System.Windows;
namespace Metalama.Patterns.Xaml.AspectTests.DependencyPropertyTests.Callbacks.InstanceValidateValue;
public partial class InstanceValidateValue : DependencyObject
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
      this.SetValue(InstanceValidateValue.FooProperty, value);
    }
  }
  private bool ValidateFoo(int value) => true;
  [DependencyProperty]
  public List<int> AcceptAssignable
  {
    get
    {
      return (List<int>)GetValue(AcceptAssignableProperty);
    }
    set
    {
      this.SetValue(InstanceValidateValue.AcceptAssignableProperty, value);
    }
  }
  private bool ValidateAcceptAssignable(IEnumerable<int> value) => true;
  [DependencyProperty]
  public int AcceptGeneric
  {
    get
    {
      return (int)GetValue(AcceptGenericProperty);
    }
    set
    {
      this.SetValue(InstanceValidateValue.AcceptGenericProperty, value);
    }
  }
  private bool ValidateAcceptGeneric<T>(T value) => true;
  [DependencyProperty]
  public int AcceptObject
  {
    get
    {
      return (int)GetValue(AcceptObjectProperty);
    }
    set
    {
      this.SetValue(InstanceValidateValue.AcceptObjectProperty, value);
    }
  }
  private bool ValidateAcceptObject(object value) => true;
  public static readonly DependencyProperty AcceptAssignableProperty;
  public static readonly DependencyProperty AcceptGenericProperty;
  public static readonly DependencyProperty AcceptObjectProperty;
  public static readonly DependencyProperty FooProperty;
  static InstanceValidateValue()
  {
    object CoerceValue_1(DependencyObject d, object value_1)
    {
      if (!((InstanceValidateValue)d).ValidateFoo((int)value_1))
      {
        throw new ArgumentException("Invalid property value.", "value");
      }
      return value_1;
    }
    var metadata = new PropertyMetadata();
    metadata.CoerceValueCallback = CoerceValue_1;
    InstanceValidateValue.FooProperty = DependencyProperty.Register("Foo", typeof(int), typeof(InstanceValidateValue), metadata);
    object CoerceValue_2(DependencyObject d_1, object value_2)
    {
      if (!((InstanceValidateValue)d_1).ValidateAcceptAssignable((List<int>)value_2))
      {
        throw new ArgumentException("Invalid property value.", "value");
      }
      return value_2;
    }
    var metadata_1 = new PropertyMetadata();
    metadata_1.CoerceValueCallback = CoerceValue_2;
    InstanceValidateValue.AcceptAssignableProperty = DependencyProperty.Register("AcceptAssignable", typeof(List<int>), typeof(InstanceValidateValue), metadata_1);
    object CoerceValue_3(DependencyObject d_2, object value_3)
    {
      if (!((InstanceValidateValue)d_2).ValidateAcceptGeneric<int>((int)value_3))
      {
        throw new ArgumentException("Invalid property value.", "value");
      }
      return value_3;
    }
    var metadata_2 = new PropertyMetadata();
    metadata_2.CoerceValueCallback = CoerceValue_3;
    InstanceValidateValue.AcceptGenericProperty = DependencyProperty.Register("AcceptGeneric", typeof(int), typeof(InstanceValidateValue), metadata_2);
    object CoerceValue_4(DependencyObject d_3, object value_4)
    {
      if (!((InstanceValidateValue)d_3).ValidateAcceptObject(value_4))
      {
        throw new ArgumentException("Invalid property value.", "value");
      }
      return value_4;
    }
    var metadata_3 = new PropertyMetadata();
    metadata_3.CoerceValueCallback = CoerceValue_4;
    InstanceValidateValue.AcceptObjectProperty = DependencyProperty.Register("AcceptObject", typeof(int), typeof(InstanceValidateValue), metadata_3);
  }
}