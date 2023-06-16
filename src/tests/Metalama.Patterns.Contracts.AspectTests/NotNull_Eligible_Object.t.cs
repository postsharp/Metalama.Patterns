namespace Metalama.Patterns.Contracts.AspectTests;
public class NotNull_Eligible_Object
{
    private global::System.Object _field1 = default!;
    [global::Metalama.Patterns.Contracts.NotNullAttribute]
    private global::System.Object field
    {
        get
        {
            return this._field1;
        }
        set
        {
            if ( value == null! )
            {
                throw global::Metalama.Patterns.Contracts.ContractsServices.Default.ExceptionFactory.CreateException( global::Metalama.Patterns.Contracts.ContractExceptionInfo.Create( typeof( global::System.ArgumentNullException ), typeof( global::Metalama.Patterns.Contracts.NotNullAttribute ), value, "field", global::Metalama.Patterns.Contracts.ContractTargetKind.Property, global::Metalama.Framework.Aspects.ContractDirection.Input, global::Metalama.Patterns.Contracts.ContractLocalizedTextProvider.NotNullErrorMessage ) );
            }
            this._field1 = value;
        }
    }
}