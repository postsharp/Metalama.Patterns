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
      this.SetValue(FooProperty, value);
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
      this.SetValue(AcceptsDependencyObjectProperty, value);
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
      this.SetValue(AcceptsObjectProperty, value);
    }
  }
  private static void OnAcceptsObjectChanging(object instance)
  {
  }
  public static readonly DependencyProperty AcceptsDependencyObjectProperty = CreateAcceptsDependencyObjectProperty();
  public static readonly DependencyProperty AcceptsObjectProperty = CreateAcceptsObjectProperty();
  public static readonly DependencyProperty FooProperty = CreateFooProperty();
  private static DependencyProperty CreateAcceptsDependencyObjectProperty()
  {
    object CoerceValue_1(DependencyObject d, object value)
    {
      OnAcceptsDependencyObjectChanging(d);
      return value;
    }
    var metadata = new PropertyMetadata();
    metadata.CoerceValueCallback = CoerceValue_1;
    return DependencyProperty.Register("AcceptsDependencyObject", typeof(int), typeof(StaticOnChangingInstance), metadata);
  }
  private static DependencyProperty CreateAcceptsObjectProperty()
  {
    object CoerceValue_1(DependencyObject d, object value)
    {
      OnAcceptsObjectChanging(d);
      return value;
    }
    var metadata = new PropertyMetadata();
    metadata.CoerceValueCallback = CoerceValue_1;
    return DependencyProperty.Register("AcceptsObject", typeof(int), typeof(StaticOnChangingInstance), metadata);
  }
  private static DependencyProperty CreateFooProperty()
  {
    object CoerceValue_1(DependencyObject d, object value)
    {
      OnFooChanging((StaticOnChangingInstance)d);
      return value;
    }
    var metadata = new PropertyMetadata();
    metadata.CoerceValueCallback = CoerceValue_1;
    return DependencyProperty.Register("Foo", typeof(int), typeof(StaticOnChangingInstance), metadata);
  }
}