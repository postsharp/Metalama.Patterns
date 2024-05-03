internal class Regex : DependencyObject
{
  [DependencyProperty]
  public int Foo
  {
    get
    {
      return (int)GetValue(FooProperty);
    }
    set
    {
      this.SetValue(FooProperty, value);
    }
  }
  private void OnFooChanging()
  {
  }
  private void OnFooChanged()
  {
  }
  private bool ValidateFoo(int v) => true;
  [DependencyProperty]
  public string YodaFoo
  {
    get
    {
      return (string)GetValue(TheFooPropertyItIs);
    }
    set
    {
      this.SetValue(TheFooPropertyItIs, value);
    }
  }
  private void DoFooChanging()
  {
  }
  private void MakeFooChanged(string a, string b)
  {
  }
  private bool IsFooValid(string s) => true;
  public static readonly DependencyProperty FooProperty;
  public static readonly DependencyProperty TheFooPropertyItIs;
  static Regex()
  {
    var metadata = new PropertyMetadata();
    metadata.PropertyChangedCallback = new PropertyChangedCallback((d_1, e) =>
    {
      ((Regex)d_1).OnFooChanged();
    });
    metadata.CoerceValueCallback = new CoerceValueCallback((d, value) =>
    {
      if (!((Regex)d).ValidateFoo((int)value))
      {
        throw new ArgumentException("Invalid property value.", "value");
      }
      ((Regex)d).OnFooChanging();
      return value;
    });
    FooProperty = DependencyProperty.Register("Foo", typeof(int), typeof(Regex), metadata);
    var metadata_1 = new PropertyMetadata();
    metadata_1.PropertyChangedCallback = new PropertyChangedCallback((d_3, e_1) =>
    {
      ((Regex)d_3).MakeFooChanged((string)e_1.OldValue, (string)e_1.NewValue);
    });
    metadata_1.CoerceValueCallback = new CoerceValueCallback((d_2, value_1) =>
    {
      if (!((Regex)d_2).IsFooValid((string)value_1))
      {
        throw new ArgumentException("Invalid property value.", "value");
      }
      ((Regex)d_2).DoFooChanging();
      return value_1;
    });
    TheFooPropertyItIs = DependencyProperty.Register("Foo", typeof(string), typeof(Regex), metadata_1);
  }
}