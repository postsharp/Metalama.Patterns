// Warning LAMA5206 on `Foo`: `No property-changed method was found using the default naming convention, with candidate member name 'OnFooChanged'.`
// Warning LAMA5206 on `Foo`: `No validate method was found using the default naming convention, with candidate member name 'ValidateFoo'.`
// Warning LAMA5206 on `AcceptsDependencyObjectForInstance`: `No property-changed method was found using the default naming convention, with candidate member name 'OnAcceptsDependencyObjectForInstanceChanged'.`
// Warning LAMA5206 on `AcceptsDependencyObjectForInstance`: `No validate method was found using the default naming convention, with candidate member name 'ValidateAcceptsDependencyObjectForInstance'.`
// Warning LAMA5206 on `AcceptsObjectForInstance`: `No property-changed method was found using the default naming convention, with candidate member name 'OnAcceptsObjectForInstanceChanged'.`
// Warning LAMA5206 on `AcceptsObjectForInstance`: `No validate method was found using the default naming convention, with candidate member name 'ValidateAcceptsObjectForInstance'.`
using System.Windows;
namespace Metalama.Patterns.Xaml.AspectTests.DependencyPropertyTests.Callbacks.StaticOnChangingDependencyPropertyAndInstance;
public partial class StaticOnChangingDependencyPropertyAndInstance : DependencyObject
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
  private static void OnFooChanging(DependencyProperty d, StaticOnChangingDependencyPropertyAndInstance instance)
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
  private static void OnAcceptsDependencyObjectForInstanceChanging(DependencyProperty d, DependencyObject instance)
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
  private static void OnAcceptsObjectForInstanceChanging(DependencyProperty d, object instance)
  {
  }
  public static readonly DependencyProperty AcceptsDependencyObjectForInstanceProperty;
  public static readonly DependencyProperty AcceptsObjectForInstanceProperty;
  public static readonly DependencyProperty FooProperty;
  static StaticOnChangingDependencyPropertyAndInstance()
  {
    FooProperty = DependencyProperty.Register("Foo", typeof(int), typeof(StaticOnChangingDependencyPropertyAndInstance), new PropertyMetadata() { CoerceValueCallback = (d_1, value) =>
    {
      OnFooChanging(FooProperty, (StaticOnChangingDependencyPropertyAndInstance)d_1);
      return value;
    } });
    AcceptsDependencyObjectForInstanceProperty = DependencyProperty.Register("AcceptsDependencyObjectForInstance", typeof(int), typeof(StaticOnChangingDependencyPropertyAndInstance), new PropertyMetadata() { CoerceValueCallback = (d_2, value_1) =>
    {
      OnAcceptsDependencyObjectForInstanceChanging(AcceptsDependencyObjectForInstanceProperty, d_2);
      return value_1;
    } });
    AcceptsObjectForInstanceProperty = DependencyProperty.Register("AcceptsObjectForInstance", typeof(int), typeof(StaticOnChangingDependencyPropertyAndInstance), new PropertyMetadata() { CoerceValueCallback = (d_3, value_2) =>
    {
      OnAcceptsObjectForInstanceChanging(AcceptsObjectForInstanceProperty, d_3);
      return value_2;
    } });
  }
}