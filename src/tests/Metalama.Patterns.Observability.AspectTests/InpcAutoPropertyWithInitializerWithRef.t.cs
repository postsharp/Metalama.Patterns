using System.ComponentModel;
// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.
// @Include(Include/SimpleInpcByHand.cs)
using Metalama.Patterns.Observability.AspectTests.Include;
using Metalama.Patterns.Observability.Metadata;
namespace Metalama.Patterns.Observability.AspectTests.InpcAutoPropertyWithInitializerWithRef;
[Observable]
public class InpcAutoPropertyWithInitializerWithRef : INotifyPropertyChanged
{
  private SimpleInpcByHand _x = new(42);
  public SimpleInpcByHand X
  {
    get
    {
      return this._x;
    }
    set
    {
      if (!object.ReferenceEquals(value, this._x))
      {
        var oldValue = this._x;
        if (oldValue != null)
        {
          oldValue.PropertyChanged -= this._onXPropertyChangedHandler;
        }
        this._x = value;
        this.OnPropertyChanged("Y");
        this.OnPropertyChanged("X");
        this.SubscribeToX(value);
      }
    }
  }
  public int Y => this.X.A;
  private PropertyChangedEventHandler? _onXPropertyChangedHandler;
  public InpcAutoPropertyWithInitializerWithRef()
  {
    this.SubscribeToX(this.X);
  }
  [OnChildPropertyChangedMethod(new string[] { "X" })]
  protected virtual void OnChildPropertyChanged(string parentPropertyPath, string propertyName)
  {
  }
  protected virtual void OnPropertyChanged(string propertyName)
  {
    this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
  }
  [OnUnmonitoredObservablePropertyChangedMethod(new string[] { })]
  protected virtual void OnUnmonitoredObservablePropertyChanged(string propertyPath, INotifyPropertyChanged? oldValue, INotifyPropertyChanged? newValue)
  {
  }
  private void SubscribeToX(SimpleInpcByHand value)
  {
    if (value != null)
    {
      this._onXPropertyChangedHandler ??= OnChildPropertyChanged_1;
      value.PropertyChanged += this._onXPropertyChangedHandler;
    }
    void OnChildPropertyChanged_1(object? sender, PropertyChangedEventArgs e)
    {
      {
        var propertyName = e.PropertyName;
        if (propertyName == "A")
        {
          this.OnPropertyChanged("Y");
          this.OnChildPropertyChanged("X", "A");
          return;
        }
        this.OnChildPropertyChanged("X", propertyName);
      }
    }
  }
  public event PropertyChangedEventHandler? PropertyChanged;
}