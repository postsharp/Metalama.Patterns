namespace Metalama.Patterns.Contracts.AspectTests;
public class NotNull_Eligible_GenericUnconstrained
{
    public void Method<T>( [NotNull] T x )
    {
        if ( x == null! )
        {
            throw global::Metalama.Patterns.Contracts.ContractsServices.Default.ExceptionFactory.CreateException( global::Metalama.Patterns.Contracts.ContractExceptionInfo.Create( typeof( global::System.ArgumentNullException ), typeof( global::Metalama.Patterns.Contracts.NotNullAttribute ), x, "x", global::Metalama.Patterns.Contracts.ContractTargetKind.Parameter, global::Metalama.Framework.Aspects.ContractDirection.Input, global::Metalama.Patterns.Contracts.ContractLocalizedTextProvider.NotNullErrorMessage ) );
        }
    }
}