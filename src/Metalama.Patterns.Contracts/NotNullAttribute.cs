using Metalama.Framework.Aspects;
using Metalama.Framework.Code;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Metalama.Patterns.Contracts
{
    /// <summary>
    /// Custom attribute that, when added to a field, property or parameter, throws
    /// an <see cref="ArgumentNullException"/> if the target is assigned a null value.
    /// </summary>
    /// <remarks>
    /// <para>Error message is identified by <see cref="ContractLocalizedTextProvider.NotNullErrorMessage"/>.</para>
    /// </remarks>
    public sealed class NotNullAttribute : ContractAspect
    {
        public override void Validate(dynamic? value)
        {
            meta.Target.Declaration.DeclarationKind
            if (value == null!)
            {
                throw new ArgumentNullException(nameof(value));
            }
            if ( value != null )
                return null;

            return this.CreateException( typeof( ArgumentNullException ), value, locationName, locationKind, context,
                                         ContractLocalizedTextProvider.NotNullErrorMessage );
        }

        private protected Exception CreateException( Type exceptionType, object value, string locationName, ContractTargetKind targetKind,
                                             ContractDirection direction, string messageId, params object[] additionalArguments )
        {
            if ( direction == ContractDirection.Output )
            {
                exceptionType = typeof( PostconditionFailedException );
            }

            ContractExceptionInfo exceptionInfo = new ContractExceptionInfo( exceptionType, this, value, locationName, locationKind,
                                                                             context, messageId, additionalArguments );
            return ContractServices.ExceptionFactory.CreateException( exceptionInfo );
        }

    }
}
