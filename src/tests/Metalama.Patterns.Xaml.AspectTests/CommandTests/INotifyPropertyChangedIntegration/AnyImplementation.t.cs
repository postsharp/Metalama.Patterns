using System.ComponentModel;
using System.Windows.Input;
using Metalama.Patterns.Xaml.Implementation;
namespace Metalama.Patterns.Xaml.AspectTests.CommandTests.INotifyPropertyChangedIntegration.AnyImplementation;
public class AnyImplementation : INotifyPropertyChanged
{
  public event PropertyChangedEventHandler? PropertyChanged;
  [Command]
  private void ExecuteFoo1()
  {
  }
  public bool CanExecuteFoo1 => true;
  public AnyImplementation()
  {
    this.Foo1Command = new DelegateCommand(_ => this.ExecuteFoo1(), _ => CanExecuteFoo1, this, "CanExecuteFoo1");
  }
  public ICommand Foo1Command { get; }
}
public class ImplementedByBase : AnyImplementation
{
  [Command]
  private void ExecuteFoo2()
  {
  }
  public bool CanExecuteFoo2 => true;
  public ImplementedByBase()
  {
    this.Foo2Command = new DelegateCommand(_ => this.ExecuteFoo2(), _ => CanExecuteFoo2, this, "CanExecuteFoo2");
  }
  public ICommand Foo2Command { get; }
}