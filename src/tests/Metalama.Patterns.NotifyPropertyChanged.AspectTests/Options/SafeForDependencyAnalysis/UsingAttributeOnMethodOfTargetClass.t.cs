using Metalama.Patterns.NotifyPropertyChanged.Options;
namespace Metalama.Patterns.NotifyPropertyChanged.AspectTests.Options.SafeForDependencyAnalysis.UsingAttributeOnMethodOfTargetClass;
[NotifyPropertyChanged]
public class UsingAttributeOnMethodOfTargetClass : global::System.ComponentModel.INotifyPropertyChanged
{
  public int X => this.Foo();
  [SafeForDependencyAnalysis]
  private int Foo() => 42;
  [global::Metalama.Patterns.NotifyPropertyChanged.Metadata.OnChildPropertyChangedMethodAttribute(new global::System.String[] { })]
  protected virtual void OnChildPropertyChanged(global::System.String parentPropertyPath, global::System.String propertyName)
  {
  }
  protected virtual void OnPropertyChanged(global::System.String propertyName)
  {
    this.PropertyChanged?.Invoke(this, new global::System.ComponentModel.PropertyChangedEventArgs(propertyName));
  }
  [global::Metalama.Patterns.NotifyPropertyChanged.Metadata.OnUnmonitoredObservablePropertyChangedMethodAttribute(new global::System.String[] { })]
  protected virtual void OnUnmonitoredObservablePropertyChanged(global::System.String propertyPath, global::System.ComponentModel.INotifyPropertyChanged? oldValue, global::System.ComponentModel.INotifyPropertyChanged? newValue)
  {
  }
  public event global::System.ComponentModel.PropertyChangedEventHandler? PropertyChanged;
}