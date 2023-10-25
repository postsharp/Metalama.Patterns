// Warning LAMA5206 on `Foo`: `No property-changed method was found using the default naming convention, with candidate member name 'OnFooChanged'.`
// Warning LAMA5206 on `Foo`: `No property-changing method was found using the default naming convention, with candidate member name 'OnFooChanging'.`
// Warning LAMA5206 on `Foo`: `No validate method was found using the default naming convention, with candidate member name 'ValidateFoo'.`
using System.Windows;
namespace Metalama.Patterns.Xaml.AspectTests.DependencyPropertyTests.PropertyInitializer.ImplicitObjectCreation;
public class ImplicitObjectCreation : DependencyObject
{
  [DependencyProperty]
  public List<int> Foo
  {
    get
    {
      return (List<int>)GetValue(FooProperty);
    }
    set
    {
      this.SetValue(ImplicitObjectCreation.FooProperty, value);
    }
  }
  public static readonly DependencyProperty FooProperty;
  static ImplicitObjectCreation()
  {
    ImplicitObjectCreation.FooProperty = DependencyProperty.Register("Foo", typeof(List<int>), typeof(ImplicitObjectCreation), new PropertyMetadata((List<int>)new(3) { 1, 2, 3 }));
  }
}