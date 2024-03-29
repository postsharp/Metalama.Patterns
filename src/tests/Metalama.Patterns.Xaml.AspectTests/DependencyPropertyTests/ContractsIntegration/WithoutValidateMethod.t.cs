using Metalama.Patterns.Contracts;
using System.Windows;
namespace Metalama.Patterns.Xaml.AspectTests.DependencyPropertyTests.ContractsIntegration;
internal class WithoutValidateMethod : DependencyObject
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
      this.SetValue(WithoutValidateMethod.NameProperty, value);
    }
  }
  public static readonly DependencyProperty NameProperty;
  static WithoutValidateMethod()
  {
    object CoerceValue_1(DependencyObject d, object value)
    {
      value = WithoutValidateMethod.ApplyNameContracts((string)value);
      return value;
    }
    var metadata = new PropertyMetadata();
    metadata.CoerceValueCallback = CoerceValue_1;
    WithoutValidateMethod.NameProperty = DependencyProperty.Register("Name", typeof(string), typeof(WithoutValidateMethod), metadata);
  }
  private static string ApplyNameContracts(string value)
  {
    value = value.Trim();
    return value;
  }
}