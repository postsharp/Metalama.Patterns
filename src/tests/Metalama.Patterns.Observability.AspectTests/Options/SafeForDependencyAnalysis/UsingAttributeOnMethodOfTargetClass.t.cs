using System.ComponentModel;
using Metalama.Patterns.Observability.Metadata;
using Metalama.Patterns.Observability.Options;
namespace Metalama.Patterns.Observability.AspectTests.Options.SafeForDependencyAnalysis.UsingAttributeOnMethodOfTargetClass;
[Observable]
public class UsingAttributeOnMethodOfTargetClass : INotifyPropertyChanged
{
  public int X => this.Foo();
  [SafeForDependencyAnalysis]
  private int Foo() => 42;
  [OnChildPropertyChangedMethod]
  protected virtual void OnChildPropertyChanged(string parentPropertyPath, string propertyName)
  {
  }
  protected virtual void OnPropertyChanged(string propertyName)
  {
    this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
  }
  [OnUnmonitoredObservablePropertyChangedMethod]
  protected virtual void OnUnmonitoredObservablePropertyChanged(string propertyPath, INotifyPropertyChanged? oldValue, INotifyPropertyChanged? newValue)
  {
  }
  public event PropertyChangedEventHandler? PropertyChanged;
}