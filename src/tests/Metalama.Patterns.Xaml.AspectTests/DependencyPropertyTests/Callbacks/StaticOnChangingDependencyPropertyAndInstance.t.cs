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
  public static readonly DependencyProperty AcceptsDependencyObjectForInstanceProperty;
  public static readonly DependencyProperty AcceptsObjectForInstanceProperty;
  public static readonly DependencyProperty FooProperty;
  static StaticOnChangingDependencyPropertyAndInstance()
  {
    object CoerceValue_1(DependencyObject d_1, object value)
    {
      StaticOnChangingDependencyPropertyAndInstance.OnFooChanging(StaticOnChangingDependencyPropertyAndInstance.FooProperty, (StaticOnChangingDependencyPropertyAndInstance)d_1);
      return value;
    }
    var metadata = new PropertyMetadata();
    metadata.CoerceValueCallback = CoerceValue_1;
    StaticOnChangingDependencyPropertyAndInstance.FooProperty = DependencyProperty.Register("Foo", typeof(int), typeof(StaticOnChangingDependencyPropertyAndInstance), metadata);
    object CoerceValue_2(DependencyObject d_2, object value_1)
    {
      StaticOnChangingDependencyPropertyAndInstance.OnAcceptsDependencyObjectForInstanceChanging(StaticOnChangingDependencyPropertyAndInstance.AcceptsDependencyObjectForInstanceProperty, d_2);
      return value_1;
    }
    var metadata_1 = new PropertyMetadata();
    metadata_1.CoerceValueCallback = CoerceValue_2;
    StaticOnChangingDependencyPropertyAndInstance.AcceptsDependencyObjectForInstanceProperty = DependencyProperty.Register("AcceptsDependencyObjectForInstance", typeof(int), typeof(StaticOnChangingDependencyPropertyAndInstance), metadata_1);
    object CoerceValue_3(DependencyObject d_3, object value_2)
    {
      StaticOnChangingDependencyPropertyAndInstance.OnAcceptsObjectForInstanceChanging(StaticOnChangingDependencyPropertyAndInstance.AcceptsObjectForInstanceProperty, d_3);
      return value_2;
    }
    var metadata_2 = new PropertyMetadata();
    metadata_2.CoerceValueCallback = CoerceValue_3;
    StaticOnChangingDependencyPropertyAndInstance.AcceptsObjectForInstanceProperty = DependencyProperty.Register("AcceptsObjectForInstance", typeof(int), typeof(StaticOnChangingDependencyPropertyAndInstance), metadata_2);
  }
}