// Warning LAMA5206 on `Foo`: `No property-changed method was found using the default naming convention, with candidate member name 'OnFooChanged'.`
// Warning LAMA5206 on `Foo`: `No validate method was found using the default naming convention, with candidate member name 'ValidateFoo'.`
using System.Windows;
namespace Metalama.Patterns.Xaml.AspectTests.DependencyPropertyTests.Callbacks.StaticOnChangingDependencyProperty;
public partial class StaticOnChangingDependencyProperty : DependencyObject
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
      this.SetValue(StaticOnChangingDependencyProperty.FooProperty, value);
    }
  }
  private static void OnFooChanging(DependencyProperty d)
  {
  }
  public static readonly DependencyProperty FooProperty;
  static StaticOnChangingDependencyProperty()
  {
    object CoerceValue_1(DependencyObject d_1, object value)
    {
      StaticOnChangingDependencyProperty.OnFooChanging(StaticOnChangingDependencyProperty.FooProperty);
      return value;
    }
    var metadata = new PropertyMetadata();
    metadata.CoerceValueCallback = CoerceValue_1;
    StaticOnChangingDependencyProperty.FooProperty = DependencyProperty.Register("Foo", typeof(int), typeof(StaticOnChangingDependencyProperty), metadata);
  }
}