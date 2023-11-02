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
    bool CanExecute(object? parameter)
    {
      return this.CanExecuteFoo1;
    }
    void Execute(object? parameter_1)
    {
      this.ExecuteFoo1();
    }
    this.Foo1Command = new DelegateCommand(Execute, CanExecute, this, "CanExecuteFoo1");
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
    bool CanExecute(object? parameter)
    {
      return this.CanExecuteFoo2;
    }
    void Execute(object? parameter_1)
    {
      this.ExecuteFoo2();
    }
    this.Foo2Command = new DelegateCommand(Execute, CanExecute, this, "CanExecuteFoo2");
  }
  public ICommand Foo2Command { get; }
}