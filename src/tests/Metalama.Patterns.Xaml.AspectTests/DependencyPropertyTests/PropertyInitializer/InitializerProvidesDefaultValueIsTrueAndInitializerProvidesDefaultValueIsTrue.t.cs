// Warning LAMA5206 on `Foo`: `No property-changed method was found using the default naming convention, with candidate member name 'OnFooChanged'.`
// Warning LAMA5206 on `Foo`: `No property-changing method was found using the default naming convention, with candidate member name 'OnFooChanging'.`
// Warning LAMA5206 on `Foo`: `No validate method was found using the default naming convention, with candidate member name 'ValidateFoo'.`
using System.Windows;
namespace Metalama.Patterns.Xaml.AspectTests.DependencyPropertyTests.PropertyInitializer.InitializerProvidesDefaultValueIsTrueAndInitializerProvidesDefaultValueIsTrue;
public class InitializerProvidesDefaultValueIsTrueAndInitializerProvidesDefaultValueIsTrue : DependencyObject
{
  private static List<int> InitMethod() => new(3)
  {
    1,
    2,
    3
  };
  [DependencyProperty(InitializerProvidesInitialValue = true, InitializerProvidesDefaultValue = true)]
  public List<int> Foo
  {
    get
    {
      return (List<int>)GetValue(FooProperty);
    }
    set
    {
      this.SetValue(InitializerProvidesDefaultValueIsTrueAndInitializerProvidesDefaultValueIsTrue.FooProperty, value);
    }
  }
  public static readonly DependencyProperty FooProperty = InitializerProvidesDefaultValueIsTrueAndInitializerProvidesDefaultValueIsTrue.CreateFooProperty();
  public InitializerProvidesDefaultValueIsTrueAndInitializerProvidesDefaultValueIsTrue()
  {
    this.Foo = InitMethod();
  }
  private static DependencyProperty CreateFooProperty()
  {
    return DependencyProperty.Register("Foo", typeof(List<int>), typeof(InitializerProvidesDefaultValueIsTrueAndInitializerProvidesDefaultValueIsTrue), new PropertyMetadata(InitMethod()));
  }
}