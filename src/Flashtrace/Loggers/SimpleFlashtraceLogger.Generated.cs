﻿// THIS FILE IS T4-GENERATED.
// To edit, go to LegacySourceLogger.Generated.tt.
// To transform, run RunT4.ps1.

 
    

using Flashtrace.Contexts;
using Flashtrace.Records;
using Microsoft.Extensions.Logging;

namespace Flashtrace.Loggers
{
	partial class SimpleFlashtraceLogger
	{

	    void IFlashtraceLogger.Write<T1>( ILoggingContext context, FlashtraceLevel level, LogRecordKind logRecordKind, string text, T1 arg1, Exception exception,  in CallerInfo callerInfo )
        {
			if ( this.IsEnabled( level ) )
			{
              this.WriteFormatted( context, level, logRecordKind, text, new object[] {  arg1 }, exception );
			}
        }

		
	    void IFlashtraceLogger.Write<T1, T2>( ILoggingContext context, FlashtraceLevel level, LogRecordKind logRecordKind, string text, T1 arg1, T2 arg2, Exception exception,  in CallerInfo callerInfo )
        {
			if ( this.IsEnabled( level ) )
			{
              this.WriteFormatted( context, level, logRecordKind, text, new object[] {  arg1, arg2 }, exception );
			}
        }

		
	    void IFlashtraceLogger.Write<T1, T2, T3>( ILoggingContext context, FlashtraceLevel level, LogRecordKind logRecordKind, string text, T1 arg1, T2 arg2, T3 arg3, Exception exception,  in CallerInfo callerInfo )
        {
			if ( this.IsEnabled( level ) )
			{
              this.WriteFormatted( context, level, logRecordKind, text, new object[] {  arg1, arg2, arg3 }, exception );
			}
        }

		
	    void IFlashtraceLogger.Write<T1, T2, T3, T4>( ILoggingContext context, FlashtraceLevel level, LogRecordKind logRecordKind, string text, T1 arg1, T2 arg2, T3 arg3, T4 arg4, Exception exception,  in CallerInfo callerInfo )
        {
			if ( this.IsEnabled( level ) )
			{
              this.WriteFormatted( context, level, logRecordKind, text, new object[] {  arg1, arg2, arg3, arg4 }, exception );
			}
        }

		
	    void IFlashtraceLogger.Write<T1, T2, T3, T4, T5>( ILoggingContext context, FlashtraceLevel level, LogRecordKind logRecordKind, string text, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, Exception exception,  in CallerInfo callerInfo )
        {
			if ( this.IsEnabled( level ) )
			{
              this.WriteFormatted( context, level, logRecordKind, text, new object[] {  arg1, arg2, arg3, arg4, arg5 }, exception );
			}
        }

		
	    void IFlashtraceLogger.Write<T1, T2, T3, T4, T5, T6>( ILoggingContext context, FlashtraceLevel level, LogRecordKind logRecordKind, string text, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, Exception exception,  in CallerInfo callerInfo )
        {
			if ( this.IsEnabled( level ) )
			{
              this.WriteFormatted( context, level, logRecordKind, text, new object[] {  arg1, arg2, arg3, arg4, arg5, arg6 }, exception );
			}
        }

		
	    void IFlashtraceLogger.Write<T1, T2, T3, T4, T5, T6, T7>( ILoggingContext context, FlashtraceLevel level, LogRecordKind logRecordKind, string text, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, Exception exception,  in CallerInfo callerInfo )
        {
			if ( this.IsEnabled( level ) )
			{
              this.WriteFormatted( context, level, logRecordKind, text, new object[] {  arg1, arg2, arg3, arg4, arg5, arg6, arg7 }, exception );
			}
        }

		
	    void IFlashtraceLogger.Write<T1, T2, T3, T4, T5, T6, T7, T8>( ILoggingContext context, FlashtraceLevel level, LogRecordKind logRecordKind, string text, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, Exception exception,  in CallerInfo callerInfo )
        {
			if ( this.IsEnabled( level ) )
			{
              this.WriteFormatted( context, level, logRecordKind, text, new object[] {  arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8 }, exception );
			}
        }

		
	    void IFlashtraceLogger.Write<T1, T2, T3, T4, T5, T6, T7, T8, T9>( ILoggingContext context, FlashtraceLevel level, LogRecordKind logRecordKind, string text, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, Exception exception,  in CallerInfo callerInfo )
        {
			if ( this.IsEnabled( level ) )
			{
              this.WriteFormatted( context, level, logRecordKind, text, new object[] {  arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9 }, exception );
			}
        }

		
	    void IFlashtraceLogger.Write<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>( ILoggingContext context, FlashtraceLevel level, LogRecordKind logRecordKind, string text, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, Exception exception,  in CallerInfo callerInfo )
        {
			if ( this.IsEnabled( level ) )
			{
              this.WriteFormatted( context, level, logRecordKind, text, new object[] {  arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10 }, exception );
			}
        }

			}    
}