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
      this.SetValue(StaticValidateDependencyPropertyAndValue.FooProperty, value);
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
      this.SetValue(StaticValidateDependencyPropertyAndValue.AcceptsAssignableProperty, value);
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
      this.SetValue(StaticValidateDependencyPropertyAndValue.AcceptsGenericProperty, value);
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
      this.SetValue(StaticValidateDependencyPropertyAndValue.AcceptsObjectProperty, value);
    }
  }
  private static bool ValidateAcceptsObject(DependencyProperty d, object value) => true;
  public static readonly DependencyProperty AcceptsAssignableProperty = StaticValidateDependencyPropertyAndValue.CreateAcceptsAssignableDependencyProperty();
  public static readonly DependencyProperty AcceptsGenericProperty = StaticValidateDependencyPropertyAndValue.CreateAcceptsGenericDependencyProperty();
  public static readonly DependencyProperty AcceptsObjectProperty = StaticValidateDependencyPropertyAndValue.CreateAcceptsObjectDependencyProperty();
  public static readonly DependencyProperty FooProperty = StaticValidateDependencyPropertyAndValue.CreateFooDependencyProperty();
  private static DependencyProperty CreateAcceptsAssignableDependencyProperty()
  {
    object CoerceValue_1(DependencyObject d, object value)
    {
      if (!StaticValidateDependencyPropertyAndValue.ValidateAcceptsAssignable(StaticValidateDependencyPropertyAndValue.AcceptsAssignableProperty, (List<int>)value))
      {
        throw new ArgumentException("Invalid property value.", "value");
      }
      return value;
    }
    var metadata = new PropertyMetadata();
    metadata.CoerceValueCallback = CoerceValue_1;
    return DependencyProperty.Register("AcceptsAssignable", typeof(List<int>), typeof(StaticValidateDependencyPropertyAndValue), metadata);
  }
  private static DependencyProperty CreateAcceptsGenericDependencyProperty()
  {
    object CoerceValue_1(DependencyObject d, object value)
    {
      if (!StaticValidateDependencyPropertyAndValue.ValidateAcceptsGeneric<int>(StaticValidateDependencyPropertyAndValue.AcceptsGenericProperty, (int)value))
      {
        throw new ArgumentException("Invalid property value.", "value");
      }
      return value;
    }
    var metadata = new PropertyMetadata();
    metadata.CoerceValueCallback = CoerceValue_1;
    return DependencyProperty.Register("AcceptsGeneric", typeof(int), typeof(StaticValidateDependencyPropertyAndValue), metadata);
  }
  private static DependencyProperty CreateAcceptsObjectDependencyProperty()
  {
    object CoerceValue_1(DependencyObject d, object value)
    {
      if (!StaticValidateDependencyPropertyAndValue.ValidateAcceptsObject(StaticValidateDependencyPropertyAndValue.AcceptsObjectProperty, value))
      {
        throw new ArgumentException("Invalid property value.", "value");
      }
      return value;
    }
    var metadata = new PropertyMetadata();
    metadata.CoerceValueCallback = CoerceValue_1;
    return DependencyProperty.Register("AcceptsObject", typeof(int), typeof(StaticValidateDependencyPropertyAndValue), metadata);
  }
  private static DependencyProperty CreateFooDependencyProperty()
  {
    object CoerceValue_1(DependencyObject d, object value)
    {
      if (!StaticValidateDependencyPropertyAndValue.ValidateFoo(StaticValidateDependencyPropertyAndValue.FooProperty, (int)value))
      {
        throw new ArgumentException("Invalid property value.", "value");
      }
      return value;
    }
    var metadata = new PropertyMetadata();
    metadata.CoerceValueCallback = CoerceValue_1;
    return DependencyProperty.Register("Foo", typeof(int), typeof(StaticValidateDependencyPropertyAndValue), metadata);
  }
}