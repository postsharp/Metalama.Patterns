namespace Metalama.Patterns.Contracts.AspectTests;
public sealed class RangeOnlyTestsRequiredBounds
{
  public void LessThan([LessThan(42)] int x)
  {
    if (x > 42)
    {
      throw new global::System.ArgumentOutOfRangeException("x", "The 'x' parameter must be less than 42.");
    }
  }
  public void GreaterThan([GreaterThan(42)] int x)
  {
    if (x < 42)
    {
      throw new global::System.ArgumentOutOfRangeException("x", "The 'x' parameter must be greater than 42.");
    }
  }
  public void Negative([Negative] int x)
  {
    if (x > 0)
    {
      throw new global::System.ArgumentOutOfRangeException("x", "The 'x' parameter must be less than 0.");
    }
  }
  public void Positive([Positive] int x)
  {
    if (x < 0)
    {
      throw new global::System.ArgumentOutOfRangeException("x", "The 'x' parameter must be greater than 0.");
    }
  }
  public void Range([Range(10, 20)] int x)
  {
    if (x < 10 || x > 20)
    {
      throw new global::System.ArgumentOutOfRangeException("x", "The 'x' parameter must be between 10 and 20.");
    }
  }
  public void StrictlyGreaterThan([StrictlyGreaterThan(42)] int x)
  {
    if (x < 43)
    {
      throw new global::System.ArgumentOutOfRangeException("x", "The 'x' parameter must be strictly greater than 42.");
    }
  }
  public void StrictlyLessThan([StrictlyLessThan(42)] int x)
  {
    if (x > 41)
    {
      throw new global::System.ArgumentOutOfRangeException("x", "The 'x' parameter must be strictly less than 42.");
    }
  }
  public void StrictlyNegative([StrictlyNegative] int x)
  {
    if (x > -1)
    {
      throw new global::System.ArgumentOutOfRangeException("x", "The 'x' parameter must be strictly less than 0.");
    }
  }
  public void StrictlyPositive([StrictlyPositive] int x)
  {
    if (x < 1)
    {
      throw new global::System.ArgumentOutOfRangeException("x", "The 'x' parameter must be strictly greater than 0.");
    }
  }
  public void StrictRange([StrictRange(10, 20)] int x)
  {
    if (x < 11 || x > 19)
    {
      throw new global::System.ArgumentOutOfRangeException("x", "The 'x' parameter must be strictly between 10 and 20.");
    }
  }
}