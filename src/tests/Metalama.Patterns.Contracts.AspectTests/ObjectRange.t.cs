using System;
namespace Metalama.Patterns.Contracts.AspectTests.ObjectRange;
internal class C
{
  [return: StrictlyPositive]
  public object M([PositiveOrZero] object a, [Range(0, 100)] object b, [LessThanOrEqualTo(101, decimalPlaces: 2)] out object c)
  {
    if (global::Metalama.Patterns.Contracts.Numeric.NumberComparer.IsStrictlySmallerThan(a, 0) == true)
    {
      throw new ArgumentOutOfRangeException("a", "The 'a' parameter must be greater than or equal to 0.");
    }
    if (global::Metalama.Patterns.Contracts.Numeric.NumberComparer.IsStrictlySmallerThan(b, 0) == true || global::Metalama.Patterns.Contracts.Numeric.NumberComparer.IsStrictlyGreaterThan(b, 100) == true)
    {
      throw new ArgumentOutOfRangeException("The 'b' parameter must be in the range [0, 100].", "b");
    }
    object returnValue;
    c = a;
    returnValue = b;
    if (global::Metalama.Patterns.Contracts.Numeric.NumberComparer.IsStrictlyGreaterThan(c, 1.01M) == true)
    {
      throw new PostconditionViolationException("The 'c' parameter must be less than or equal to 1.01.");
    }
    if (global::Metalama.Patterns.Contracts.Numeric.NumberComparer.IsGreaterThan(returnValue, 0) == true)
    {
      throw new PostconditionViolationException("The return value must be strictly greater than 0.");
    }
    return returnValue;
  }
}