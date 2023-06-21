namespace Flashtrace.Correlation
{

#pragma warning disable CA1815 // Override equals and operator equals on value types


    /// <summary>
    /// Logging options sent by the caller in a distributed logging transaction.
    /// </summary>
    public readonly struct IncomingRequestOptions
    {
        private readonly byte isParentSampled;

        /// <summary>
        /// Initializes a new <see cref="IncomingRequestOptions"/>.
        /// </summary>
        /// <param name="isParentSampled">Determines whether the parent request was logged as a result of sampling,
        /// i.e. it was logged by a policy that had a sampling clause. It corresponds
        /// to the <c>sampled</c> flag of the W3C Trace Context specification
        /// (https://www.w3.org/TR/trace-context/#sampled-flag). 
        /// </param>

        public IncomingRequestOptions( bool isParentSampled )
        {
            this.isParentSampled = isParentSampled ? (byte) 1 : (byte) 0;
        }

        /// <summary>
        /// Determines whether the parent request was logged as a result of sampling,
        /// i.e. it was logged by a policy that had a sampling clause. It corresponds
        /// to the <c>sampled</c> flag of the W3C Trace Context specification
        /// (https://www.w3.org/TR/trace-context/#sampled-flag). 
        /// </summary>
        public bool IsParentSampled => this.isParentSampled != 0;
    }
}
