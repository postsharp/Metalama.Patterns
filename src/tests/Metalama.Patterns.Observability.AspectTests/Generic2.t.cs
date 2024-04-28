using System.ComponentModel;
namespace Metalama.Patterns.Observability.AspectTests.Generic2;
public interface IFoo
{
  int X { get; }
  int Y { get; }
}
[Observable(DiagnosticCommentVerbosity = 1)]
public partial class D<T> : INotifyPropertyChanged where T : class, INotifyPropertyChanged, IFoo
{
  private T _d1 = default !;
  public T D1
  {
    get
    {
      return this._d1;
    }
    set
    {
      // Template: OverrideInpcRefTypePropertySetter
      if (!object.ReferenceEquals(value, this._d1))
      {
        var oldValue = this._d1;
        if (oldValue != null)
        {
          oldValue.PropertyChanged -= this._handleD1PropertyChanged;
        }
        this._d1 = value;
        this.OnObservablePropertyChanged("D1", oldValue, value);
        this.OnPropertyChanged("FooX");
        this.OnPropertyChanged("D1");
        this.SubscribeToD1(value);
      }
    }
  }
  public int FooX => this.D1.X;
  private PropertyChangedEventHandler? _handleD1PropertyChanged;
  [InvokedForProperties("D1")]
  protected virtual void OnChildPropertyChanged(string parentPropertyPath, string propertyName)
  {
  // Template: OnChildPropertyChanged
  }
  [InvokedForProperties("D1")]
  protected virtual void OnObservablePropertyChanged(string propertyPath, INotifyPropertyChanged? oldValue, INotifyPropertyChanged? newValue)
  {
  // Template: OnObservablePropertyChanged
  }
  protected virtual void OnPropertyChanged(string propertyName)
  {
    // Template: OnPropertyChanged
    // Skipping 'D1': The field or property is defined by the current type.
    // Skipping 'FooX': The field or property is defined by the current type.
    this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
  }
  private void SubscribeToD1(T value)
  {
    if (value != null)
    {
      this._handleD1PropertyChanged ??= HandlePropertyChanged;
      value.PropertyChanged += this._handleD1PropertyChanged;
    }
    void HandlePropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
      {
        // Template: HandleChildPropertyChangedDelegateBody
        var propertyName = e.PropertyName;
        if (propertyName == "X")
        {
          this.OnPropertyChanged("FooX");
          this.OnChildPropertyChanged("D1", "X");
          return;
        }
        this.OnChildPropertyChanged("D1", propertyName);
      }
    }
  }
  public event PropertyChangedEventHandler? PropertyChanged;
}
public partial class DD<T> : D<T> where T : class, INotifyPropertyChanged, IFoo
{
  public int FooY => this.D1.Y;
  protected override void OnChildPropertyChanged(string parentPropertyPath, string propertyName)
  {
    // Template: OnChildPropertyChanged
    if (parentPropertyPath == "D1" && propertyName == "Y")
    {
      this.OnPropertyChanged("FooY");
      base.OnChildPropertyChanged(parentPropertyPath, propertyName);
      return;
    }
    base.OnChildPropertyChanged(parentPropertyPath, propertyName);
  }
  protected override void OnPropertyChanged(string propertyName)
  {
    // Template: OnPropertyChanged
    if (propertyName == "D1")
    {
      // InpcBaseHandling = OnChildPropertyChanged
      this.OnPropertyChanged("FooY");
    }
    // Skipping 'FooY': The field or property is defined by the current type.
    base.OnPropertyChanged(propertyName);
  }
}
[Observable(DiagnosticCommentVerbosity = 1)]
public partial class MyFoo : IFoo, INotifyPropertyChanged
{
  private int _x;
  public int X
  {
    get
    {
      return this._x;
    }
    set
    {
      // Template: OverrideUninstrumentedTypePropertySetter
      if (this._x != value)
      {
        this._x = value;
        this.OnPropertyChanged("X");
      }
    }
  }
  private int _y;
  public int Y
  {
    get
    {
      return this._y;
    }
    set
    {
      // Template: OverrideUninstrumentedTypePropertySetter
      if (this._y != value)
      {
        this._y = value;
        this.OnPropertyChanged("Y");
      }
    }
  }
  protected virtual void OnPropertyChanged(string propertyName)
  {
    // Template: OnPropertyChanged
    this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
  }
  public event PropertyChangedEventHandler? PropertyChanged;
}