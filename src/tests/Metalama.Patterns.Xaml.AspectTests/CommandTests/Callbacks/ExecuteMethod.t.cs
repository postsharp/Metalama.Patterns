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
    void Execute(object? parameter)
    {
      this.ExecuteInstanceNoParameters();
    }
    this.InstanceNoParametersCommand = new DelegateCommand(Execute, null);
    void Execute_1(object? parameter_1)
    {
      ExecuteMethod.ExecuteStaticNoParameters();
    }
    this.StaticNoParametersCommand = new DelegateCommand(Execute_1, null);
    void Execute_2(object? parameter_2)
    {
      this.ExecuteInstanceWithParameter((int)parameter_2);
    }
    this.InstanceWithParameterCommand = new DelegateCommand(Execute_2, null);
    void Execute_3(object? parameter_3)
    {
      ExecuteMethod.ExecuteStaticWithParameter((int)parameter_3);
    }
    this.StaticWithParameterCommand = new DelegateCommand(Execute_3, null);
  }
  public ICommand InstanceNoParametersCommand { get; }
  public ICommand InstanceWithParameterCommand { get; }
  public ICommand StaticNoParametersCommand { get; }
  public ICommand StaticWithParameterCommand { get; }
}