using System.Windows;
namespace Metalama.Patterns.Xaml.AspectTests.Callbacks.StaticOnChangingNoParameters;
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
      this.SetValue(StaticOnChangingNoParameters.FooProperty, value);
    }
  }
  private static void OnFooChanging()
  {
  }
  public static readonly DependencyProperty FooProperty;
  static StaticOnChangingNoParameters()
  {
    object CoerceValue_1(DependencyObject d, object value)
    {
      StaticOnChangingNoParameters.OnFooChanging();
      return value;
    }
    var metadata = new PropertyMetadata();
    metadata.CoerceValueCallback = CoerceValue_1;
    StaticOnChangingNoParameters.FooProperty = DependencyProperty.Register("Foo", typeof(int), typeof(StaticOnChangingNoParameters), metadata);
  }
}