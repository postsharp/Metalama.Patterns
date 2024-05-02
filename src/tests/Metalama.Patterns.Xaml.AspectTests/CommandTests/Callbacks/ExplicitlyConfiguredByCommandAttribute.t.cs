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
    this.Exec1Command = new DelegateCommand(new Action<object>(__1 =>
    {
      this.Exec1();
    }), new Func<object, bool>(_ => this.SomeWeirdName1()));
    this.ConfiguredCanExecuteMethodCommand = new DelegateCommand(new Action<object>(__3 =>
    {
      this.ExecuteConfiguredCanExecuteMethod();
    }), new Func<object, bool>(__2 => this.CanExec1()));
    this.ConfiguredCanExecutePropertyCommand = new DelegateCommand(new Action<object>(__5 =>
    {
      this.ExecuteConfiguredCanExecuteProperty();
    }), new Func<object, bool>(__4 => CanExec2));
  }
  public ICommand ConfiguredCanExecuteMethodCommand { get; }
  public ICommand ConfiguredCanExecutePropertyCommand { get; }
  public ICommand Exec1Command { get; }
}