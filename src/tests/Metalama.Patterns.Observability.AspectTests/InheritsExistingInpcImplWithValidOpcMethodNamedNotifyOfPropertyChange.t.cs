[Observable]
public partial class InheritsExistingInpcImplWithValidOpcMethodNamedNotifyOfPropertyChange : ExistingInpcImplWithValidOpcMethodNamedNotifyOfPropertyChange
{
  protected override void NotifyOfPropertyChange(string propertyName)
  {
    base.NotifyOfPropertyChange(propertyName);
  }
  [OnChildPropertyChangedMethod]
  protected virtual void OnChildPropertyChanged(string parentPropertyPath, string propertyName)
  {
  }
  [OnUnmonitoredObservablePropertyChangedMethod]
  protected virtual void OnUnmonitoredObservablePropertyChanged(string propertyPath, INotifyPropertyChanged? oldValue, INotifyPropertyChanged? newValue)
  {
  }
}