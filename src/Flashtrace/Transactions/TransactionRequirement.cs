// Copyright (c) SharpCrafters s.r.o. This file is not open source. It is released under a commercial
// source-available license. Please see the LICENSE.md file in the repository root for details.

namespace PostSharp.Patterns.Diagnostics.Transactions
{

#pragma warning disable CA1815 // Override equals and operator equals on value types

    /// <summary>
    /// Represents the decision whether, why and how a transaction must be opened for an activity.
    /// </summary>
    public readonly struct TransactionRequirement
    {
        private const short isSampledFlag = 1;
        private readonly short flags;

        
        internal short State { [ExplicitCrossPackageInternal] get; }

        [ExplicitCrossPackageInternal]
        internal TransactionRequirement(short state, bool isSampled)
        {
            this.flags = (isSampled && state != 0 )? isSampledFlag : (short) 0;
            this.State = state;
        }

        /// <summary>
        /// Determines whether the transaction must be opened based on a sampling policy.
        /// </summary>
        public bool IsSampled => (this.flags & isSampledFlag) == isSampledFlag;

        /// <summary>
        /// Determines whether the activity requires a transaction to be opened.
        /// </summary>
        public bool RequiresTransaction => this.State != 0;

        /// <summary>
        /// Represents the request to open a transaction based on a sampling policy.
        /// </summary>
        public static TransactionRequirement SampledTransaction => new TransactionRequirement(-1, true);

        /// <summary>
        /// Represents the request to open a transaction based on a deterministic policy (without sampling).
        /// </summary>
        public static TransactionRequirement DeterministicTransaction => new TransactionRequirement(-1, false);

        /// <summary>
        /// Represents the absence of request to open a transaction.
        /// </summary>
        public static TransactionRequirement NoTransaction => new TransactionRequirement(0, false);

        /// <summary>
        /// Returns a copy of the current <see cref="TransactionRequirement"/> but with a different value of <see cref="RequiresTransaction"/>.
        /// Specifically, this method preserves the <see cref="IsSampled"/> property if the <paramref name="value"/> parameter is <c>true</c>.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public TransactionRequirement WithRequiresTransaction( bool value ) => new TransactionRequirement( value ? this.State : (short) 0, this.IsSampled );

        /// <inheritdoc/>
        public override string ToString()
        {
            return $"RequiresTransaction={this.RequiresTransaction}, IsSampled={this.IsSampled}";
        }

    }


}