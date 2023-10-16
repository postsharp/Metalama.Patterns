using System.Windows;
namespace Metalama.Patterns.Xaml.AspectTests.PropertyInitializer.InitializerProvidesDefaultValueIsTrueAndInitializerProvidesDefaultValueIsTrue;
public class InitializerProvidesDefaultValueIsTrueAndInitializerProvidesDefaultValueIsTrue : DependencyObject
{
  [DependencyProperty(InitializerProvidesInitialValue = true, InitializerProvidesDefaultValue = true)]
  public List<int> Foo
  {
    get
    {
      return ((global::System.Collections.Generic.List<global::System.Int32>)this.GetValue(global::Metalama.Patterns.Xaml.AspectTests.PropertyInitializer.InitializerProvidesDefaultValueIsTrueAndInitializerProvidesDefaultValueIsTrue.InitializerProvidesDefaultValueIsTrueAndInitializerProvidesDefaultValueIsTrue.FooProperty));
    }
    set
    {
      this.SetValue(global::Metalama.Patterns.Xaml.AspectTests.PropertyInitializer.InitializerProvidesDefaultValueIsTrueAndInitializerProvidesDefaultValueIsTrue.InitializerProvidesDefaultValueIsTrueAndInitializerProvidesDefaultValueIsTrue.FooProperty, value);
    }
  }
  public static readonly global::System.Windows.DependencyProperty FooProperty;
  static InitializerProvidesDefaultValueIsTrueAndInitializerProvidesDefaultValueIsTrue()
  {
    global::Metalama.Patterns.Xaml.AspectTests.PropertyInitializer.InitializerProvidesDefaultValueIsTrueAndInitializerProvidesDefaultValueIsTrue.InitializerProvidesDefaultValueIsTrueAndInitializerProvidesDefaultValueIsTrue.FooProperty = global::System.Windows.DependencyProperty.Register("Foo", typeof(global::System.Collections.Generic.List<global::System.Int32>), typeof(global::Metalama.Patterns.Xaml.AspectTests.PropertyInitializer.InitializerProvidesDefaultValueIsTrueAndInitializerProvidesDefaultValueIsTrue.InitializerProvidesDefaultValueIsTrueAndInitializerProvidesDefaultValueIsTrue), new global::System.Windows.PropertyMetadata(((global::System.Object)new List<int>(3) { 1, 2, 3 })));
  }
  public InitializerProvidesDefaultValueIsTrueAndInitializerProvidesDefaultValueIsTrue()
  {
    this.Foo = new List<int>(3)
    {
      1,
      2,
      3
    };
  }
}