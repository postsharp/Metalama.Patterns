// Error LAMA5150 on `Test`: `Class 'Test' implements INotifyPropertyChanged but does not define a public or protected OnPropertyChanged method with the following signature: virtual void OnPropertyChanged(string propertyName). The method name can also be NotifyOfPropertyChange or RaisePropertyChanged.`
// Error LAMA5154 on `VirtualProperty`: `The 'Test.VirtualProperty' property is virtual. This is not supported by the [Observable] aspect.`
// Warning LAMA5162 on `Method`: `The 'Test.Method()' method cannot be analysed, and has not been configured as safe for dependency analysis. Use [IgnoreUnsupportedDependencies] or ConfigureObservability via a fabric to ignore this warning.`