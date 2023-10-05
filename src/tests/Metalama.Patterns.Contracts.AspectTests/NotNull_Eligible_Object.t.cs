using System;
namespace Metalama.Patterns.Contracts.AspectTests;
public class NotNull_Eligible_Object
{
  private object _field1 = default !;
  [NotNull]
  private object field
  {
    get
    {
      return this._field1;
    }
    set
    {
      if (value == null !)
      {
        throw new ArgumentNullException("value", "The 'field' property must not be null.");
      }
      this._field1 = value;
    }
  }
}