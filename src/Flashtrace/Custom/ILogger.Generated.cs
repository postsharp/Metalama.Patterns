// Copyright (c) SharpCrafters s.r.o. This file is not open source. It is released under a commercial
// source-available license. Please see the LICENSE.md file in the repository root for details.
// THIS FILE IS T4-GENERATED.
// To edit, go to GenericLogger.tt.
// To transform, run this: "C:\Program Files (x86)\Common Files\Microsoft Shared\TextTemplating\14.0\TextTransform.exe" ILogger.Generated.tt
// The transformation is not automatic because we are in a shared project.



using System;
using System.ComponentModel;
using PostSharp.Patterns.Diagnostics;
using PostSharp.Patterns.Diagnostics.Contexts;


namespace PostSharp.Patterns.Diagnostics.Custom
{
	public partial interface ILogger
	{
	
		/// <summary>
        /// Writes a custom log record with 1 parameter.
        /// </summary>
		/// <param name="context">The context for which the message has to be written.</param>
        /// <param name="level">The severity of the record (e.g. <see cref="LogLevel.Info"/> or <see cref="LogLevel.Warning"/>).</param>
        /// <param name="text">The text of the log record, including parameters (e.g. <c>Opening {Path} file {ShareMode} sharing mode</c>).</param>
        /// <param name="recordKind">Kind of record.</param>
        /// <param name="exception">The <see cref="Exception"/> associated with the record, or <c>null</c>.</param>
        /// <param name="recordInfo">Information about the caller souICustomActivityLoggingContextrce code.</param>
				/// <typeparam name="T1">Type of the 1-th parameter.</typeparam>
		/// <param name="arg1">Value of the 1-th parameter.</param>
		   void Write<T1>( ILoggingContext context, LogLevel level, LogRecordKind recordKind, string text, T1 arg1, Exception exception, ref CallerInfo recordInfo  );


	
		/// <summary>
        /// Writes a custom log record with 2 parameters.
        /// </summary>
		/// <param name="context">The context for which the message has to be written.</param>
        /// <param name="level">The severity of the record (e.g. <see cref="LogLevel.Info"/> or <see cref="LogLevel.Warning"/>).</param>
        /// <param name="text">The text of the log record, including parameters (e.g. <c>Opening {Path} file {ShareMode} sharing mode</c>).</param>
        /// <param name="recordKind">Kind of record.</param>
        /// <param name="exception">The <see cref="Exception"/> associated with the record, or <c>null</c>.</param>
        /// <param name="recordInfo">Information about the caller source code.</param>
				/// <typeparam name="T1">Type of the 1-th parameter.</typeparam>
		/// <param name="arg1">Value of the 1-th parameter.</param>
			/// <typeparam name="T2">Type of the 2-th parameter.</typeparam>
		/// <param name="arg2">Value of the 2-th parameter.</param>
		   void Write<T1, T2>( ILoggingContext context, LogLevel level, LogRecordKind recordKind, string text, T1 arg1, T2 arg2, Exception exception, ref CallerInfo recordInfo  );


	
		/// <summary>
        /// Writes a custom log record with 3 parameters.
        /// </summary>
		/// <param name="context">The context for which the message has to be written.</param>
        /// <param name="level">The severity of the record (e.g. <see cref="LogLevel.Info"/> or <see cref="LogLevel.Warning"/>).</param>
        /// <param name="text">The text of the log record, including parameters (e.g. <c>Opening {Path} file {ShareMode} sharing mode</c>).</param>
        /// <param name="recordKind">Kind of record.</param>
        /// <param name="exception">The <see cref="Exception"/> associated with the record, or <c>null</c>.</param>
        /// <param name="recordInfo">Information about the caller source code.</param>
				/// <typeparam name="T1">Type of the 1-th parameter.</typeparam>
		/// <param name="arg1">Value of the 1-th parameter.</param>
			/// <typeparam name="T2">Type of the 2-th parameter.</typeparam>
		/// <param name="arg2">Value of the 2-th parameter.</param>
			/// <typeparam name="T3">Type of the 3-th parameter.</typeparam>
		/// <param name="arg3">Value of the 3-th parameter.</param>
		   void Write<T1, T2, T3>( ILoggingContext context, LogLevel level, LogRecordKind recordKind, string text, T1 arg1, T2 arg2, T3 arg3, Exception exception, ref CallerInfo recordInfo  );


	
		/// <summary>
        /// Writes a custom log record with 4 parameters.
        /// </summary>
		/// <param name="context">The context for which the message has to be written.</param>
        /// <param name="level">The severity of the record (e.g. <see cref="LogLevel.Info"/> or <see cref="LogLevel.Warning"/>).</param>
        /// <param name="text">The text of the log record, including parameters (e.g. <c>Opening {Path} file {ShareMode} sharing mode</c>).</param>
        /// <param name="recordKind">Kind of record.</param>
        /// <param name="exception">The <see cref="Exception"/> associated with the record, or <c>null</c>.</param>
        /// <param name="recordInfo">Information about the caller source code.</param>
				/// <typeparam name="T1">Type of the 1-th parameter.</typeparam>
		/// <param name="arg1">Value of the 1-th parameter.</param>
			/// <typeparam name="T2">Type of the 2-th parameter.</typeparam>
		/// <param name="arg2">Value of the 2-th parameter.</param>
			/// <typeparam name="T3">Type of the 3-th parameter.</typeparam>
		/// <param name="arg3">Value of the 3-th parameter.</param>
			/// <typeparam name="T4">Type of the 4-th parameter.</typeparam>
		/// <param name="arg4">Value of the 4-th parameter.</param>
		   void Write<T1, T2, T3, T4>( ILoggingContext context, LogLevel level, LogRecordKind recordKind, string text, T1 arg1, T2 arg2, T3 arg3, T4 arg4, Exception exception, ref CallerInfo recordInfo  );


	
		/// <summary>
        /// Writes a custom log record with 5 parameters.
        /// </summary>
		/// <param name="context">The context for which the message has to be written.</param>
        /// <param name="level">The severity of the record (e.g. <see cref="LogLevel.Info"/> or <see cref="LogLevel.Warning"/>).</param>
        /// <param name="text">The text of the log record, including parameters (e.g. <c>Opening {Path} file {ShareMode} sharing mode</c>).</param>
        /// <param name="recordKind">Kind of record.</param>
        /// <param name="exception">The <see cref="Exception"/> associated with the record, or <c>null</c>.</param>
        /// <param name="recordInfo">Information about the caller source code.</param>
				/// <typeparam name="T1">Type of the 1-th parameter.</typeparam>
		/// <param name="arg1">Value of the 1-th parameter.</param>
			/// <typeparam name="T2">Type of the 2-th parameter.</typeparam>
		/// <param name="arg2">Value of the 2-th parameter.</param>
			/// <typeparam name="T3">Type of the 3-th parameter.</typeparam>
		/// <param name="arg3">Value of the 3-th parameter.</param>
			/// <typeparam name="T4">Type of the 4-th parameter.</typeparam>
		/// <param name="arg4">Value of the 4-th parameter.</param>
			/// <typeparam name="T5">Type of the 5-th parameter.</typeparam>
		/// <param name="arg5">Value of the 5-th parameter.</param>
		   void Write<T1, T2, T3, T4, T5>( ILoggingContext context, LogLevel level, LogRecordKind recordKind, string text, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, Exception exception, ref CallerInfo recordInfo  );


	
		/// <summary>
        /// Writes a custom log record with 6 parameters.
        /// </summary>
		/// <param name="context">The context for which the message has to be written.</param>
        /// <param name="level">The severity of the record (e.g. <see cref="LogLevel.Info"/> or <see cref="LogLevel.Warning"/>).</param>
        /// <param name="text">The text of the log record, including parameters (e.g. <c>Opening {Path} file {ShareMode} sharing mode</c>).</param>
        /// <param name="recordKind">Kind of record.</param>
        /// <param name="exception">The <see cref="Exception"/> associated with the record, or <c>null</c>.</param>
        /// <param name="recordInfo">Information about the caller source code.</param>
				/// <typeparam name="T1">Type of the 1-th parameter.</typeparam>
		/// <param name="arg1">Value of the 1-th parameter.</param>
			/// <typeparam name="T2">Type of the 2-th parameter.</typeparam>
		/// <param name="arg2">Value of the 2-th parameter.</param>
			/// <typeparam name="T3">Type of the 3-th parameter.</typeparam>
		/// <param name="arg3">Value of the 3-th parameter.</param>
			/// <typeparam name="T4">Type of the 4-th parameter.</typeparam>
		/// <param name="arg4">Value of the 4-th parameter.</param>
			/// <typeparam name="T5">Type of the 5-th parameter.</typeparam>
		/// <param name="arg5">Value of the 5-th parameter.</param>
			/// <typeparam name="T6">Type of the 6-th parameter.</typeparam>
		/// <param name="arg6">Value of the 6-th parameter.</param>
		   void Write<T1, T2, T3, T4, T5, T6>( ILoggingContext context, LogLevel level, LogRecordKind recordKind, string text, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, Exception exception, ref CallerInfo recordInfo  );


	
		/// <summary>
        /// Writes a custom log record with 7 parameters.
        /// </summary>
		/// <param name="context">The context for which the message has to be written.</param>
        /// <param name="level">The severity of the record (e.g. <see cref="LogLevel.Info"/> or <see cref="LogLevel.Warning"/>).</param>
        /// <param name="text">The text of the log record, including parameters (e.g. <c>Opening {Path} file {ShareMode} sharing mode</c>).</param>
        /// <param name="recordKind">Kind of record.</param>
        /// <param name="exception">The <see cref="Exception"/> associated with the record, or <c>null</c>.</param>
        /// <param name="recordInfo">Information about the caller source code.</param>
				/// <typeparam name="T1">Type of the 1-th parameter.</typeparam>
		/// <param name="arg1">Value of the 1-th parameter.</param>
			/// <typeparam name="T2">Type of the 2-th parameter.</typeparam>
		/// <param name="arg2">Value of the 2-th parameter.</param>
			/// <typeparam name="T3">Type of the 3-th parameter.</typeparam>
		/// <param name="arg3">Value of the 3-th parameter.</param>
			/// <typeparam name="T4">Type of the 4-th parameter.</typeparam>
		/// <param name="arg4">Value of the 4-th parameter.</param>
			/// <typeparam name="T5">Type of the 5-th parameter.</typeparam>
		/// <param name="arg5">Value of the 5-th parameter.</param>
			/// <typeparam name="T6">Type of the 6-th parameter.</typeparam>
		/// <param name="arg6">Value of the 6-th parameter.</param>
			/// <typeparam name="T7">Type of the 7-th parameter.</typeparam>
		/// <param name="arg7">Value of the 7-th parameter.</param>
		   void Write<T1, T2, T3, T4, T5, T6, T7>( ILoggingContext context, LogLevel level, LogRecordKind recordKind, string text, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, Exception exception, ref CallerInfo recordInfo  );


	
		/// <summary>
        /// Writes a custom log record with 8 parameters.
        /// </summary>
		/// <param name="context">The context for which the message has to be written.</param>
        /// <param name="level">The severity of the record (e.g. <see cref="LogLevel.Info"/> or <see cref="LogLevel.Warning"/>).</param>
        /// <param name="text">The text of the log record, including parameters (e.g. <c>Opening {Path} file {ShareMode} sharing mode</c>).</param>
        /// <param name="recordKind">Kind of record.</param>
        /// <param name="exception">The <see cref="Exception"/> associated with the record, or <c>null</c>.</param>
        /// <param name="recordInfo">Information about the caller source code.</param>
				/// <typeparam name="T1">Type of the 1-th parameter.</typeparam>
		/// <param name="arg1">Value of the 1-th parameter.</param>
			/// <typeparam name="T2">Type of the 2-th parameter.</typeparam>
		/// <param name="arg2">Value of the 2-th parameter.</param>
			/// <typeparam name="T3">Type of the 3-th parameter.</typeparam>
		/// <param name="arg3">Value of the 3-th parameter.</param>
			/// <typeparam name="T4">Type of the 4-th parameter.</typeparam>
		/// <param name="arg4">Value of the 4-th parameter.</param>
			/// <typeparam name="T5">Type of the 5-th parameter.</typeparam>
		/// <param name="arg5">Value of the 5-th parameter.</param>
			/// <typeparam name="T6">Type of the 6-th parameter.</typeparam>
		/// <param name="arg6">Value of the 6-th parameter.</param>
			/// <typeparam name="T7">Type of the 7-th parameter.</typeparam>
		/// <param name="arg7">Value of the 7-th parameter.</param>
			/// <typeparam name="T8">Type of the 8-th parameter.</typeparam>
		/// <param name="arg8">Value of the 8-th parameter.</param>
		   void Write<T1, T2, T3, T4, T5, T6, T7, T8>( ILoggingContext context, LogLevel level, LogRecordKind recordKind, string text, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, Exception exception, ref CallerInfo recordInfo  );


	
		/// <summary>
        /// Writes a custom log record with 9 parameters.
        /// </summary>
		/// <param name="context">The context for which the message has to be written.</param>
        /// <param name="level">The severity of the record (e.g. <see cref="LogLevel.Info"/> or <see cref="LogLevel.Warning"/>).</param>
        /// <param name="text">The text of the log record, including parameters (e.g. <c>Opening {Path} file {ShareMode} sharing mode</c>).</param>
        /// <param name="recordKind">Kind of record.</param>
        /// <param name="exception">The <see cref="Exception"/> associated with the record, or <c>null</c>.</param>
        /// <param name="recordInfo">Information about the caller source code.</param>
				/// <typeparam name="T1">Type of the 1-th parameter.</typeparam>
		/// <param name="arg1">Value of the 1-th parameter.</param>
			/// <typeparam name="T2">Type of the 2-th parameter.</typeparam>
		/// <param name="arg2">Value of the 2-th parameter.</param>
			/// <typeparam name="T3">Type of the 3-th parameter.</typeparam>
		/// <param name="arg3">Value of the 3-th parameter.</param>
			/// <typeparam name="T4">Type of the 4-th parameter.</typeparam>
		/// <param name="arg4">Value of the 4-th parameter.</param>
			/// <typeparam name="T5">Type of the 5-th parameter.</typeparam>
		/// <param name="arg5">Value of the 5-th parameter.</param>
			/// <typeparam name="T6">Type of the 6-th parameter.</typeparam>
		/// <param name="arg6">Value of the 6-th parameter.</param>
			/// <typeparam name="T7">Type of the 7-th parameter.</typeparam>
		/// <param name="arg7">Value of the 7-th parameter.</param>
			/// <typeparam name="T8">Type of the 8-th parameter.</typeparam>
		/// <param name="arg8">Value of the 8-th parameter.</param>
			/// <typeparam name="T9">Type of the 9-th parameter.</typeparam>
		/// <param name="arg9">Value of the 9-th parameter.</param>
		   void Write<T1, T2, T3, T4, T5, T6, T7, T8, T9>( ILoggingContext context, LogLevel level, LogRecordKind recordKind, string text, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, Exception exception, ref CallerInfo recordInfo  );


	
		/// <summary>
        /// Writes a custom log record with 10 parameters.
        /// </summary>
		/// <param name="context">The context for which the message has to be written.</param>
        /// <param name="level">The severity of the record (e.g. <see cref="LogLevel.Info"/> or <see cref="LogLevel.Warning"/>).</param>
        /// <param name="text">The text of the log record, including parameters (e.g. <c>Opening {Path} file {ShareMode} sharing mode</c>).</param>
        /// <param name="recordKind">Kind of record.</param>
        /// <param name="exception">The <see cref="Exception"/> associated with the record, or <c>null</c>.</param>
        /// <param name="recordInfo">Information about the caller source code.</param>
				/// <typeparam name="T1">Type of the 1-th parameter.</typeparam>
		/// <param name="arg1">Value of the 1-th parameter.</param>
			/// <typeparam name="T2">Type of the 2-th parameter.</typeparam>
		/// <param name="arg2">Value of the 2-th parameter.</param>
			/// <typeparam name="T3">Type of the 3-th parameter.</typeparam>
		/// <param name="arg3">Value of the 3-th parameter.</param>
			/// <typeparam name="T4">Type of the 4-th parameter.</typeparam>
		/// <param name="arg4">Value of the 4-th parameter.</param>
			/// <typeparam name="T5">Type of the 5-th parameter.</typeparam>
		/// <param name="arg5">Value of the 5-th parameter.</param>
			/// <typeparam name="T6">Type of the 6-th parameter.</typeparam>
		/// <param name="arg6">Value of the 6-th parameter.</param>
			/// <typeparam name="T7">Type of the 7-th parameter.</typeparam>
		/// <param name="arg7">Value of the 7-th parameter.</param>
			/// <typeparam name="T8">Type of the 8-th parameter.</typeparam>
		/// <param name="arg8">Value of the 8-th parameter.</param>
			/// <typeparam name="T9">Type of the 9-th parameter.</typeparam>
		/// <param name="arg9">Value of the 9-th parameter.</param>
			/// <typeparam name="T10">Type of the 10-th parameter.</typeparam>
		/// <param name="arg10">Value of the 10-th parameter.</param>
		   void Write<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>( ILoggingContext context, LogLevel level, LogRecordKind recordKind, string text, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, Exception exception, ref CallerInfo recordInfo  );


		
	
	}

	
	partial class NullLogger
	{

	    void ILogger.Write<T1>( ILoggingContext context, LogLevel level, LogRecordKind logRecordKind, string text, T1 arg1, Exception exception, ref CallerInfo callerInfo )
        {
             EmitWarning( level );
        }

		
	    void ILogger.Write<T1, T2>( ILoggingContext context, LogLevel level, LogRecordKind logRecordKind, string text, T1 arg1, T2 arg2, Exception exception, ref CallerInfo callerInfo )
        {
             EmitWarning( level );
        }

		
	    void ILogger.Write<T1, T2, T3>( ILoggingContext context, LogLevel level, LogRecordKind logRecordKind, string text, T1 arg1, T2 arg2, T3 arg3, Exception exception, ref CallerInfo callerInfo )
        {
             EmitWarning( level );
        }

		
	    void ILogger.Write<T1, T2, T3, T4>( ILoggingContext context, LogLevel level, LogRecordKind logRecordKind, string text, T1 arg1, T2 arg2, T3 arg3, T4 arg4, Exception exception, ref CallerInfo callerInfo )
        {
             EmitWarning( level );
        }

		
	    void ILogger.Write<T1, T2, T3, T4, T5>( ILoggingContext context, LogLevel level, LogRecordKind logRecordKind, string text, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, Exception exception, ref CallerInfo callerInfo )
        {
             EmitWarning( level );
        }

		
	    void ILogger.Write<T1, T2, T3, T4, T5, T6>( ILoggingContext context, LogLevel level, LogRecordKind logRecordKind, string text, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, Exception exception, ref CallerInfo callerInfo )
        {
             EmitWarning( level );
        }

		
	    void ILogger.Write<T1, T2, T3, T4, T5, T6, T7>( ILoggingContext context, LogLevel level, LogRecordKind logRecordKind, string text, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, Exception exception, ref CallerInfo callerInfo )
        {
             EmitWarning( level );
        }

		
	    void ILogger.Write<T1, T2, T3, T4, T5, T6, T7, T8>( ILoggingContext context, LogLevel level, LogRecordKind logRecordKind, string text, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, Exception exception, ref CallerInfo callerInfo )
        {
             EmitWarning( level );
        }

		
	    void ILogger.Write<T1, T2, T3, T4, T5, T6, T7, T8, T9>( ILoggingContext context, LogLevel level, LogRecordKind logRecordKind, string text, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, Exception exception, ref CallerInfo callerInfo )
        {
             EmitWarning( level );
        }

		
	    void ILogger.Write<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>( ILoggingContext context, LogLevel level, LogRecordKind logRecordKind, string text, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, Exception exception, ref CallerInfo callerInfo )
        {
             EmitWarning( level );
        }

        
    }


	partial class LegacySourceLogger
	{

	    void ILogger.Write<T1>( ILoggingContext context, LogLevel level, LogRecordKind logRecordKind, string text, T1 arg1, Exception exception,  ref CallerInfo callerInfo )
        {
			if ( this.IsEnabled( level ) )
			{
              this.WriteFormatted( context, level, logRecordKind, text, new object[] {  arg1 }, exception );
			}
        }

		
	    void ILogger.Write<T1, T2>( ILoggingContext context, LogLevel level, LogRecordKind logRecordKind, string text, T1 arg1, T2 arg2, Exception exception,  ref CallerInfo callerInfo )
        {
			if ( this.IsEnabled( level ) )
			{
              this.WriteFormatted( context, level, logRecordKind, text, new object[] {  arg1, arg2 }, exception );
			}
        }

		
	    void ILogger.Write<T1, T2, T3>( ILoggingContext context, LogLevel level, LogRecordKind logRecordKind, string text, T1 arg1, T2 arg2, T3 arg3, Exception exception,  ref CallerInfo callerInfo )
        {
			if ( this.IsEnabled( level ) )
			{
              this.WriteFormatted( context, level, logRecordKind, text, new object[] {  arg1, arg2, arg3 }, exception );
			}
        }

		
	    void ILogger.Write<T1, T2, T3, T4>( ILoggingContext context, LogLevel level, LogRecordKind logRecordKind, string text, T1 arg1, T2 arg2, T3 arg3, T4 arg4, Exception exception,  ref CallerInfo callerInfo )
        {
			if ( this.IsEnabled( level ) )
			{
              this.WriteFormatted( context, level, logRecordKind, text, new object[] {  arg1, arg2, arg3, arg4 }, exception );
			}
        }

		
	    void ILogger.Write<T1, T2, T3, T4, T5>( ILoggingContext context, LogLevel level, LogRecordKind logRecordKind, string text, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, Exception exception,  ref CallerInfo callerInfo )
        {
			if ( this.IsEnabled( level ) )
			{
              this.WriteFormatted( context, level, logRecordKind, text, new object[] {  arg1, arg2, arg3, arg4, arg5 }, exception );
			}
        }

		
	    void ILogger.Write<T1, T2, T3, T4, T5, T6>( ILoggingContext context, LogLevel level, LogRecordKind logRecordKind, string text, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, Exception exception,  ref CallerInfo callerInfo )
        {
			if ( this.IsEnabled( level ) )
			{
              this.WriteFormatted( context, level, logRecordKind, text, new object[] {  arg1, arg2, arg3, arg4, arg5, arg6 }, exception );
			}
        }

		
	    void ILogger.Write<T1, T2, T3, T4, T5, T6, T7>( ILoggingContext context, LogLevel level, LogRecordKind logRecordKind, string text, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, Exception exception,  ref CallerInfo callerInfo )
        {
			if ( this.IsEnabled( level ) )
			{
              this.WriteFormatted( context, level, logRecordKind, text, new object[] {  arg1, arg2, arg3, arg4, arg5, arg6, arg7 }, exception );
			}
        }

		
	    void ILogger.Write<T1, T2, T3, T4, T5, T6, T7, T8>( ILoggingContext context, LogLevel level, LogRecordKind logRecordKind, string text, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, Exception exception,  ref CallerInfo callerInfo )
        {
			if ( this.IsEnabled( level ) )
			{
              this.WriteFormatted( context, level, logRecordKind, text, new object[] {  arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8 }, exception );
			}
        }

		
	    void ILogger.Write<T1, T2, T3, T4, T5, T6, T7, T8, T9>( ILoggingContext context, LogLevel level, LogRecordKind logRecordKind, string text, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, Exception exception,  ref CallerInfo callerInfo )
        {
			if ( this.IsEnabled( level ) )
			{
              this.WriteFormatted( context, level, logRecordKind, text, new object[] {  arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9 }, exception );
			}
        }

		
	    void ILogger.Write<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>( ILoggingContext context, LogLevel level, LogRecordKind logRecordKind, string text, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, Exception exception,  ref CallerInfo callerInfo )
        {
			if ( this.IsEnabled( level ) )
			{
              this.WriteFormatted( context, level, logRecordKind, text, new object[] {  arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10 }, exception );
			}
        }

			}

    
}


