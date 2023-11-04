using System.ComponentModel;
using System.Windows.Input;
using Metalama.Patterns.Observability;
using Metalama.Patterns.Observability.Metadata;
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
      return this._canExecuteFoo1;
    }
    set
    {
      if (this._canExecuteFoo1 != value)
      {
        this._canExecuteFoo1 = value;
        this.OnPropertyChanged("CanExecuteFoo1");
      }
    }
  }
  public ImplementedByObservableAspect()
  {
    bool CanExecute(object? parameter)
    {
      return this.CanExecuteFoo1;
    }
    void Execute(object? parameter_1)
    {
      this.ExecuteFoo1();
    }
    this.Foo1Command = new DelegateCommand(Execute, CanExecute, this, "CanExecuteFoo1");
  }
  public ICommand Foo1Command { get; }
  [OnChildPropertyChangedMethod(new string[] { })]
  protected virtual void OnChildPropertyChanged(string parentPropertyPath, string propertyName)
  {
  }
  protected virtual void OnPropertyChanged(string propertyName)
  {
    this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
  }
  [OnUnmonitoredObservablePropertyChangedMethod(new string[] { })]
  protected virtual void OnUnmonitoredObservablePropertyChanged(string propertyPath, INotifyPropertyChanged? oldValue, INotifyPropertyChanged? newValue)
  {
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
      return this._canExecuteFoo2;
    }
    set
    {
      if (this._canExecuteFoo2 != value)
      {
        this._canExecuteFoo2 = value;
        this.OnPropertyChanged("CanExecuteFoo2");
      }
    }
  }
  public ImplementedByBase()
  {
    bool CanExecute(object? parameter)
    {
      return this.CanExecuteFoo2;
    }
    void Execute(object? parameter_1)
    {
      this.ExecuteFoo2();
    }
    this.Foo2Command = new DelegateCommand(Execute, CanExecute, this, "CanExecuteFoo2");
  }
  public ICommand Foo2Command { get; }
  [OnChildPropertyChangedMethod(new string[] { })]
  protected override void OnChildPropertyChanged(string parentPropertyPath, string propertyName)
  {
    base.OnChildPropertyChanged(parentPropertyPath, propertyName);
  }
  protected override void OnPropertyChanged(string propertyName)
  {
    base.OnPropertyChanged(propertyName);
  }
  [OnUnmonitoredObservablePropertyChangedMethod(new string[] { })]
  protected override void OnUnmonitoredObservablePropertyChanged(string propertyPath, INotifyPropertyChanged? oldValue, INotifyPropertyChanged? newValue)
  {
    base.OnUnmonitoredObservablePropertyChanged(propertyPath, oldValue, newValue);
  }
}