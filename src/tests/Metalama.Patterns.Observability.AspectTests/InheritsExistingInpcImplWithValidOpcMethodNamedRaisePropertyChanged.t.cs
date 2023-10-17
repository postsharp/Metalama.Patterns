[Observable]
public partial class InheritsExistingInpcImplWithValidOpcMethodNamedRaisePropertyChanged : ExistingInpcImplWithValidOpcMethodNamedRaisePropertyChanged
{
  [global::Metalama.Patterns.Observability.Metadata.OnChildPropertyChangedMethodAttribute(new global::System.String[] { })]
  protected virtual void OnChildPropertyChanged(global::System.String parentPropertyPath, global::System.String propertyName)
  {
  }
  [global::Metalama.Patterns.Observability.Metadata.OnUnmonitoredObservablePropertyChangedMethodAttribute(new global::System.String[] { })]
  protected virtual void OnUnmonitoredObservablePropertyChanged(global::System.String propertyPath, global::System.ComponentModel.INotifyPropertyChanged? oldValue, global::System.ComponentModel.INotifyPropertyChanged? newValue)
  {
  }
  protected override void RaisePropertyChanged(global::System.String propertyName)
  {
    base.RaisePropertyChanged(propertyName);
  }
}