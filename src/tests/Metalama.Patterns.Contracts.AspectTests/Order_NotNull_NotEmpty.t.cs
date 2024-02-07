public class TestClass
{
  public TestClass([NotNull][NotEmpty] IReadOnlyCollection<int> list)
  {
    if (list == null !)
    {
      throw new ArgumentNullException("list", "The 'list' parameter must not be null.");
    }
    if (list.Count <= 0)
    {
      throw new ArgumentException("The 'list' parameter must not be null or empty.", "list");
    }
  }
  [return: NotNull]
  [return: NotEmpty]
  public IReadOnlyCollection<int> Foo([NotNull][NotEmpty] IReadOnlyCollection<int> list)
  {
    IReadOnlyCollection<int> returnValue_1;
    if (list == null !)
    {
      throw new ArgumentNullException("list", "The 'list' parameter must not be null.");
    }
    IReadOnlyCollection<int> returnValue;
    if (list.Count <= 0)
    {
      throw new ArgumentException("The 'list' parameter must not be null or empty.", "list");
    }
    returnValue = list;
    if (returnValue.Count <= 0)
    {
      throw new PostconditionViolationException("The return value must not be null or empty.");
    }
    returnValue_1 = returnValue;
    if (returnValue_1 == null !)
    {
      throw new PostconditionViolationException("The return value must not be null.");
    }
    return returnValue_1;
  }
  private IReadOnlyCollection<int> _property = default !;
  [NotNull]
  [NotEmpty]
  public IReadOnlyCollection<int> Property
  {
    get
    {
      return this._property;
    }
    set
    {
      if (value == null !)
      {
        throw new ArgumentNullException("value", "The 'Property' property must not be null.");
      }
      if (value.Count <= 0)
      {
        throw new ArgumentException("The 'Property' property must not be null or empty.", "value");
      }
      this._property = value;
    }
  }
}