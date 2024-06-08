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
    FooProperty = DependencyProperty.Register("Foo", typeof(int), typeof(InstanceValidateDependencyPropertyAndValue), new PropertyMetadata() { CoerceValueCallback = (d_1, value_1) =>
    {
      if (!((InstanceValidateDependencyPropertyAndValue)d_1).ValidateFoo(FooProperty, (int)value_1))
      {
        throw new ArgumentException("Invalid property value.", "value");
      }
      return value_1;
    } });
    AcceptAssignableProperty = DependencyProperty.Register("AcceptAssignable", typeof(List<int>), typeof(InstanceValidateDependencyPropertyAndValue), new PropertyMetadata() { CoerceValueCallback = (d_2, value_2) =>
    {
      if (!((InstanceValidateDependencyPropertyAndValue)d_2).ValidateAcceptAssignable(AcceptAssignableProperty, (List<int>)value_2))
      {
        throw new ArgumentException("Invalid property value.", "value");
      }
      return value_2;
    } });
    AcceptGenericProperty = DependencyProperty.Register("AcceptGeneric", typeof(int), typeof(InstanceValidateDependencyPropertyAndValue), new PropertyMetadata() { CoerceValueCallback = (d_3, value_3) =>
    {
      if (!((InstanceValidateDependencyPropertyAndValue)d_3).ValidateAcceptGeneric(AcceptGenericProperty, (int)value_3))
      {
        throw new ArgumentException("Invalid property value.", "value");
      }
      return value_3;
    } });
    AcceptObjectProperty = DependencyProperty.Register("AcceptObject", typeof(int), typeof(InstanceValidateDependencyPropertyAndValue), new PropertyMetadata() { CoerceValueCallback = (d_4, value_4) =>
    {
      if (!((InstanceValidateDependencyPropertyAndValue)d_4).ValidateAcceptObject(AcceptObjectProperty, value_4))
      {
        throw new ArgumentException("Invalid property value.", "value");
      }
      return value_4;
    } });
  }
}