namespace Metalama.Patterns.NotifyPropertyChanged.AspectTests.FieldBackedIntProperty;
[NotifyPropertyChanged]
public class FieldBackedIntProperty : global::System.ComponentModel.INotifyPropertyChanged
{
  private global::System.Int32 _x1;
  private global::System.Int32 _x
  {
    get
    {
      return this._x1;
    }
    set
    {
      if ((this._x1 != value))
      {
        this._x1 = value;
        this.OnPropertyChanged("X");
      }
    }
  }
  public int X => this._x;
  public int Y => this.X;
  [global::Metalama.Patterns.NotifyPropertyChanged.Metadata.OnChildPropertyChangedMethodAttribute(new global::System.String[] { })]
  protected virtual void OnChildPropertyChanged(global::System.String parentPropertyPath, global::System.String propertyName)
  {
  }
  protected virtual void OnPropertyChanged(global::System.String propertyName)
  {
    if (propertyName == "X")
    {
      this.OnPropertyChanged("Y");
    }
    this.PropertyChanged?.Invoke(this, new global::System.ComponentModel.PropertyChangedEventArgs(propertyName));
  }
  [global::Metalama.Patterns.NotifyPropertyChanged.Metadata.OnUnmonitoredObservablePropertyChangedMethodAttribute(new global::System.String[] { })]
  protected virtual void OnUnmonitoredObservablePropertyChanged(global::System.String propertyPath, global::System.ComponentModel.INotifyPropertyChanged? oldValue, global::System.ComponentModel.INotifyPropertyChanged? newValue)
  {
  }
  public event global::System.ComponentModel.PropertyChangedEventHandler? PropertyChanged;
}