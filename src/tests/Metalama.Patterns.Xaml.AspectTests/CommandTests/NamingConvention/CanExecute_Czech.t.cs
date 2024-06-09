using Metalama.Patterns.Xaml;
using Metalama.Patterns.Xaml.Implementation;
using System.Windows;
using System.Windows.Input;
namespace Doc.Command.CanExecute_Czech;
public class MojeOkno : Window
{
  public int Počitadlo { get; private set; }
  [Command]
  public void VykonatZvýšení()
  {
    this.Počitadlo++;
  }
  public bool MůžemeVykonatZvýšení => this.Počitadlo < 10;
  [Command]
  public void Snížit()
  {
    this.Počitadlo--;
  }
  public bool MůžemeSnížit => this.Počitadlo > 0;
  public MojeOkno()
  {
    ZvýšeníPříkaz = new DelegateCommand(_ => VykonatZvýšení(), _ => MůžemeVykonatZvýšení);
    SnížitPříkaz = new DelegateCommand(_ => Snížit(), _ => MůžemeSnížit);
  }
  public ICommand SnížitPříkaz { get; }
  public ICommand ZvýšeníPříkaz { get; }
}