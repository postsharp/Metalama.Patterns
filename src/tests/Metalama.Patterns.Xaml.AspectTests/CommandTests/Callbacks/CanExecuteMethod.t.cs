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
    this.InstanceNoParametersCommand = new DelegateCommand(_ => this.ExecuteInstanceNoParameters(), _ => this.CanExecuteInstanceNoParameters());
    this.StaticNoParametersCommand = new DelegateCommand(_ => CanExecuteMethod.ExecuteStaticNoParameters(), _ => CanExecuteMethod.CanExecuteStaticNoParameters());
    this.InstanceWithParameterCommand = new DelegateCommand(parameter_1 => this.ExecuteInstanceWithParameter((int)parameter_1), parameter => this.CanExecuteInstanceWithParameter((int)parameter));
    this.StaticWithParameterCommand = new DelegateCommand(parameter_3 => CanExecuteMethod.ExecuteStaticWithParameter((int)parameter_3), parameter_2 => CanExecuteMethod.CanExecuteStaticWithParameter((int)parameter_2));
  }
  public ICommand InstanceNoParametersCommand { get; }
  public ICommand InstanceWithParameterCommand { get; }
  public ICommand StaticNoParametersCommand { get; }
  public ICommand StaticWithParameterCommand { get; }
}