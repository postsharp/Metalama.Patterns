[Observable]
public sealed class SealedInpcAutoPropertyWithRef : INotifyPropertyChanged
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
      if (!object.ReferenceEquals(value, this._x))
      {
        var oldValue = this._x;
        if (oldValue != null)
        {
          oldValue.PropertyChanged -= this._handleXPropertyChanged;
        }
        this._x = value;
        this.OnPropertyChanged("Y");
        this.OnPropertyChanged("X");
        this.SubscribeToX(value);
      }
    }
  }
  public int Y => this.X.A;
  private PropertyChangedEventHandler? _handleXPropertyChanged;
  [InvokedForProperties("X")]
  private void OnChildPropertyChanged(string parentPropertyPath, string propertyName)
  {
  }
  private void OnPropertyChanged(string propertyName)
  {
    this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
  }
  private void SubscribeToX(SimpleInpcByHand value)
  {
    if (value != null)
    {
      this._handleXPropertyChanged ??= HandlePropertyChanged;
      value.PropertyChanged += this._handleXPropertyChanged;
    }
    void HandlePropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
      {
        var propertyName = e.PropertyName;
        if (propertyName == "A")
        {
          this.OnPropertyChanged("Y");
          this.OnChildPropertyChanged("X", "A");
          return;
        }
        this.OnChildPropertyChanged("X", propertyName);
      }
    }
  }
  public event PropertyChangedEventHandler? PropertyChanged;
}