[Observable]
public sealed class SealedInpcAutoPropertyNoRefs : global::System.ComponentModel.INotifyPropertyChanged
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
        this._x = value;
        this.OnPropertyChanged("X");
      }
    }
  }
  [global::Metalama.Patterns.Observability.Metadata.OnChildPropertyChangedMethodAttribute(new global::System.String[] { })]
  private void OnChildPropertyChanged(global::System.String parentPropertyPath, global::System.String propertyName)
  {
  }
  private void OnPropertyChanged(global::System.String propertyName)
  {
    this.PropertyChanged?.Invoke(this, new global::System.ComponentModel.PropertyChangedEventArgs(propertyName));
  }
  public event global::System.ComponentModel.PropertyChangedEventHandler? PropertyChanged;
}