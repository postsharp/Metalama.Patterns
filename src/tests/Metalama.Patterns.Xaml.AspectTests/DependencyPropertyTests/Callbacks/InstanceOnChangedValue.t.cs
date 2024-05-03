// Warning LAMA5206 on `Foo`: `No property-changing method was found using the default naming convention, with candidate member name 'OnFooChanging'.`
// Warning LAMA5206 on `Foo`: `No validate method was found using the default naming convention, with candidate member name 'ValidateFoo'.`
// Warning LAMA5206 on `AcceptAssignable`: `No property-changing method was found using the default naming convention, with candidate member name 'OnAcceptAssignableChanging'.`
// Warning LAMA5206 on `AcceptAssignable`: `No validate method was found using the default naming convention, with candidate member name 'ValidateAcceptAssignable'.`
// Warning LAMA5206 on `AcceptGeneric`: `No property-changing method was found using the default naming convention, with candidate member name 'OnAcceptGenericChanging'.`
// Warning LAMA5206 on `AcceptGeneric`: `No validate method was found using the default naming convention, with candidate member name 'ValidateAcceptGeneric'.`
// Warning LAMA5206 on `AcceptObject`: `No property-changing method was found using the default naming convention, with candidate member name 'OnAcceptObjectChanging'.`
// Warning LAMA5206 on `AcceptObject`: `No validate method was found using the default naming convention, with candidate member name 'ValidateAcceptObject'.`
using System.Windows;
namespace Metalama.Patterns.Xaml.AspectTests.DependencyPropertyTests.Callbacks.InstanceOnChangedValue;
public partial class InstanceOnChangedValue : DependencyObject
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
      this.SetValue(InstanceOnChangedValue.FooProperty, value);
    }
  }
  private void OnFooChanged(int value)
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
      this.SetValue(InstanceOnChangedValue.AcceptAssignableProperty, value);
    }
  }
  private void OnAcceptAssignableChanged(IEnumerable<int> value)
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
      this.SetValue(InstanceOnChangedValue.AcceptGenericProperty, value);
    }
  }
  private void OnAcceptGenericChanged<T>(T value)
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
      this.SetValue(InstanceOnChangedValue.AcceptObjectProperty, value);
    }
  }
  private void OnAcceptObjectChanged(object value)
  {
  }
  public static readonly DependencyProperty AcceptAssignableProperty = InstanceOnChangedValue.CreateAcceptAssignableProperty();
  public static readonly DependencyProperty AcceptGenericProperty = InstanceOnChangedValue.CreateAcceptGenericProperty();
  public static readonly DependencyProperty AcceptObjectProperty = InstanceOnChangedValue.CreateAcceptObjectProperty();
  public static readonly DependencyProperty FooProperty = InstanceOnChangedValue.CreateFooProperty();
  private static DependencyProperty CreateAcceptAssignableProperty()
  {
    void PropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((InstanceOnChangedValue)d).OnAcceptAssignableChanged((List<int>)e.NewValue);
    }
    return DependencyProperty.Register("AcceptAssignable", typeof(List<int>), typeof(InstanceOnChangedValue), new PropertyMetadata(PropertyChanged));
  }
  private static DependencyProperty CreateAcceptGenericProperty()
  {
    void PropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((InstanceOnChangedValue)d).OnAcceptGenericChanged<int>((int)e.NewValue);
    }
    return DependencyProperty.Register("AcceptGeneric", typeof(int), typeof(InstanceOnChangedValue), new PropertyMetadata(PropertyChanged));
  }
  private static DependencyProperty CreateAcceptObjectProperty()
  {
    void PropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((InstanceOnChangedValue)d).OnAcceptObjectChanged(e.NewValue);
    }
    return DependencyProperty.Register("AcceptObject", typeof(int), typeof(InstanceOnChangedValue), new PropertyMetadata(PropertyChanged));
  }
  private static DependencyProperty CreateFooProperty()
  {
    void PropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((InstanceOnChangedValue)d).OnFooChanged((int)e.NewValue);
    }
    return DependencyProperty.Register("Foo", typeof(int), typeof(InstanceOnChangedValue), new PropertyMetadata(PropertyChanged));
  }
}