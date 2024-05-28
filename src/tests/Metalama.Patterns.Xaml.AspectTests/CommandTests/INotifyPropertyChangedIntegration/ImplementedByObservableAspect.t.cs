using System.ComponentModel;
using System.Windows.Input;
// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.
// TODO: #34044 Can't use RequireOrderedAspects yet. If not fixed, re-enable when DependencyPropertyAttribute aspect is also ordered.
// __RequireOrderedAspects__
using Metalama.Patterns.Observability;
using Metalama.Patterns.Xaml.Implementation;
namespace Metalama.Patterns.Xaml.AspectTests.CommandTests.INotifyPropertyChangedIntegration.ImplementedByObservableAspect;
[Observable]
public class ImplementedByObservableAspect : INotifyPropertyChanged
{
  [Command]
  private void ExecuteFoo1()
  {
  }
  private bool _canExecuteFoo1;
  public bool CanExecuteFoo1
  {
    get
    {
      return _canExecuteFoo1;
    }
    set
    {
      if (_canExecuteFoo1 != value)
      {
        _canExecuteFoo1 = value;
        OnPropertyChanged("CanExecuteFoo1");
      }
    }
  }
  public ImplementedByObservableAspect()
  {
    Foo1Command = new DelegateCommand(_ => ExecuteFoo1(), _ => CanExecuteFoo1, this, "CanExecuteFoo1");
  }
  public ICommand Foo1Command { get; }
  protected virtual void OnPropertyChanged(string propertyName)
  {
    this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
  }
  public event PropertyChangedEventHandler? PropertyChanged;
}
public class ImplementedByBase : ImplementedByObservableAspect
{
  [Command]
  private void ExecuteFoo2()
  {
  }
  private bool _canExecuteFoo2;
  public bool CanExecuteFoo2
  {
    get
    {
      return _canExecuteFoo2;
    }
    set
    {
      if (_canExecuteFoo2 != value)
      {
        _canExecuteFoo2 = value;
        OnPropertyChanged("CanExecuteFoo2");
      }
    }
  }
  public ImplementedByBase()
  {
    Foo2Command = new DelegateCommand(_ => ExecuteFoo2(), _ => CanExecuteFoo2, this, "CanExecuteFoo2");
  }
  public ICommand Foo2Command { get; }
  protected override void OnPropertyChanged(string propertyName)
  {
    base.OnPropertyChanged(propertyName);
  }
}