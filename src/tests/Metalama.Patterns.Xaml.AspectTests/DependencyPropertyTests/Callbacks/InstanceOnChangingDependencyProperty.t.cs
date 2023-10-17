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
      this.SetValue(InstanceOnChangingDependencyProperty.FooProperty, value);
    }
  }
  private void OnFooChanging(DependencyProperty d)
  {
  }
  public static readonly DependencyProperty FooProperty;
  static InstanceOnChangingDependencyProperty()
  {
    object CoerceValue_1(DependencyObject d_1, object value)
    {
      ((InstanceOnChangingDependencyProperty)d_1).OnFooChanging(InstanceOnChangingDependencyProperty.FooProperty);
      return value;
    }
    var metadata = new PropertyMetadata();
    metadata.CoerceValueCallback = CoerceValue_1;
    InstanceOnChangingDependencyProperty.FooProperty = DependencyProperty.Register("Foo", typeof(int), typeof(InstanceOnChangingDependencyProperty), metadata);
  }
}