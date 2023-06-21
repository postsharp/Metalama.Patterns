// Copyright (c) SharpCrafters s.r.o. This file is not open source. It is released under a commercial
// source-available license. Please see the LICENSE.md file in the repository root for details.

using Flashtrace.Custom.Messages;

namespace Flashtrace
{
        /// <summary>
    /// Creates semantic messages composed of a message name and a list of properties given as name-value pairs. These messages are ideal for machine analysis.
    /// For more succinct code, consider including the <c>using static PostSharp.Patterns.Diagnostics.MessageBuilder</c> statement.
    /// </summary>
    public static partial class SemanticMessageBuilder
    { 

        /// <summary>
        /// Creates a semantic message with an arbitrary number of parameters.
        /// </summary>
        /// <param name="messageName">Message name.</param>
        /// <param name="parameters">Array of parameters (name-value pairs).</param>
        /// <returns></returns>
#if VALUE_TUPLE
#if AGGRESSIVE_INLINING
    [MethodImpl(MethodImplOptions.AggressiveInlining)] // To avoid copying the struct.
#endif
        // Intentionally removing the params modifier because otherwise the C# compiler picks the wrong overload which causes a performance issue.
        public static SemanticMessageArray Semantic(string messageName, ValueTuple<string, object>[] parameters) => new SemanticMessageArray(messageName, parameters);

        /// <summary>
        /// Creates a semantic message with an arbitrary number of parameters.
        /// </summary>
        /// <param name="messageName">Message name.</param>
        /// <param name="parameters">Array of parameters (name-value pairs).</param>
        /// <returns></returns>
#if AGGRESSIVE_INLINING
        [MethodImpl(MethodImplOptions.AggressiveInlining)] // To avoid copying the struct.
#endif
        public static SemanticMessageArray Semantic(string messageName, IReadOnlyList<ValueTuple<string, object>> parameters) => new SemanticMessageArray(messageName, parameters);
#endif

        /// <summary>
        /// Creates a semantic message without parameter.
        /// </summary>
        /// <param name="messageName">Message name.</param>
        /// <returns></returns>
#if AGGRESSIVE_INLINING
        [MethodImpl(MethodImplOptions.AggressiveInlining)] // To avoid copying the struct.
#endif
        public static SemanticMessage Semantic(string messageName) => new SemanticMessage(messageName);

    }

}