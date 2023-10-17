using System.Windows.Input;
using Metalama.Patterns.Xaml.Implementation;
namespace Metalama.Patterns.Xaml.AspectTests.CommandTests.Callbacks;
public class ExecuteMethod
{
  [Command]
  public ICommand InstanceNoParametersCommand { get; }
  private void ExecuteInstanceNoParameters()
  {
  }
  [Command]
  public ICommand StaticNoParametersCommand { get; }
  private static void ExecuteStaticNoParameters()
  {
  }
  [Command]
  public ICommand InstanceWithParameterCommand { get; }
  private void ExecuteInstanceWithParameter(int v)
  {
  }
  [Command]
  public ICommand StaticWithParameterCommand { get; }
  private static void ExecuteStaticWithParameter(int v)
  {
  }
  public ExecuteMethod()
  {
    void Execute_3(object parameter_3)
    {
      ExecuteMethod.ExecuteStaticWithParameter((int)parameter_3);
    }
    this.StaticWithParameterCommand = new DelegateCommand(Execute_3, null);
    void Execute_2(object parameter_2)
    {
      this.ExecuteInstanceWithParameter((int)parameter_2);
    }
    this.InstanceWithParameterCommand = new DelegateCommand(Execute_2, null);
    void Execute_1(object parameter_1)
    {
      ExecuteMethod.ExecuteStaticNoParameters();
    }
    this.StaticNoParametersCommand = new DelegateCommand(Execute_1, null);
    void Execute(object parameter)
    {
      this.ExecuteInstanceNoParameters();
    }
    this.InstanceNoParametersCommand = new DelegateCommand(Execute, null);
  }
}