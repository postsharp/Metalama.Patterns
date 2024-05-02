using System.Windows.Input;
using Metalama.Patterns.Xaml.Implementation;
namespace Metalama.Patterns.Xaml.AspectTests.CommandTests.Callbacks;
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
    this.Exec1Command = new DelegateCommand(_ => this.Exec1(), _ => this.SomeWeirdName1());
    this.ConfiguredCanExecuteMethodCommand = new DelegateCommand(_ => this.ExecuteConfiguredCanExecuteMethod(), _ => this.CanExec1());
    this.ConfiguredCanExecutePropertyCommand = new DelegateCommand(_ => this.ExecuteConfiguredCanExecuteProperty(), _ => CanExec2);
  }
  public ICommand ConfiguredCanExecuteMethodCommand { get; }
  public ICommand ConfiguredCanExecutePropertyCommand { get; }
  public ICommand Exec1Command { get; }
}