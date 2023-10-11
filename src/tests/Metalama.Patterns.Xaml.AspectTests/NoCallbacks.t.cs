namespace Metalama.Patterns.Xaml.AspectTests.NoCallbacks;
public class NoCallbacks
{
  [DependencyProperty]
  public int Foo { get; set; }
  public static readonly global::System.Windows.DependencyProperty FooProperty;
  static NoCallbacks()
  {
    global::Metalama.Patterns.Xaml.AspectTests.NoCallbacks.NoCallbacks.FooProperty = global::System.Windows.DependencyProperty.Register("Foo", typeof(global::System.Int32), typeof(global::Metalama.Patterns.Xaml.AspectTests.NoCallbacks.NoCallbacks));
  }
}