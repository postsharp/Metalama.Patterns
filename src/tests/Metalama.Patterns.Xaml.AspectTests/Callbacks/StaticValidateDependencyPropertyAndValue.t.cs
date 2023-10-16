using System.Windows;
namespace Metalama.Patterns.Xaml.AspectTests.Callbacks.StaticValidateDependencyPropertyAndValue;
public partial class StaticValidateDependencyPropertyAndValue : DependencyObject
{
  [DependencyProperty]
  public int Foo
  {
    get
    {
      return ((global::System.Int32)this.GetValue(global::Metalama.Patterns.Xaml.AspectTests.Callbacks.StaticValidateDependencyPropertyAndValue.StaticValidateDependencyPropertyAndValue.FooProperty));
    }
    set
    {
      this.SetValue(global::Metalama.Patterns.Xaml.AspectTests.Callbacks.StaticValidateDependencyPropertyAndValue.StaticValidateDependencyPropertyAndValue.FooProperty, value);
    }
  }
  private static bool ValidateFoo(DependencyProperty d, int value) => true;
  [DependencyProperty]
  public List<int> AcceptsAssignable
  {
    get
    {
      return ((global::System.Collections.Generic.List<global::System.Int32>)this.GetValue(global::Metalama.Patterns.Xaml.AspectTests.Callbacks.StaticValidateDependencyPropertyAndValue.StaticValidateDependencyPropertyAndValue.AcceptsAssignableProperty));
    }
    set
    {
      this.SetValue(global::Metalama.Patterns.Xaml.AspectTests.Callbacks.StaticValidateDependencyPropertyAndValue.StaticValidateDependencyPropertyAndValue.AcceptsAssignableProperty, value);
    }
  }
  private static bool ValidateAcceptsAssignable(DependencyProperty d, IEnumerable<int> value) => true;
  [DependencyProperty]
  public int AcceptsGeneric
  {
    get
    {
      return ((global::System.Int32)this.GetValue(global::Metalama.Patterns.Xaml.AspectTests.Callbacks.StaticValidateDependencyPropertyAndValue.StaticValidateDependencyPropertyAndValue.AcceptsGenericProperty));
    }
    set
    {
      this.SetValue(global::Metalama.Patterns.Xaml.AspectTests.Callbacks.StaticValidateDependencyPropertyAndValue.StaticValidateDependencyPropertyAndValue.AcceptsGenericProperty, value);
    }
  }
  private static bool ValidateAcceptsGeneric<T>(DependencyProperty d, T value) => true;
  [DependencyProperty]
  public int AcceptsObject
  {
    get
    {
      return ((global::System.Int32)this.GetValue(global::Metalama.Patterns.Xaml.AspectTests.Callbacks.StaticValidateDependencyPropertyAndValue.StaticValidateDependencyPropertyAndValue.AcceptsObjectProperty));
    }
    set
    {
      this.SetValue(global::Metalama.Patterns.Xaml.AspectTests.Callbacks.StaticValidateDependencyPropertyAndValue.StaticValidateDependencyPropertyAndValue.AcceptsObjectProperty, value);
    }
  }
  private static bool ValidateAcceptsObject(DependencyProperty d, object value) => true;
  public static readonly global::System.Windows.DependencyProperty AcceptsAssignableProperty;
  public static readonly global::System.Windows.DependencyProperty AcceptsGenericProperty;
  public static readonly global::System.Windows.DependencyProperty AcceptsObjectProperty;
  public static readonly global::System.Windows.DependencyProperty FooProperty;
  static StaticValidateDependencyPropertyAndValue()
  {
    object CoerceValue_4(global::System.Windows.DependencyObject d_4, object value_4)
    {
      if (!global::Metalama.Patterns.Xaml.AspectTests.Callbacks.StaticValidateDependencyPropertyAndValue.StaticValidateDependencyPropertyAndValue.ValidateAcceptsObject(global::Metalama.Patterns.Xaml.AspectTests.Callbacks.StaticValidateDependencyPropertyAndValue.StaticValidateDependencyPropertyAndValue.AcceptsObjectProperty, value_4))
      {
        throw new global::System.ArgumentException("Invalid property value.", "value");
      }
      return (global::System.Object)value_4;
    }
    var metadata_3 = new global::System.Windows.PropertyMetadata();
    metadata_3.CoerceValueCallback = (global::System.Windows.CoerceValueCallback)CoerceValue_4;
    global::Metalama.Patterns.Xaml.AspectTests.Callbacks.StaticValidateDependencyPropertyAndValue.StaticValidateDependencyPropertyAndValue.AcceptsObjectProperty = global::System.Windows.DependencyProperty.Register("AcceptsObject", typeof(global::System.Int32), typeof(global::Metalama.Patterns.Xaml.AspectTests.Callbacks.StaticValidateDependencyPropertyAndValue.StaticValidateDependencyPropertyAndValue), metadata_3);
    object CoerceValue_3(global::System.Windows.DependencyObject d_3, object value_3)
    {
      if (!global::Metalama.Patterns.Xaml.AspectTests.Callbacks.StaticValidateDependencyPropertyAndValue.StaticValidateDependencyPropertyAndValue.ValidateAcceptsGeneric<global::System.Int32>(global::Metalama.Patterns.Xaml.AspectTests.Callbacks.StaticValidateDependencyPropertyAndValue.StaticValidateDependencyPropertyAndValue.AcceptsGenericProperty, (global::System.Int32)value_3))
      {
        throw new global::System.ArgumentException("Invalid property value.", "value");
      }
      return (global::System.Object)value_3;
    }
    var metadata_2 = new global::System.Windows.PropertyMetadata();
    metadata_2.CoerceValueCallback = (global::System.Windows.CoerceValueCallback)CoerceValue_3;
    global::Metalama.Patterns.Xaml.AspectTests.Callbacks.StaticValidateDependencyPropertyAndValue.StaticValidateDependencyPropertyAndValue.AcceptsGenericProperty = global::System.Windows.DependencyProperty.Register("AcceptsGeneric", typeof(global::System.Int32), typeof(global::Metalama.Patterns.Xaml.AspectTests.Callbacks.StaticValidateDependencyPropertyAndValue.StaticValidateDependencyPropertyAndValue), metadata_2);
    object CoerceValue_2(global::System.Windows.DependencyObject d_2, object value_2)
    {
      if (!global::Metalama.Patterns.Xaml.AspectTests.Callbacks.StaticValidateDependencyPropertyAndValue.StaticValidateDependencyPropertyAndValue.ValidateAcceptsAssignable(global::Metalama.Patterns.Xaml.AspectTests.Callbacks.StaticValidateDependencyPropertyAndValue.StaticValidateDependencyPropertyAndValue.AcceptsAssignableProperty, (global::System.Collections.Generic.List<global::System.Int32>)value_2))
      {
        throw new global::System.ArgumentException("Invalid property value.", "value");
      }
      return (global::System.Object)value_2;
    }
    var metadata_1 = new global::System.Windows.PropertyMetadata();
    metadata_1.CoerceValueCallback = (global::System.Windows.CoerceValueCallback)CoerceValue_2;
    global::Metalama.Patterns.Xaml.AspectTests.Callbacks.StaticValidateDependencyPropertyAndValue.StaticValidateDependencyPropertyAndValue.AcceptsAssignableProperty = global::System.Windows.DependencyProperty.Register("AcceptsAssignable", typeof(global::System.Collections.Generic.List<global::System.Int32>), typeof(global::Metalama.Patterns.Xaml.AspectTests.Callbacks.StaticValidateDependencyPropertyAndValue.StaticValidateDependencyPropertyAndValue), metadata_1);
    object CoerceValue_1(global::System.Windows.DependencyObject d_1, object value_1)
    {
      if (!global::Metalama.Patterns.Xaml.AspectTests.Callbacks.StaticValidateDependencyPropertyAndValue.StaticValidateDependencyPropertyAndValue.ValidateFoo(global::Metalama.Patterns.Xaml.AspectTests.Callbacks.StaticValidateDependencyPropertyAndValue.StaticValidateDependencyPropertyAndValue.FooProperty, (global::System.Int32)value_1))
      {
        throw new global::System.ArgumentException("Invalid property value.", "value");
      }
      return (global::System.Object)value_1;
    }
    var metadata = new global::System.Windows.PropertyMetadata();
    metadata.CoerceValueCallback = (global::System.Windows.CoerceValueCallback)CoerceValue_1;
    global::Metalama.Patterns.Xaml.AspectTests.Callbacks.StaticValidateDependencyPropertyAndValue.StaticValidateDependencyPropertyAndValue.FooProperty = global::System.Windows.DependencyProperty.Register("Foo", typeof(global::System.Int32), typeof(global::Metalama.Patterns.Xaml.AspectTests.Callbacks.StaticValidateDependencyPropertyAndValue.StaticValidateDependencyPropertyAndValue), metadata);
  }
}