using System.ComponentModel;
namespace Metalama.Patterns.Observability.AspectTests;
[Observable]
public partial class A<T> : INotifyPropertyChanged where T : class, INotifyPropertyChanged
{
  private T _a1 = default !;
  public T A1
  {
    get
    {
      return this._a1;
    }
    set
    {
      if (!object.ReferenceEquals(value, this._a1))
      {
        var oldValue = this._a1;
        this._a1 = value;
        this.OnObservablePropertyChanged("A1", (INotifyPropertyChanged? )oldValue, value);
        this.OnPropertyChanged("A1");
      }
    }
  }
  [InvokedFor("A1")]
  protected virtual void OnObservablePropertyChanged(string propertyPath, INotifyPropertyChanged? oldValue, INotifyPropertyChanged? newValue)
  {
  }
  protected virtual void OnPropertyChanged(string propertyName)
  {
    this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
  }
  public event PropertyChangedEventHandler? PropertyChanged;
}
public partial class AOfSimple : A<Simple>
{
  public int RefA1S1 => this.A1.S1;
  private PropertyChangedEventHandler? _onA1PropertyChangedHandler;
  [InvokedFor("A1")]
  protected virtual void OnChildPropertyChanged(string parentPropertyPath, string propertyName)
  {
  }
  [InvokedFor]
  protected override void OnObservablePropertyChanged(string propertyPath, INotifyPropertyChanged? oldValue, INotifyPropertyChanged? newValue)
  {
    if (propertyPath == "A1")
    {
      if (oldValue != null)
      {
        oldValue.PropertyChanged -= this._onA1PropertyChangedHandler;
      }
      if (newValue != null)
      {
        this._onA1PropertyChangedHandler ??= OnChildPropertyChanged_1;
        newValue.PropertyChanged += this._onA1PropertyChangedHandler;
        void OnChildPropertyChanged_1(object? sender, PropertyChangedEventArgs e)
        {
          {
            var propertyName = e.PropertyName;
            if (propertyName == "S1")
            {
              this.OnPropertyChanged("RefA1S1");
              this.OnChildPropertyChanged("A1", "S1");
              return;
            }
            this.OnChildPropertyChanged("A1", propertyName);
          }
        }
      }
      this.OnPropertyChanged("RefA1S1");
    }
    base.OnObservablePropertyChanged(propertyPath, oldValue, newValue);
  }
  protected override void OnPropertyChanged(string propertyName)
  {
    base.OnPropertyChanged(propertyName);
  }
}
[Observable]
public partial class Simple : INotifyPropertyChanged
{
  private int _s1;
  public int S1
  {
    get
    {
      return this._s1;
    }
    set
    {
      if (this._s1 != value)
      {
        this._s1 = value;
        this.OnPropertyChanged("S1");
      }
    }
  }
  private int _s2;
  public int S2
  {
    get
    {
      return this._s2;
    }
    set
    {
      if (this._s2 != value)
      {
        this._s2 = value;
        this.OnPropertyChanged("S2");
      }
    }
  }
  private int _s3;
  public int S3
  {
    get
    {
      return this._s3;
    }
    set
    {
      if (this._s3 != value)
      {
        this._s3 = value;
        this.OnPropertyChanged("S3");
      }
    }
  }
  protected virtual void OnPropertyChanged(string propertyName)
  {
    this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
  }
  public event PropertyChangedEventHandler? PropertyChanged;
}