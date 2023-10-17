using System.Windows.Input;
using Metalama.Patterns.Xaml.Implementation;
namespace Metalama.Patterns.Xaml.AspectTests.CommandTests.Callbacks;
public class AlternativeNames
{
  [Command]
  public ICommand AlternativeExecuteMethodCommand { get; }
  private void AlternativeExecuteMethod()
  {
  }
  [Command]
  public ICommand AlternativeCanExecuteMethodCommand { get; }
  private void ExecuteAlternativeCanExecuteMethod()
  {
  }
  private bool CanAlternativeCanExecuteMethod() => true;
  [Command]
  public ICommand AlternativeCanExecutePropertyCommand { get; }
  private void ExecuteAlternativeCanExecuteProperty()
  {
  }
  private bool CanAlternativeCanExecuteProperty => true;
  public AlternativeNames()
  {
    bool CanExecute_1(object parameter_3)
    {
      return this.CanAlternativeCanExecuteProperty;
    }
    void Execute_2(object parameter_4)
    {
      this.ExecuteAlternativeCanExecuteProperty();
    }
    this.AlternativeCanExecutePropertyCommand = new DelegateCommand(Execute_2, CanExecute_1);
    bool CanExecute(object parameter_1)
    {
      return this.CanAlternativeCanExecuteMethod();
    }
    void Execute_1(object parameter_2)
    {
      this.ExecuteAlternativeCanExecuteMethod();
    }
    this.AlternativeCanExecuteMethodCommand = new DelegateCommand(Execute_1, CanExecute);
    void Execute(object parameter)
    {
      this.AlternativeExecuteMethod();
    }
    this.AlternativeExecuteMethodCommand = new DelegateCommand(Execute, null);
  }
}