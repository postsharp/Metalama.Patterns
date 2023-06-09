// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Aspects;
using Metalama.Framework.Code;

namespace Metalama.Patterns.Contracts
{
    [CompileTime]
    internal static class CompileTimeHelpers
    {
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