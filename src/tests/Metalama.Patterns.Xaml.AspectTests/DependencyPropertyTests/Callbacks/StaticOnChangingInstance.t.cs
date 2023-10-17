using System.Windows;
namespace Metalama.Patterns.Xaml.AspectTests.DependencyPropertyTests.Callbacks.StaticOnChangingInstance;
public partial class StaticOnChangingInstance : DependencyObject
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
      this.SetValue(StaticOnChangingInstance.FooProperty, value);
    }
  }
  private static void OnFooChanging(StaticOnChangingInstance instance)
  {
  }
  [DependencyProperty]
  public int AcceptsDependencyObject
  {
    get
    {
      return (int)GetValue(AcceptsDependencyObjectProperty);
    }
    set
    {
      this.SetValue(StaticOnChangingInstance.AcceptsDependencyObjectProperty, value);
    }
  }
  private static void OnAcceptsDependencyObjectChanging(DependencyObject instance)
  {
  }
  [DependencyProperty]
  public int AcceptsObject
  {
    get
    {
      return (int)GetValue(AcceptsObjectProperty);
    }
    set
    {
      this.SetValue(StaticOnChangingInstance.AcceptsObjectProperty, value);
    }
  }
  private static void OnAcceptsObjectChanging(object instance)
  {
  }
  public static readonly DependencyProperty AcceptsDependencyObjectProperty;
  public static readonly DependencyProperty AcceptsObjectProperty;
  public static readonly DependencyProperty FooProperty;
  static StaticOnChangingInstance()
  {
    object CoerceValue_3(DependencyObject d_2, object value_2)
    {
      StaticOnChangingInstance.OnAcceptsObjectChanging(d_2);
      return value_2;
    }
    var metadata_2 = new PropertyMetadata();
    metadata_2.CoerceValueCallback = CoerceValue_3;
    StaticOnChangingInstance.AcceptsObjectProperty = DependencyProperty.Register("AcceptsObject", typeof(int), typeof(StaticOnChangingInstance), metadata_2);
    object CoerceValue_2(DependencyObject d_1, object value_1)
    {
      StaticOnChangingInstance.OnAcceptsDependencyObjectChanging(d_1);
      return value_1;
    }
    var metadata_1 = new PropertyMetadata();
    metadata_1.CoerceValueCallback = CoerceValue_2;
    StaticOnChangingInstance.AcceptsDependencyObjectProperty = DependencyProperty.Register("AcceptsDependencyObject", typeof(int), typeof(StaticOnChangingInstance), metadata_1);
    object CoerceValue_1(DependencyObject d, object value)
    {
      StaticOnChangingInstance.OnFooChanging((StaticOnChangingInstance)d);
      return value;
    }
    var metadata = new PropertyMetadata();
    metadata.CoerceValueCallback = CoerceValue_1;
    StaticOnChangingInstance.FooProperty = DependencyProperty.Register("Foo", typeof(int), typeof(StaticOnChangingInstance), metadata);
  }
}