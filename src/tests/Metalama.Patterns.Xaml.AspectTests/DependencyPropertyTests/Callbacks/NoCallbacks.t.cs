using System.Windows;
namespace Metalama.Patterns.Xaml.AspectTests.DependencyPropertyTests.Callbacks.NoCallbacks;
public partial class NoCallbacks : DependencyObject
{
  [DependencyProperty]
  public int Foo
  {
    get
    {
      return (int)GetValue(FooProperty);
    }
    set
    {
      this.SetValue(NoCallbacks.FooProperty, value);
    }
  }
  public static readonly DependencyProperty FooProperty;
  static NoCallbacks()
  {
    NoCallbacks.FooProperty = DependencyProperty.Register("Foo", typeof(int), typeof(NoCallbacks));
  }
}