using System.Windows;
namespace Metalama.Patterns.Xaml.AspectTests.PropertyInitializer.InitializerProvidesDefaultValueIsFalse;
public class InitializerProvidesDefaultValueIsFalse : DependencyObject
{
  [DependencyProperty(InitializerProvidesDefaultValue = false)]
  public List<int> Foo
  {
    get
    {
      return ((global::System.Collections.Generic.List<global::System.Int32>)this.GetValue(global::Metalama.Patterns.Xaml.AspectTests.PropertyInitializer.InitializerProvidesDefaultValueIsFalse.InitializerProvidesDefaultValueIsFalse.FooProperty));
    }
    set
    {
      this.SetValue(global::Metalama.Patterns.Xaml.AspectTests.PropertyInitializer.InitializerProvidesDefaultValueIsFalse.InitializerProvidesDefaultValueIsFalse.FooProperty, value);
    }
  }
  public static readonly global::System.Windows.DependencyProperty FooProperty;
  static InitializerProvidesDefaultValueIsFalse()
  {
    global::Metalama.Patterns.Xaml.AspectTests.PropertyInitializer.InitializerProvidesDefaultValueIsFalse.InitializerProvidesDefaultValueIsFalse.FooProperty = global::System.Windows.DependencyProperty.Register("Foo", typeof(global::System.Collections.Generic.List<global::System.Int32>), typeof(global::Metalama.Patterns.Xaml.AspectTests.PropertyInitializer.InitializerProvidesDefaultValueIsFalse.InitializerProvidesDefaultValueIsFalse));
  }
  public InitializerProvidesDefaultValueIsFalse()
  {
    this.Foo = new List<int>(3)
    {
      1,
      2,
      3
    };
  }
}