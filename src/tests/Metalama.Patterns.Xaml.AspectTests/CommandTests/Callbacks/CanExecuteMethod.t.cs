using System.Windows.Input;
using Metalama.Patterns.Xaml.Implementation;
namespace Metalama.Patterns.Xaml.AspectTests.CommandTests.Callbacks;
public class CanExecuteMethod
{
  [Command]
  private void ExecuteInstanceNoParameters()
  {
  }
  private bool CanExecuteInstanceNoParameters() => true;
  [Command]
  private static void ExecuteStaticNoParameters()
  {
  }
  private static bool CanExecuteStaticNoParameters() => true;
  [Command]
  private void ExecuteInstanceWithParameter(int v)
  {
  }
  private bool CanExecuteInstanceWithParameter(int v) => true;
  [Command]
  private static void ExecuteStaticWithParameter(int v)
  {
  }
  private static bool CanExecuteStaticWithParameter(int v) => true;
  public CanExecuteMethod()
  {
    bool CanExecute(object? parameter)
    {
      return this.CanExecuteInstanceNoParameters();
    }
    void Execute(object? parameter_1)
    {
      this.ExecuteInstanceNoParameters();
    }
    this.InstanceNoParametersCommand = new DelegateCommand(Execute, CanExecute);
    bool CanExecute_1(object? parameter_2)
    {
      return CanExecuteMethod.CanExecuteStaticNoParameters();
    }
    void Execute_1(object? parameter_3)
    {
      CanExecuteMethod.ExecuteStaticNoParameters();
    }
    this.StaticNoParametersCommand = new DelegateCommand(Execute_1, CanExecute_1);
    bool CanExecute_2(object? parameter_4)
    {
      return this.CanExecuteInstanceWithParameter((int)parameter_4);
    }
    void Execute_2(object? parameter_5)
    {
      this.ExecuteInstanceWithParameter((int)parameter_5);
    }
    this.InstanceWithParameterCommand = new DelegateCommand(Execute_2, CanExecute_2);
    bool CanExecute_3(object? parameter_6)
    {
      return CanExecuteMethod.CanExecuteStaticWithParameter((int)parameter_6);
    }
    void Execute_3(object? parameter_7)
    {
      CanExecuteMethod.ExecuteStaticWithParameter((int)parameter_7);
    }
    this.StaticWithParameterCommand = new DelegateCommand(Execute_3, CanExecute_3);
  }
  public ICommand InstanceNoParametersCommand { get; }
  public ICommand InstanceWithParameterCommand { get; }
  public ICommand StaticNoParametersCommand { get; }
  public ICommand StaticWithParameterCommand { get; }
}