using System.Windows;
namespace Metalama.Patterns.Xaml.AspectTests.DependencyPropertyTests.Callbacks.InstanceOnChangingDependencyProperty;
public partial class InstanceOnChangingDependencyProperty : DependencyObject
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
  private void OnFooChanging(DependencyProperty d)
  {
  }
  public static readonly DependencyProperty FooProperty;
  static InstanceOnChangingDependencyProperty()
  {
    FooProperty = DependencyProperty.Register("Foo", typeof(int), typeof(InstanceOnChangingDependencyProperty), new PropertyMetadata() { CoerceValueCallback = (d_1, value) =>
    {
      ((InstanceOnChangingDependencyProperty)d_1).OnFooChanging(FooProperty);
      return value;
    } });
  }
}