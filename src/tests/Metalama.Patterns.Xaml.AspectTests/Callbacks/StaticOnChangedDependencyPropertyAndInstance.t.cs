using System.Windows;
namespace Metalama.Patterns.Xaml.AspectTests.Callbacks.StaticOnChangedDependencyPropertyAndInstance;
public partial class StaticOnChangedDependencyPropertyAndInstance : DependencyObject
{
  [DependencyProperty]
  public int Foo
  {
    get
    {
      return ((global::System.Int32)this.GetValue(global::Metalama.Patterns.Xaml.AspectTests.Callbacks.StaticOnChangedDependencyPropertyAndInstance.StaticOnChangedDependencyPropertyAndInstance.FooProperty));
    }
    set
    {
      this.SetValue(global::Metalama.Patterns.Xaml.AspectTests.Callbacks.StaticOnChangedDependencyPropertyAndInstance.StaticOnChangedDependencyPropertyAndInstance.FooProperty, value);
    }
  }
  private static void OnFooChanged(DependencyProperty d, StaticOnChangedDependencyPropertyAndInstance instance)
  {
  }
  [DependencyProperty]
  public int AcceptsDependencyObjectForInstance
  {
    get
    {
      return ((global::System.Int32)this.GetValue(global::Metalama.Patterns.Xaml.AspectTests.Callbacks.StaticOnChangedDependencyPropertyAndInstance.StaticOnChangedDependencyPropertyAndInstance.AcceptsDependencyObjectForInstanceProperty));
    }
    set
    {
      this.SetValue(global::Metalama.Patterns.Xaml.AspectTests.Callbacks.StaticOnChangedDependencyPropertyAndInstance.StaticOnChangedDependencyPropertyAndInstance.AcceptsDependencyObjectForInstanceProperty, value);
    }
  }
  private static void OnAcceptsDependencyObjectForInstanceChanged(DependencyProperty d, DependencyObject instance)
  {
  }
  [DependencyProperty]
  public int AcceptsObjectForInstance
  {
    get
    {
      return ((global::System.Int32)this.GetValue(global::Metalama.Patterns.Xaml.AspectTests.Callbacks.StaticOnChangedDependencyPropertyAndInstance.StaticOnChangedDependencyPropertyAndInstance.AcceptsObjectForInstanceProperty));
    }
    set
    {
      this.SetValue(global::Metalama.Patterns.Xaml.AspectTests.Callbacks.StaticOnChangedDependencyPropertyAndInstance.StaticOnChangedDependencyPropertyAndInstance.AcceptsObjectForInstanceProperty, value);
    }
  }
  private static void OnAcceptsObjectForInstanceChanged(DependencyProperty d, object instance)
  {
  }
  public static readonly global::System.Windows.DependencyProperty AcceptsDependencyObjectForInstanceProperty;
  public static readonly global::System.Windows.DependencyProperty AcceptsObjectForInstanceProperty;
  public static readonly global::System.Windows.DependencyProperty FooProperty;
  static StaticOnChangedDependencyPropertyAndInstance()
  {
    void PropertyChanged_2(global::System.Windows.DependencyObject d_3, global::System.Windows.DependencyPropertyChangedEventArgs e_2)
    {
      global::Metalama.Patterns.Xaml.AspectTests.Callbacks.StaticOnChangedDependencyPropertyAndInstance.StaticOnChangedDependencyPropertyAndInstance.OnAcceptsObjectForInstanceChanged(global::Metalama.Patterns.Xaml.AspectTests.Callbacks.StaticOnChangedDependencyPropertyAndInstance.StaticOnChangedDependencyPropertyAndInstance.AcceptsObjectForInstanceProperty, d_3);
    }
    global::Metalama.Patterns.Xaml.AspectTests.Callbacks.StaticOnChangedDependencyPropertyAndInstance.StaticOnChangedDependencyPropertyAndInstance.AcceptsObjectForInstanceProperty = global::System.Windows.DependencyProperty.Register("AcceptsObjectForInstance", typeof(global::System.Int32), typeof(global::Metalama.Patterns.Xaml.AspectTests.Callbacks.StaticOnChangedDependencyPropertyAndInstance.StaticOnChangedDependencyPropertyAndInstance), new global::System.Windows.PropertyMetadata((global::System.Windows.PropertyChangedCallback)PropertyChanged_2));
    void PropertyChanged_1(global::System.Windows.DependencyObject d_2, global::System.Windows.DependencyPropertyChangedEventArgs e_1)
    {
      global::Metalama.Patterns.Xaml.AspectTests.Callbacks.StaticOnChangedDependencyPropertyAndInstance.StaticOnChangedDependencyPropertyAndInstance.OnAcceptsDependencyObjectForInstanceChanged(global::Metalama.Patterns.Xaml.AspectTests.Callbacks.StaticOnChangedDependencyPropertyAndInstance.StaticOnChangedDependencyPropertyAndInstance.AcceptsDependencyObjectForInstanceProperty, d_2);
    }
    global::Metalama.Patterns.Xaml.AspectTests.Callbacks.StaticOnChangedDependencyPropertyAndInstance.StaticOnChangedDependencyPropertyAndInstance.AcceptsDependencyObjectForInstanceProperty = global::System.Windows.DependencyProperty.Register("AcceptsDependencyObjectForInstance", typeof(global::System.Int32), typeof(global::Metalama.Patterns.Xaml.AspectTests.Callbacks.StaticOnChangedDependencyPropertyAndInstance.StaticOnChangedDependencyPropertyAndInstance), new global::System.Windows.PropertyMetadata((global::System.Windows.PropertyChangedCallback)PropertyChanged_1));
    void PropertyChanged(global::System.Windows.DependencyObject d_1, global::System.Windows.DependencyPropertyChangedEventArgs e)
    {
      global::Metalama.Patterns.Xaml.AspectTests.Callbacks.StaticOnChangedDependencyPropertyAndInstance.StaticOnChangedDependencyPropertyAndInstance.OnFooChanged(global::Metalama.Patterns.Xaml.AspectTests.Callbacks.StaticOnChangedDependencyPropertyAndInstance.StaticOnChangedDependencyPropertyAndInstance.FooProperty, (global::Metalama.Patterns.Xaml.AspectTests.Callbacks.StaticOnChangedDependencyPropertyAndInstance.StaticOnChangedDependencyPropertyAndInstance)d_1);
    }
    global::Metalama.Patterns.Xaml.AspectTests.Callbacks.StaticOnChangedDependencyPropertyAndInstance.StaticOnChangedDependencyPropertyAndInstance.FooProperty = global::System.Windows.DependencyProperty.Register("Foo", typeof(global::System.Int32), typeof(global::Metalama.Patterns.Xaml.AspectTests.Callbacks.StaticOnChangedDependencyPropertyAndInstance.StaticOnChangedDependencyPropertyAndInstance), new global::System.Windows.PropertyMetadata((global::System.Windows.PropertyChangedCallback)PropertyChanged));
  }
}