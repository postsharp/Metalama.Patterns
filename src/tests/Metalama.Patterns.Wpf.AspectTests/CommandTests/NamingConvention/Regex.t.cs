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
    TheBeepCommand = new DelegateCommand(_ => MakeItBeep(), _ => CanItBeep());
    TheUseTheForceCommand = new DelegateCommand(_ => MakeItUseTheForce(), _ => UseTheForceItCan());
    FooCommand = new DelegateCommand(_ => ExecuteFoo(), _ => CanExecuteFoo());
  }
  public ICommand FooCommand { get; }
  public ICommand TheBeepCommand { get; }
  public ICommand TheUseTheForceCommand { get; }
}