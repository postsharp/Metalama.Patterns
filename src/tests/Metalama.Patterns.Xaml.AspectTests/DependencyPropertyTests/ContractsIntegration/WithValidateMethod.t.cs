using Metalama.Patterns.Contracts;
using System.Windows;
namespace Metalama.Patterns.Xaml.AspectTests.DependencyPropertyTests.ContractsIntegration;
internal class WithValidateMethod : DependencyObject
{
  [DependencyProperty]
  [Trim]
  public string Name
  {
    get
    {
      return (string)GetValue(NameProperty);
    }
    set
    {
      this.SetValue(WithValidateMethod.NameProperty, value);
    }
  }
  private bool ValidateName(string name) => name.Length > 3;
  public static readonly DependencyProperty NameProperty;
  static WithValidateMethod()
  {
    object CoerceValue_1(DependencyObject d, object value)
    {
      value = WithValidateMethod.ApplyNameContracts((string)value);
      if (!((WithValidateMethod)d).ValidateName((string)value))
      {
        throw new ArgumentException("Invalid property value.", "value");
      }
      return value;
    }
    var metadata = new PropertyMetadata();
    metadata.CoerceValueCallback = CoerceValue_1;
    WithValidateMethod.NameProperty = DependencyProperty.Register("Name", typeof(string), typeof(WithValidateMethod), metadata);
  }
  private static string ApplyNameContracts(string value)
  {
    value = value.Trim();
    return value;
  }
}