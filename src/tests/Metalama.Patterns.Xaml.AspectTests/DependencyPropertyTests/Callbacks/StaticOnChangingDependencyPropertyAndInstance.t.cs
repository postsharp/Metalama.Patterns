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
      this.SetValue(StaticOnChangingDependencyPropertyAndInstance.FooProperty, value);
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
      this.SetValue(StaticOnChangingDependencyPropertyAndInstance.AcceptsDependencyObjectForInstanceProperty, value);
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
      this.SetValue(StaticOnChangingDependencyPropertyAndInstance.AcceptsObjectForInstanceProperty, value);
    }
  }
  private static void OnAcceptsObjectForInstanceChanging(DependencyProperty d, object instance)
  {
  }
  public static readonly DependencyProperty AcceptsDependencyObjectForInstanceProperty = StaticOnChangingDependencyPropertyAndInstance.CreateAcceptsDependencyObjectForInstanceDependencyProperty();
  public static readonly DependencyProperty AcceptsObjectForInstanceProperty = StaticOnChangingDependencyPropertyAndInstance.CreateAcceptsObjectForInstanceDependencyProperty();
  public static readonly DependencyProperty FooProperty = StaticOnChangingDependencyPropertyAndInstance.CreateFooDependencyProperty();
  private static DependencyProperty CreateAcceptsDependencyObjectForInstanceDependencyProperty()
  {
    object CoerceValue_1(DependencyObject d, object value)
    {
      StaticOnChangingDependencyPropertyAndInstance.OnAcceptsDependencyObjectForInstanceChanging(StaticOnChangingDependencyPropertyAndInstance.AcceptsDependencyObjectForInstanceProperty, d);
      return value;
    }
    var metadata = new PropertyMetadata();
    metadata.CoerceValueCallback = CoerceValue_1;
    return DependencyProperty.Register("AcceptsDependencyObjectForInstance", typeof(int), typeof(StaticOnChangingDependencyPropertyAndInstance), metadata);
  }
  private static DependencyProperty CreateAcceptsObjectForInstanceDependencyProperty()
  {
    object CoerceValue_1(DependencyObject d, object value)
    {
      StaticOnChangingDependencyPropertyAndInstance.OnAcceptsObjectForInstanceChanging(StaticOnChangingDependencyPropertyAndInstance.AcceptsObjectForInstanceProperty, d);
      return value;
    }
    var metadata = new PropertyMetadata();
    metadata.CoerceValueCallback = CoerceValue_1;
    return DependencyProperty.Register("AcceptsObjectForInstance", typeof(int), typeof(StaticOnChangingDependencyPropertyAndInstance), metadata);
  }
  private static DependencyProperty CreateFooDependencyProperty()
  {
    object CoerceValue_1(DependencyObject d, object value)
    {
      StaticOnChangingDependencyPropertyAndInstance.OnFooChanging(StaticOnChangingDependencyPropertyAndInstance.FooProperty, (StaticOnChangingDependencyPropertyAndInstance)d);
      return value;
    }
    var metadata = new PropertyMetadata();
    metadata.CoerceValueCallback = CoerceValue_1;
    return DependencyProperty.Register("Foo", typeof(int), typeof(StaticOnChangingDependencyPropertyAndInstance), metadata);
  }
}