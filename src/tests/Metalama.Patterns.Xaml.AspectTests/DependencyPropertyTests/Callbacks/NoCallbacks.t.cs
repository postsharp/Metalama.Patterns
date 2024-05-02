// Warning LAMA5206 on `Foo`: `No property-changed method was found using the default naming convention, with candidate member name 'OnFooChanged'.`
// Warning LAMA5206 on `Foo`: `No property-changing method was found using the default naming convention, with candidate member name 'OnFooChanging'.`
// Warning LAMA5206 on `Foo`: `No validate method was found using the default naming convention, with candidate member name 'ValidateFoo'.`
using System.Windows;
namespace Metalama.Patterns.Xaml.AspectTests.DependencyPropertyTests.Callbacks.NoCallbacks;
public partial class NoCallbacks : DependencyObject
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
      this.SetValue(NoCallbacks.FooProperty, value);
    }
  }
  public static readonly DependencyProperty FooProperty = NoCallbacks.CreateFooDependencyProperty();
  private static DependencyProperty CreateFooDependencyProperty()
  {
    return DependencyProperty.Register("Foo", typeof(int), typeof(NoCallbacks));
  }
}