using System.ComponentModel;
using Metalama.Patterns.Observability.Options;
namespace Metalama.Patterns.Observability.AspectTests.Options.SafeForDependencyAnalysis.UsingAttributeOnMethodOfTargetClass;
[Observable]
public class UsingAttributeOnMethodOfTargetClass : INotifyPropertyChanged
{
  public int X => this.Foo();
  [SafeForDependencyAnalysis]
  private int Foo() => 42;
  protected virtual void OnPropertyChanged(string propertyName)
  {
    this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
  }
  public event PropertyChangedEventHandler? PropertyChanged;
}