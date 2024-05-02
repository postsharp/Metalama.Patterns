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
  public static readonly DependencyProperty AcceptAssignableProperty = InstanceValidateValue.CreateAcceptAssignableDependencyProperty();
  public static readonly DependencyProperty AcceptGenericProperty = InstanceValidateValue.CreateAcceptGenericDependencyProperty();
  public static readonly DependencyProperty AcceptObjectProperty = InstanceValidateValue.CreateAcceptObjectDependencyProperty();
  public static readonly DependencyProperty FooProperty = InstanceValidateValue.CreateFooDependencyProperty();
  private static DependencyProperty CreateAcceptAssignableDependencyProperty()
  {
    object CoerceValue_1(DependencyObject d, object value)
    {
      if (!((InstanceValidateValue)d).ValidateAcceptAssignable((List<int>)value))
      {
        throw new ArgumentException("Invalid property value.", "value");
      }
      return value;
    }
    var metadata = new PropertyMetadata();
    metadata.CoerceValueCallback = CoerceValue_1;
    return DependencyProperty.Register("AcceptAssignable", typeof(List<int>), typeof(InstanceValidateValue), metadata);
  }
  private static DependencyProperty CreateAcceptGenericDependencyProperty()
  {
    object CoerceValue_1(DependencyObject d, object value)
    {
      if (!((InstanceValidateValue)d).ValidateAcceptGeneric<int>((int)value))
      {
        throw new ArgumentException("Invalid property value.", "value");
      }
      return value;
    }
    var metadata = new PropertyMetadata();
    metadata.CoerceValueCallback = CoerceValue_1;
    return DependencyProperty.Register("AcceptGeneric", typeof(int), typeof(InstanceValidateValue), metadata);
  }
  private static DependencyProperty CreateAcceptObjectDependencyProperty()
  {
    object CoerceValue_1(DependencyObject d, object value)
    {
      if (!((InstanceValidateValue)d).ValidateAcceptObject(value))
      {
        throw new ArgumentException("Invalid property value.", "value");
      }
      return value;
    }
    var metadata = new PropertyMetadata();
    metadata.CoerceValueCallback = CoerceValue_1;
    return DependencyProperty.Register("AcceptObject", typeof(int), typeof(InstanceValidateValue), metadata);
  }
  private static DependencyProperty CreateFooDependencyProperty()
  {
    object CoerceValue_1(DependencyObject d, object value)
    {
      if (!((InstanceValidateValue)d).ValidateFoo((int)value))
      {
        throw new ArgumentException("Invalid property value.", "value");
      }
      return value;
    }
    var metadata = new PropertyMetadata();
    metadata.CoerceValueCallback = CoerceValue_1;
    return DependencyProperty.Register("Foo", typeof(int), typeof(InstanceValidateValue), metadata);
  }
}