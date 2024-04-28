using System.ComponentModel;
namespace Metalama.Patterns.Observability.AspectTests.ChildPropertiesFourDeep;
[Observable]
public partial class A : INotifyPropertyChanged
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
      if (this._a1 != value)
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
      if (!object.ReferenceEquals(value, this._a2))
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
  private C? _lastA2B2;
  private D? _lastA2B2C2;
  private PropertyChangedEventHandler? _onA2B2C2PropertyChangedHandler;
  private PropertyChangedEventHandler? _onA2B2PropertyChangedHandler;
  private PropertyChangedEventHandler? _onA2PropertyChangedHandler;
  [InvokedFor("A2", "A2.B2", "A2.B2.C2")]
  protected virtual void OnChildPropertyChanged(string parentPropertyPath, string propertyName)
  {
  }
  protected virtual void OnPropertyChanged(string propertyName)
  {
    this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
  }
  private void SubscribeToA2(B value)
  {
    if (value != null)
    {
      this._onA2PropertyChangedHandler ??= Handle;
      value.PropertyChanged += this._onA2PropertyChangedHandler;
    }
    void Handle(object? sender, PropertyChangedEventArgs e)
    {
      {
        var propertyName = e.PropertyName;
        if (propertyName == "B2")
        {
          this.UpdateA2B2();
          return;
        }
        this.OnChildPropertyChanged("A2", propertyName);
      }
    }
  }
  private void UpdateA2B2()
  {
    var newValue = A2?.B2;
    if (!object.ReferenceEquals(newValue, this._lastA2B2))
    {
      if (!object.ReferenceEquals(this._lastA2B2, null))
      {
        this._lastA2B2!.PropertyChanged -= this._onA2B2PropertyChangedHandler;
      }
      if (newValue != null)
      {
        this._onA2B2PropertyChangedHandler ??= OnChildPropertyChanged_1;
        newValue.PropertyChanged += this._onA2B2PropertyChangedHandler;
        void OnChildPropertyChanged_1(object? sender, PropertyChangedEventArgs e)
        {
          {
            var propertyName = e.PropertyName;
            if (propertyName == "C2")
            {
              this.UpdateA2B2C2();
              return;
            }
            this.OnChildPropertyChanged("A2.B2", propertyName);
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
    if (!object.ReferenceEquals(newValue, this._lastA2B2C2))
    {
      if (!object.ReferenceEquals(this._lastA2B2C2, null))
      {
        this._lastA2B2C2!.PropertyChanged -= this._onA2B2C2PropertyChangedHandler;
      }
      if (newValue != null)
      {
        this._onA2B2C2PropertyChangedHandler ??= OnChildPropertyChanged_1;
        newValue.PropertyChanged += this._onA2B2C2PropertyChangedHandler;
        void OnChildPropertyChanged_1(object? sender, PropertyChangedEventArgs e)
        {
          {
            var propertyName = e.PropertyName;
            if (propertyName == "D1")
            {
              this.OnPropertyChanged("A3");
              this.OnChildPropertyChanged("A2.B2.C2", "D1");
              return;
            }
            this.OnChildPropertyChanged("A2.B2.C2", propertyName);
          }
        }
      }
      this._lastA2B2C2 = newValue;
      this.OnPropertyChanged("A3");
      this.OnChildPropertyChanged("A2.B2", "C2");
    }
  }
  public event PropertyChangedEventHandler? PropertyChanged;
}
[Observable]
public partial class B : INotifyPropertyChanged
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
      if (this._b1 != value)
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
      if (!object.ReferenceEquals(value, this._b2))
      {
        var oldValue = this._b2;
        this._b2 = value;
        this.OnObservablePropertyChanged("B2", (INotifyPropertyChanged? )oldValue, (INotifyPropertyChanged? )value);
        this.OnPropertyChanged("B2");
      }
    }
  }
  [InvokedFor("B2")]
  protected virtual void OnObservablePropertyChanged(string propertyPath, INotifyPropertyChanged? oldValue, INotifyPropertyChanged? newValue)
  {
  }
  protected virtual void OnPropertyChanged(string propertyName)
  {
    this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
  }
  public event PropertyChangedEventHandler? PropertyChanged;
}
[Observable]
public partial class C : INotifyPropertyChanged
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
      if (this._c1 != value)
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
      if (!object.ReferenceEquals(value, this._c2))
      {
        var oldValue = this._c2;
        this._c2 = value;
        this.OnObservablePropertyChanged("C2", (INotifyPropertyChanged? )oldValue, (INotifyPropertyChanged? )value);
        this.OnPropertyChanged("C2");
      }
    }
  }
  [InvokedFor("C2")]
  protected virtual void OnObservablePropertyChanged(string propertyPath, INotifyPropertyChanged? oldValue, INotifyPropertyChanged? newValue)
  {
  }
  protected virtual void OnPropertyChanged(string propertyName)
  {
    this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
  }
  public event PropertyChangedEventHandler? PropertyChanged;
}
[Observable]
public partial class D : INotifyPropertyChanged
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
      if (this._d1 != value)
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
      if (this._d2 != value)
      {
        this._d2 = value;
        this.OnPropertyChanged("D2");
      }
    }
  }
  protected virtual void OnPropertyChanged(string propertyName)
  {
    this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
  }
  public event PropertyChangedEventHandler? PropertyChanged;
}