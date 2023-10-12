using System.Windows;
namespace Metalama.Patterns.Xaml.AspectTests.DefaultValue.NotConstant;
public class NotConstant : DependencyObject
{
  [DependencyProperty]
  public List<int> Foo
  {
    get
    {
      return ((global::System.Collections.Generic.List<global::System.Int32>)this.GetValue(global::Metalama.Patterns.Xaml.AspectTests.DefaultValue.NotConstant.NotConstant.FooProperty));
    }
    set
    {
      this.SetValue(global::Metalama.Patterns.Xaml.AspectTests.DefaultValue.NotConstant.NotConstant.FooProperty, value);
    }
  }
  public static readonly global::System.Windows.DependencyProperty FooProperty;
  private static readonly global::System.Collections.Generic.List<global::System.Int32> _initialValueOfFoo = new List<int>(3)
  {
    1,
    2,
    3
  };
  static NotConstant()
  {
    object CoerceValue_1(global::System.Windows.DependencyObject d, object value)
    {
      return (global::System.Object)value;
    }
    void PropertyChanged(global::System.Windows.DependencyObject d_1, global::System.Windows.DependencyPropertyChangedEventArgs e)
    {
    }
    global::Metalama.Patterns.Xaml.AspectTests.DefaultValue.NotConstant.NotConstant.FooProperty = global::System.Windows.DependencyProperty.Register("Foo", typeof(global::System.Collections.Generic.List<global::System.Int32>), typeof(global::Metalama.Patterns.Xaml.AspectTests.DefaultValue.NotConstant.NotConstant), new global::System.Windows.PropertyMetadata(((global::System.Object)global::Metalama.Patterns.Xaml.AspectTests.DefaultValue.NotConstant.NotConstant._initialValueOfFoo)));
  }
  public NotConstant()
  {
    this.Foo = global::Metalama.Patterns.Xaml.AspectTests.DefaultValue.NotConstant.NotConstant._initialValueOfFoo;
  }
}