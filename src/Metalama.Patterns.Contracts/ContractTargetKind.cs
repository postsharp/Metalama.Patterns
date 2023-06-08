// Copyright (c) SharpCrafters s.r.o. This file is not open source. It is released under a commercial
// source-available license. Please see the LICENSE.md file in the repository root for details.

using Metalama.Framework.Aspects;

namespace Metalama.Patterns.Contracts
{
    // TODO: Review. Maybe this should live alongside ContractAspect.

    /// <summary>
    /// Specifies the application elements to which a <see cref="ContractAspect"/> can be applied.
    /// </summary>
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