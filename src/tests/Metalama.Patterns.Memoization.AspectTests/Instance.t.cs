using System.Globalization;
using System.Runtime.CompilerServices;
// ReSharper disable MemberCanBeMadeStatic.Global
// ReSharper disable EqualExpressionComparison
#pragma warning disable CA2201
#pragma warning disable SA1402
namespace Metalama.Patterns.Memoize.AspectTests.Instance;
internal sealed class TheClass
{
  private int _counter;
  [Memoize]
  public string NonNullableMethod()
  {
    if (this._NonNullableMethod == null)
    {
      string value;
      value = this._counter++.ToString(CultureInfo.InvariantCulture);
      global::System.Threading.Interlocked.CompareExchange(ref this._NonNullableMethod, value, null);
    }
    return _NonNullableMethod;
  }
  [Memoize]
  public string? NullableMethod()
  {
    if (this._NullableMethod == null)
    {
      var value = new StrongBox<string?>(this.NullableMethod_Source());
      global::System.Threading.Interlocked.CompareExchange(ref this._NullableMethod, value, null);
    }
    return _NullableMethod!.Value;
  }
  private string? NullableMethod_Source() => this._counter++.ToString(CultureInfo.InvariantCulture);
  [Memoize]
  public string NonNullableProperty
  {
    get
    {
      if (this._NonNullableProperty == null)
      {
        string value;
        value = this._counter++.ToString(CultureInfo.InvariantCulture);
        global::System.Threading.Interlocked.CompareExchange(ref this._NonNullableProperty, value, null);
      }
      return _NonNullableProperty;
    }
  }
  [Memoize]
  public string? NullableProperty
  {
    get
    {
      if (this._NullableProperty == null)
      {
        var value = new StrongBox<string?>(this.NullableProperty_Source);
        global::System.Threading.Interlocked.CompareExchange(ref this._NullableProperty, value, null);
      }
      return _NullableProperty!.Value;
    }
  }
  private string? NullableProperty_Source => this._counter++.ToString(CultureInfo.InvariantCulture);
  [Memoize]
  public Guid MethodReturnsStruct()
  {
    if (this._MethodReturnsStruct == null)
    {
      var value = new StrongBox<Guid>(this.MethodReturnsStruct_Source());
      global::System.Threading.Interlocked.CompareExchange(ref this._MethodReturnsStruct, value, null);
    }
    return _MethodReturnsStruct!.Value;
  }
  private Guid MethodReturnsStruct_Source() => Guid.NewGuid();
  [Memoize]
  public Guid PropertyReturnsStruct
  {
    get
    {
      if (this._PropertyReturnsStruct == null)
      {
        var value = new StrongBox<Guid>(this.PropertyReturnsStruct_Source);
        global::System.Threading.Interlocked.CompareExchange(ref this._PropertyReturnsStruct, value, null);
      }
      return _PropertyReturnsStruct!.Value;
    }
  }
  private Guid PropertyReturnsStruct_Source => Guid.NewGuid();
  private StrongBox<Guid> _MethodReturnsStruct;
  private string _NonNullableMethod;
  private string _NonNullableProperty;
  private StrongBox<string?> _NullableMethod;
  private StrongBox<string?> _NullableProperty;
  private StrongBox<Guid> _PropertyReturnsStruct;
}
internal static class Program
{
  public static void Main()
  {
    var o = new TheClass();
    if (o.MethodReturnsStruct() != o.MethodReturnsStruct())
    {
      throw new Exception();
    }
    if (o.PropertyReturnsStruct != o.PropertyReturnsStruct)
    {
      throw new Exception();
    }
    if (o.NonNullableMethod() != o.NonNullableMethod())
    {
      throw new Exception();
    }
    if (o.NullableMethod() != o.NullableMethod())
    {
      throw new Exception();
    }
    if (o.NonNullableProperty != o.NonNullableProperty)
    {
      throw new Exception();
    }
    if (o.NullableProperty != o.NullableProperty)
    {
      throw new Exception();
    }
    Console.WriteLine("Execution OK.");
  }
}