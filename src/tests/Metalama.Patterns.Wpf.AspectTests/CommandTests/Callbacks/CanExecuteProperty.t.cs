using System.Windows.Input;
using Metalama.Patterns.Wpf.Implementation;
namespace Metalama.Patterns.Wpf.AspectTests.CommandTests.Callbacks;
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
    InstanceCommand = new DelegateCommand(_ => ExecuteInstance(), _ => CanExecuteInstance);
    StaticCommand = new DelegateCommand(_ => ExecuteStatic(), _ => CanExecuteStatic);
  }
  public ICommand InstanceCommand { get; }
  public ICommand StaticCommand { get; }
}