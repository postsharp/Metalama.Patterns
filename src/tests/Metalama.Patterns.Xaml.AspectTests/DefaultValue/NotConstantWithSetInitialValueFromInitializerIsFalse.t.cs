using System.Windows;
namespace Metalama.Patterns.Xaml.AspectTests.DefaultValue.NotConstantWithSetInitialValueFromInitializerIsFalse;
public class NotConstantWithSetInitialValueFromInitializerIsFalse : DependencyObject
{
  [DependencyProperty(SetInitialValueFromInitializer = false)]
  public List<int> Foo
  {
    get
    {
      return ((global::System.Collections.Generic.List<global::System.Int32>)this.GetValue(global::Metalama.Patterns.Xaml.AspectTests.DefaultValue.NotConstantWithSetInitialValueFromInitializerIsFalse.NotConstantWithSetInitialValueFromInitializerIsFalse.FooProperty));
    }
    set
    {
      this.SetValue(global::Metalama.Patterns.Xaml.AspectTests.DefaultValue.NotConstantWithSetInitialValueFromInitializerIsFalse.NotConstantWithSetInitialValueFromInitializerIsFalse.FooProperty, value);
    }
  }
  public static readonly global::System.Windows.DependencyProperty FooProperty;
  static NotConstantWithSetInitialValueFromInitializerIsFalse()
  {
    object CoerceValue_1(global::System.Windows.DependencyObject d, object value)
    {
      return (global::System.Object)value;
    }
    void PropertyChanged(global::System.Windows.DependencyObject d_1, global::System.Windows.DependencyPropertyChangedEventArgs e)
    {
    }
    global::Metalama.Patterns.Xaml.AspectTests.DefaultValue.NotConstantWithSetInitialValueFromInitializerIsFalse.NotConstantWithSetInitialValueFromInitializerIsFalse.FooProperty = global::System.Windows.DependencyProperty.Register("Foo", typeof(global::System.Collections.Generic.List<global::System.Int32>), typeof(global::Metalama.Patterns.Xaml.AspectTests.DefaultValue.NotConstantWithSetInitialValueFromInitializerIsFalse.NotConstantWithSetInitialValueFromInitializerIsFalse), new global::System.Windows.PropertyMetadata(((global::System.Object)new List<int>(3) { 1, 2, 3 })));
  }
}