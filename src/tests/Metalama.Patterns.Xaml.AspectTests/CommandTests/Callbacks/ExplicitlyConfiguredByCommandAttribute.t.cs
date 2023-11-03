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
    bool CanExecute_2(object? parameter_4)
    {
      return this.CanExec2;
    }
    void Execute_2(object? parameter_5)
    {
      this.ExecuteConfiguredCanExecuteProperty();
    }
    this.ConfiguredCanExecutePropertyCommand = new DelegateCommand(Execute_2, CanExecute_2);
    bool CanExecute_1(object? parameter_2)
    {
      return this.CanExec1();
    }
    void Execute_1(object? parameter_3)
    {
      this.ExecuteConfiguredCanExecuteMethod();
    }
    this.ConfiguredCanExecuteMethodCommand = new DelegateCommand(Execute_1, CanExecute_1);
    bool CanExecute(object? parameter)
    {
      return this.SomeWeirdName1();
    }
    void Execute(object? parameter_1)
    {
      this.Exec1();
    }
    this.Exec1Command = new DelegateCommand(Execute, CanExecute);
  }
  public ICommand ConfiguredCanExecuteMethodCommand { get; }
  public ICommand ConfiguredCanExecutePropertyCommand { get; }
  public ICommand Exec1Command { get; }
}