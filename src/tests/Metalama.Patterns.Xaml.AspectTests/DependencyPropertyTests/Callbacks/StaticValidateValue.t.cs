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
      this.SetValue(StaticValidateValue.FooProperty, value);
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
      this.SetValue(StaticValidateValue.AcceptsAssignableProperty, value);
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
      this.SetValue(StaticValidateValue.AcceptsGenericProperty, value);
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
      this.SetValue(StaticValidateValue.AcceptsObjectProperty, value);
    }
  }
  private static bool ValidateAcceptsObject(object value) => true;
  public static readonly DependencyProperty AcceptsAssignableProperty;
  public static readonly DependencyProperty AcceptsGenericProperty;
  public static readonly DependencyProperty AcceptsObjectProperty;
  public static readonly DependencyProperty FooProperty;
  static StaticValidateValue()
  {
    object CoerceValue_4(DependencyObject d_3, object value_4)
    {
      if (!StaticValidateValue.ValidateAcceptsObject(value_4))
      {
        throw new ArgumentException("Invalid property value.", "value");
      }
      return value_4;
    }
    var metadata_3 = new PropertyMetadata();
    metadata_3.CoerceValueCallback = CoerceValue_4;
    StaticValidateValue.AcceptsObjectProperty = DependencyProperty.Register("AcceptsObject", typeof(int), typeof(StaticValidateValue), metadata_3);
    object CoerceValue_3(DependencyObject d_2, object value_3)
    {
      if (!StaticValidateValue.ValidateAcceptsGeneric<int>((int)value_3))
      {
        throw new ArgumentException("Invalid property value.", "value");
      }
      return value_3;
    }
    var metadata_2 = new PropertyMetadata();
    metadata_2.CoerceValueCallback = CoerceValue_3;
    StaticValidateValue.AcceptsGenericProperty = DependencyProperty.Register("AcceptsGeneric", typeof(int), typeof(StaticValidateValue), metadata_2);
    object CoerceValue_2(DependencyObject d_1, object value_2)
    {
      if (!StaticValidateValue.ValidateAcceptsAssignable((List<int>)value_2))
      {
        throw new ArgumentException("Invalid property value.", "value");
      }
      return value_2;
    }
    var metadata_1 = new PropertyMetadata();
    metadata_1.CoerceValueCallback = CoerceValue_2;
    StaticValidateValue.AcceptsAssignableProperty = DependencyProperty.Register("AcceptsAssignable", typeof(List<int>), typeof(StaticValidateValue), metadata_1);
    object CoerceValue_1(DependencyObject d, object value_1)
    {
      if (!StaticValidateValue.ValidateFoo((int)value_1))
      {
        throw new ArgumentException("Invalid property value.", "value");
      }
      return value_1;
    }
    var metadata = new PropertyMetadata();
    metadata.CoerceValueCallback = CoerceValue_1;
    StaticValidateValue.FooProperty = DependencyProperty.Register("Foo", typeof(int), typeof(StaticValidateValue), metadata);
  }
}