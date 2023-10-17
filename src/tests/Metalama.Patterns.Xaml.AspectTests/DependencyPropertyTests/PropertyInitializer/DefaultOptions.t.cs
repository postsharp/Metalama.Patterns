using System.Windows;
namespace Metalama.Patterns.Xaml.AspectTests.DependencyPropertyTests.PropertyInitializer.DefaultOptions;
public class DefaultOptions : DependencyObject
{
  private static List<int> InitMethod() => new List<int>(3)
  {
    1,
    2,
    3
  };
  [DependencyProperty]
  public List<int> Foo
  {
    get
    {
      return (List<int>)GetValue(FooProperty);
    }
    set
    {
      this.SetValue(DefaultOptions.FooProperty, value);
    }
  }
  public static readonly DependencyProperty FooProperty;
  static DefaultOptions()
  {
    DefaultOptions.FooProperty = DependencyProperty.Register("Foo", typeof(List<int>), typeof(DefaultOptions), new PropertyMetadata(InitMethod()));
  }
}