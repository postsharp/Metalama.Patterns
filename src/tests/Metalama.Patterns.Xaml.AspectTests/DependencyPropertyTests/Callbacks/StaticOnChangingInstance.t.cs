// Warning LAMA5206 on `Foo`: `No property-changed method was found using the default naming convention, with candidate member name 'OnFooChanged'.`
// Warning LAMA5206 on `Foo`: `No validate method was found using the default naming convention, with candidate member name 'ValidateFoo'.`
// Warning LAMA5206 on `AcceptsDependencyObject`: `No property-changed method was found using the default naming convention, with candidate member name 'OnAcceptsDependencyObjectChanged'.`
// Warning LAMA5206 on `AcceptsDependencyObject`: `No validate method was found using the default naming convention, with candidate member name 'ValidateAcceptsDependencyObject'.`
// Warning LAMA5206 on `AcceptsObject`: `No property-changed method was found using the default naming convention, with candidate member name 'OnAcceptsObjectChanged'.`
// Warning LAMA5206 on `AcceptsObject`: `No validate method was found using the default naming convention, with candidate member name 'ValidateAcceptsObject'.`
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
  public static readonly DependencyProperty AcceptsDependencyObjectProperty = StaticOnChangingInstance.CreateAcceptsDependencyObjectDependencyProperty();
  public static readonly DependencyProperty AcceptsObjectProperty = StaticOnChangingInstance.CreateAcceptsObjectDependencyProperty();
  public static readonly DependencyProperty FooProperty = StaticOnChangingInstance.CreateFooDependencyProperty();
  private static DependencyProperty CreateAcceptsDependencyObjectDependencyProperty()
  {
    object CoerceValue_1(DependencyObject d, object value)
    {
      StaticOnChangingInstance.OnAcceptsDependencyObjectChanging(d);
      return value;
    }
    var metadata = new PropertyMetadata();
    metadata.CoerceValueCallback = CoerceValue_1;
    return DependencyProperty.Register("AcceptsDependencyObject", typeof(int), typeof(StaticOnChangingInstance), metadata);
  }
  private static DependencyProperty CreateAcceptsObjectDependencyProperty()
  {
    object CoerceValue_1(DependencyObject d, object value)
    {
      StaticOnChangingInstance.OnAcceptsObjectChanging(d);
      return value;
    }
    var metadata = new PropertyMetadata();
    metadata.CoerceValueCallback = CoerceValue_1;
    return DependencyProperty.Register("AcceptsObject", typeof(int), typeof(StaticOnChangingInstance), metadata);
  }
  private static DependencyProperty CreateFooDependencyProperty()
  {
    object CoerceValue_1(DependencyObject d, object value)
    {
      StaticOnChangingInstance.OnFooChanging((StaticOnChangingInstance)d);
      return value;
    }
    var metadata = new PropertyMetadata();
    metadata.CoerceValueCallback = CoerceValue_1;
    return DependencyProperty.Register("Foo", typeof(int), typeof(StaticOnChangingInstance), metadata);
  }
}