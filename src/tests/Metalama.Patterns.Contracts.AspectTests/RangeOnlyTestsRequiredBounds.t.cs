using System;
namespace Metalama.Patterns.Contracts.AspectTests;
public sealed class RangeOnlyTestsRequiredBounds
{
  public void Negative([NegativeOrZero] int x)
  {
    if (x is> 0)
    {
      throw new ArgumentOutOfRangeException("x", "The 'x' parameter must be less than or equal to 0.");
    }
  }
  public void Positive([PositiveOrZero] int x)
  {
    if (x is < 0)
    {
      throw new ArgumentOutOfRangeException("x", "The 'x' parameter must be greater than or equal to 0.");
    }
  }
  public void StrictlyNegative([StrictlyNegative] int x)
  {
    if (x is >= 0)
    {
      throw new ArgumentOutOfRangeException("x", "The 'x' parameter must be strictly less than 0.");
    }
  }
  public void StrictlyPositive([StrictlyPositive] int x)
  {
    if (x is <= 0)
    {
      throw new ArgumentOutOfRangeException("x", "The 'x' parameter must be strictly greater than 0.");
    }
  }
  public void LessThanInt([LessThanOrEqualTo(42)] int x)
  {
    if (x is> 42)
    {
      throw new ArgumentOutOfRangeException("x", "The 'x' parameter must be less than or equal to 42.");
    }
  }
  public void GreaterThanInt([GreaterThanOrEqualTo(42)] int x)
  {
    if (x is < 42)
    {
      throw new ArgumentOutOfRangeException("x", "The 'x' parameter must be greater than or equal to 42.");
    }
  }
  public void RangeInt([Range(10, 20)] int x)
  {
    if (x is < 10 or > 20)
    {
      throw new ArgumentOutOfRangeException("The 'x' parameter must be in the range [10, 20].", "x");
    }
  }
  public void StrictlyGreaterThanInt([StrictlyGreaterThan(42)] int x)
  {
    if (x is <= 42)
    {
      throw new ArgumentOutOfRangeException("x", "The 'x' parameter must be strictly greater than 42.");
    }
  }
  public void StrictlyLessThanInt([StrictlyLessThan(42)] int x)
  {
    if (x is >= 42)
    {
      throw new ArgumentOutOfRangeException("x", "The 'x' parameter must be strictly less than 42.");
    }
  }
  public void StrictRangeInt([StrictRange(10.0, 20.0)] int x)
  {
    if (x is <= 10 or >= 20)
    {
      throw new ArgumentOutOfRangeException("The 'x' parameter must be strictly in the range ]10, 20[.", "x");
    }
  }
  public void LessThanDouble([LessThanOrEqualTo(42.0)] int x)
  {
    if (x is> 42)
    {
      throw new ArgumentOutOfRangeException("x", "The 'x' parameter must be less than or equal to 42.");
    }
  }
  public void GreaterThanDouble([GreaterThanOrEqualTo(42.0)] int x)
  {
    if (x is < 42)
    {
      throw new ArgumentOutOfRangeException("x", "The 'x' parameter must be greater than or equal to 42.");
    }
  }
  public void RangeDouble([Range(10.0, 20.0)] int x)
  {
    if (x is < 10 or > 20)
    {
      throw new ArgumentOutOfRangeException("The 'x' parameter must be in the range [10, 20].", "x");
    }
  }
  public void StrictlyGreaterThanDouble([StrictlyGreaterThan(42.0)] int x)
  {
    if (x is <= 42)
    {
      throw new ArgumentOutOfRangeException("x", "The 'x' parameter must be strictly greater than 42.");
    }
  }
  public void StrictlyLessThanDouble([StrictlyLessThan(42.0)] int x)
  {
    if (x is >= 42)
    {
      throw new ArgumentOutOfRangeException("x", "The 'x' parameter must be strictly less than 42.");
    }
  }
  public void StrictRangeDouble([StrictRange(10.0, 20.0)] int x)
  {
    if (x is <= 10 or >= 20)
    {
      throw new ArgumentOutOfRangeException("The 'x' parameter must be strictly in the range ]10, 20[.", "x");
    }
  }
  public void LessThanUnsigned([LessThanOrEqualTo(42ul)] int x)
  {
    if (x is> 42)
    {
      throw new ArgumentOutOfRangeException("x", "The 'x' parameter must be less than or equal to 42.");
    }
  }
  public void GreaterThanUnsigned([GreaterThanOrEqualTo(42ul)] int x)
  {
    if (x is < 42)
    {
      throw new ArgumentOutOfRangeException("x", "The 'x' parameter must be greater than or equal to 42.");
    }
  }
  public void RangeUnsigned([Range(10ul, 20ul)] int x)
  {
    if (x is < 10 or > 20)
    {
      throw new ArgumentOutOfRangeException("The 'x' parameter must be in the range [10, 20].", "x");
    }
  }
  public void StrictlyGreaterThanUnsigned([StrictlyGreaterThan(42ul)] int x)
  {
    if (x is <= 42)
    {
      throw new ArgumentOutOfRangeException("x", "The 'x' parameter must be strictly greater than 42.");
    }
  }
  public void StrictlyLessThanUnsigned([StrictlyLessThan(42ul)] int x)
  {
    if (x is >= 42)
    {
      throw new ArgumentOutOfRangeException("x", "The 'x' parameter must be strictly less than 42.");
    }
  }
  public void StrictRangeUnsigned([StrictRange(10ul, 20ul)] int x)
  {
    if (x is <= 10 or >= 20)
    {
      throw new ArgumentOutOfRangeException("The 'x' parameter must be strictly in the range ]10, 20[.", "x");
    }
  }
}