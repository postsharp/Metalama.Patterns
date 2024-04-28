[Observable]
public partial class InheritsExistingInpcImplWithValidOpcMethodNamedRaisePropertyChanged : ExistingInpcImplWithValidOpcMethodNamedRaisePropertyChanged
{
  [OnChildPropertyChangedMethod]
  protected virtual void OnChildPropertyChanged(string parentPropertyPath, string propertyName)
  {
  }
  [OnUnmonitoredObservablePropertyChangedMethod]
  protected virtual void OnUnmonitoredObservablePropertyChanged(string propertyPath, INotifyPropertyChanged? oldValue, INotifyPropertyChanged? newValue)
  {
  }
  protected override void RaisePropertyChanged(string propertyName)
  {
    base.RaisePropertyChanged(propertyName);
  }
}