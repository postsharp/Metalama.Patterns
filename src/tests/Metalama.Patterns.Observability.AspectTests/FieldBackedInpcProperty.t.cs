[Observable]
public class FieldBackedInpcProperty : global::System.ComponentModel.INotifyPropertyChanged
{
  private global::Metalama.Patterns.Observability.AspectTests.FieldBackedInpcProperty.A _x1 = default !;
  private global::Metalama.Patterns.Observability.AspectTests.FieldBackedInpcProperty.A _x
  {
    get
    {
      return this._x1;
    }
    set
    {
      if (!global::System.Object.ReferenceEquals(value, this._x1))
      {
        var oldValue = this._x1;
        if (oldValue != null)
        {
          oldValue.PropertyChanged -= this._on_xPropertyChangedHandler;
        }
        this._x1 = value;
        this.OnPropertyChanged("P1");
        this.OnPropertyChanged("P2");
        this.SubscribeTo_x(value);
      }
    }
  }
  public A P1 => this._x;
  public int P2 => this._x.A1;
  private global::System.ComponentModel.PropertyChangedEventHandler? _on_xPropertyChangedHandler;
  [global::Metalama.Patterns.Observability.Metadata.OnChildPropertyChangedMethodAttribute(new global::System.String[] { })]
  protected virtual void OnChildPropertyChanged(global::System.String parentPropertyPath, global::System.String propertyName)
  {
  }
  protected virtual void OnPropertyChanged(global::System.String propertyName)
  {
    this.PropertyChanged?.Invoke(this, new global::System.ComponentModel.PropertyChangedEventArgs(propertyName));
  }
  [global::Metalama.Patterns.Observability.Metadata.OnUnmonitoredObservablePropertyChangedMethodAttribute(new global::System.String[] { })]
  protected virtual void OnUnmonitoredObservablePropertyChanged(global::System.String propertyPath, global::System.ComponentModel.INotifyPropertyChanged? oldValue, global::System.ComponentModel.INotifyPropertyChanged? newValue)
  {
  }
  private void SubscribeTo_x(global::Metalama.Patterns.Observability.AspectTests.FieldBackedInpcProperty.A value)
  {
    if (value != null)
    {
      this._on_xPropertyChangedHandler ??= (global::System.ComponentModel.PropertyChangedEventHandler)OnChildPropertyChanged_1;
      value.PropertyChanged += this._on_xPropertyChangedHandler;
    }
    void OnChildPropertyChanged_1(object? sender, global::System.ComponentModel.PropertyChangedEventArgs e)
    {
      {
        var propertyName = e.PropertyName;
        if (propertyName == "A1")
        {
          this.OnPropertyChanged("P2");
          return;
        }
      }
    }
  }
  public event global::System.ComponentModel.PropertyChangedEventHandler? PropertyChanged;
}