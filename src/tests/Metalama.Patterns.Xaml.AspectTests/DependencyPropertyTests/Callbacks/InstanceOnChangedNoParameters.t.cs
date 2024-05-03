// Warning LAMA5206 on `Foo`: `No property-changing method was found using the default naming convention, with candidate member name 'OnFooChanging'.`
// Warning LAMA5206 on `Foo`: `No validate method was found using the default naming convention, with candidate member name 'ValidateFoo'.`
using System.Windows;
namespace Metalama.Patterns.Xaml.AspectTests.DependencyPropertyTests.Callbacks.InstanceOnChangedNoParameters;
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
  public static readonly DependencyProperty FooProperty = InstanceOnChangedNoParameters.CreateFooProperty();
  private static DependencyProperty CreateFooProperty()
  {
    void PropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((InstanceOnChangedNoParameters)d).OnFooChanged();
    }
    return DependencyProperty.Register("Foo", typeof(int), typeof(InstanceOnChangedNoParameters), new PropertyMetadata(PropertyChanged));
  }
}