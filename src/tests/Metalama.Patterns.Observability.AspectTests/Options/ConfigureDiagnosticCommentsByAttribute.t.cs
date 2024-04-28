[Observable(DiagnosticCommentVerbosity = 1)]
public class ConfigureDiagnosticCommentsByAttribute : INotifyPropertyChanged
{
  [OnChildPropertyChangedMethod]
  protected virtual void OnChildPropertyChanged(string parentPropertyPath, string propertyName)
  {
  // Template: OnChildPropertyChanged
  }
  protected virtual void OnPropertyChanged(string propertyName)
  {
    // Template: OnPropertyChanged
    this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
  }
  [OnUnmonitoredObservablePropertyChangedMethod]
  protected virtual void OnUnmonitoredObservablePropertyChanged(string propertyPath, INotifyPropertyChanged? oldValue, INotifyPropertyChanged? newValue)
  {
  // Template: OnUnmonitoredObservablePropertyChanged
  }
  public event PropertyChangedEventHandler? PropertyChanged;
}