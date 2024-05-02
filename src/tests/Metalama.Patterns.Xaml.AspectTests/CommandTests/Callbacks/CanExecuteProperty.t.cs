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
    this.InstanceCommand = new DelegateCommand(_ => this.ExecuteInstance(), _ => CanExecuteInstance);
    this.StaticCommand = new DelegateCommand(_ => CanExecuteProperty.ExecuteStatic(), _ => CanExecuteStatic);
  }
  public ICommand InstanceCommand { get; }
  public ICommand StaticCommand { get; }
}