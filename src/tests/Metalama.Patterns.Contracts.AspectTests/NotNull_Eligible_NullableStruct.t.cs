// Warning LAMA5002 on `field`: `The [NotNull] contract has been applied to 'NotNull_Eligible_NullableStruct.field', but its type is nullable.`
using System;
namespace Metalama.Patterns.Contracts.AspectTests;
public class NotNull_Eligible_NullableStruct
{
  private global::System.DateTime? _field1;
  [global::Metalama.Patterns.Contracts.NotNullAttribute]
  private global::System.DateTime? field
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