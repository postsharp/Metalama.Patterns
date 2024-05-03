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
    InstanceNoParametersCommand = new DelegateCommand(_ => ExecuteInstanceNoParameters(), _ => CanExecuteInstanceNoParameters());
    StaticNoParametersCommand = new DelegateCommand(_ => ExecuteStaticNoParameters(), _ => CanExecuteStaticNoParameters());
    InstanceWithParameterCommand = new DelegateCommand(parameter_1 => ExecuteInstanceWithParameter((int)parameter_1), parameter => CanExecuteInstanceWithParameter((int)parameter));
    StaticWithParameterCommand = new DelegateCommand(parameter_3 => ExecuteStaticWithParameter((int)parameter_3), parameter_2 => CanExecuteStaticWithParameter((int)parameter_2));
  }
  public ICommand InstanceNoParametersCommand { get; }
  public ICommand InstanceWithParameterCommand { get; }
  public ICommand StaticNoParametersCommand { get; }
  public ICommand StaticWithParameterCommand { get; }
}