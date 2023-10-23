using System.Windows.Input;
using Metalama.Patterns.Xaml.Implementation;
namespace Metalama.Patterns.Xaml.AspectTests.CommandTests.Callbacks;
public class CanExecuteProperty
{
  [Command]
  private void ExecuteInstance()
  {
  }
  private bool CanExecuteInstance => true;
  [Command]
  private static void ExecuteStatic()
  {
  }
  private static bool CanExecuteStatic => true;
  public CanExecuteProperty()
  {
    bool CanExecute_1(object? parameter_2)
    {
      return CanExecuteProperty.CanExecuteStatic;
    }
    void Execute_1(object? parameter_3)
    {
      CanExecuteProperty.ExecuteStatic();
    }
    this.StaticCommand = new DelegateCommand(Execute_1, CanExecute_1);
    bool CanExecute(object? parameter)
    {
      return this.CanExecuteInstance;
    }
    void Execute(object? parameter_1)
    {
      this.ExecuteInstance();
    }
    this.InstanceCommand = new DelegateCommand(Execute, CanExecute);
  }
  public ICommand InstanceCommand { get; }
  public ICommand StaticCommand { get; }
}