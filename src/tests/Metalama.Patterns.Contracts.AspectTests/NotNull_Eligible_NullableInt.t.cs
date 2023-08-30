namespace Metalama.Patterns.Contracts.AspectTests;
public class NotNull_Eligible_NullableInt
{
  private global::System.Int32? _field1;
  [global::Metalama.Patterns.Contracts.NotNullAttribute]
  private global::System.Int32? field
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