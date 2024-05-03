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
      this.SetValue(FooProperty, value);
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
      this.SetValue(AcceptsDependencyObjectForInstanceProperty, value);
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
      this.SetValue(AcceptsObjectForInstanceProperty, value);
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
    FooProperty = DependencyProperty.Register("Foo", typeof(int), typeof(StaticOnChangedDependencyPropertyAndInstance), new PropertyMetadata((d_1, e) =>
    {
      OnFooChanged(FooProperty, (StaticOnChangedDependencyPropertyAndInstance)d_1);
    }));
    AcceptsDependencyObjectForInstanceProperty = DependencyProperty.Register("AcceptsDependencyObjectForInstance", typeof(int), typeof(StaticOnChangedDependencyPropertyAndInstance), new PropertyMetadata((d_2, e_1) =>
    {
      OnAcceptsDependencyObjectForInstanceChanged(AcceptsDependencyObjectForInstanceProperty, d_2);
    }));
    AcceptsObjectForInstanceProperty = DependencyProperty.Register("AcceptsObjectForInstance", typeof(int), typeof(StaticOnChangedDependencyPropertyAndInstance), new PropertyMetadata((d_3, e_2) =>
    {
      OnAcceptsObjectForInstanceChanged(AcceptsObjectForInstanceProperty, d_3);
    }));
  }
}