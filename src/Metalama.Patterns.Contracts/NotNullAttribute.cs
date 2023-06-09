// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Aspects;

namespace Metalama.Patterns.Contracts
{
    /// <summary>
    /// Custom attribute that, when added to a field, property or parameter, throws
    /// an <see cref="ArgumentNullException"/> if the target is assigned a <see langword="null"/> value.
    /// </summary>
    /// <remarks>
    /// <para>Error message is identified by <see cref="ContractLocalizedTextProvider.NotNullErrorMessage"/>.</para>
    /// </remarks>
    public sealed class NotNullAttribute : ContractAspect
    {
        public override void Validate( dynamic? value )
        {
            CompileTimeHelpers.GetTargetKindAndName( meta.Target, out var targetKind, out var targetName );

            if ( value == null! )
            {
                var exceptionInfo = ContractExceptionInfo.Create(
                    typeof( ArgumentNullException ),
                    typeof( NotNullAttribute ),
                    value,
                    targetName,
                    targetKind,
                    meta.Target.ContractDirection,
                    ContractLocalizedTextProvider.NotNullErrorMessage );

                throw ContractServices.ExceptionFactory.CreateException( exceptionInfo );
            }
        }
    }
}