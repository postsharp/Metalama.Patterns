// Warning LAMA5206 on `Foo`: `No property-changing method was found using the default naming convention, with candidate member name 'OnFooChanging'.`
// Warning LAMA5206 on `Foo`: `No validate method was found using the default naming convention, with candidate member name 'ValidateFoo'.`
// Warning LAMA5206 on `AcceptAssignable`: `No property-changing method was found using the default naming convention, with candidate member name 'OnAcceptAssignableChanging'.`
// Warning LAMA5206 on `AcceptAssignable`: `No validate method was found using the default naming convention, with candidate member name 'ValidateAcceptAssignable'.`
// Warning LAMA5206 on `AcceptGeneric`: `No property-changing method was found using the default naming convention, with candidate member name 'OnAcceptGenericChanging'.`
// Warning LAMA5206 on `AcceptGeneric`: `No validate method was found using the default naming convention, with candidate member name 'ValidateAcceptGeneric'.`
// Warning LAMA5206 on `AcceptObject`: `No property-changing method was found using the default naming convention, with candidate member name 'OnAcceptObjectChanging'.`
// Warning LAMA5206 on `AcceptObject`: `No validate method was found using the default naming convention, with candidate member name 'ValidateAcceptObject'.`
using System.Windows;
namespace Metalama.Patterns.Xaml.AspectTests.DependencyPropertyTests.Callbacks.InstanceOnChangedOldValueAndNewValue;
public partial class InstanceOnChangedOldValueAndNewValue : DependencyObject
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
  private void OnFooChanged(int oldValue, int newValue)
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
  private void OnAcceptAssignableChanged(IEnumerable<int> oldValue, IEnumerable<int> newValue)
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
  private void OnAcceptGenericChanged<T>(T oldValue, T newValue)
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
  private void OnAcceptObjectChanged(object oldValue, object newValue)
  {
  }
  public static readonly DependencyProperty AcceptAssignableProperty;
  public static readonly DependencyProperty AcceptGenericProperty;
  public static readonly DependencyProperty AcceptObjectProperty;
  public static readonly DependencyProperty FooProperty;
  static InstanceOnChangedOldValueAndNewValue()
  {
    FooProperty = DependencyProperty.Register("Foo", typeof(int), typeof(InstanceOnChangedOldValueAndNewValue), new PropertyMetadata((d, e) =>
    {
      ((InstanceOnChangedOldValueAndNewValue)d).OnFooChanged((int)e.OldValue, (int)e.NewValue);
    }));
    AcceptAssignableProperty = DependencyProperty.Register("AcceptAssignable", typeof(List<int>), typeof(InstanceOnChangedOldValueAndNewValue), new PropertyMetadata((d_1, e_1) =>
    {
      ((InstanceOnChangedOldValueAndNewValue)d_1).OnAcceptAssignableChanged((List<int>)e_1.OldValue, (List<int>)e_1.NewValue);
    }));
    AcceptGenericProperty = DependencyProperty.Register("AcceptGeneric", typeof(int), typeof(InstanceOnChangedOldValueAndNewValue), new PropertyMetadata((d_2, e_2) =>
    {
      ((InstanceOnChangedOldValueAndNewValue)d_2).OnAcceptGenericChanged((int)e_2.OldValue, (int)e_2.NewValue);
    }));
    AcceptObjectProperty = DependencyProperty.Register("AcceptObject", typeof(int), typeof(InstanceOnChangedOldValueAndNewValue), new PropertyMetadata((d_3, e_3) =>
    {
      ((InstanceOnChangedOldValueAndNewValue)d_3).OnAcceptObjectChanged(e_3.OldValue, e_3.NewValue);
    }));
  }
}