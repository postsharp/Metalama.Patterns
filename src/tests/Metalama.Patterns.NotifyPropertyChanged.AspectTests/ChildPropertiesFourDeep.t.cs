namespace Metalama.Patterns.NotifyPropertyChanged.AspectTests.ChildPropertiesFourDeep;
[NotifyPropertyChanged]
public partial class A : global::System.ComponentModel.INotifyPropertyChanged
{
  public A()
  {
    this.A2 = new B();
  }
  private int _a1;
  public int A1
  {
    get
    {
      return this._a1;
    }
    set
    {
      if ((this._a1 != value))
      {
        this._a1 = value;
        this.OnPropertyChanged("A1");
      }
    }
  }
  private B _a2 = default !;
  public B A2
  {
    get
    {
      return this._a2;
    }
    set
    {
      if (!global::System.Object.ReferenceEquals(value, this._a2))
      {
        var oldValue = this._a2;
        if (oldValue != null)
        {
          oldValue.PropertyChanged -= this._onA2PropertyChangedHandler;
        }
        this._a2 = value;
        this.UpdateA2B2();
        this.OnPropertyChanged("A2");
        this.SubscribeToA2(value);
      }
    }
  }
  public int A3 => this.A2.B2.C2.D1;
  private global::Metalama.Patterns.NotifyPropertyChanged.AspectTests.ChildPropertiesFourDeep.C? _lastA2B2;
  private global::Metalama.Patterns.NotifyPropertyChanged.AspectTests.ChildPropertiesFourDeep.D? _lastA2B2C2;
  private global::System.ComponentModel.PropertyChangedEventHandler? _onA2B2C2PropertyChangedHandler;
  private global::System.ComponentModel.PropertyChangedEventHandler? _onA2B2PropertyChangedHandler;
  private global::System.ComponentModel.PropertyChangedEventHandler? _onA2PropertyChangedHandler;
  [global::Metalama.Patterns.NotifyPropertyChanged.Metadata.OnChildPropertyChangedMethodAttribute(new global::System.String[] { "A2", "A2.B2", "A2.B2.C2" })]
  protected virtual void OnChildPropertyChanged(global::System.String parentPropertyPath, global::System.String propertyName)
  {
  }
  protected virtual void OnPropertyChanged(global::System.String propertyName)
  {
    this.PropertyChanged?.Invoke(this, new global::System.ComponentModel.PropertyChangedEventArgs(propertyName));
  }
  [global::Metalama.Patterns.NotifyPropertyChanged.Metadata.OnUnmonitoredObservablePropertyChangedMethodAttribute(new global::System.String[] { })]
  protected virtual void OnUnmonitoredObservablePropertyChanged(global::System.String propertyPath, global::System.ComponentModel.INotifyPropertyChanged? oldValue, global::System.ComponentModel.INotifyPropertyChanged? newValue)
  {
  }
  private void SubscribeToA2(global::Metalama.Patterns.NotifyPropertyChanged.AspectTests.ChildPropertiesFourDeep.B value)
  {
    if (value != null)
    {
      this._onA2PropertyChangedHandler ??= (global::System.ComponentModel.PropertyChangedEventHandler)OnChildPropertyChanged_1;
      value.PropertyChanged += this._onA2PropertyChangedHandler;
    }
    void OnChildPropertyChanged_1(object? sender, global::System.ComponentModel.PropertyChangedEventArgs e)
    {
      {
        var propertyName = e.PropertyName;
        if (propertyName == "B2")
        {
          this.UpdateA2B2();
          return;
        }
        this.OnChildPropertyChanged("A2", (global::System.String)propertyName);
      }
    }
  }
  private void UpdateA2B2()
  {
    var newValue = A2?.B2;
    if (!global::System.Object.ReferenceEquals(newValue, this._lastA2B2))
    {
      if (!global::System.Object.ReferenceEquals(this._lastA2B2, null))
      {
        this._lastA2B2!.PropertyChanged -= this._onA2B2PropertyChangedHandler;
      }
      if (newValue != null)
      {
        this._onA2B2PropertyChangedHandler ??= (global::System.ComponentModel.PropertyChangedEventHandler)OnChildPropertyChanged_1;
        newValue.PropertyChanged += this._onA2B2PropertyChangedHandler;
        void OnChildPropertyChanged_1(object? sender, global::System.ComponentModel.PropertyChangedEventArgs e)
        {
          {
            var propertyName = e.PropertyName;
            if (propertyName == "C2")
            {
              this.UpdateA2B2C2();
              return;
            }
            this.OnChildPropertyChanged("A2.B2", (global::System.String)propertyName);
          }
        }
      }
      this._lastA2B2 = newValue;
      this.UpdateA2B2C2();
      this.OnChildPropertyChanged("A2", "B2");
    }
  }
  private void UpdateA2B2C2()
  {
    var newValue = A2?.B2?.C2;
    if (!global::System.Object.ReferenceEquals(newValue, this._lastA2B2C2))
    {
      if (!global::System.Object.ReferenceEquals(this._lastA2B2C2, null))
      {
        this._lastA2B2C2!.PropertyChanged -= this._onA2B2C2PropertyChangedHandler;
      }
      if (newValue != null)
      {
        this._onA2B2C2PropertyChangedHandler ??= (global::System.ComponentModel.PropertyChangedEventHandler)OnChildPropertyChanged_1;
        newValue.PropertyChanged += this._onA2B2C2PropertyChangedHandler;
        void OnChildPropertyChanged_1(object? sender, global::System.ComponentModel.PropertyChangedEventArgs e)
        {
          {
            var propertyName = e.PropertyName;
            if (propertyName == "D1")
            {
              this.OnPropertyChanged("A3");
              this.OnChildPropertyChanged("A2.B2.C2", "D1");
              return;
            }
            this.OnChildPropertyChanged("A2.B2.C2", (global::System.String)propertyName);
          }
        }
      }
      this._lastA2B2C2 = newValue;
      this.OnPropertyChanged("A3");
      this.OnChildPropertyChanged("A2.B2", "C2");
    }
  }
  public event global::System.ComponentModel.PropertyChangedEventHandler? PropertyChanged;
}
[NotifyPropertyChanged]
public partial class B : global::System.ComponentModel.INotifyPropertyChanged
{
  public B()
  {
    this.B2 = new C();
  }
  private int _b1;
  public int B1
  {
    get
    {
      return this._b1;
    }
    set
    {
      if ((this._b1 != value))
      {
        this._b1 = value;
        this.OnPropertyChanged("B1");
      }
    }
  }
  private C _b2 = default !;
  public C B2
  {
    get
    {
      return this._b2;
    }
    set
    {
      if (!global::System.Object.ReferenceEquals(value, this._b2))
      {
        var oldValue = this._b2;
        this._b2 = value;
        this.OnUnmonitoredObservablePropertyChanged("B2", (global::System.ComponentModel.INotifyPropertyChanged? )oldValue, (global::System.ComponentModel.INotifyPropertyChanged? )value);
        this.OnPropertyChanged("B2");
      }
    }
  }
  [global::Metalama.Patterns.NotifyPropertyChanged.Metadata.OnChildPropertyChangedMethodAttribute(new global::System.String[] { })]
  protected virtual void OnChildPropertyChanged(global::System.String parentPropertyPath, global::System.String propertyName)
  {
  }
  protected virtual void OnPropertyChanged(global::System.String propertyName)
  {
    this.PropertyChanged?.Invoke(this, new global::System.ComponentModel.PropertyChangedEventArgs(propertyName));
  }
  [global::Metalama.Patterns.NotifyPropertyChanged.Metadata.OnUnmonitoredObservablePropertyChangedMethodAttribute(new global::System.String[] { "B2" })]
  protected virtual void OnUnmonitoredObservablePropertyChanged(global::System.String propertyPath, global::System.ComponentModel.INotifyPropertyChanged? oldValue, global::System.ComponentModel.INotifyPropertyChanged? newValue)
  {
  }
  public event global::System.ComponentModel.PropertyChangedEventHandler? PropertyChanged;
}
[NotifyPropertyChanged]
public partial class C : global::System.ComponentModel.INotifyPropertyChanged
{
  public C()
  {
    this.C2 = new D();
  }
  private int _c1;
  public int C1
  {
    get
    {
      return this._c1;
    }
    set
    {
      if ((this._c1 != value))
      {
        this._c1 = value;
        this.OnPropertyChanged("C1");
      }
    }
  }
  private D _c2 = default !;
  public D C2
  {
    get
    {
      return this._c2;
    }
    set
    {
      if (!global::System.Object.ReferenceEquals(value, this._c2))
      {
        var oldValue = this._c2;
        this._c2 = value;
        this.OnUnmonitoredObservablePropertyChanged("C2", (global::System.ComponentModel.INotifyPropertyChanged? )oldValue, (global::System.ComponentModel.INotifyPropertyChanged? )value);
        this.OnPropertyChanged("C2");
      }
    }
  }
  [global::Metalama.Patterns.NotifyPropertyChanged.Metadata.OnChildPropertyChangedMethodAttribute(new global::System.String[] { })]
  protected virtual void OnChildPropertyChanged(global::System.String parentPropertyPath, global::System.String propertyName)
  {
  }
  protected virtual void OnPropertyChanged(global::System.String propertyName)
  {
    this.PropertyChanged?.Invoke(this, new global::System.ComponentModel.PropertyChangedEventArgs(propertyName));
  }
  [global::Metalama.Patterns.NotifyPropertyChanged.Metadata.OnUnmonitoredObservablePropertyChangedMethodAttribute(new global::System.String[] { "C2" })]
  protected virtual void OnUnmonitoredObservablePropertyChanged(global::System.String propertyPath, global::System.ComponentModel.INotifyPropertyChanged? oldValue, global::System.ComponentModel.INotifyPropertyChanged? newValue)
  {
  }
  public event global::System.ComponentModel.PropertyChangedEventHandler? PropertyChanged;
}
[NotifyPropertyChanged]
public partial class D : global::System.ComponentModel.INotifyPropertyChanged
{
  private int _d1;
  public int D1
  {
    get
    {
      return this._d1;
    }
    set
    {
      if ((this._d1 != value))
      {
        this._d1 = value;
        this.OnPropertyChanged("D1");
      }
    }
  }
  private int _d2;
  public int D2
  {
    get
    {
      return this._d2;
    }
    set
    {
      if ((this._d2 != value))
      {
        this._d2 = value;
        this.OnPropertyChanged("D2");
      }
    }
  }
  [global::Metalama.Patterns.NotifyPropertyChanged.Metadata.OnChildPropertyChangedMethodAttribute(new global::System.String[] { })]
  protected virtual void OnChildPropertyChanged(global::System.String parentPropertyPath, global::System.String propertyName)
  {
  }
  protected virtual void OnPropertyChanged(global::System.String propertyName)
  {
    this.PropertyChanged?.Invoke(this, new global::System.ComponentModel.PropertyChangedEventArgs(propertyName));
  }
  [global::Metalama.Patterns.NotifyPropertyChanged.Metadata.OnUnmonitoredObservablePropertyChangedMethodAttribute(new global::System.String[] { })]
  protected virtual void OnUnmonitoredObservablePropertyChanged(global::System.String propertyPath, global::System.ComponentModel.INotifyPropertyChanged? oldValue, global::System.ComponentModel.INotifyPropertyChanged? newValue)
  {
  }
  public event global::System.ComponentModel.PropertyChangedEventHandler? PropertyChanged;
}