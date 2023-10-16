using System.Windows;
namespace Metalama.Patterns.Xaml.AspectTests.Callbacks.StaticValidateInstanceAndValue;
public partial class StaticValidateInstanceAndValue : DependencyObject
{
  [DependencyProperty]
  public int Foo
  {
    get
    {
      return ((global::System.Int32)this.GetValue(global::Metalama.Patterns.Xaml.AspectTests.Callbacks.StaticValidateInstanceAndValue.StaticValidateInstanceAndValue.FooProperty));
    }
    set
    {
      this.SetValue(global::Metalama.Patterns.Xaml.AspectTests.Callbacks.StaticValidateInstanceAndValue.StaticValidateInstanceAndValue.FooProperty, value);
    }
  }
  private static bool ValidateFoo(StaticValidateInstanceAndValue instance, int value) => true;
  [DependencyProperty]
  public List<int> AcceptsAssignableForValue
  {
    get
    {
      return ((global::System.Collections.Generic.List<global::System.Int32>)this.GetValue(global::Metalama.Patterns.Xaml.AspectTests.Callbacks.StaticValidateInstanceAndValue.StaticValidateInstanceAndValue.AcceptsAssignableForValueProperty));
    }
    set
    {
      this.SetValue(global::Metalama.Patterns.Xaml.AspectTests.Callbacks.StaticValidateInstanceAndValue.StaticValidateInstanceAndValue.AcceptsAssignableForValueProperty, value);
    }
  }
  private static bool ValidateAcceptsAssignableForValue(StaticValidateInstanceAndValue instance, IEnumerable<int> value) => true;
  [DependencyProperty]
  public int AcceptsGenericForValue
  {
    get
    {
      return ((global::System.Int32)this.GetValue(global::Metalama.Patterns.Xaml.AspectTests.Callbacks.StaticValidateInstanceAndValue.StaticValidateInstanceAndValue.AcceptsGenericForValueProperty));
    }
    set
    {
      this.SetValue(global::Metalama.Patterns.Xaml.AspectTests.Callbacks.StaticValidateInstanceAndValue.StaticValidateInstanceAndValue.AcceptsGenericForValueProperty, value);
    }
  }
  private static bool ValidateAcceptsGenericForValue<T>(StaticValidateInstanceAndValue instance, T value) => true;
  [DependencyProperty]
  public int AcceptsObjectForValue
  {
    get
    {
      return ((global::System.Int32)this.GetValue(global::Metalama.Patterns.Xaml.AspectTests.Callbacks.StaticValidateInstanceAndValue.StaticValidateInstanceAndValue.AcceptsObjectForValueProperty));
    }
    set
    {
      this.SetValue(global::Metalama.Patterns.Xaml.AspectTests.Callbacks.StaticValidateInstanceAndValue.StaticValidateInstanceAndValue.AcceptsObjectForValueProperty, value);
    }
  }
  private static bool ValidateAcceptsObjectForValue(StaticValidateInstanceAndValue instance, object value) => true;
  [DependencyProperty]
  public int AcceptsDependencyObjectForInstance
  {
    get
    {
      return ((global::System.Int32)this.GetValue(global::Metalama.Patterns.Xaml.AspectTests.Callbacks.StaticValidateInstanceAndValue.StaticValidateInstanceAndValue.AcceptsDependencyObjectForInstanceProperty));
    }
    set
    {
      this.SetValue(global::Metalama.Patterns.Xaml.AspectTests.Callbacks.StaticValidateInstanceAndValue.StaticValidateInstanceAndValue.AcceptsDependencyObjectForInstanceProperty, value);
    }
  }
  private static bool ValidateAcceptsDependencyObjectForInstance(DependencyObject instance, int value) => true;
  [DependencyProperty]
  public int AcceptsObjectForInstance
  {
    get
    {
      return ((global::System.Int32)this.GetValue(global::Metalama.Patterns.Xaml.AspectTests.Callbacks.StaticValidateInstanceAndValue.StaticValidateInstanceAndValue.AcceptsObjectForInstanceProperty));
    }
    set
    {
      this.SetValue(global::Metalama.Patterns.Xaml.AspectTests.Callbacks.StaticValidateInstanceAndValue.StaticValidateInstanceAndValue.AcceptsObjectForInstanceProperty, value);
    }
  }
  private static bool ValidateAcceptsObjectForInstance(object instance, int value) => true;
  public static readonly global::System.Windows.DependencyProperty AcceptsAssignableForValueProperty;
  public static readonly global::System.Windows.DependencyProperty AcceptsDependencyObjectForInstanceProperty;
  public static readonly global::System.Windows.DependencyProperty AcceptsGenericForValueProperty;
  public static readonly global::System.Windows.DependencyProperty AcceptsObjectForInstanceProperty;
  public static readonly global::System.Windows.DependencyProperty AcceptsObjectForValueProperty;
  public static readonly global::System.Windows.DependencyProperty FooProperty;
  static StaticValidateInstanceAndValue()
  {
    object CoerceValue_6(global::System.Windows.DependencyObject d_5, object value_6)
    {
      if (!global::Metalama.Patterns.Xaml.AspectTests.Callbacks.StaticValidateInstanceAndValue.StaticValidateInstanceAndValue.ValidateAcceptsObjectForInstance(d_5, (global::System.Int32)value_6))
      {
        throw new global::System.ArgumentException("Invalid property value.", "value");
      }
      return (global::System.Object)value_6;
    }
    var metadata_5 = new global::System.Windows.PropertyMetadata();
    metadata_5.CoerceValueCallback = (global::System.Windows.CoerceValueCallback)CoerceValue_6;
    global::Metalama.Patterns.Xaml.AspectTests.Callbacks.StaticValidateInstanceAndValue.StaticValidateInstanceAndValue.AcceptsObjectForInstanceProperty = global::System.Windows.DependencyProperty.Register("AcceptsObjectForInstance", typeof(global::System.Int32), typeof(global::Metalama.Patterns.Xaml.AspectTests.Callbacks.StaticValidateInstanceAndValue.StaticValidateInstanceAndValue), metadata_5);
    object CoerceValue_5(global::System.Windows.DependencyObject d_4, object value_5)
    {
      if (!global::Metalama.Patterns.Xaml.AspectTests.Callbacks.StaticValidateInstanceAndValue.StaticValidateInstanceAndValue.ValidateAcceptsDependencyObjectForInstance(d_4, (global::System.Int32)value_5))
      {
        throw new global::System.ArgumentException("Invalid property value.", "value");
      }
      return (global::System.Object)value_5;
    }
    var metadata_4 = new global::System.Windows.PropertyMetadata();
    metadata_4.CoerceValueCallback = (global::System.Windows.CoerceValueCallback)CoerceValue_5;
    global::Metalama.Patterns.Xaml.AspectTests.Callbacks.StaticValidateInstanceAndValue.StaticValidateInstanceAndValue.AcceptsDependencyObjectForInstanceProperty = global::System.Windows.DependencyProperty.Register("AcceptsDependencyObjectForInstance", typeof(global::System.Int32), typeof(global::Metalama.Patterns.Xaml.AspectTests.Callbacks.StaticValidateInstanceAndValue.StaticValidateInstanceAndValue), metadata_4);
    object CoerceValue_4(global::System.Windows.DependencyObject d_3, object value_4)
    {
      if (!global::Metalama.Patterns.Xaml.AspectTests.Callbacks.StaticValidateInstanceAndValue.StaticValidateInstanceAndValue.ValidateAcceptsObjectForValue((global::Metalama.Patterns.Xaml.AspectTests.Callbacks.StaticValidateInstanceAndValue.StaticValidateInstanceAndValue)d_3, value_4))
      {
        throw new global::System.ArgumentException("Invalid property value.", "value");
      }
      return (global::System.Object)value_4;
    }
    var metadata_3 = new global::System.Windows.PropertyMetadata();
    metadata_3.CoerceValueCallback = (global::System.Windows.CoerceValueCallback)CoerceValue_4;
    global::Metalama.Patterns.Xaml.AspectTests.Callbacks.StaticValidateInstanceAndValue.StaticValidateInstanceAndValue.AcceptsObjectForValueProperty = global::System.Windows.DependencyProperty.Register("AcceptsObjectForValue", typeof(global::System.Int32), typeof(global::Metalama.Patterns.Xaml.AspectTests.Callbacks.StaticValidateInstanceAndValue.StaticValidateInstanceAndValue), metadata_3);
    object CoerceValue_3(global::System.Windows.DependencyObject d_2, object value_3)
    {
      if (!global::Metalama.Patterns.Xaml.AspectTests.Callbacks.StaticValidateInstanceAndValue.StaticValidateInstanceAndValue.ValidateAcceptsGenericForValue<global::System.Int32>((global::Metalama.Patterns.Xaml.AspectTests.Callbacks.StaticValidateInstanceAndValue.StaticValidateInstanceAndValue)d_2, (global::System.Int32)value_3))
      {
        throw new global::System.ArgumentException("Invalid property value.", "value");
      }
      return (global::System.Object)value_3;
    }
    var metadata_2 = new global::System.Windows.PropertyMetadata();
    metadata_2.CoerceValueCallback = (global::System.Windows.CoerceValueCallback)CoerceValue_3;
    global::Metalama.Patterns.Xaml.AspectTests.Callbacks.StaticValidateInstanceAndValue.StaticValidateInstanceAndValue.AcceptsGenericForValueProperty = global::System.Windows.DependencyProperty.Register("AcceptsGenericForValue", typeof(global::System.Int32), typeof(global::Metalama.Patterns.Xaml.AspectTests.Callbacks.StaticValidateInstanceAndValue.StaticValidateInstanceAndValue), metadata_2);
    object CoerceValue_2(global::System.Windows.DependencyObject d_1, object value_2)
    {
      if (!global::Metalama.Patterns.Xaml.AspectTests.Callbacks.StaticValidateInstanceAndValue.StaticValidateInstanceAndValue.ValidateAcceptsAssignableForValue((global::Metalama.Patterns.Xaml.AspectTests.Callbacks.StaticValidateInstanceAndValue.StaticValidateInstanceAndValue)d_1, (global::System.Collections.Generic.List<global::System.Int32>)value_2))
      {
        throw new global::System.ArgumentException("Invalid property value.", "value");
      }
      return (global::System.Object)value_2;
    }
    var metadata_1 = new global::System.Windows.PropertyMetadata();
    metadata_1.CoerceValueCallback = (global::System.Windows.CoerceValueCallback)CoerceValue_2;
    global::Metalama.Patterns.Xaml.AspectTests.Callbacks.StaticValidateInstanceAndValue.StaticValidateInstanceAndValue.AcceptsAssignableForValueProperty = global::System.Windows.DependencyProperty.Register("AcceptsAssignableForValue", typeof(global::System.Collections.Generic.List<global::System.Int32>), typeof(global::Metalama.Patterns.Xaml.AspectTests.Callbacks.StaticValidateInstanceAndValue.StaticValidateInstanceAndValue), metadata_1);
    object CoerceValue_1(global::System.Windows.DependencyObject d, object value_1)
    {
      if (!global::Metalama.Patterns.Xaml.AspectTests.Callbacks.StaticValidateInstanceAndValue.StaticValidateInstanceAndValue.ValidateFoo((global::Metalama.Patterns.Xaml.AspectTests.Callbacks.StaticValidateInstanceAndValue.StaticValidateInstanceAndValue)d, (global::System.Int32)value_1))
      {
        throw new global::System.ArgumentException("Invalid property value.", "value");
      }
      return (global::System.Object)value_1;
    }
    var metadata = new global::System.Windows.PropertyMetadata();
    metadata.CoerceValueCallback = (global::System.Windows.CoerceValueCallback)CoerceValue_1;
    global::Metalama.Patterns.Xaml.AspectTests.Callbacks.StaticValidateInstanceAndValue.StaticValidateInstanceAndValue.FooProperty = global::System.Windows.DependencyProperty.Register("Foo", typeof(global::System.Int32), typeof(global::Metalama.Patterns.Xaml.AspectTests.Callbacks.StaticValidateInstanceAndValue.StaticValidateInstanceAndValue), metadata);
  }
}