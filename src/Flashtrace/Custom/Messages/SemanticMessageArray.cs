// Copyright (c) SharpCrafters s.r.o. This file is not open source. It is released under a commercial
// source-available license. Please see the LICENSE.md file in the repository root for details.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using PostSharp.Patterns.Diagnostics.Custom;

namespace PostSharp.Patterns.Diagnostics.Custom.Messages
{
#if VALUE_TUPLE
    /// <summary>
    /// Encapsulates a semantic message with an arbitrary number of parameters. Use the <see cref="SemanticMessageBuilder"/> class to create an instance of this type.
    /// </summary>
    [SuppressMessage("Microsoft.Performance","CA1815", Justification = "Equal is not a use case")]
    public readonly struct SemanticMessageArray : IMessage
    {
        private readonly string messageName;
        private readonly IReadOnlyList<ValueTuple<string, object>> parameters;

#if AGGRESSIVE_INLINING
        [MethodImpl(MethodImplOptions.AggressiveInlining)] // To avoid copying the struct.
#endif
        internal SemanticMessageArray( string messageName, IReadOnlyList<(string, object)> parameters )
        {
            this.messageName = messageName;
            this.parameters = parameters;
        }
        
          void IMessage.Write( ICustomLogRecordBuilder builder, CustomLogRecordItem item)
        {
            builder.BeginWriteItem(item, new CustomLogRecordTextOptions(this.parameters.Count, this.messageName));
            
            for ( int i = 0 ; i < this.parameters.Count ; i++ )
            {
                (string name, object value) = this.parameters[i];

                builder.WriteCustomParameter( i, name, value, CustomLogParameterOptions.SemanticParameter );
            }

            builder.EndWriteItem(item);
        }


        /// <inheritdoc/>
        public override string ToString() => DebugMessageFormatter.Format( this );

    }
#endif
}
