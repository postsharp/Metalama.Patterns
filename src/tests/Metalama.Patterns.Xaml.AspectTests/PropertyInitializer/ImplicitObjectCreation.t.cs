using System.Windows;
namespace Metalama.Patterns.Xaml.AspectTests.PropertyInitializer.ImplicitObjectCreation;
public class ImplicitObjectCreation : DependencyObject
{
  [DependencyProperty]
  public List<int> Foo
  {
    get
    {
      return ((global::System.Collections.Generic.List<global::System.Int32>)this.GetValue(global::Metalama.Patterns.Xaml.AspectTests.PropertyInitializer.ImplicitObjectCreation.ImplicitObjectCreation.FooProperty));
    }
    set
    {
      this.SetValue(global::Metalama.Patterns.Xaml.AspectTests.PropertyInitializer.ImplicitObjectCreation.ImplicitObjectCreation.FooProperty, value);
    }
  }
  public static readonly global::System.Windows.DependencyProperty FooProperty;
  static ImplicitObjectCreation()
  {
    global::Metalama.Patterns.Xaml.AspectTests.PropertyInitializer.ImplicitObjectCreation.ImplicitObjectCreation.FooProperty = global::System.Windows.DependencyProperty.Register("Foo", typeof(global::System.Collections.Generic.List<global::System.Int32>), typeof(global::Metalama.Patterns.Xaml.AspectTests.PropertyInitializer.ImplicitObjectCreation.ImplicitObjectCreation), new global::System.Windows.PropertyMetadata(((global::System.Object)((global::System.Collections.Generic.List<global::System.Int32>)(new(3) { 1, 2, 3 })))));
  }
}