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
  public static readonly DependencyProperty AcceptAssignableProperty;
  public static readonly DependencyProperty AcceptGenericProperty;
  public static readonly DependencyProperty AcceptObjectProperty;
  public static readonly DependencyProperty FooProperty;
  static InstanceOnChangedValue()
  {
    void PropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((InstanceOnChangedValue)d).OnFooChanged((int)e.NewValue);
    }
    InstanceOnChangedValue.FooProperty = DependencyProperty.Register("Foo", typeof(int), typeof(InstanceOnChangedValue), new PropertyMetadata(PropertyChanged));
    void PropertyChanged_1(DependencyObject d_1, DependencyPropertyChangedEventArgs e_1)
    {
      ((InstanceOnChangedValue)d_1).OnAcceptAssignableChanged((List<int>)e_1.NewValue);
    }
    InstanceOnChangedValue.AcceptAssignableProperty = DependencyProperty.Register("AcceptAssignable", typeof(List<int>), typeof(InstanceOnChangedValue), new PropertyMetadata(PropertyChanged_1));
    void PropertyChanged_2(DependencyObject d_2, DependencyPropertyChangedEventArgs e_2)
    {
      ((InstanceOnChangedValue)d_2).OnAcceptGenericChanged<int>((int)e_2.NewValue);
    }
    InstanceOnChangedValue.AcceptGenericProperty = DependencyProperty.Register("AcceptGeneric", typeof(int), typeof(InstanceOnChangedValue), new PropertyMetadata(PropertyChanged_2));
    void PropertyChanged_3(DependencyObject d_3, DependencyPropertyChangedEventArgs e_3)
    {
      ((InstanceOnChangedValue)d_3).OnAcceptObjectChanged(e_3.NewValue);
    }
    InstanceOnChangedValue.AcceptObjectProperty = DependencyProperty.Register("AcceptObject", typeof(int), typeof(InstanceOnChangedValue), new PropertyMetadata(PropertyChanged_3));
  }
}