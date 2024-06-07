using System.Windows;
namespace Metalama.Patterns.Xaml.AspectTests.DependencyPropertyTests.Callbacks.StaticOnChangingNoParameters;
public partial class StaticOnChangingNoParameters : DependencyObject
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
      this.SetValue(FooProperty, value);
    }
  }
  private static void OnFooChanging()
  {
  }
  public static readonly DependencyProperty FooProperty;
  static StaticOnChangingNoParameters()
  {
    FooProperty = DependencyProperty.Register("Foo", typeof(int), typeof(StaticOnChangingNoParameters), new PropertyMetadata() { CoerceValueCallback = (d, value) =>
    {
      OnFooChanging();
      return value;
    } });
  }
}