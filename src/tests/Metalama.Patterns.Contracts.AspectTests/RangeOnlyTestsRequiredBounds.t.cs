using System;
namespace Metalama.Patterns.Contracts.AspectTests;
public sealed class RangeOnlyTestsRequiredBounds
{
  public void Negative([Negative] int x)
  {
    if (x > 0)
    {
      throw new ArgumentOutOfRangeException("x", "The 'x' parameter must be less than 0.");
    }
  }
  public void Positive([Positive] int x)
  {
    if (x < 0)
    {
      throw new ArgumentOutOfRangeException("x", "The 'x' parameter must be greater than 0.");
    }
  }
  public void StrictlyNegative([StrictlyNegative] int x)
  {
    if (x > -1)
    {
      throw new ArgumentOutOfRangeException("x", "The 'x' parameter must be strictly less than 0.");
    }
  }
  public void StrictlyPositive([StrictlyPositive] int x)
  {
    if (x < 1)
    {
      throw new ArgumentOutOfRangeException("x", "The 'x' parameter must be strictly greater than 0.");
    }
  }
  public void LessThanInt([LessThan(42)] int x)
  {
    if (x > 42)
    {
      throw new ArgumentOutOfRangeException("x", "The 'x' parameter must be less than 42.");
    }
  }
  public void GreaterThanInt([GreaterThan(42)] int x)
  {
    if (x < 42)
    {
      throw new ArgumentOutOfRangeException("x", "The 'x' parameter must be greater than 42.");
    }
  }
  public void RangeInt([Range(10, 20)] int x)
  {
    if (x < 10 || x > 20)
    {
      throw new ArgumentOutOfRangeException("x", "The 'x' parameter must be between 10 and 20.");
    }
  }
  public void StrictlyGreaterThanInt([StrictlyGreaterThan(42)] int x)
  {
    if (x < 43)
    {
      throw new ArgumentOutOfRangeException("x", "The 'x' parameter must be strictly greater than 42.");
    }
  }
  public void StrictlyLessThanInt([StrictlyLessThan(42)] int x)
  {
    if (x > 41)
    {
      throw new ArgumentOutOfRangeException("x", "The 'x' parameter must be strictly less than 42.");
    }
  }
  public void StrictRangeInt([StrictRange(10.0, 20.0)] int x)
  {
    if (x < 11 || x > 19)
    {
      throw new ArgumentOutOfRangeException("x", "The 'x' parameter must be strictly between 10 and 20.");
    }
  }
  public void LessThanDouble([LessThan(42.0)] int x)
  {
    if (x > 42)
    {
      throw new ArgumentOutOfRangeException("x", "The 'x' parameter must be less than 42.");
    }
  }
  public void GreaterThanDouble([GreaterThan(42.0)] int x)
  {
    if (x < 42)
    {
      throw new ArgumentOutOfRangeException("x", "The 'x' parameter must be greater than 42.");
    }
  }
  public void RangeDouble([Range(10.0, 20.0)] int x)
  {
    if (x < 10 || x > 20)
    {
      throw new ArgumentOutOfRangeException("x", "The 'x' parameter must be between 10 and 20.");
    }
  }
  public void StrictlyGreaterThanDouble([StrictlyGreaterThan(42.0)] int x)
  {
    if (x < 43)
    {
      throw new ArgumentOutOfRangeException("x", "The 'x' parameter must be strictly greater than 42.");
    }
  }
  public void StrictlyLessThanDouble([StrictlyLessThan(42.0)] int x)
  {
    if (x > 41)
    {
      throw new ArgumentOutOfRangeException("x", "The 'x' parameter must be strictly less than 42.");
    }
  }
  public void StrictRangeDouble([StrictRange(10.0, 20.0)] int x)
  {
    if (x < 11 || x > 19)
    {
      throw new ArgumentOutOfRangeException("x", "The 'x' parameter must be strictly between 10 and 20.");
    }
  }
  public void LessThanUnsigned([LessThan(42ul)] int x)
  {
    if (x > 42)
    {
      throw new ArgumentOutOfRangeException("x", "The 'x' parameter must be less than 42.");
    }
  }
  public void GreaterThanUnsigned([GreaterThan(42ul)] int x)
  {
    if (x < 42)
    {
      throw new ArgumentOutOfRangeException("x", "The 'x' parameter must be greater than 42.");
    }
  }
  public void RangeUnsigned([Range(10ul, 20ul)] int x)
  {
    if (x < 10 || x > 20)
    {
      throw new ArgumentOutOfRangeException("x", "The 'x' parameter must be between 10 and 20.");
    }
  }
  public void StrictlyGreaterThanUnsigned([StrictlyGreaterThan(42ul)] int x)
  {
    if (x < 43)
    {
      throw new ArgumentOutOfRangeException("x", "The 'x' parameter must be strictly greater than 42.");
    }
  }
  public void StrictlyLessThanUnsigned([StrictlyLessThan(42ul)] int x)
  {
    if (x > 41)
    {
      throw new ArgumentOutOfRangeException("x", "The 'x' parameter must be strictly less than 42.");
    }
  }
  public void StrictRangeUnsigned([StrictRange(10ul, 20ul)] int x)
  {
    if (x < 11 || x > 19)
    {
      throw new ArgumentOutOfRangeException("x", "The 'x' parameter must be strictly between 10 and 20.");
    }
  }
}