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
            if ( value == null! )
            {
                throw global::Metalama.Patterns.Contracts.ContractsServices.Default.ExceptionFactory.CreateException( global::Metalama.Patterns.Contracts.ContractExceptionInfo.Create( typeof( global::System.ArgumentNullException ), typeof( global::Metalama.Patterns.Contracts.NotNullAttribute ), value, "field", global::Metalama.Patterns.Contracts.ContractTargetKind.Property, global::Metalama.Framework.Aspects.ContractDirection.Input, global::Metalama.Patterns.Contracts.ContractLocalizedTextProvider.NotNullErrorMessage ) );
            }
            this._field1 = value;
        }
    }
}