using Metalama.Framework.Aspects;
using Metalama.Framework.Code;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Metalama.Patterns.Contracts
{
    [CompileTime]
    internal static class CompileTimeHelpers
    {
        // TODO: Review. Consider building this logic in to ContractAspect, perhaps store result as fields in ctor.
        public static void GetTargetKindAndName( IMetaTarget target, out ContractTargetKind kind, out string? name)
        {
            switch (target.Declaration.DeclarationKind)
            {
                case DeclarationKind.Parameter:
                    if ( target.Parameter.IsReturnParameter )
                    {
                        kind = ContractTargetKind.ReturnValue;
                        name = null;
                    }
                    else
                    {
                        kind = ContractTargetKind.Parameter;
                        name = target.Parameter.Name;
                    }

                    break;

                case DeclarationKind.Property:
                    kind = ContractTargetKind.Property;
                    name = target.Property.Name;
                    break;

                case DeclarationKind.Field:
                    kind = ContractTargetKind.Field;
                    name = target.Field.Name;
                    break;

                default:
                    throw new ArgumentOutOfRangeException( nameof( target ) + "." + nameof( target.Declaration ) + "." + nameof( target.Declaration.DeclarationKind ) );
            }
        }
    }
}
