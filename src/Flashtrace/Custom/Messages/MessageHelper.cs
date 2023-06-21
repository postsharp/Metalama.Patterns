// Copyright (c) SharpCrafters s.r.o. This file is not open source. It is released under a commercial
// source-available license. Please see the LICENSE.md file in the repository root for details.
namespace PostSharp.Patterns.Diagnostics.Custom.Messages
{
    internal static class MessageHelper
    {
        public static void Write<T>( in T message, ICustomLogRecordBuilder recordBuilder, CustomLogRecordItem item ) 
            where T : IMessage
        {
            ref T mutableRef = ref Post.GetMutableRef(message);
            mutableRef.Write(recordBuilder, item);
        }
    }
}
