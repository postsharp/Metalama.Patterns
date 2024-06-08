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
    FooProperty = DependencyProperty.Register("Foo", typeof(int), typeof(StaticValidateDependencyPropertyAndValue), new PropertyMetadata() { CoerceValueCallback = (d_1, value_1) =>
    {
      if (!ValidateFoo(FooProperty, (int)value_1))
      {
        throw new ArgumentException("Invalid property value.", "value");
      }
      return value_1;
    } });
    AcceptsAssignableProperty = DependencyProperty.Register("AcceptsAssignable", typeof(List<int>), typeof(StaticValidateDependencyPropertyAndValue), new PropertyMetadata() { CoerceValueCallback = (d_2, value_2) =>
    {
      if (!ValidateAcceptsAssignable(AcceptsAssignableProperty, (List<int>)value_2))
      {
        throw new ArgumentException("Invalid property value.", "value");
      }
      return value_2;
    } });
    AcceptsGenericProperty = DependencyProperty.Register("AcceptsGeneric", typeof(int), typeof(StaticValidateDependencyPropertyAndValue), new PropertyMetadata() { CoerceValueCallback = (d_3, value_3) =>
    {
      if (!ValidateAcceptsGeneric(AcceptsGenericProperty, (int)value_3))
      {
        throw new ArgumentException("Invalid property value.", "value");
      }
      return value_3;
    } });
    AcceptsObjectProperty = DependencyProperty.Register("AcceptsObject", typeof(int), typeof(StaticValidateDependencyPropertyAndValue), new PropertyMetadata() { CoerceValueCallback = (d_4, value_4) =>
    {
      if (!ValidateAcceptsObject(AcceptsObjectProperty, value_4))
      {
        throw new ArgumentException("Invalid property value.", "value");
      }
      return value_4;
    } });
  }
}