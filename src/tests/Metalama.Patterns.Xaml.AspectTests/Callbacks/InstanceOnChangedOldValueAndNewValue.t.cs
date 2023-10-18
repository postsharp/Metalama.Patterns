using System.Windows;
namespace Metalama.Patterns.Xaml.AspectTests.Callbacks.InstanceOnChangedOldValueAndNewValue;
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
      this.SetValue(InstanceOnChangedOldValueAndNewValue.FooProperty, value);
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
      this.SetValue(InstanceOnChangedOldValueAndNewValue.AcceptAssignableProperty, value);
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
      this.SetValue(InstanceOnChangedOldValueAndNewValue.AcceptGenericProperty, value);
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
      this.SetValue(InstanceOnChangedOldValueAndNewValue.AcceptObjectProperty, value);
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
    void PropertyChanged_3(DependencyObject d_3, DependencyPropertyChangedEventArgs e_3)
    {
      ((InstanceOnChangedOldValueAndNewValue)d_3).OnAcceptObjectChanged(e_3.OldValue, e_3.NewValue);
    }
    InstanceOnChangedOldValueAndNewValue.AcceptObjectProperty = DependencyProperty.Register("AcceptObject", typeof(int), typeof(InstanceOnChangedOldValueAndNewValue), new PropertyMetadata(PropertyChanged_3));
    void PropertyChanged_2(DependencyObject d_2, DependencyPropertyChangedEventArgs e_2)
    {
      ((InstanceOnChangedOldValueAndNewValue)d_2).OnAcceptGenericChanged<int>((int)e_2.OldValue, (int)e_2.NewValue);
    }
    InstanceOnChangedOldValueAndNewValue.AcceptGenericProperty = DependencyProperty.Register("AcceptGeneric", typeof(int), typeof(InstanceOnChangedOldValueAndNewValue), new PropertyMetadata(PropertyChanged_2));
    void PropertyChanged_1(DependencyObject d_1, DependencyPropertyChangedEventArgs e_1)
    {
      ((InstanceOnChangedOldValueAndNewValue)d_1).OnAcceptAssignableChanged((List<int>)e_1.OldValue, (List<int>)e_1.NewValue);
    }
    InstanceOnChangedOldValueAndNewValue.AcceptAssignableProperty = DependencyProperty.Register("AcceptAssignable", typeof(List<int>), typeof(InstanceOnChangedOldValueAndNewValue), new PropertyMetadata(PropertyChanged_1));
    void PropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((InstanceOnChangedOldValueAndNewValue)d).OnFooChanged((int)e.OldValue, (int)e.NewValue);
    }
    InstanceOnChangedOldValueAndNewValue.FooProperty = DependencyProperty.Register("Foo", typeof(int), typeof(InstanceOnChangedOldValueAndNewValue), new PropertyMetadata(PropertyChanged));
  }
}