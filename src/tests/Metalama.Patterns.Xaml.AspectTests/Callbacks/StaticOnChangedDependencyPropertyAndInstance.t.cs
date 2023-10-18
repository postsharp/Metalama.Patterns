using System.Windows;
namespace Metalama.Patterns.Xaml.AspectTests.Callbacks.StaticOnChangedDependencyPropertyAndInstance;
public partial class StaticOnChangedDependencyPropertyAndInstance : DependencyObject
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
      this.SetValue(StaticOnChangedDependencyPropertyAndInstance.FooProperty, value);
    }
  }
  private static void OnFooChanged(DependencyProperty d, StaticOnChangedDependencyPropertyAndInstance instance)
  {
  }
  [DependencyProperty]
  public int AcceptsDependencyObjectForInstance
  {
    get
    {
      return (int)GetValue(AcceptsDependencyObjectForInstanceProperty);
    }
    set
    {
      this.SetValue(StaticOnChangedDependencyPropertyAndInstance.AcceptsDependencyObjectForInstanceProperty, value);
    }
  }
  private static void OnAcceptsDependencyObjectForInstanceChanged(DependencyProperty d, DependencyObject instance)
  {
  }
  [DependencyProperty]
  public int AcceptsObjectForInstance
  {
    get
    {
      return (int)GetValue(AcceptsObjectForInstanceProperty);
    }
    set
    {
      this.SetValue(StaticOnChangedDependencyPropertyAndInstance.AcceptsObjectForInstanceProperty, value);
    }
  }
  private static void OnAcceptsObjectForInstanceChanged(DependencyProperty d, object instance)
  {
  }
  public static readonly DependencyProperty AcceptsDependencyObjectForInstanceProperty;
  public static readonly DependencyProperty AcceptsObjectForInstanceProperty;
  public static readonly DependencyProperty FooProperty;
  static StaticOnChangedDependencyPropertyAndInstance()
  {
    void PropertyChanged_2(DependencyObject d_3, DependencyPropertyChangedEventArgs e_2)
    {
      StaticOnChangedDependencyPropertyAndInstance.OnAcceptsObjectForInstanceChanged(StaticOnChangedDependencyPropertyAndInstance.AcceptsObjectForInstanceProperty, d_3);
    }
    StaticOnChangedDependencyPropertyAndInstance.AcceptsObjectForInstanceProperty = DependencyProperty.Register("AcceptsObjectForInstance", typeof(int), typeof(StaticOnChangedDependencyPropertyAndInstance), new PropertyMetadata(PropertyChanged_2));
    void PropertyChanged_1(DependencyObject d_2, DependencyPropertyChangedEventArgs e_1)
    {
      StaticOnChangedDependencyPropertyAndInstance.OnAcceptsDependencyObjectForInstanceChanged(StaticOnChangedDependencyPropertyAndInstance.AcceptsDependencyObjectForInstanceProperty, d_2);
    }
    StaticOnChangedDependencyPropertyAndInstance.AcceptsDependencyObjectForInstanceProperty = DependencyProperty.Register("AcceptsDependencyObjectForInstance", typeof(int), typeof(StaticOnChangedDependencyPropertyAndInstance), new PropertyMetadata(PropertyChanged_1));
    void PropertyChanged(DependencyObject d_1, DependencyPropertyChangedEventArgs e)
    {
      StaticOnChangedDependencyPropertyAndInstance.OnFooChanged(StaticOnChangedDependencyPropertyAndInstance.FooProperty, (StaticOnChangedDependencyPropertyAndInstance)d_1);
    }
    StaticOnChangedDependencyPropertyAndInstance.FooProperty = DependencyProperty.Register("Foo", typeof(int), typeof(StaticOnChangedDependencyPropertyAndInstance), new PropertyMetadata(PropertyChanged));
  }
}