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
      this.SetValue(NameProperty, value);
    }
  }
  public static readonly DependencyProperty NameProperty;
  static WithoutValidateMethod()
  {
    var metadata = new PropertyMetadata();
    metadata.CoerceValueCallback = new CoerceValueCallback((d, value) =>
    {
      value = ApplyNameContracts((string)value);
      return value;
    });
    NameProperty = DependencyProperty.Register("Name", typeof(string), typeof(WithoutValidateMethod), metadata);
  }
  private static string ApplyNameContracts(string value)
  {
    value = value.Trim();
    return value;
  }
}