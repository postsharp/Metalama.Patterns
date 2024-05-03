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
  public static readonly DependencyProperty AcceptAssignableProperty = InstanceOnChangingValue.CreateAcceptAssignableProperty();
  public static readonly DependencyProperty AcceptGenericProperty = InstanceOnChangingValue.CreateAcceptGenericProperty();
  public static readonly DependencyProperty AcceptObjectProperty = InstanceOnChangingValue.CreateAcceptObjectProperty();
  public static readonly DependencyProperty FooProperty = InstanceOnChangingValue.CreateFooProperty();
  private static DependencyProperty CreateAcceptAssignableProperty()
  {
    object CoerceValue_1(DependencyObject d, object value)
    {
      ((InstanceOnChangingValue)d).OnAcceptAssignableChanging((List<int>)value);
      return value;
    }
    var metadata = new PropertyMetadata();
    metadata.CoerceValueCallback = CoerceValue_1;
    return DependencyProperty.Register("AcceptAssignable", typeof(List<int>), typeof(InstanceOnChangingValue), metadata);
  }
  private static DependencyProperty CreateAcceptGenericProperty()
  {
    object CoerceValue_1(DependencyObject d, object value)
    {
      ((InstanceOnChangingValue)d).OnAcceptGenericChanging<int>((int)value);
      return value;
    }
    var metadata = new PropertyMetadata();
    metadata.CoerceValueCallback = CoerceValue_1;
    return DependencyProperty.Register("AcceptGeneric", typeof(int), typeof(InstanceOnChangingValue), metadata);
  }
  private static DependencyProperty CreateAcceptObjectProperty()
  {
    object CoerceValue_1(DependencyObject d, object value)
    {
      ((InstanceOnChangingValue)d).OnAcceptObjectChanging(value);
      return value;
    }
    var metadata = new PropertyMetadata();
    metadata.CoerceValueCallback = CoerceValue_1;
    return DependencyProperty.Register("AcceptObject", typeof(int), typeof(InstanceOnChangingValue), metadata);
  }
  private static DependencyProperty CreateFooProperty()
  {
    object CoerceValue_1(DependencyObject d, object value)
    {
      ((InstanceOnChangingValue)d).OnFooChanging((int)value);
      return value;
    }
    var metadata = new PropertyMetadata();
    metadata.CoerceValueCallback = CoerceValue_1;
    return DependencyProperty.Register("Foo", typeof(int), typeof(InstanceOnChangingValue), metadata);
  }
}