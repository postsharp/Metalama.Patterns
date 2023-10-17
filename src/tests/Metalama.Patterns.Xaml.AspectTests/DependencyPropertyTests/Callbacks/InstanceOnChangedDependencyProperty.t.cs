using System.Windows;
namespace Metalama.Patterns.Xaml.AspectTests.DependencyPropertyTests.Callbacks.InstanceOnChangedDependencyProperty;
public partial class InstanceOnChangedDependencyProperty : DependencyObject
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
      this.SetValue(InstanceOnChangedDependencyProperty.FooProperty, value);
    }
  }
  private void OnFooChanged(DependencyProperty d)
  {
  }
  public static readonly DependencyProperty FooProperty;
  static InstanceOnChangedDependencyProperty()
  {
    void PropertyChanged(DependencyObject d_1, DependencyPropertyChangedEventArgs e)
    {
      ((InstanceOnChangedDependencyProperty)d_1).OnFooChanged(InstanceOnChangedDependencyProperty.FooProperty);
    }
    InstanceOnChangedDependencyProperty.FooProperty = DependencyProperty.Register("Foo", typeof(int), typeof(InstanceOnChangedDependencyProperty), new PropertyMetadata(PropertyChanged));
  }
}