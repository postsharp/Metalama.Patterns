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
      this.SetValue(InstanceValidateDependencyPropertyAndValue.FooProperty, value);
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
      this.SetValue(InstanceValidateDependencyPropertyAndValue.AcceptAssignableProperty, value);
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
      this.SetValue(InstanceValidateDependencyPropertyAndValue.AcceptGenericProperty, value);
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
      this.SetValue(InstanceValidateDependencyPropertyAndValue.AcceptObjectProperty, value);
    }
  }
  private bool ValidateAcceptObject(DependencyProperty d, object value) => true;
  public static readonly DependencyProperty AcceptAssignableProperty = InstanceValidateDependencyPropertyAndValue.CreateAcceptAssignableProperty();
  public static readonly DependencyProperty AcceptGenericProperty = InstanceValidateDependencyPropertyAndValue.CreateAcceptGenericProperty();
  public static readonly DependencyProperty AcceptObjectProperty = InstanceValidateDependencyPropertyAndValue.CreateAcceptObjectProperty();
  public static readonly DependencyProperty FooProperty = InstanceValidateDependencyPropertyAndValue.CreateFooProperty();
  private static DependencyProperty CreateAcceptAssignableProperty()
  {
    object CoerceValue_1(DependencyObject d, object value)
    {
      if (!((InstanceValidateDependencyPropertyAndValue)d).ValidateAcceptAssignable(InstanceValidateDependencyPropertyAndValue.AcceptAssignableProperty, (List<int>)value))
      {
        throw new ArgumentException("Invalid property value.", "value");
      }
      return value;
    }
    var metadata = new PropertyMetadata();
    metadata.CoerceValueCallback = CoerceValue_1;
    return DependencyProperty.Register("AcceptAssignable", typeof(List<int>), typeof(InstanceValidateDependencyPropertyAndValue), metadata);
  }
  private static DependencyProperty CreateAcceptGenericProperty()
  {
    object CoerceValue_1(DependencyObject d, object value)
    {
      if (!((InstanceValidateDependencyPropertyAndValue)d).ValidateAcceptGeneric<int>(InstanceValidateDependencyPropertyAndValue.AcceptGenericProperty, (int)value))
      {
        throw new ArgumentException("Invalid property value.", "value");
      }
      return value;
    }
    var metadata = new PropertyMetadata();
    metadata.CoerceValueCallback = CoerceValue_1;
    return DependencyProperty.Register("AcceptGeneric", typeof(int), typeof(InstanceValidateDependencyPropertyAndValue), metadata);
  }
  private static DependencyProperty CreateAcceptObjectProperty()
  {
    object CoerceValue_1(DependencyObject d, object value)
    {
      if (!((InstanceValidateDependencyPropertyAndValue)d).ValidateAcceptObject(InstanceValidateDependencyPropertyAndValue.AcceptObjectProperty, value))
      {
        throw new ArgumentException("Invalid property value.", "value");
      }
      return value;
    }
    var metadata = new PropertyMetadata();
    metadata.CoerceValueCallback = CoerceValue_1;
    return DependencyProperty.Register("AcceptObject", typeof(int), typeof(InstanceValidateDependencyPropertyAndValue), metadata);
  }
  private static DependencyProperty CreateFooProperty()
  {
    object CoerceValue_1(DependencyObject d, object value)
    {
      if (!((InstanceValidateDependencyPropertyAndValue)d).ValidateFoo(InstanceValidateDependencyPropertyAndValue.FooProperty, (int)value))
      {
        throw new ArgumentException("Invalid property value.", "value");
      }
      return value;
    }
    var metadata = new PropertyMetadata();
    metadata.CoerceValueCallback = CoerceValue_1;
    return DependencyProperty.Register("Foo", typeof(int), typeof(InstanceValidateDependencyPropertyAndValue), metadata);
  }
}