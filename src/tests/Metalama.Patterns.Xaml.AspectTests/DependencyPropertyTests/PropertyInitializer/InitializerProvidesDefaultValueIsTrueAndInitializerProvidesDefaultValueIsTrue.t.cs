using System.Windows;
namespace Metalama.Patterns.Xaml.AspectTests.DependencyPropertyTests.PropertyInitializer.InitializerProvidesDefaultValueIsTrueAndInitializerProvidesDefaultValueIsTrue;
public class InitializerProvidesDefaultValueIsTrueAndInitializerProvidesDefaultValueIsTrue : DependencyObject
{
  private static List<int> InitMethod() => new List<int>(3)
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
  public static readonly DependencyProperty FooProperty;
  static InitializerProvidesDefaultValueIsTrueAndInitializerProvidesDefaultValueIsTrue()
  {
    InitializerProvidesDefaultValueIsTrueAndInitializerProvidesDefaultValueIsTrue.FooProperty = DependencyProperty.Register("Foo", typeof(List<int>), typeof(InitializerProvidesDefaultValueIsTrueAndInitializerProvidesDefaultValueIsTrue), new PropertyMetadata(InitMethod()));
  }
  public InitializerProvidesDefaultValueIsTrueAndInitializerProvidesDefaultValueIsTrue()
  {
    this.Foo = InitMethod();
  }
}