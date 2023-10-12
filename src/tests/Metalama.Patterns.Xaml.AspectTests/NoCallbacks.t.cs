using System.Windows;
namespace Metalama.Patterns.Xaml.AspectTests.NoCallbacks;
public class NoCallbacks : DependencyObject
{
  [DependencyProperty]
  public int Foo
  {
    get
    {
      return ((global::System.Int32)this.GetValue(global::Metalama.Patterns.Xaml.AspectTests.NoCallbacks.NoCallbacks.FooProperty));
    }
    set
    {
      this.SetValue(global::Metalama.Patterns.Xaml.AspectTests.NoCallbacks.NoCallbacks.FooProperty, value);
    }
  }
  public static readonly global::System.Windows.DependencyProperty FooProperty;
  static NoCallbacks()
  {
    global::Metalama.Patterns.Xaml.AspectTests.NoCallbacks.NoCallbacks.FooProperty = global::System.Windows.DependencyProperty.Register("Foo", typeof(global::System.Int32), typeof(global::Metalama.Patterns.Xaml.AspectTests.NoCallbacks.NoCallbacks));
  }
}