// Warning LAMA5206 on `Foo`: `No property-changed method was found using the default naming convention, with candidate member name 'OnFooChanged'.`
// Warning LAMA5206 on `Foo`: `No property-changing method was found using the default naming convention, with candidate member name 'OnFooChanging'.`
// Warning LAMA5206 on `Foo`: `No validate method was found using the default naming convention, with candidate member name 'ValidateFoo'.`
using System.Windows;
namespace Metalama.Patterns.Xaml.AspectTests.DependencyPropertyTests.PropertyInitializer.InitializerProvidesDefaultValueIsFalseAndInitializerProvidesInitialValueIsTrue;
public class InitializerProvidesDefaultValueIsFalseAndInitializerProvidesInitialValueIsTrue : DependencyObject
{
  private static List<int> InitMethod() => new List<int>(3)
  {
    1,
    2,
    3
  };
  [DependencyProperty(InitializerProvidesDefaultValue = false, InitializerProvidesInitialValue = true)]
  public List<int> Foo
  {
    get
    {
      return (List<int>)GetValue(FooProperty);
    }
    set
    {
      this.SetValue(InitializerProvidesDefaultValueIsFalseAndInitializerProvidesInitialValueIsTrue.FooProperty, value);
    }
  }
  public static readonly DependencyProperty FooProperty;
  static InitializerProvidesDefaultValueIsFalseAndInitializerProvidesInitialValueIsTrue()
  {
    InitializerProvidesDefaultValueIsFalseAndInitializerProvidesInitialValueIsTrue.FooProperty = DependencyProperty.Register("Foo", typeof(List<int>), typeof(InitializerProvidesDefaultValueIsFalseAndInitializerProvidesInitialValueIsTrue));
  }
  public InitializerProvidesDefaultValueIsFalseAndInitializerProvidesInitialValueIsTrue()
  {
    this.Foo = InitMethod();
  }
}