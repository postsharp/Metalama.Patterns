using System.Windows.Input;
using Metalama.Patterns.Xaml.Implementation;
namespace Metalama.Patterns.Xaml.AspectTests.CommandTests.Callbacks;
public class ExecuteMethod
{
  [Command]
  private void ExecuteInstanceNoParameters()
  {
  }
  [Command]
  private static void ExecuteStaticNoParameters()
  {
  }
  [Command]
  private void ExecuteInstanceWithParameter(int v)
  {
  }
  [Command]
  private static void ExecuteStaticWithParameter(int v)
  {
  }
  public ExecuteMethod()
  {
    InstanceNoParametersCommand = new DelegateCommand(_ => ExecuteInstanceNoParameters(), null);
    StaticNoParametersCommand = new DelegateCommand(_ => ExecuteStaticNoParameters(), null);
    InstanceWithParameterCommand = new DelegateCommand(parameter => ExecuteInstanceWithParameter((int)parameter), null);
    StaticWithParameterCommand = new DelegateCommand(parameter_1 => ExecuteStaticWithParameter((int)parameter_1), null);
  }
  public ICommand InstanceNoParametersCommand { get; }
  public ICommand InstanceWithParameterCommand { get; }
  public ICommand StaticNoParametersCommand { get; }
  public ICommand StaticWithParameterCommand { get; }
}