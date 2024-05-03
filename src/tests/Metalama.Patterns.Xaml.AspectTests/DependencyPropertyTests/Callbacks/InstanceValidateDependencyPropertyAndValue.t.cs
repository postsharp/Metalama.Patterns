// Warning LAMA5206 on `Foo`: `No property-changed method was found using the default naming convention, with candidate member name 'OnFooChanged'.`
// Warning LAMA5206 on `Foo`: `No property-changing method was found using the default naming convention, with candidate member name 'OnFooChanging'.`
// Warning LAMA5206 on `AcceptAssignable`: `No property-changed method was found using the default naming convention, with candidate member name 'OnAcceptAssignableChanged'.`
// Warning LAMA5206 on `AcceptAssignable`: `No property-changing method was found using the default naming convention, with candidate member name 'OnAcceptAssignableChanging'.`
// Warning LAMA5206 on `AcceptGeneric`: `No property-changed method was found using the default naming convention, with candidate member name 'OnAcceptGenericChanged'.`
// Warning LAMA5206 on `AcceptGeneric`: `No property-changing method was found using the default naming convention, with candidate member name 'OnAcceptGenericChanging'.`
// Warning LAMA5206 on `AcceptObject`: `No property-changed method was found using the default naming convention, with candidate member name 'OnAcceptObjectChanged'.`
// Warning LAMA5206 on `AcceptObject`: `No property-changing method was found using the default naming convention, with candidate member name 'OnAcceptObjectChanging'.`
using System.Windows;
namespace Metalama.Patterns.Xaml.AspectTests.DependencyPropertyTests.Callbacks.InstanceValidateDependencyPropertyAndValue;
public partial class InstanceValidateDependencyPropertyAndValue : DependencyObject
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
  private bool ValidateFoo(DependencyProperty d, int value) => true;
  [DependencyProperty]
  public List<int> AcceptAssignable
  {
    get
    {
      return (List<int>)GetValue(AcceptAssignableProperty);
    }
    set
    {
      this.SetValue(AcceptAssignableProperty, value);
    }
  }
  private bool ValidateAcceptAssignable(DependencyProperty d, IEnumerable<int> value) => true;
  [DependencyProperty]
  public int AcceptGeneric
  {
    get
    {
      return (int)GetValue(AcceptGenericProperty);
    }
    set
    {
      this.SetValue(AcceptGenericProperty, value);
    }
  }
  private bool ValidateAcceptGeneric<T>(DependencyProperty d, T value) => true;
  [DependencyProperty]
  public int AcceptObject
  {
    get
    {
      return (int)GetValue(AcceptObjectProperty);
    }
    set
    {
      this.SetValue(AcceptObjectProperty, value);
    }
  }
  private bool ValidateAcceptObject(DependencyProperty d, object value) => true;
  public static readonly DependencyProperty AcceptAssignableProperty;
  public static readonly DependencyProperty AcceptGenericProperty;
  public static readonly DependencyProperty AcceptObjectProperty;
  public static readonly DependencyProperty FooProperty;
  static InstanceValidateDependencyPropertyAndValue()
  {
    var metadata = new PropertyMetadata();
    metadata.CoerceValueCallback = new CoerceValueCallback((d_1, value_1) =>
    {
      if (!((InstanceValidateDependencyPropertyAndValue)d_1).ValidateFoo(FooProperty, (int)value_1))
      {
        throw new ArgumentException("Invalid property value.", "value");
      }
      return value_1;
    });
    FooProperty = DependencyProperty.Register("Foo", typeof(int), typeof(InstanceValidateDependencyPropertyAndValue), metadata);
    var metadata_1 = new PropertyMetadata();
    metadata_1.CoerceValueCallback = new CoerceValueCallback((d_2, value_2) =>
    {
      if (!((InstanceValidateDependencyPropertyAndValue)d_2).ValidateAcceptAssignable(AcceptAssignableProperty, (List<int>)value_2))
      {
        throw new ArgumentException("Invalid property value.", "value");
      }
      return value_2;
    });
    AcceptAssignableProperty = DependencyProperty.Register("AcceptAssignable", typeof(List<int>), typeof(InstanceValidateDependencyPropertyAndValue), metadata_1);
    var metadata_2 = new PropertyMetadata();
    metadata_2.CoerceValueCallback = new CoerceValueCallback((d_3, value_3) =>
    {
      if (!((InstanceValidateDependencyPropertyAndValue)d_3).ValidateAcceptGeneric(AcceptGenericProperty, (int)value_3))
      {
        throw new ArgumentException("Invalid property value.", "value");
      }
      return value_3;
    });
    AcceptGenericProperty = DependencyProperty.Register("AcceptGeneric", typeof(int), typeof(InstanceValidateDependencyPropertyAndValue), metadata_2);
    var metadata_3 = new PropertyMetadata();
    metadata_3.CoerceValueCallback = new CoerceValueCallback((d_4, value_4) =>
    {
      if (!((InstanceValidateDependencyPropertyAndValue)d_4).ValidateAcceptObject(AcceptObjectProperty, value_4))
      {
        throw new ArgumentException("Invalid property value.", "value");
      }
      return value_4;
    });
    AcceptObjectProperty = DependencyProperty.Register("AcceptObject", typeof(int), typeof(InstanceValidateDependencyPropertyAndValue), metadata_3);
  }
}