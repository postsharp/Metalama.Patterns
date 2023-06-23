// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

namespace Flashtrace.Messages;

internal static class MessageHelper
{
    public static void Write<T>( in T message, ILogRecordBuilder recordBuilder, LogRecordItem item )
        where T : IMessage
    {
        // TODO: [FT-Review] Use of Post.GetMutableRef.
#if false
            ref T mutableRef = ref Post.GetMutableRef(message);
            mutableRef.Write(recordBuilder, item);
#else
        message.Write( recordBuilder, item );
#endif
    }
}