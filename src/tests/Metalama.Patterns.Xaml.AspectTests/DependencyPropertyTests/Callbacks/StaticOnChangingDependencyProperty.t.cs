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
      this.SetValue(FooProperty, value);
    }
  }
  private static void OnFooChanging(DependencyProperty d)
  {
  }
  public static readonly DependencyProperty FooProperty;
  static StaticOnChangingDependencyProperty()
  {
    var metadata = new PropertyMetadata();
    metadata.CoerceValueCallback = new CoerceValueCallback((d_1, value) =>
    {
      OnFooChanging(FooProperty);
      return value;
    });
    FooProperty = DependencyProperty.Register("Foo", typeof(int), typeof(StaticOnChangingDependencyProperty), metadata);
  }
}