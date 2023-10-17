[Observable]
public sealed class SealedInpcAutoPropertyWithRef : global::System.ComponentModel.INotifyPropertyChanged
{
  private SimpleInpcByHand _x = default !;
  public SimpleInpcByHand X
  {
    get
    {
      return this._x;
    }
    set
    {
      if (!global::System.Object.ReferenceEquals(value, this._x))
      {
        var oldValue = this._x;
        if (oldValue != null)
        {
          oldValue.PropertyChanged -= this._onXPropertyChangedHandler;
        }
        this._x = value;
        this.OnPropertyChanged("Y");
        this.OnPropertyChanged("X");
        this.SubscribeToX(value);
      }
    }
  }
  public int Y => this.X.A;
  private global::System.ComponentModel.PropertyChangedEventHandler? _onXPropertyChangedHandler;
  [global::Metalama.Patterns.Observability.Metadata.OnChildPropertyChangedMethodAttribute(new global::System.String[] { "X" })]
  private void OnChildPropertyChanged(global::System.String parentPropertyPath, global::System.String propertyName)
  {
  }
  private void OnPropertyChanged(global::System.String propertyName)
  {
    this.PropertyChanged?.Invoke(this, new global::System.ComponentModel.PropertyChangedEventArgs(propertyName));
  }
  private void SubscribeToX(global::Metalama.Patterns.Observability.AspectTests.Include.SimpleInpcByHand value)
  {
    if (value != null)
    {
      this._onXPropertyChangedHandler ??= (global::System.ComponentModel.PropertyChangedEventHandler)OnChildPropertyChanged_1;
      value.PropertyChanged += this._onXPropertyChangedHandler;
    }
    void OnChildPropertyChanged_1(object? sender, global::System.ComponentModel.PropertyChangedEventArgs e)
    {
      {
        var propertyName = e.PropertyName;
        if (propertyName == "A")
        {
          this.OnPropertyChanged("Y");
          this.OnChildPropertyChanged("X", "A");
          return;
        }
        this.OnChildPropertyChanged("X", (global::System.String)propertyName);
      }
    }
  }
  public event global::System.ComponentModel.PropertyChangedEventHandler? PropertyChanged;
}