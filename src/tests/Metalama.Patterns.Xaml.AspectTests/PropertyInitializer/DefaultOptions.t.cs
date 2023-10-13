using System.Windows;
namespace Metalama.Patterns.Xaml.AspectTests.PropertyInitializer.DefaultOptions;
public class DefaultOptions : DependencyObject
{
  [DependencyProperty]
  public List<int> Foo
  {
    get
    {
      return ((global::System.Collections.Generic.List<global::System.Int32>)this.GetValue(global::Metalama.Patterns.Xaml.AspectTests.PropertyInitializer.DefaultOptions.DefaultOptions.FooProperty));
    }
    set
    {
      this.SetValue(global::Metalama.Patterns.Xaml.AspectTests.PropertyInitializer.DefaultOptions.DefaultOptions.FooProperty, value);
    }
  }
  public static readonly global::System.Windows.DependencyProperty FooProperty;
  static DefaultOptions()
  {
    object CoerceValue_1(global::System.Windows.DependencyObject d, object value)
    {
      return (global::System.Object)value;
    }
    void PropertyChanged(global::System.Windows.DependencyObject d_1, global::System.Windows.DependencyPropertyChangedEventArgs e)
    {
    }
    global::Metalama.Patterns.Xaml.AspectTests.PropertyInitializer.DefaultOptions.DefaultOptions.FooProperty = global::System.Windows.DependencyProperty.Register("Foo", typeof(global::System.Collections.Generic.List<global::System.Int32>), typeof(global::Metalama.Patterns.Xaml.AspectTests.PropertyInitializer.DefaultOptions.DefaultOptions), new global::System.Windows.PropertyMetadata(((global::System.Object)new List<int>(3) { 1, 2, 3 })));
  }
  public DefaultOptions()
  {
    this.Foo = new List<int>(3)
    {
      1,
      2,
      3
    };
  }
}