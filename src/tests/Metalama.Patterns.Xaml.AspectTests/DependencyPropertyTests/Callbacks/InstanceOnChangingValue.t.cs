// Warning LAMA5206 on `Foo`: `No property-changed method was found using the default naming convention, with candidate member name 'OnFooChanged'.`
// Warning LAMA5206 on `Foo`: `No validate method was found using the default naming convention, with candidate member name 'ValidateFoo'.`
// Warning LAMA5206 on `AcceptAssignable`: `No property-changed method was found using the default naming convention, with candidate member name 'OnAcceptAssignableChanged'.`
// Warning LAMA5206 on `AcceptAssignable`: `No validate method was found using the default naming convention, with candidate member name 'ValidateAcceptAssignable'.`
// Warning LAMA5206 on `AcceptGeneric`: `No property-changed method was found using the default naming convention, with candidate member name 'OnAcceptGenericChanged'.`
// Warning LAMA5206 on `AcceptGeneric`: `No validate method was found using the default naming convention, with candidate member name 'ValidateAcceptGeneric'.`
// Warning LAMA5206 on `AcceptObject`: `No property-changed method was found using the default naming convention, with candidate member name 'OnAcceptObjectChanged'.`
// Warning LAMA5206 on `AcceptObject`: `No validate method was found using the default naming convention, with candidate member name 'ValidateAcceptObject'.`
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
      this.SetValue(FooProperty, value);
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
      this.SetValue(AcceptAssignableProperty, value);
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
      this.SetValue(AcceptGenericProperty, value);
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
      this.SetValue(AcceptObjectProperty, value);
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
    var metadata = new PropertyMetadata();
    metadata.CoerceValueCallback = new CoerceValueCallback((d, value_1) =>
    {
      ((InstanceOnChangingValue)d).OnFooChanging((int)value_1);
      return value_1;
    });
    FooProperty = DependencyProperty.Register("Foo", typeof(int), typeof(InstanceOnChangingValue), metadata);
    var metadata_1 = new PropertyMetadata();
    metadata_1.CoerceValueCallback = new CoerceValueCallback((d_1, value_2) =>
    {
      ((InstanceOnChangingValue)d_1).OnAcceptAssignableChanging((List<int>)value_2);
      return value_2;
    });
    AcceptAssignableProperty = DependencyProperty.Register("AcceptAssignable", typeof(List<int>), typeof(InstanceOnChangingValue), metadata_1);
    var metadata_2 = new PropertyMetadata();
    metadata_2.CoerceValueCallback = new CoerceValueCallback((d_2, value_3) =>
    {
      ((InstanceOnChangingValue)d_2).OnAcceptGenericChanging((int)value_3);
      return value_3;
    });
    AcceptGenericProperty = DependencyProperty.Register("AcceptGeneric", typeof(int), typeof(InstanceOnChangingValue), metadata_2);
    var metadata_3 = new PropertyMetadata();
    metadata_3.CoerceValueCallback = new CoerceValueCallback((d_3, value_4) =>
    {
      ((InstanceOnChangingValue)d_3).OnAcceptObjectChanging(value_4);
      return value_4;
    });
    AcceptObjectProperty = DependencyProperty.Register("AcceptObject", typeof(int), typeof(InstanceOnChangingValue), metadata_3);
  }
}