// Warning LAMA5206 on `ExecuteInstanceNoParameters`: `No can-execute method or can-execute property was found using the default naming convention, with candidate member name 'CanExecuteInstanceNoParameters'.`
// Warning LAMA5206 on `ExecuteStaticNoParameters`: `No can-execute method or can-execute property was found using the default naming convention, with candidate member name 'CanExecuteStaticNoParameters'.`
// Warning LAMA5206 on `ExecuteInstanceWithParameter`: `No can-execute method or can-execute property was found using the default naming convention, with candidate member name 'CanExecuteInstanceWithParameter'.`
// Warning LAMA5206 on `ExecuteStaticWithParameter`: `No can-execute method or can-execute property was found using the default naming convention, with candidate member name 'CanExecuteStaticWithParameter'.`
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
    this.InstanceNoParametersCommand = new DelegateCommand(_ => this.ExecuteInstanceNoParameters(), null);
    this.StaticNoParametersCommand = new DelegateCommand(_ => ExecuteMethod.ExecuteStaticNoParameters(), null);
    this.InstanceWithParameterCommand = new DelegateCommand(parameter => this.ExecuteInstanceWithParameter((int)parameter), null);
    this.StaticWithParameterCommand = new DelegateCommand(parameter_1 => ExecuteMethod.ExecuteStaticWithParameter((int)parameter_1), null);
  }
  public ICommand InstanceNoParametersCommand { get; }
  public ICommand InstanceWithParameterCommand { get; }
  public ICommand StaticNoParametersCommand { get; }
  public ICommand StaticWithParameterCommand { get; }
}