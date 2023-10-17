using System.Windows;
namespace Metalama.Patterns.Xaml.AspectTests.Callbacks.InstanceOnChangedNoParameters;
public partial class InstanceOnChangedNoParameters : DependencyObject
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
      this.SetValue(InstanceOnChangedNoParameters.FooProperty, value);
    }
  }
  private void OnFooChanged()
  {
  }
  public static readonly DependencyProperty FooProperty;
  static InstanceOnChangedNoParameters()
  {
    void PropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((InstanceOnChangedNoParameters)d).OnFooChanged();
    }
    InstanceOnChangedNoParameters.FooProperty = DependencyProperty.Register("Foo", typeof(int), typeof(InstanceOnChangedNoParameters), new PropertyMetadata(PropertyChanged));
  }
}