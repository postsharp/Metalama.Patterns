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
    this.InstanceCommand = new DelegateCommand(new Action<object>(__1 =>
    {
      this.ExecuteInstance();
    }), new Func<object, bool>(_ => CanExecuteInstance));
    this.StaticCommand = new DelegateCommand(new Action<object>(__3 =>
    {
      CanExecuteProperty.ExecuteStatic();
    }), new Func<object, bool>(__2 => CanExecuteStatic));
  }
  public ICommand InstanceCommand { get; }
  public ICommand StaticCommand { get; }
}