// Copyright (c) SharpCrafters s.r.o. This file is not open source. It is released under a commercial
// source-available license. Please see the LICENSE.md file in the repository root for details.
namespace Flashtrace.Custom.Messages
{
    internal static class MessageHelper
    {
        public static void Write<T>( in T message, ICustomLogRecordBuilder recordBuilder, CustomLogRecordItem item ) 
            where T : IMessage
        {
            // TODO: Post.GetMutableRef
#if false
            ref T mutableRef = ref Post.GetMutableRef(message);
            mutableRef.Write(recordBuilder, item);
#else
            message.Write( recordBuilder, item );
#endif
        }
    }
}
