using System.Windows;
namespace Metalama.Patterns.Xaml.AspectTests.PropertyInitializer.InitializerProvidesInitialValueIsFalse;
public class InitializerProvidesInitialValueIsFalse : DependencyObject
{
  [DependencyProperty(InitializerProvidesInitialValue = false)]
  public List<int> Foo
  {
    get
    {
      return ((global::System.Collections.Generic.List<global::System.Int32>)this.GetValue(global::Metalama.Patterns.Xaml.AspectTests.PropertyInitializer.InitializerProvidesInitialValueIsFalse.InitializerProvidesInitialValueIsFalse.FooProperty));
    }
    set
    {
      this.SetValue(global::Metalama.Patterns.Xaml.AspectTests.PropertyInitializer.InitializerProvidesInitialValueIsFalse.InitializerProvidesInitialValueIsFalse.FooProperty, value);
    }
  }
  public static readonly global::System.Windows.DependencyProperty FooProperty;
  static InitializerProvidesInitialValueIsFalse()
  {
    object CoerceValue_1(global::System.Windows.DependencyObject d, object value)
    {
      return (global::System.Object)value;
    }
    void PropertyChanged(global::System.Windows.DependencyObject d_1, global::System.Windows.DependencyPropertyChangedEventArgs e)
    {
    }
    global::Metalama.Patterns.Xaml.AspectTests.PropertyInitializer.InitializerProvidesInitialValueIsFalse.InitializerProvidesInitialValueIsFalse.FooProperty = global::System.Windows.DependencyProperty.Register("Foo", typeof(global::System.Collections.Generic.List<global::System.Int32>), typeof(global::Metalama.Patterns.Xaml.AspectTests.PropertyInitializer.InitializerProvidesInitialValueIsFalse.InitializerProvidesInitialValueIsFalse), new global::System.Windows.PropertyMetadata(((global::System.Object)new List<int>(3) { 1, 2, 3 })));
  }
}