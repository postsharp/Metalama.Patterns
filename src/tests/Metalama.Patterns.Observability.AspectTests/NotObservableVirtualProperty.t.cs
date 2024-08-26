using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Metalama.Patterns.Observability.AspectTests.NotObservableVirtualProperty
{
  [Observable]
  public class TestClass : INotifyPropertyChanged
  {
    [NotObservable]
    public virtual int Foo { get; set; }
    protected virtual void OnPropertyChanged(string propertyName)
    {
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
    public event PropertyChangedEventHandler? PropertyChanged;
  }
}