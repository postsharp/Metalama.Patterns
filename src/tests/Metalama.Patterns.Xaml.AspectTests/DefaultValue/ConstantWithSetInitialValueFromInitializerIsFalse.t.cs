using System.Windows;
namespace Metalama.Patterns.Xaml.AspectTests.DefaultValue.ConstantWithSetInitialValueFromInitializerIsFalse;
public class ConstantWithSetInitialValueFromInitializerIsFalse : DependencyObject
{
  [DependencyProperty(SetInitialValueFromInitializer = false)]
  public int Foo
  {
    get
    {
      return ((global::System.Int32)this.GetValue(global::Metalama.Patterns.Xaml.AspectTests.DefaultValue.ConstantWithSetInitialValueFromInitializerIsFalse.ConstantWithSetInitialValueFromInitializerIsFalse.FooProperty));
    }
    set
    {
      this.SetValue(global::Metalama.Patterns.Xaml.AspectTests.DefaultValue.ConstantWithSetInitialValueFromInitializerIsFalse.ConstantWithSetInitialValueFromInitializerIsFalse.FooProperty, value);
    }
  }
  public static readonly global::System.Windows.DependencyProperty FooProperty;
  static ConstantWithSetInitialValueFromInitializerIsFalse()
  {
    object CoerceValue_1(global::System.Windows.DependencyObject d, object value)
    {
      return (global::System.Object)value;
    }
    void PropertyChanged(global::System.Windows.DependencyObject d_1, global::System.Windows.DependencyPropertyChangedEventArgs e)
    {
    }
    global::Metalama.Patterns.Xaml.AspectTests.DefaultValue.ConstantWithSetInitialValueFromInitializerIsFalse.ConstantWithSetInitialValueFromInitializerIsFalse.FooProperty = global::System.Windows.DependencyProperty.Register("Foo", typeof(global::System.Int32), typeof(global::Metalama.Patterns.Xaml.AspectTests.DefaultValue.ConstantWithSetInitialValueFromInitializerIsFalse.ConstantWithSetInitialValueFromInitializerIsFalse), new global::System.Windows.PropertyMetadata(((global::System.Object)42)));
  }
}