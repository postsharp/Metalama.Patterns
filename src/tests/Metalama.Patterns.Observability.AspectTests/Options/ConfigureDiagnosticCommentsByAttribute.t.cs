[Observable]
public class ConfigureDiagnosticCommentsByAttribute : INotifyPropertyChanged
{
  protected virtual void OnPropertyChanged(string propertyName)
  {
    this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
  }
  public event PropertyChangedEventHandler? PropertyChanged;
}