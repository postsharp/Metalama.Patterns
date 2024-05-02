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
    this.TheBeepCommand = new DelegateCommand(new Action<object>(_ =>
    {
      this.MakeItBeep();
    }), new Func<object, bool>(_ => this.CanItBeep()));
    this.TheUseTheForceCommand = new DelegateCommand(new Action<object>(_ =>
    {
      this.MakeItUseTheForce();
    }), new Func<object, bool>(_ => this.UseTheForceItCan()));
    this.FooCommand = new DelegateCommand(new Action<object>(_ =>
    {
      this.ExecuteFoo();
    }), new Func<object, bool>(_ => this.CanExecuteFoo()));
  }
  public ICommand FooCommand { get; }
  public ICommand TheBeepCommand { get; }
  public ICommand TheUseTheForceCommand { get; }
}