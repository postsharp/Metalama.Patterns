// Warning LAMA5206 on `Foo`: `No property-changing method was found using the default naming convention, with candidate member name 'OnFooChanging'.`
// Warning LAMA5206 on `Foo`: `No validate method was found using the default naming convention, with candidate member name 'ValidateFoo'.`
// Warning LAMA5206 on `AcceptsDependencyObject`: `No property-changing method was found using the default naming convention, with candidate member name 'OnAcceptsDependencyObjectChanging'.`
// Warning LAMA5206 on `AcceptsDependencyObject`: `No validate method was found using the default naming convention, with candidate member name 'ValidateAcceptsDependencyObject'.`
// Warning LAMA5206 on `AcceptsObject`: `No property-changing method was found using the default naming convention, with candidate member name 'OnAcceptsObjectChanging'.`
// Warning LAMA5206 on `AcceptsObject`: `No validate method was found using the default naming convention, with candidate member name 'ValidateAcceptsObject'.`
using System.Windows;
namespace Metalama.Patterns.Xaml.AspectTests.DependencyPropertyTests.Callbacks.StaticOnChangedInstance;
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
      this.SetValue(FooProperty, value);
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
      this.SetValue(AcceptsDependencyObjectProperty, value);
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
      this.SetValue(AcceptsObjectProperty, value);
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
    FooProperty = DependencyProperty.Register("Foo", typeof(int), typeof(StaticOnChangedInstance), new PropertyMetadata((d, e) => OnFooChanged((StaticOnChangedInstance)d)));
    AcceptsDependencyObjectProperty = DependencyProperty.Register("AcceptsDependencyObject", typeof(int), typeof(StaticOnChangedInstance), new PropertyMetadata((d_1, e_1) => OnAcceptsDependencyObjectChanged(d_1)));
    AcceptsObjectProperty = DependencyProperty.Register("AcceptsObject", typeof(int), typeof(StaticOnChangedInstance), new PropertyMetadata((d_2, e_2) => OnAcceptsObjectChanged(d_2)));
  }
}