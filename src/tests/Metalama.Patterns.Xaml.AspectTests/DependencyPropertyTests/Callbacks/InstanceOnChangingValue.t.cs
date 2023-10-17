using System.Windows;
namespace Metalama.Patterns.Xaml.AspectTests.DependencyPropertyTests.Callbacks.InstanceOnChangingValue;
public partial class InstanceOnChangingValue : DependencyObject
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
      this.SetValue(InstanceOnChangingValue.FooProperty, value);
    }
  }
  private void OnFooChanging(int value)
  {
  }
  [DependencyProperty]
  public List<int> AcceptAssignable
  {
    get
    {
      return (List<int>)GetValue(AcceptAssignableProperty);
    }
    set
    {
      this.SetValue(InstanceOnChangingValue.AcceptAssignableProperty, value);
    }
  }
  private void OnAcceptAssignableChanging(IEnumerable<int> value)
  {
  }
  [DependencyProperty]
  public int AcceptGeneric
  {
    get
    {
      return (int)GetValue(AcceptGenericProperty);
    }
    set
    {
      this.SetValue(InstanceOnChangingValue.AcceptGenericProperty, value);
    }
  }
  private void OnAcceptGenericChanging<T>(T value)
  {
  }
  [DependencyProperty]
  public int AcceptObject
  {
    get
    {
      return (int)GetValue(AcceptObjectProperty);
    }
    set
    {
      this.SetValue(InstanceOnChangingValue.AcceptObjectProperty, value);
    }
  }
  private void OnAcceptObjectChanging(object value)
  {
  }
  public static readonly DependencyProperty AcceptAssignableProperty;
  public static readonly DependencyProperty AcceptGenericProperty;
  public static readonly DependencyProperty AcceptObjectProperty;
  public static readonly DependencyProperty FooProperty;
  static InstanceOnChangingValue()
  {
    object CoerceValue_4(DependencyObject d_3, object value_4)
    {
      ((InstanceOnChangingValue)d_3).OnAcceptObjectChanging(value_4);
      return value_4;
    }
    var metadata_3 = new PropertyMetadata();
    metadata_3.CoerceValueCallback = CoerceValue_4;
    InstanceOnChangingValue.AcceptObjectProperty = DependencyProperty.Register("AcceptObject", typeof(int), typeof(InstanceOnChangingValue), metadata_3);
    object CoerceValue_3(DependencyObject d_2, object value_3)
    {
      ((InstanceOnChangingValue)d_2).OnAcceptGenericChanging<int>((int)value_3);
      return value_3;
    }
    var metadata_2 = new PropertyMetadata();
    metadata_2.CoerceValueCallback = CoerceValue_3;
    InstanceOnChangingValue.AcceptGenericProperty = DependencyProperty.Register("AcceptGeneric", typeof(int), typeof(InstanceOnChangingValue), metadata_2);
    object CoerceValue_2(DependencyObject d_1, object value_2)
    {
      ((InstanceOnChangingValue)d_1).OnAcceptAssignableChanging((List<int>)value_2);
      return value_2;
    }
    var metadata_1 = new PropertyMetadata();
    metadata_1.CoerceValueCallback = CoerceValue_2;
    InstanceOnChangingValue.AcceptAssignableProperty = DependencyProperty.Register("AcceptAssignable", typeof(List<int>), typeof(InstanceOnChangingValue), metadata_1);
    object CoerceValue_1(DependencyObject d, object value_1)
    {
      ((InstanceOnChangingValue)d).OnFooChanging((int)value_1);
      return value_1;
    }
    var metadata = new PropertyMetadata();
    metadata.CoerceValueCallback = CoerceValue_1;
    InstanceOnChangingValue.FooProperty = DependencyProperty.Register("Foo", typeof(int), typeof(InstanceOnChangingValue), metadata);
  }
}