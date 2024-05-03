// Warning LAMA5206 on `Foo`: `No property-changing method was found using the default naming convention, with candidate member name 'OnFooChanging'.`
// Warning LAMA5206 on `Foo`: `No validate method was found using the default naming convention, with candidate member name 'ValidateFoo'.`
using System.Windows;
namespace Metalama.Patterns.Xaml.AspectTests.DependencyPropertyTests.Callbacks.StaticOnChangedNoParameters;
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
  public static readonly DependencyProperty FooProperty = StaticOnChangedNoParameters.CreateFooProperty();
  private static DependencyProperty CreateFooProperty()
  {
    void PropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      StaticOnChangedNoParameters.OnFooChanged();
    }
    return DependencyProperty.Register("Foo", typeof(int), typeof(StaticOnChangedNoParameters), new PropertyMetadata(PropertyChanged));
  }
}