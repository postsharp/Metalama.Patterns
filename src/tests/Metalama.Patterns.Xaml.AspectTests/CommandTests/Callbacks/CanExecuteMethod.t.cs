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
    this.InstanceNoParametersCommand = new DelegateCommand(new Action<object>(__1 =>
    {
      this.ExecuteInstanceNoParameters();
    }), new Func<object, bool>(_ => this.CanExecuteInstanceNoParameters()));
    this.StaticNoParametersCommand = new DelegateCommand(new Action<object>(__3 =>
    {
      CanExecuteMethod.ExecuteStaticNoParameters();
    }), new Func<object, bool>(__2 => CanExecuteMethod.CanExecuteStaticNoParameters()));
    this.InstanceWithParameterCommand = new DelegateCommand(new Action<object>(parameter_1 =>
    {
      this.ExecuteInstanceWithParameter((int)parameter_1);
    }), new Func<object, bool>(parameter => this.CanExecuteInstanceWithParameter((int)parameter)));
    this.StaticWithParameterCommand = new DelegateCommand(new Action<object>(parameter_3 =>
    {
      CanExecuteMethod.ExecuteStaticWithParameter((int)parameter_3);
    }), new Func<object, bool>(parameter_2 => CanExecuteMethod.CanExecuteStaticWithParameter((int)parameter_2)));
  }
  public ICommand InstanceNoParametersCommand { get; }
  public ICommand InstanceWithParameterCommand { get; }
  public ICommand StaticNoParametersCommand { get; }
  public ICommand StaticWithParameterCommand { get; }
}