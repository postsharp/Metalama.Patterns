// Warning LAMA5206 on `Foo`: `No property-changed method was found using the default naming convention, with candidate member name 'OnFooChanged'.`
// Warning LAMA5206 on `Foo`: `No validate method was found using the default naming convention, with candidate member name 'ValidateFoo'.`
using System.Windows;
namespace Metalama.Patterns.Xaml.AspectTests.DependencyPropertyTests.Callbacks.InstanceOnChangingNoParameters;
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
      this.SetValue(FooProperty, value);
    }
  }
  private void OnFooChanging()
  {
  }
  public static readonly DependencyProperty FooProperty;
  static InstanceOnChangingNoParameters()
  {
    FooProperty = DependencyProperty.Register("Foo", typeof(int), typeof(InstanceOnChangingNoParameters), new PropertyMetadata() { CoerceValueCallback = (d, value) =>
    {
      ((InstanceOnChangingNoParameters)d).OnFooChanging();
      return value;
    } });
  }
}