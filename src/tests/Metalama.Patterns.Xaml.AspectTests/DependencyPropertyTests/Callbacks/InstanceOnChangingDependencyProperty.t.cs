// Warning LAMA5206 on `Foo`: `No property-changed method was found using the default naming convention, with candidate member name 'OnFooChanged'.`
// Warning LAMA5206 on `Foo`: `No validate method was found using the default naming convention, with candidate member name 'ValidateFoo'.`
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