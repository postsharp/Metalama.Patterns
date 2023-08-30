namespace Metalama.Patterns.Contracts.AspectTests;
public class NotNull_Eligible_Object
{
  private global::System.Object _field1 = default !;
  [global::Metalama.Patterns.Contracts.NotNullAttribute]
  private global::System.Object field
  {
    get
    {
      return this._field1;
    }
    set
    {
      if (value == null !)
      {
        throw new global::System.ArgumentNullException("value", "The 'field' property must not be null.");
      }
      this._field1 = value;
    }
  }
}