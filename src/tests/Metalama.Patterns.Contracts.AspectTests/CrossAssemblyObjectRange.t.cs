using System;
namespace Metalama.Patterns.Contracts.AspectTests.CrossAssemblyObjectRange;
internal class C : IValidated
{
  public object M(object a, object b, out object c)
  {
    object returnValue_1;
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
    if (global::Metalama.Patterns.Contracts.Numeric.NumberComparer.IsGreaterThan(returnValue, 0) == true)
    {
      throw new PostconditionViolationException("The return value must be strictly greater than 0.");
    }
    returnValue_1 = returnValue;
    if (global::Metalama.Patterns.Contracts.Numeric.NumberComparer.IsStrictlyGreaterThan(c, 100) == true)
    {
      throw new PostconditionViolationException("The 'c' parameter must be less than or equal to 100.");
    }
    return returnValue_1;
  }
}