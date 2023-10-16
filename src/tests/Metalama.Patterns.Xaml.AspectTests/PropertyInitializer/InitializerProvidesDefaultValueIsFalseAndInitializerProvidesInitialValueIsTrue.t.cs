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
      return ((global::System.Collections.Generic.List<global::System.Int32>)this.GetValue(global::Metalama.Patterns.Xaml.AspectTests.PropertyInitializer.InitializerProvidesDefaultValueIsFalseAndInitializerProvidesInitialValueIsTrue.InitializerProvidesDefaultValueIsFalseAndInitializerProvidesInitialValueIsTrue.FooProperty));
    }
    set
    {
      this.SetValue(global::Metalama.Patterns.Xaml.AspectTests.PropertyInitializer.InitializerProvidesDefaultValueIsFalseAndInitializerProvidesInitialValueIsTrue.InitializerProvidesDefaultValueIsFalseAndInitializerProvidesInitialValueIsTrue.FooProperty, value);
    }
  }
  public static readonly global::System.Windows.DependencyProperty FooProperty;
  static InitializerProvidesDefaultValueIsFalseAndInitializerProvidesInitialValueIsTrue()
  {
    global::Metalama.Patterns.Xaml.AspectTests.PropertyInitializer.InitializerProvidesDefaultValueIsFalseAndInitializerProvidesInitialValueIsTrue.InitializerProvidesDefaultValueIsFalseAndInitializerProvidesInitialValueIsTrue.FooProperty = global::System.Windows.DependencyProperty.Register("Foo", typeof(global::System.Collections.Generic.List<global::System.Int32>), typeof(global::Metalama.Patterns.Xaml.AspectTests.PropertyInitializer.InitializerProvidesDefaultValueIsFalseAndInitializerProvidesInitialValueIsTrue.InitializerProvidesDefaultValueIsFalseAndInitializerProvidesInitialValueIsTrue));
  }
  public InitializerProvidesDefaultValueIsFalseAndInitializerProvidesInitialValueIsTrue()
  {
    this.Foo = InitMethod();
  }
}