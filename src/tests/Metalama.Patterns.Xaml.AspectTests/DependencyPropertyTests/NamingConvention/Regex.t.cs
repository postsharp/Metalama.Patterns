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
      this.SetValue(Regex.FooProperty, value);
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
      this.SetValue(Regex.TheFooPropertyItIs, value);
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
    object CoerceValue_1(DependencyObject d, object value)
    {
      if (!((Regex)d).ValidateFoo((int)value))
      {
        throw new ArgumentException("Invalid property value.", "value");
      }
      ((Regex)d).OnFooChanging();
      return value;
    }
    void PropertyChanged(DependencyObject d_1, DependencyPropertyChangedEventArgs e)
    {
      ((Regex)d_1).OnFooChanged();
    }
    var metadata = new PropertyMetadata();
    metadata.PropertyChangedCallback = PropertyChanged;
    metadata.CoerceValueCallback = CoerceValue_1;
    Regex.FooProperty = DependencyProperty.Register("Foo", typeof(int), typeof(Regex), metadata);
    object CoerceValue_2(DependencyObject d_2, object value_1)
    {
      if (!((Regex)d_2).IsFooValid((string)value_1))
      {
        throw new ArgumentException("Invalid property value.", "value");
      }
      ((Regex)d_2).DoFooChanging();
      return value_1;
    }
    void PropertyChanged_1(DependencyObject d_3, DependencyPropertyChangedEventArgs e_1)
    {
      ((Regex)d_3).MakeFooChanged((string)e_1.OldValue, (string)e_1.NewValue);
    }
    var metadata_1 = new PropertyMetadata();
    metadata_1.PropertyChangedCallback = PropertyChanged_1;
    metadata_1.CoerceValueCallback = CoerceValue_2;
    Regex.TheFooPropertyItIs = DependencyProperty.Register("Foo", typeof(string), typeof(Regex), metadata_1);
  }
}