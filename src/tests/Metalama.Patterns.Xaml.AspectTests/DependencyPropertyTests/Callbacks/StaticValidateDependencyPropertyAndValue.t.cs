// Warning LAMA5206 on `Foo`: `No property-changed method was found using the default naming convention, with candidate member name 'OnFooChanged'.`
// Warning LAMA5206 on `Foo`: `No property-changing method was found using the default naming convention, with candidate member name 'OnFooChanging'.`
// Warning LAMA5206 on `AcceptsAssignable`: `No property-changed method was found using the default naming convention, with candidate member name 'OnAcceptsAssignableChanged'.`
// Warning LAMA5206 on `AcceptsAssignable`: `No property-changing method was found using the default naming convention, with candidate member name 'OnAcceptsAssignableChanging'.`
// Warning LAMA5206 on `AcceptsGeneric`: `No property-changed method was found using the default naming convention, with candidate member name 'OnAcceptsGenericChanged'.`
// Warning LAMA5206 on `AcceptsGeneric`: `No property-changing method was found using the default naming convention, with candidate member name 'OnAcceptsGenericChanging'.`
// Warning LAMA5206 on `AcceptsObject`: `No property-changed method was found using the default naming convention, with candidate member name 'OnAcceptsObjectChanged'.`
// Warning LAMA5206 on `AcceptsObject`: `No property-changing method was found using the default naming convention, with candidate member name 'OnAcceptsObjectChanging'.`
using System.Windows;
namespace Metalama.Patterns.Xaml.AspectTests.DependencyPropertyTests.Callbacks.StaticValidateDependencyPropertyAndValue;
public partial class StaticValidateDependencyPropertyAndValue : DependencyObject
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
  private static bool ValidateFoo(DependencyProperty d, int value) => true;
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
  private static bool ValidateAcceptsAssignable(DependencyProperty d, IEnumerable<int> value) => true;
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
  private static bool ValidateAcceptsGeneric<T>(DependencyProperty d, T value) => true;
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
  private static bool ValidateAcceptsObject(DependencyProperty d, object value) => true;
  public static readonly DependencyProperty AcceptsAssignableProperty;
  public static readonly DependencyProperty AcceptsGenericProperty;
  public static readonly DependencyProperty AcceptsObjectProperty;
  public static readonly DependencyProperty FooProperty;
  static StaticValidateDependencyPropertyAndValue()
  {
    var metadata = new PropertyMetadata();
    metadata.CoerceValueCallback = new CoerceValueCallback((d_1, value_1) =>
    {
      if (!ValidateFoo(FooProperty, (int)value_1))
      {
        throw new ArgumentException("Invalid property value.", "value");
      }
      return value_1;
    });
    FooProperty = DependencyProperty.Register("Foo", typeof(int), typeof(StaticValidateDependencyPropertyAndValue), metadata);
    var metadata_1 = new PropertyMetadata();
    metadata_1.CoerceValueCallback = new CoerceValueCallback((d_2, value_2) =>
    {
      if (!ValidateAcceptsAssignable(AcceptsAssignableProperty, (List<int>)value_2))
      {
        throw new ArgumentException("Invalid property value.", "value");
      }
      return value_2;
    });
    AcceptsAssignableProperty = DependencyProperty.Register("AcceptsAssignable", typeof(List<int>), typeof(StaticValidateDependencyPropertyAndValue), metadata_1);
    var metadata_2 = new PropertyMetadata();
    metadata_2.CoerceValueCallback = new CoerceValueCallback((d_3, value_3) =>
    {
      if (!ValidateAcceptsGeneric(AcceptsGenericProperty, (int)value_3))
      {
        throw new ArgumentException("Invalid property value.", "value");
      }
      return value_3;
    });
    AcceptsGenericProperty = DependencyProperty.Register("AcceptsGeneric", typeof(int), typeof(StaticValidateDependencyPropertyAndValue), metadata_2);
    var metadata_3 = new PropertyMetadata();
    metadata_3.CoerceValueCallback = new CoerceValueCallback((d_4, value_4) =>
    {
      if (!ValidateAcceptsObject(AcceptsObjectProperty, value_4))
      {
        throw new ArgumentException("Invalid property value.", "value");
      }
      return value_4;
    });
    AcceptsObjectProperty = DependencyProperty.Register("AcceptsObject", typeof(int), typeof(StaticValidateDependencyPropertyAndValue), metadata_3);
  }
}