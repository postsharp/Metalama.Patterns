// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Aspects;

namespace Metalama.Patterns.Contracts
{
    /// <summary>
    /// Specifies the kinds of declaration to which a <see cref="ContractAspect"/> can be applied.
    /// </summary>
    [RunTimeOrCompileTime]
    public enum ContractTargetKind
    {
        /// <summary>
        /// The contract is applied to a field.
        /// </summary>
        Field,

        /// <summary>
        /// The contract is applied to a property.
        /// </summary>
        Property,

        /// <summary>
        /// The contract is applied to a parameter.
        /// </summary>
        Parameter,

        /// <summary>
        /// The contract is applied to a return value.
        /// </summary>
        ReturnValue
    }
}