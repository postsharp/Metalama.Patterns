using System.Windows;
namespace Metalama.Patterns.Xaml.AspectTests.Callbacks.StaticOnChangedInstance;
public partial class StaticOnChangedInstance : DependencyObject
{
  [DependencyProperty]
  public int Foo
  {
    get
    {
      return ((global::System.Int32)this.GetValue(global::Metalama.Patterns.Xaml.AspectTests.Callbacks.StaticOnChangedInstance.StaticOnChangedInstance.FooProperty));
    }
    set
    {
      this.SetValue(global::Metalama.Patterns.Xaml.AspectTests.Callbacks.StaticOnChangedInstance.StaticOnChangedInstance.FooProperty, value);
    }
  }
  private static void OnFooChanged(StaticOnChangedInstance instance)
  {
  }
  [DependencyProperty]
  public int AcceptsDependencyObject
  {
    get
    {
      return ((global::System.Int32)this.GetValue(global::Metalama.Patterns.Xaml.AspectTests.Callbacks.StaticOnChangedInstance.StaticOnChangedInstance.AcceptsDependencyObjectProperty));
    }
    set
    {
      this.SetValue(global::Metalama.Patterns.Xaml.AspectTests.Callbacks.StaticOnChangedInstance.StaticOnChangedInstance.AcceptsDependencyObjectProperty, value);
    }
  }
  private static void OnAcceptsDependencyObjectChanged(DependencyObject instance)
  {
  }
  [DependencyProperty]
  public int AcceptsObject
  {
    get
    {
      return ((global::System.Int32)this.GetValue(global::Metalama.Patterns.Xaml.AspectTests.Callbacks.StaticOnChangedInstance.StaticOnChangedInstance.AcceptsObjectProperty));
    }
    set
    {
      this.SetValue(global::Metalama.Patterns.Xaml.AspectTests.Callbacks.StaticOnChangedInstance.StaticOnChangedInstance.AcceptsObjectProperty, value);
    }
  }
  private static void OnAcceptsObjectChanged(object instance)
  {
  }
  public static readonly global::System.Windows.DependencyProperty AcceptsDependencyObjectProperty;
  public static readonly global::System.Windows.DependencyProperty AcceptsObjectProperty;
  public static readonly global::System.Windows.DependencyProperty FooProperty;
  static StaticOnChangedInstance()
  {
    void PropertyChanged_2(global::System.Windows.DependencyObject d_2, global::System.Windows.DependencyPropertyChangedEventArgs e_2)
    {
      global::Metalama.Patterns.Xaml.AspectTests.Callbacks.StaticOnChangedInstance.StaticOnChangedInstance.OnAcceptsObjectChanged(d_2);
    }
    global::Metalama.Patterns.Xaml.AspectTests.Callbacks.StaticOnChangedInstance.StaticOnChangedInstance.AcceptsObjectProperty = global::System.Windows.DependencyProperty.Register("AcceptsObject", typeof(global::System.Int32), typeof(global::Metalama.Patterns.Xaml.AspectTests.Callbacks.StaticOnChangedInstance.StaticOnChangedInstance), new global::System.Windows.PropertyMetadata((global::System.Windows.PropertyChangedCallback)PropertyChanged_2));
    void PropertyChanged_1(global::System.Windows.DependencyObject d_1, global::System.Windows.DependencyPropertyChangedEventArgs e_1)
    {
      global::Metalama.Patterns.Xaml.AspectTests.Callbacks.StaticOnChangedInstance.StaticOnChangedInstance.OnAcceptsDependencyObjectChanged(d_1);
    }
    global::Metalama.Patterns.Xaml.AspectTests.Callbacks.StaticOnChangedInstance.StaticOnChangedInstance.AcceptsDependencyObjectProperty = global::System.Windows.DependencyProperty.Register("AcceptsDependencyObject", typeof(global::System.Int32), typeof(global::Metalama.Patterns.Xaml.AspectTests.Callbacks.StaticOnChangedInstance.StaticOnChangedInstance), new global::System.Windows.PropertyMetadata((global::System.Windows.PropertyChangedCallback)PropertyChanged_1));
    void PropertyChanged(global::System.Windows.DependencyObject d, global::System.Windows.DependencyPropertyChangedEventArgs e)
    {
      global::Metalama.Patterns.Xaml.AspectTests.Callbacks.StaticOnChangedInstance.StaticOnChangedInstance.OnFooChanged((global::Metalama.Patterns.Xaml.AspectTests.Callbacks.StaticOnChangedInstance.StaticOnChangedInstance)d);
    }
    global::Metalama.Patterns.Xaml.AspectTests.Callbacks.StaticOnChangedInstance.StaticOnChangedInstance.FooProperty = global::System.Windows.DependencyProperty.Register("Foo", typeof(global::System.Int32), typeof(global::Metalama.Patterns.Xaml.AspectTests.Callbacks.StaticOnChangedInstance.StaticOnChangedInstance), new global::System.Windows.PropertyMetadata((global::System.Windows.PropertyChangedCallback)PropertyChanged));
  }
}