[NotifyPropertyChanged]
public class ConfigureDiagnosticCommentsByFabric : global::System.ComponentModel.INotifyPropertyChanged
{
  [global::Metalama.Patterns.NotifyPropertyChanged.Metadata.OnChildPropertyChangedMethodAttribute(new global::System.String[] { })]
  protected virtual void OnChildPropertyChanged(global::System.String parentPropertyPath, global::System.String propertyName)
  {
  // Template: OnChildPropertyChanged
  }
  protected virtual void OnPropertyChanged(global::System.String propertyName)
  {
    // Template: OnPropertyChanged
    this.PropertyChanged?.Invoke(this, new global::System.ComponentModel.PropertyChangedEventArgs(propertyName));
  }
  [global::Metalama.Patterns.NotifyPropertyChanged.Metadata.OnUnmonitoredObservablePropertyChangedMethodAttribute(new global::System.String[] { })]
  protected virtual void OnUnmonitoredObservablePropertyChanged(global::System.String propertyPath, global::System.ComponentModel.INotifyPropertyChanged? oldValue, global::System.ComponentModel.INotifyPropertyChanged? newValue)
  {
  // Template: OnUnmonitoredObservablePropertyChanged
  }
  public event global::System.ComponentModel.PropertyChangedEventHandler? PropertyChanged;
}