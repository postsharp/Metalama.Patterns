using System.Windows.Input;
using Metalama.Patterns.Xaml.Implementation;
namespace Metalama.Patterns.Xaml.AspectTests.CommandTests.Callbacks;
public class Configured
{
  [Command(ExecuteMethod = nameof(Exec1))]
  public ICommand ConfiguredExecuteMethodCommand { get; }
  private void Exec1()
  {
  }
  private bool CanExecuteConfiguredExecuteMethod() => true;
  [Command(CanExecuteMethod = nameof(CanExec1))]
  public ICommand ConfiguredCanExecuteMethodCommand { get; }
  private void ExecuteConfiguredCanExecuteMethod()
  {
  }
  private bool CanExec1() => true;
  [Command(CanExecuteProperty = nameof(CanExec2))]
  public ICommand ConfiguredCanExecutePropertyCommand { get; }
  private void ExecuteConfiguredCanExecuteProperty()
  {
  }
  private bool CanExec2 => true;
  public Configured()
  {
    bool CanExecute_2(object parameter_4)
    {
      return this.CanExec2;
    }
    void Execute_2(object parameter_5)
    {
      this.ExecuteConfiguredCanExecuteProperty();
    }
    this.ConfiguredCanExecutePropertyCommand = new DelegateCommand(Execute_2, CanExecute_2);
    bool CanExecute_1(object parameter_2)
    {
      return this.CanExec1();
    }
    void Execute_1(object parameter_3)
    {
      this.ExecuteConfiguredCanExecuteMethod();
    }
    this.ConfiguredCanExecuteMethodCommand = new DelegateCommand(Execute_1, CanExecute_1);
    bool CanExecute(object parameter)
    {
      return this.CanExecuteConfiguredExecuteMethod();
    }
    void Execute(object parameter_1)
    {
      this.Exec1();
    }
    this.ConfiguredExecuteMethodCommand = new DelegateCommand(Execute, CanExecute);
  }
}