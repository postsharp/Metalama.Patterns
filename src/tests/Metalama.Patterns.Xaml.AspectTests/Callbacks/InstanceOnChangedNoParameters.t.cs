using System.Windows;
namespace Metalama.Patterns.Xaml.AspectTests.Callbacks.InstanceOnChangedNoParameters;
public partial class InstanceOnChangedNoParameters : DependencyObject
{
  [DependencyProperty]
  public int Foo
  {
    get
    {
      return ((global::System.Int32)this.GetValue(global::Metalama.Patterns.Xaml.AspectTests.Callbacks.InstanceOnChangedNoParameters.InstanceOnChangedNoParameters.FooProperty));
    }
    set
    {
      this.SetValue(global::Metalama.Patterns.Xaml.AspectTests.Callbacks.InstanceOnChangedNoParameters.InstanceOnChangedNoParameters.FooProperty, value);
    }
  }
  private void OnFooChanged()
  {
  }
  public static readonly global::System.Windows.DependencyProperty FooProperty;
  static InstanceOnChangedNoParameters()
  {
    void PropertyChanged(global::System.Windows.DependencyObject d, global::System.Windows.DependencyPropertyChangedEventArgs e)
    {
      ((global::Metalama.Patterns.Xaml.AspectTests.Callbacks.InstanceOnChangedNoParameters.InstanceOnChangedNoParameters)d).OnFooChanged();
    }
    global::Metalama.Patterns.Xaml.AspectTests.Callbacks.InstanceOnChangedNoParameters.InstanceOnChangedNoParameters.FooProperty = global::System.Windows.DependencyProperty.Register("Foo", typeof(global::System.Int32), typeof(global::Metalama.Patterns.Xaml.AspectTests.Callbacks.InstanceOnChangedNoParameters.InstanceOnChangedNoParameters), new global::System.Windows.PropertyMetadata((global::System.Windows.PropertyChangedCallback)PropertyChanged));
  }
}