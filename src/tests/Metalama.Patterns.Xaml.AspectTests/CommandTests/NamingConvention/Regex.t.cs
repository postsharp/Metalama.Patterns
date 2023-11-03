internal class Regex
{
  [Command]
  private void MakeItBeep()
  {
  }
  // Matches regex naming convention:
  private bool CanItBeep() => true;
  // Not matched by regex naming convention, so should not be ambiguous:
  private bool CanExecuteBeep() => true;
  [Command]
  private void MakeItUseTheForce()
  {
  }
  // Also matches regex naming convention:
  private bool UseTheForceItCan() => true;
  // Does not match regex naming convention, fallthrough to default naming convention:
  [Command]
  private void ExecuteFoo()
  {
  }
  private bool CanExecuteFoo() => true;
  // Not matched by default naming convention, so should not be ambiguous:
  private bool ItCanFoo() => true;
  public Regex()
  {
    bool CanExecute_2(object? parameter_4)
    {
      return this.CanExecuteFoo();
    }
    void Execute_2(object? parameter_5)
    {
      this.ExecuteFoo();
    }
    this.FooCommand = new DelegateCommand(Execute_2, CanExecute_2);
    bool CanExecute_1(object? parameter_2)
    {
      return this.UseTheForceItCan();
    }
    void Execute_1(object? parameter_3)
    {
      this.MakeItUseTheForce();
    }
    this.TheUseTheForceCommand = new DelegateCommand(Execute_1, CanExecute_1);
    bool CanExecute(object? parameter)
    {
      return this.CanItBeep();
    }
    void Execute(object? parameter_1)
    {
      this.MakeItBeep();
    }
    this.TheBeepCommand = new DelegateCommand(Execute, CanExecute);
  }
  public ICommand FooCommand { get; }
  public ICommand TheBeepCommand { get; }
  public ICommand TheUseTheForceCommand { get; }
}