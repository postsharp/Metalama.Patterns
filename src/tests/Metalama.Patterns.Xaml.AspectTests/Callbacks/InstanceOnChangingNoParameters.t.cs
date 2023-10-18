using System.Windows;
namespace Metalama.Patterns.Xaml.AspectTests.Callbacks.InstanceOnChangingNoParameters;
public partial class InstanceOnChangingNoParameters : DependencyObject
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
      this.SetValue(InstanceOnChangingNoParameters.FooProperty, value);
    }
  }
  private void OnFooChanging()
  {
  }
  public static readonly DependencyProperty FooProperty;
  static InstanceOnChangingNoParameters()
  {
    object CoerceValue_1(DependencyObject d, object value)
    {
      ((InstanceOnChangingNoParameters)d).OnFooChanging();
      return value;
    }
    var metadata = new PropertyMetadata();
    metadata.CoerceValueCallback = CoerceValue_1;
    InstanceOnChangingNoParameters.FooProperty = DependencyProperty.Register("Foo", typeof(int), typeof(InstanceOnChangingNoParameters), metadata);
  }
}