using System.ComponentModel;
namespace Metalama.Patterns.Observability.AspectTests.Options.IgnoreUnsupportedDependencies.UsingAttributeOnMethodOfTargetClass;
[Observable]
public class UsingAttributeOnMethodOfTargetClass : INotifyPropertyChanged
{
  public int X => this.Foo();
  [IgnoreUnsupportedDependencies]
  private int Foo() => 42;
  protected virtual void OnPropertyChanged(string propertyName)
  {
    this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
  }
  public event PropertyChangedEventHandler? PropertyChanged;
}