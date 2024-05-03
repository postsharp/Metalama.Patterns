// Warning LAMA5206 on `Foo`: `No property-changing method was found using the default naming convention, with candidate member name 'OnFooChanging'.`
// Warning LAMA5206 on `Foo`: `No validate method was found using the default naming convention, with candidate member name 'ValidateFoo'.`
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
  public static readonly DependencyProperty FooProperty = InstanceOnChangedDependencyProperty.CreateFooProperty();
  private static DependencyProperty CreateFooProperty()
  {
    void PropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((InstanceOnChangedDependencyProperty)d).OnFooChanged(InstanceOnChangedDependencyProperty.FooProperty);
    }
    return DependencyProperty.Register("Foo", typeof(int), typeof(InstanceOnChangedDependencyProperty), new PropertyMetadata(PropertyChanged));
  }
}