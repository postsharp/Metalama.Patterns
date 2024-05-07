// Warning LAMA5206 on `Foo`: `No property-changed method was found using the default naming convention, with candidate member name 'OnFooChanged'.`
// Warning LAMA5206 on `Foo`: `No property-changing method was found using the default naming convention, with candidate member name 'OnFooChanging'.`
// Warning LAMA5206 on `AcceptsAssignable`: `No property-changed method was found using the default naming convention, with candidate member name 'OnAcceptsAssignableChanged'.`
// Warning LAMA5206 on `AcceptsAssignable`: `No property-changing method was found using the default naming convention, with candidate member name 'OnAcceptsAssignableChanging'.`
// Warning LAMA5206 on `AcceptsGeneric`: `No property-changed method was found using the default naming convention, with candidate member name 'OnAcceptsGenericChanged'.`
// Warning LAMA5206 on `AcceptsGeneric`: `No property-changing method was found using the default naming convention, with candidate member name 'OnAcceptsGenericChanging'.`
// Warning LAMA5206 on `AcceptsObject`: `No property-changed method was found using the default naming convention, with candidate member name 'OnAcceptsObjectChanged'.`
// Warning LAMA5206 on `AcceptsObject`: `No property-changing method was found using the default naming convention, with candidate member name 'OnAcceptsObjectChanging'.`
using System.Windows;
namespace Metalama.Patterns.Xaml.AspectTests.DependencyPropertyTests.Callbacks.StaticValidateValue;
public partial class StaticValidateValue : DependencyObject
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
  private static bool ValidateFoo(int value) => true;
  [DependencyProperty]
  public List<int> AcceptsAssignable
  {
    get
    {
      return (List<int>)GetValue(AcceptsAssignableProperty);
    }
    set
    {
      this.SetValue(AcceptsAssignableProperty, value);
    }
  }
  private static bool ValidateAcceptsAssignable(IEnumerable<int> value) => true;
  [DependencyProperty]
  public int AcceptsGeneric
  {
    get
    {
      return (int)GetValue(AcceptsGenericProperty);
    }
    set
    {
      this.SetValue(AcceptsGenericProperty, value);
    }
  }
  private static bool ValidateAcceptsGeneric<T>(T value) => true;
  [DependencyProperty]
  public int AcceptsObject
  {
    get
    {
      return (int)GetValue(AcceptsObjectProperty);
    }
    set
    {
      this.SetValue(AcceptsObjectProperty, value);
    }
  }
  private static bool ValidateAcceptsObject(object value) => true;
  public static readonly DependencyProperty AcceptsAssignableProperty;
  public static readonly DependencyProperty AcceptsGenericProperty;
  public static readonly DependencyProperty AcceptsObjectProperty;
  public static readonly DependencyProperty FooProperty;
  static StaticValidateValue()
  {
    FooProperty = DependencyProperty.Register("Foo", typeof(int), typeof(StaticValidateValue), new PropertyMetadata() { CoerceValueCallback = (d, value_1) =>
    {
      if (!ValidateFoo((int)value_1))
      {
        throw new ArgumentException("Invalid property value.", "value");
      }
      return value_1;
    } });
    AcceptsAssignableProperty = DependencyProperty.Register("AcceptsAssignable", typeof(List<int>), typeof(StaticValidateValue), new PropertyMetadata() { CoerceValueCallback = (d_1, value_2) =>
    {
      if (!ValidateAcceptsAssignable((List<int>)value_2))
      {
        throw new ArgumentException("Invalid property value.", "value");
      }
      return value_2;
    } });
    AcceptsGenericProperty = DependencyProperty.Register("AcceptsGeneric", typeof(int), typeof(StaticValidateValue), new PropertyMetadata() { CoerceValueCallback = (d_2, value_3) =>
    {
      if (!ValidateAcceptsGeneric((int)value_3))
      {
        throw new ArgumentException("Invalid property value.", "value");
      }
      return value_3;
    } });
    AcceptsObjectProperty = DependencyProperty.Register("AcceptsObject", typeof(int), typeof(StaticValidateValue), new PropertyMetadata() { CoerceValueCallback = (d_3, value_4) =>
    {
      if (!ValidateAcceptsObject(value_4))
      {
        throw new ArgumentException("Invalid property value.", "value");
      }
      return value_4;
    } });
  }
}