using System.Windows;
namespace Metalama.Patterns.Xaml.AspectTests.PropertyInitializer.InitializerProvidesDefaultValueIsFalseAndInitializerProvidesInitialValueIsTrue;
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