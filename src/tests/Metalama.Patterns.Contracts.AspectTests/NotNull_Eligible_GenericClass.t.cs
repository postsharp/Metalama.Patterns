namespace Metalama.Patterns.Contracts.AspectTests;
public class NotNull_Eligible_GenericClass
{
  public void Method<T>([NotNull] T x)
    where T : class
  {
    if (x == null !)
    {
      throw new global::System.ArgumentNullException("x", "The 'x' parameter must not be null.");
    }
  }
}