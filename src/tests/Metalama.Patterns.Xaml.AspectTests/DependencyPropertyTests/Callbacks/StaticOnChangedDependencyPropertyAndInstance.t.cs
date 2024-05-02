// Warning LAMA5206 on `Foo`: `No property-changing method was found using the default naming convention, with candidate member name 'OnFooChanging'.`
// Warning LAMA5206 on `Foo`: `No validate method was found using the default naming convention, with candidate member name 'ValidateFoo'.`
// Warning LAMA5206 on `AcceptsDependencyObjectForInstance`: `No property-changing method was found using the default naming convention, with candidate member name 'OnAcceptsDependencyObjectForInstanceChanging'.`
// Warning LAMA5206 on `AcceptsDependencyObjectForInstance`: `No validate method was found using the default naming convention, with candidate member name 'ValidateAcceptsDependencyObjectForInstance'.`
// Warning LAMA5206 on `AcceptsObjectForInstance`: `No property-changing method was found using the default naming convention, with candidate member name 'OnAcceptsObjectForInstanceChanging'.`
// Warning LAMA5206 on `AcceptsObjectForInstance`: `No validate method was found using the default naming convention, with candidate member name 'ValidateAcceptsObjectForInstance'.`
using System.Windows;
namespace Metalama.Patterns.Xaml.AspectTests.DependencyPropertyTests.Callbacks.StaticOnChangedDependencyPropertyAndInstance;
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
  public static readonly DependencyProperty AcceptsDependencyObjectForInstanceProperty = StaticOnChangedDependencyPropertyAndInstance.CreateAcceptsDependencyObjectForInstanceDependencyProperty();
  public static readonly DependencyProperty AcceptsObjectForInstanceProperty = StaticOnChangedDependencyPropertyAndInstance.CreateAcceptsObjectForInstanceDependencyProperty();
  public static readonly DependencyProperty FooProperty = StaticOnChangedDependencyPropertyAndInstance.CreateFooDependencyProperty();
  private static DependencyProperty CreateAcceptsDependencyObjectForInstanceDependencyProperty()
  {
    void PropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      StaticOnChangedDependencyPropertyAndInstance.OnAcceptsDependencyObjectForInstanceChanged(StaticOnChangedDependencyPropertyAndInstance.AcceptsDependencyObjectForInstanceProperty, d);
    }
    return DependencyProperty.Register("AcceptsDependencyObjectForInstance", typeof(int), typeof(StaticOnChangedDependencyPropertyAndInstance), new PropertyMetadata(PropertyChanged));
  }
  private static DependencyProperty CreateAcceptsObjectForInstanceDependencyProperty()
  {
    void PropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      StaticOnChangedDependencyPropertyAndInstance.OnAcceptsObjectForInstanceChanged(StaticOnChangedDependencyPropertyAndInstance.AcceptsObjectForInstanceProperty, d);
    }
    return DependencyProperty.Register("AcceptsObjectForInstance", typeof(int), typeof(StaticOnChangedDependencyPropertyAndInstance), new PropertyMetadata(PropertyChanged));
  }
  private static DependencyProperty CreateFooDependencyProperty()
  {
    void PropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      StaticOnChangedDependencyPropertyAndInstance.OnFooChanged(StaticOnChangedDependencyPropertyAndInstance.FooProperty, (StaticOnChangedDependencyPropertyAndInstance)d);
    }
    return DependencyProperty.Register("Foo", typeof(int), typeof(StaticOnChangedDependencyPropertyAndInstance), new PropertyMetadata(PropertyChanged));
  }
}