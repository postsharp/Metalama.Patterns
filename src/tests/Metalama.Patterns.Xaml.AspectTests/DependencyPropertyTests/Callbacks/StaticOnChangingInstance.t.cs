// Warning LAMA5206 on `Foo`: `No property-changed method was found using the default naming convention, with candidate member name 'OnFooChanged'.`
// Warning LAMA5206 on `Foo`: `No validate method was found using the default naming convention, with candidate member name 'ValidateFoo'.`
// Warning LAMA5206 on `AcceptsDependencyObject`: `No property-changed method was found using the default naming convention, with candidate member name 'OnAcceptsDependencyObjectChanged'.`
// Warning LAMA5206 on `AcceptsDependencyObject`: `No validate method was found using the default naming convention, with candidate member name 'ValidateAcceptsDependencyObject'.`
// Warning LAMA5206 on `AcceptsObject`: `No property-changed method was found using the default naming convention, with candidate member name 'OnAcceptsObjectChanged'.`
// Warning LAMA5206 on `AcceptsObject`: `No validate method was found using the default naming convention, with candidate member name 'ValidateAcceptsObject'.`
using System.Windows;
namespace Metalama.Patterns.Xaml.AspectTests.DependencyPropertyTests.Callbacks.StaticOnChangingInstance;
public partial class StaticOnChangingInstance : DependencyObject
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
  private static void OnFooChanging(StaticOnChangingInstance instance)
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
      this.SetValue(AcceptsDependencyObjectProperty, value);
    }
  }
  private static void OnAcceptsDependencyObjectChanging(DependencyObject instance)
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
      this.SetValue(AcceptsObjectProperty, value);
    }
  }
  private static void OnAcceptsObjectChanging(object instance)
  {
  }
  public static readonly DependencyProperty AcceptsDependencyObjectProperty;
  public static readonly DependencyProperty AcceptsObjectProperty;
  public static readonly DependencyProperty FooProperty;
  static StaticOnChangingInstance()
  {
    FooProperty = DependencyProperty.Register("Foo", typeof(int), typeof(StaticOnChangingInstance), new PropertyMetadata() { CoerceValueCallback = (d, value) =>
    {
      OnFooChanging((StaticOnChangingInstance)d);
      return value;
    } });
    AcceptsDependencyObjectProperty = DependencyProperty.Register("AcceptsDependencyObject", typeof(int), typeof(StaticOnChangingInstance), new PropertyMetadata() { CoerceValueCallback = (d_1, value_1) =>
    {
      OnAcceptsDependencyObjectChanging(d_1);
      return value_1;
    } });
    AcceptsObjectProperty = DependencyProperty.Register("AcceptsObject", typeof(int), typeof(StaticOnChangingInstance), new PropertyMetadata() { CoerceValueCallback = (d_2, value_2) =>
    {
      OnAcceptsObjectChanging(d_2);
      return value_2;
    } });
  }
}