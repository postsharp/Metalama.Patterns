using System.ComponentModel;
namespace Metalama.Patterns.Observability.AspectTests.Options.IgnoreUnobservableExpressions.UsingAttributeOnMethodOfTargetClass;
[Observable]
public class UsingAttributeOnMethodOfTargetClass : INotifyPropertyChanged
{
  public int X => this.Foo();
  [ShallNotDependOnMutableState]
  private int Foo() => 42;
  protected virtual void OnPropertyChanged(string propertyName)
  {
    this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
  }
  public event PropertyChangedEventHandler? PropertyChanged;
}