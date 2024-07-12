using System.Windows.Input;
using Metalama.Patterns.Wpf.Implementation;
namespace Metalama.Patterns.Wpf.AspectTests.CommandTests.Callbacks;
public class ExplicitlyConfiguredByCommandAttribute
{
  [Command(CanExecuteMethod = nameof(SomeWeirdName1))]
  private void Exec1()
  {
  }
  private bool SomeWeirdName1() => true;
  [Command(CanExecuteMethod = nameof(CanExec1))]
  private void ExecuteConfiguredCanExecuteMethod()
  {
  }
  // Has the default can-execute name for Exec1() above, don't be fooled.
  private bool CanExec1() => true;
  [Command(CanExecuteProperty = nameof(CanExec2))]
  private void ExecuteConfiguredCanExecuteProperty()
  {
  }
  private bool CanExec2 => true;
  public ExplicitlyConfiguredByCommandAttribute()
  {
    Exec1Command = new DelegateCommand(_ => Exec1(), _ => SomeWeirdName1());
    ConfiguredCanExecuteMethodCommand = new DelegateCommand(_ => ExecuteConfiguredCanExecuteMethod(), _ => CanExec1());
    ConfiguredCanExecutePropertyCommand = new DelegateCommand(_ => ExecuteConfiguredCanExecuteProperty(), _ => CanExec2);
  }
  public ICommand ConfiguredCanExecuteMethodCommand { get; }
  public ICommand ConfiguredCanExecutePropertyCommand { get; }
  public ICommand Exec1Command { get; }
}