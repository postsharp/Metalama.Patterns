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
  public static readonly DependencyProperty AcceptAssignableProperty;
  public static readonly DependencyProperty AcceptGenericProperty;
  public static readonly DependencyProperty AcceptObjectProperty;
  public static readonly DependencyProperty FooProperty;
  static InstanceValidateDependencyPropertyAndValue()
  {
    object CoerceValue_4(DependencyObject d_4, object value_4)
    {
      if (!((InstanceValidateDependencyPropertyAndValue)d_4).ValidateAcceptObject(InstanceValidateDependencyPropertyAndValue.AcceptObjectProperty, value_4))
      {
        throw new ArgumentException("Invalid property value.", "value");
      }
      return value_4;
    }
    var metadata_3 = new PropertyMetadata();
    metadata_3.CoerceValueCallback = CoerceValue_4;
    InstanceValidateDependencyPropertyAndValue.AcceptObjectProperty = DependencyProperty.Register("AcceptObject", typeof(int), typeof(InstanceValidateDependencyPropertyAndValue), metadata_3);
    object CoerceValue_3(DependencyObject d_3, object value_3)
    {
      if (!((InstanceValidateDependencyPropertyAndValue)d_3).ValidateAcceptGeneric<int>(InstanceValidateDependencyPropertyAndValue.AcceptGenericProperty, (int)value_3))
      {
        throw new ArgumentException("Invalid property value.", "value");
      }
      return value_3;
    }
    var metadata_2 = new PropertyMetadata();
    metadata_2.CoerceValueCallback = CoerceValue_3;
    InstanceValidateDependencyPropertyAndValue.AcceptGenericProperty = DependencyProperty.Register("AcceptGeneric", typeof(int), typeof(InstanceValidateDependencyPropertyAndValue), metadata_2);
    object CoerceValue_2(DependencyObject d_2, object value_2)
    {
      if (!((InstanceValidateDependencyPropertyAndValue)d_2).ValidateAcceptAssignable(InstanceValidateDependencyPropertyAndValue.AcceptAssignableProperty, (List<int>)value_2))
      {
        throw new ArgumentException("Invalid property value.", "value");
      }
      return value_2;
    }
    var metadata_1 = new PropertyMetadata();
    metadata_1.CoerceValueCallback = CoerceValue_2;
    InstanceValidateDependencyPropertyAndValue.AcceptAssignableProperty = DependencyProperty.Register("AcceptAssignable", typeof(List<int>), typeof(InstanceValidateDependencyPropertyAndValue), metadata_1);
    object CoerceValue_1(DependencyObject d_1, object value_1)
    {
      if (!((InstanceValidateDependencyPropertyAndValue)d_1).ValidateFoo(InstanceValidateDependencyPropertyAndValue.FooProperty, (int)value_1))
      {
        throw new ArgumentException("Invalid property value.", "value");
      }
      return value_1;
    }
    var metadata = new PropertyMetadata();
    metadata.CoerceValueCallback = CoerceValue_1;
    InstanceValidateDependencyPropertyAndValue.FooProperty = DependencyProperty.Register("Foo", typeof(int), typeof(InstanceValidateDependencyPropertyAndValue), metadata);
  }
}