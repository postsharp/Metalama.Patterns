// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Aspects;

namespace Metalama.Patterns.Contracts
{
    // TODO: Review. This replaces PS LocationKind. Maybe this should live alongside ContractAspect.
    // The term "location" does not seem to be used with ML, hence the name change. 

    /// <summary>
    /// Specifies the application elements to which a <see cref="ContractAspect"/> can be applied.
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