using System.Windows;
namespace Metalama.Patterns.Xaml.AspectTests.Callbacks.StaticOnChangedInstance;
public partial class StaticOnChangedInstance : DependencyObject
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
      this.SetValue(StaticOnChangedInstance.FooProperty, value);
    }
  }
  private static void OnFooChanged(StaticOnChangedInstance instance)
  {
  }
  [DependencyProperty]
  public int AcceptsDependencyObject
  {
    get
    {
      return (int)GetValue(AcceptsDependencyObjectProperty);
    }
    set
    {
      this.SetValue(StaticOnChangedInstance.AcceptsDependencyObjectProperty, value);
    }
  }
  private static void OnAcceptsDependencyObjectChanged(DependencyObject instance)
  {
  }
  [DependencyProperty]
  public int AcceptsObject
  {
    get
    {
      return (int)GetValue(AcceptsObjectProperty);
    }
    set
    {
      this.SetValue(StaticOnChangedInstance.AcceptsObjectProperty, value);
    }
  }
  private static void OnAcceptsObjectChanged(object instance)
  {
  }
  public static readonly DependencyProperty AcceptsDependencyObjectProperty;
  public static readonly DependencyProperty AcceptsObjectProperty;
  public static readonly DependencyProperty FooProperty;
  static StaticOnChangedInstance()
  {
    void PropertyChanged_2(DependencyObject d_2, DependencyPropertyChangedEventArgs e_2)
    {
      StaticOnChangedInstance.OnAcceptsObjectChanged(d_2);
    }
    StaticOnChangedInstance.AcceptsObjectProperty = DependencyProperty.Register("AcceptsObject", typeof(int), typeof(StaticOnChangedInstance), new PropertyMetadata(PropertyChanged_2));
    void PropertyChanged_1(DependencyObject d_1, DependencyPropertyChangedEventArgs e_1)
    {
      StaticOnChangedInstance.OnAcceptsDependencyObjectChanged(d_1);
    }
    StaticOnChangedInstance.AcceptsDependencyObjectProperty = DependencyProperty.Register("AcceptsDependencyObject", typeof(int), typeof(StaticOnChangedInstance), new PropertyMetadata(PropertyChanged_1));
    void PropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      StaticOnChangedInstance.OnFooChanged((StaticOnChangedInstance)d);
    }
    StaticOnChangedInstance.FooProperty = DependencyProperty.Register("Foo", typeof(int), typeof(StaticOnChangedInstance), new PropertyMetadata(PropertyChanged));
  }
}