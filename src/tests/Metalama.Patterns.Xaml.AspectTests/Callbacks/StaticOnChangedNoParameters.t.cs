using System.Windows;
namespace Metalama.Patterns.Xaml.AspectTests.Callbacks.StaticOnChangedNoParameters;
public partial class StaticOnChangedNoParameters : DependencyObject
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
      this.SetValue(StaticOnChangedNoParameters.FooProperty, value);
    }
  }
  private static void OnFooChanged()
  {
  }
  public static readonly DependencyProperty FooProperty;
  static StaticOnChangedNoParameters()
  {
    void PropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      StaticOnChangedNoParameters.OnFooChanged();
    }
    StaticOnChangedNoParameters.FooProperty = DependencyProperty.Register("Foo", typeof(int), typeof(StaticOnChangedNoParameters), new PropertyMetadata(PropertyChanged));
  }
}