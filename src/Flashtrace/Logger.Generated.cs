// THIS FILE IS T4-GENERATED.
// To edit, go to Logger.Generated.tt.
// To transform, run this: "C:\Program Files (x86)\Common Files\Microsoft Shared\TextTemplating\14.0\TextTransform.exe" Logger.Generated.tt
// The transformation is not automatic because we are in a shared project.


// TODO: Split T4 template into separate files with common include.

using System.Runtime.CompilerServices;
using System.Diagnostics.CodeAnalysis;
using Flashtrace.Custom.Messages;

namespace Flashtrace
{
   
	namespace Custom.Messages
	{
		/// <summary>
		/// Encapsulates a text message with 1 parameter. Use the <see cref="FormattedMessageBuilder"/> class to create an instance of this type.
		/// </summary>
	    [SuppressMessage("Microsoft.Performance","CA1815", Justification = "Equal is not a use case")]
	    [SuppressMessage("Microsoft.Design","CA1005:AvoidExcessiveParametersOnGenericTypes", Justification = "Recommended to use 'var' keyword.")]
		public readonly struct FormattedMessage<T1> : IMessage
		{
			private readonly string formattingString;
			
			private readonly T1 arg1; 

			
			[MethodImpl(MethodImplOptions.AggressiveInlining)] // To avoid copying the struct.
			internal FormattedMessage( string formattingString, T1 arg1 )
			{
				this.formattingString = formattingString;
				this.arg1 = arg1; 
			}

			void IMessage.Write( ICustomLogRecordBuilder recordBuilder, CustomLogRecordItem item )
			{
				recordBuilder.BeginWriteItem(item, new CustomLogRecordTextOptions(1, null));
    
				FormattingStringParser parser = new FormattingStringParser( this.formattingString );
				ArraySegment<char> parameter;

				   
				recordBuilder.WriteCustomString( parser.GetNextSubstring() );
				parameter = parser.GetNextParameter();

				if ( parameter.Array == null )
				{
					throw new InvalidFormattingStringException("The formatting string must have exactly 1 parameters.");
				}

				recordBuilder.WriteCustomParameter( 0, parameter, arg1, CustomLogParameterOptions.FormattedStringParameter );

				
				recordBuilder.WriteCustomString( parser.GetNextSubstring() );

				if ( parser.GetNextParameter().Array != null )
				{
					throw new InvalidFormattingStringException("The formatting string must have exactly 1 parameters.");
				}

				recordBuilder.EndWriteItem(item);


			}

			/// <inheritdoc />
			public override string ToString() => DebugMessageFormatter.Format( this );
		}

		 /// <summary>
		/// Encapsulates a semantic message with a 1 number of parameter. Use the <see cref="SemanticMessageBuilder"/> class to create an instance of this type.
		/// </summary>
		[SuppressMessage("Microsoft.Performance","CA1815", Justification = "Equal is not a use case")]
		[SuppressMessage("Microsoft.Design","CA1005:AvoidExcessiveParametersOnGenericTypes", Justification = "Recommended to use 'var' keyword.")]
		public readonly struct SemanticMessage<T1> : IMessage
		{
			private readonly string messageName;
			
			private readonly string name1; 
			private readonly T1 value1; 

			[MethodImpl(MethodImplOptions.AggressiveInlining)] // To avoid copying the struct.
			internal SemanticMessage( string messageName, string name1, T1 value1 )
			{
				this.messageName = messageName;
				this.name1 = name1; 
				this.value1 = value1; 
			}

			void IMessage.Write( ICustomLogRecordBuilder recordBuilder, CustomLogRecordItem item )
			{
				recordBuilder.BeginWriteItem(item, new CustomLogRecordTextOptions(1, this.messageName));

				   
				recordBuilder.WriteCustomParameter( 0, this.name1, this.value1, CustomLogParameterOptions.SemanticParameter );

				
				recordBuilder.EndWriteItem(item);
				
			}

			/// <inheritdoc />
			public override string ToString() => DebugMessageFormatter.Format( this );
		}

		
	
	}



	partial class FormattedMessageBuilder
	{
		
		/// <summary>
        /// Creates a formatted string with 1 parameter.
        /// </summary>
        /// <param name="formattingString">The text of the log record, including parameters (e.g. <c>Opening {Path} file {ShareMode} sharing mode</c>).</param>
		/// <typeparam name="T1">Type of the first parameter.</typeparam>
		/// <param name="arg1">Value of the first parameter.</param>
	        [MethodImpl(MethodImplOptions.AggressiveInlining)] // To avoid copying the struct.
	    public static FormattedMessage<T1> Formatted<T1>(  string formattingString, T1 arg1 )
        {
            return new FormattedMessage<T1>( formattingString, arg1 );
        }
	}

	partial class SemanticMessageBuilder
	{
		/// <summary>
        /// Create a semantic message with 1 parameter (using tuples).
        /// </summary>
        /// <param name="name">Name of the message.</param>
		/// <typeparam name="T1">Type of the first parameter value.</typeparam>
		/// <param name="parameterName1">Name of the first parameter.</param>
		/// <param name="parameterValue1">Name of the first parameter.</param>
	        [MethodImpl(MethodImplOptions.AggressiveInlining)] // To avoid copying the struct.
	    public static  SemanticMessage<T1> Semantic<T1>(  string name, string parameterName1, T1 parameterValue1 )
        {
            return new SemanticMessage<T1>( name, parameterName1, parameterValue1 );
        }

		/// <summary>
        /// Create a semantic message with 1 parameter.
        /// </summary>
        /// <param name="name">Name of the message.</param>
		/// <typeparam name="T1">Type of the first parameter value.</typeparam>
		/// <param name="parameter1">Name and value of the first parameter wrapped as a tuple.</param>
	        [MethodImpl(MethodImplOptions.AggressiveInlining)] // To avoid copying the struct.
	    public static  SemanticMessage<T1> Semantic<T1>(  string name, in (string Name,T1 Value) parameter1 )
        {
            return new SemanticMessage<T1>( name, parameter1.Name, parameter1.Value );
        }
	}
	
	namespace Custom.Messages
	{
		/// <summary>
		/// Encapsulates a text message with 2 parameters. Use the <see cref="FormattedMessageBuilder"/> class to create an instance of this type.
		/// </summary>
	    [SuppressMessage("Microsoft.Performance","CA1815", Justification = "Equal is not a use case")]
	    [SuppressMessage("Microsoft.Design","CA1005:AvoidExcessiveParametersOnGenericTypes", Justification = "Recommended to use 'var' keyword.")]
		public readonly struct FormattedMessage<T1, T2> : IMessage
		{
			private readonly string formattingString;
			
			private readonly T1 arg1; private readonly T2 arg2; 

			
			[MethodImpl(MethodImplOptions.AggressiveInlining)] // To avoid copying the struct.
			internal FormattedMessage( string formattingString, T1 arg1, T2 arg2 )
			{
				this.formattingString = formattingString;
				this.arg1 = arg1; this.arg2 = arg2; 
			}

			void IMessage.Write( ICustomLogRecordBuilder recordBuilder, CustomLogRecordItem item )
			{
				recordBuilder.BeginWriteItem(item, new CustomLogRecordTextOptions(2, null));
    
				FormattingStringParser parser = new FormattingStringParser( this.formattingString );
				ArraySegment<char> parameter;

				   
				recordBuilder.WriteCustomString( parser.GetNextSubstring() );
				parameter = parser.GetNextParameter();

				if ( parameter.Array == null )
				{
					throw new InvalidFormattingStringException("The formatting string must have exactly 2 parameters.");
				}

				recordBuilder.WriteCustomParameter( 0, parameter, arg1, CustomLogParameterOptions.FormattedStringParameter );

				
				recordBuilder.WriteCustomString( parser.GetNextSubstring() );
				parameter = parser.GetNextParameter();

				if ( parameter.Array == null )
				{
					throw new InvalidFormattingStringException("The formatting string must have exactly 2 parameters.");
				}

				recordBuilder.WriteCustomParameter( 1, parameter, arg2, CustomLogParameterOptions.FormattedStringParameter );

				
				recordBuilder.WriteCustomString( parser.GetNextSubstring() );

				if ( parser.GetNextParameter().Array != null )
				{
					throw new InvalidFormattingStringException("The formatting string must have exactly 2 parameters.");
				}

				recordBuilder.EndWriteItem(item);


			}

			/// <inheritdoc />
			public override string ToString() => DebugMessageFormatter.Format( this );
		}

		 /// <summary>
		/// Encapsulates a semantic message with a 2 number of parameters. Use the <see cref="SemanticMessageBuilder"/> class to create an instance of this type.
		/// </summary>
		[SuppressMessage("Microsoft.Performance","CA1815", Justification = "Equal is not a use case")]
		[SuppressMessage("Microsoft.Design","CA1005:AvoidExcessiveParametersOnGenericTypes", Justification = "Recommended to use 'var' keyword.")]
		public readonly struct SemanticMessage<T1, T2> : IMessage
		{
			private readonly string messageName;
			
			private readonly string name1; private readonly string name2; 
			private readonly T1 value1; private readonly T2 value2; 

			[MethodImpl(MethodImplOptions.AggressiveInlining)] // To avoid copying the struct.
			internal SemanticMessage( string messageName, string name1, T1 value1, string name2, T2 value2 )
			{
				this.messageName = messageName;
				this.name1 = name1; this.name2 = name2; 
				this.value1 = value1; this.value2 = value2; 
			}

			void IMessage.Write( ICustomLogRecordBuilder recordBuilder, CustomLogRecordItem item )
			{
				recordBuilder.BeginWriteItem(item, new CustomLogRecordTextOptions(2, this.messageName));

				   
				recordBuilder.WriteCustomParameter( 0, this.name1, this.value1, CustomLogParameterOptions.SemanticParameter );

				
				recordBuilder.WriteCustomParameter( 1, this.name2, this.value2, CustomLogParameterOptions.SemanticParameter );

				
				recordBuilder.EndWriteItem(item);
				
			}

			/// <inheritdoc />
			public override string ToString() => DebugMessageFormatter.Format( this );
		}

		
	
	}



	partial class FormattedMessageBuilder
	{
		
		/// <summary>
        /// Creates a formatted string with 2 parameters.
        /// </summary>
        /// <param name="formattingString">The text of the log record, including parameters (e.g. <c>Opening {Path} file {ShareMode} sharing mode</c>).</param>
		/// <typeparam name="T1">Type of the first parameter.</typeparam>
		/// <param name="arg1">Value of the first parameter.</param>
			/// <typeparam name="T2">Type of the second parameter.</typeparam>
		/// <param name="arg2">Value of the second parameter.</param>
	        [MethodImpl(MethodImplOptions.AggressiveInlining)] // To avoid copying the struct.
	    public static FormattedMessage<T1, T2> Formatted<T1, T2>(  string formattingString, T1 arg1, T2 arg2 )
        {
            return new FormattedMessage<T1, T2>( formattingString, arg1, arg2 );
        }
	}

	partial class SemanticMessageBuilder
	{
		/// <summary>
        /// Create a semantic message with 2 parameters (using tuples).
        /// </summary>
        /// <param name="name">Name of the message.</param>
		/// <typeparam name="T1">Type of the first parameter value.</typeparam>
		/// <param name="parameterName1">Name of the first parameter.</param>
		/// <param name="parameterValue1">Name of the first parameter.</param>
			/// <typeparam name="T2">Type of the second parameter value.</typeparam>
		/// <param name="parameterName2">Name of the second parameter.</param>
		/// <param name="parameterValue2">Name of the second parameter.</param>
	        [MethodImpl(MethodImplOptions.AggressiveInlining)] // To avoid copying the struct.
	    public static  SemanticMessage<T1, T2> Semantic<T1, T2>(  string name, string parameterName1, T1 parameterValue1, string parameterName2, T2 parameterValue2 )
        {
            return new SemanticMessage<T1, T2>( name, parameterName1, parameterValue1, parameterName2, parameterValue2 );
        }

		/// <summary>
        /// Create a semantic message with 2 parameters.
        /// </summary>
        /// <param name="name">Name of the message.</param>
		/// <typeparam name="T1">Type of the first parameter value.</typeparam>
		/// <param name="parameter1">Name and value of the first parameter wrapped as a tuple.</param>
			/// <typeparam name="T2">Type of the second parameter value.</typeparam>
		/// <param name="parameter2">Name and value of the second parameter wrapped as a tuple.</param>
	        [MethodImpl(MethodImplOptions.AggressiveInlining)] // To avoid copying the struct.
	    public static  SemanticMessage<T1, T2> Semantic<T1, T2>(  string name, in (string Name,T1 Value) parameter1, in (string Name,T2 Value) parameter2 )
        {
            return new SemanticMessage<T1, T2>( name, parameter1.Name, parameter1.Value, parameter2.Name, parameter2.Value );
        }
	}
	
	namespace Custom.Messages
	{
		/// <summary>
		/// Encapsulates a text message with 3 parameters. Use the <see cref="FormattedMessageBuilder"/> class to create an instance of this type.
		/// </summary>
	    [SuppressMessage("Microsoft.Performance","CA1815", Justification = "Equal is not a use case")]
	    [SuppressMessage("Microsoft.Design","CA1005:AvoidExcessiveParametersOnGenericTypes", Justification = "Recommended to use 'var' keyword.")]
		public readonly struct FormattedMessage<T1, T2, T3> : IMessage
		{
			private readonly string formattingString;
			
			private readonly T1 arg1; private readonly T2 arg2; private readonly T3 arg3; 

			
			[MethodImpl(MethodImplOptions.AggressiveInlining)] // To avoid copying the struct.
			internal FormattedMessage( string formattingString, T1 arg1, T2 arg2, T3 arg3 )
			{
				this.formattingString = formattingString;
				this.arg1 = arg1; this.arg2 = arg2; this.arg3 = arg3; 
			}

			void IMessage.Write( ICustomLogRecordBuilder recordBuilder, CustomLogRecordItem item )
			{
				recordBuilder.BeginWriteItem(item, new CustomLogRecordTextOptions(3, null));
    
				FormattingStringParser parser = new FormattingStringParser( this.formattingString );
				ArraySegment<char> parameter;

				   
				recordBuilder.WriteCustomString( parser.GetNextSubstring() );
				parameter = parser.GetNextParameter();

				if ( parameter.Array == null )
				{
					throw new InvalidFormattingStringException("The formatting string must have exactly 3 parameters.");
				}

				recordBuilder.WriteCustomParameter( 0, parameter, arg1, CustomLogParameterOptions.FormattedStringParameter );

				
				recordBuilder.WriteCustomString( parser.GetNextSubstring() );
				parameter = parser.GetNextParameter();

				if ( parameter.Array == null )
				{
					throw new InvalidFormattingStringException("The formatting string must have exactly 3 parameters.");
				}

				recordBuilder.WriteCustomParameter( 1, parameter, arg2, CustomLogParameterOptions.FormattedStringParameter );

				
				recordBuilder.WriteCustomString( parser.GetNextSubstring() );
				parameter = parser.GetNextParameter();

				if ( parameter.Array == null )
				{
					throw new InvalidFormattingStringException("The formatting string must have exactly 3 parameters.");
				}

				recordBuilder.WriteCustomParameter( 2, parameter, arg3, CustomLogParameterOptions.FormattedStringParameter );

				
				recordBuilder.WriteCustomString( parser.GetNextSubstring() );

				if ( parser.GetNextParameter().Array != null )
				{
					throw new InvalidFormattingStringException("The formatting string must have exactly 3 parameters.");
				}

				recordBuilder.EndWriteItem(item);


			}

			/// <inheritdoc />
			public override string ToString() => DebugMessageFormatter.Format( this );
		}

		 /// <summary>
		/// Encapsulates a semantic message with a 3 number of parameters. Use the <see cref="SemanticMessageBuilder"/> class to create an instance of this type.
		/// </summary>
		[SuppressMessage("Microsoft.Performance","CA1815", Justification = "Equal is not a use case")]
		[SuppressMessage("Microsoft.Design","CA1005:AvoidExcessiveParametersOnGenericTypes", Justification = "Recommended to use 'var' keyword.")]
		public readonly struct SemanticMessage<T1, T2, T3> : IMessage
		{
			private readonly string messageName;
			
			private readonly string name1; private readonly string name2; private readonly string name3; 
			private readonly T1 value1; private readonly T2 value2; private readonly T3 value3; 

			[MethodImpl(MethodImplOptions.AggressiveInlining)] // To avoid copying the struct.
			internal SemanticMessage( string messageName, string name1, T1 value1, string name2, T2 value2, string name3, T3 value3 )
			{
				this.messageName = messageName;
				this.name1 = name1; this.name2 = name2; this.name3 = name3; 
				this.value1 = value1; this.value2 = value2; this.value3 = value3; 
			}

			void IMessage.Write( ICustomLogRecordBuilder recordBuilder, CustomLogRecordItem item )
			{
				recordBuilder.BeginWriteItem(item, new CustomLogRecordTextOptions(3, this.messageName));

				   
				recordBuilder.WriteCustomParameter( 0, this.name1, this.value1, CustomLogParameterOptions.SemanticParameter );

				
				recordBuilder.WriteCustomParameter( 1, this.name2, this.value2, CustomLogParameterOptions.SemanticParameter );

				
				recordBuilder.WriteCustomParameter( 2, this.name3, this.value3, CustomLogParameterOptions.SemanticParameter );

				
				recordBuilder.EndWriteItem(item);
				
			}

			/// <inheritdoc />
			public override string ToString() => DebugMessageFormatter.Format( this );
		}

		
	
	}



	partial class FormattedMessageBuilder
	{
		
		/// <summary>
        /// Creates a formatted string with 3 parameters.
        /// </summary>
        /// <param name="formattingString">The text of the log record, including parameters (e.g. <c>Opening {Path} file {ShareMode} sharing mode</c>).</param>
		/// <typeparam name="T1">Type of the first parameter.</typeparam>
		/// <param name="arg1">Value of the first parameter.</param>
			/// <typeparam name="T2">Type of the second parameter.</typeparam>
		/// <param name="arg2">Value of the second parameter.</param>
			/// <typeparam name="T3">Type of the third parameter.</typeparam>
		/// <param name="arg3">Value of the third parameter.</param>
	        [MethodImpl(MethodImplOptions.AggressiveInlining)] // To avoid copying the struct.
	    public static FormattedMessage<T1, T2, T3> Formatted<T1, T2, T3>(  string formattingString, T1 arg1, T2 arg2, T3 arg3 )
        {
            return new FormattedMessage<T1, T2, T3>( formattingString, arg1, arg2, arg3 );
        }
	}

	partial class SemanticMessageBuilder
	{
		/// <summary>
        /// Create a semantic message with 3 parameters (using tuples).
        /// </summary>
        /// <param name="name">Name of the message.</param>
		/// <typeparam name="T1">Type of the first parameter value.</typeparam>
		/// <param name="parameterName1">Name of the first parameter.</param>
		/// <param name="parameterValue1">Name of the first parameter.</param>
			/// <typeparam name="T2">Type of the second parameter value.</typeparam>
		/// <param name="parameterName2">Name of the second parameter.</param>
		/// <param name="parameterValue2">Name of the second parameter.</param>
			/// <typeparam name="T3">Type of the third parameter value.</typeparam>
		/// <param name="parameterName3">Name of the third parameter.</param>
		/// <param name="parameterValue3">Name of the third parameter.</param>
	        [MethodImpl(MethodImplOptions.AggressiveInlining)] // To avoid copying the struct.
	    public static  SemanticMessage<T1, T2, T3> Semantic<T1, T2, T3>(  string name, string parameterName1, T1 parameterValue1, string parameterName2, T2 parameterValue2, string parameterName3, T3 parameterValue3 )
        {
            return new SemanticMessage<T1, T2, T3>( name, parameterName1, parameterValue1, parameterName2, parameterValue2, parameterName3, parameterValue3 );
        }

		/// <summary>
        /// Create a semantic message with 3 parameters.
        /// </summary>
        /// <param name="name">Name of the message.</param>
		/// <typeparam name="T1">Type of the first parameter value.</typeparam>
		/// <param name="parameter1">Name and value of the first parameter wrapped as a tuple.</param>
			/// <typeparam name="T2">Type of the second parameter value.</typeparam>
		/// <param name="parameter2">Name and value of the second parameter wrapped as a tuple.</param>
			/// <typeparam name="T3">Type of the third parameter value.</typeparam>
		/// <param name="parameter3">Name and value of the third parameter wrapped as a tuple.</param>
	        [MethodImpl(MethodImplOptions.AggressiveInlining)] // To avoid copying the struct.
	    public static  SemanticMessage<T1, T2, T3> Semantic<T1, T2, T3>(  string name, in (string Name,T1 Value) parameter1, in (string Name,T2 Value) parameter2, in (string Name,T3 Value) parameter3 )
        {
            return new SemanticMessage<T1, T2, T3>( name, parameter1.Name, parameter1.Value, parameter2.Name, parameter2.Value, parameter3.Name, parameter3.Value );
        }
	}
	
	namespace Custom.Messages
	{
		/// <summary>
		/// Encapsulates a text message with 4 parameters. Use the <see cref="FormattedMessageBuilder"/> class to create an instance of this type.
		/// </summary>
	    [SuppressMessage("Microsoft.Performance","CA1815", Justification = "Equal is not a use case")]
	    [SuppressMessage("Microsoft.Design","CA1005:AvoidExcessiveParametersOnGenericTypes", Justification = "Recommended to use 'var' keyword.")]
		public readonly struct FormattedMessage<T1, T2, T3, T4> : IMessage
		{
			private readonly string formattingString;
			
			private readonly T1 arg1; private readonly T2 arg2; private readonly T3 arg3; private readonly T4 arg4; 

			
			[MethodImpl(MethodImplOptions.AggressiveInlining)] // To avoid copying the struct.
			internal FormattedMessage( string formattingString, T1 arg1, T2 arg2, T3 arg3, T4 arg4 )
			{
				this.formattingString = formattingString;
				this.arg1 = arg1; this.arg2 = arg2; this.arg3 = arg3; this.arg4 = arg4; 
			}

			void IMessage.Write( ICustomLogRecordBuilder recordBuilder, CustomLogRecordItem item )
			{
				recordBuilder.BeginWriteItem(item, new CustomLogRecordTextOptions(4, null));
    
				FormattingStringParser parser = new FormattingStringParser( this.formattingString );
				ArraySegment<char> parameter;

				   
				recordBuilder.WriteCustomString( parser.GetNextSubstring() );
				parameter = parser.GetNextParameter();

				if ( parameter.Array == null )
				{
					throw new InvalidFormattingStringException("The formatting string must have exactly 4 parameters.");
				}

				recordBuilder.WriteCustomParameter( 0, parameter, arg1, CustomLogParameterOptions.FormattedStringParameter );

				
				recordBuilder.WriteCustomString( parser.GetNextSubstring() );
				parameter = parser.GetNextParameter();

				if ( parameter.Array == null )
				{
					throw new InvalidFormattingStringException("The formatting string must have exactly 4 parameters.");
				}

				recordBuilder.WriteCustomParameter( 1, parameter, arg2, CustomLogParameterOptions.FormattedStringParameter );

				
				recordBuilder.WriteCustomString( parser.GetNextSubstring() );
				parameter = parser.GetNextParameter();

				if ( parameter.Array == null )
				{
					throw new InvalidFormattingStringException("The formatting string must have exactly 4 parameters.");
				}

				recordBuilder.WriteCustomParameter( 2, parameter, arg3, CustomLogParameterOptions.FormattedStringParameter );

				
				recordBuilder.WriteCustomString( parser.GetNextSubstring() );
				parameter = parser.GetNextParameter();

				if ( parameter.Array == null )
				{
					throw new InvalidFormattingStringException("The formatting string must have exactly 4 parameters.");
				}

				recordBuilder.WriteCustomParameter( 3, parameter, arg4, CustomLogParameterOptions.FormattedStringParameter );

				
				recordBuilder.WriteCustomString( parser.GetNextSubstring() );

				if ( parser.GetNextParameter().Array != null )
				{
					throw new InvalidFormattingStringException("The formatting string must have exactly 4 parameters.");
				}

				recordBuilder.EndWriteItem(item);


			}

			/// <inheritdoc />
			public override string ToString() => DebugMessageFormatter.Format( this );
		}

		 /// <summary>
		/// Encapsulates a semantic message with a 4 number of parameters. Use the <see cref="SemanticMessageBuilder"/> class to create an instance of this type.
		/// </summary>
		[SuppressMessage("Microsoft.Performance","CA1815", Justification = "Equal is not a use case")]
		[SuppressMessage("Microsoft.Design","CA1005:AvoidExcessiveParametersOnGenericTypes", Justification = "Recommended to use 'var' keyword.")]
		public readonly struct SemanticMessage<T1, T2, T3, T4> : IMessage
		{
			private readonly string messageName;
			
			private readonly string name1; private readonly string name2; private readonly string name3; private readonly string name4; 
			private readonly T1 value1; private readonly T2 value2; private readonly T3 value3; private readonly T4 value4; 

			[MethodImpl(MethodImplOptions.AggressiveInlining)] // To avoid copying the struct.
			internal SemanticMessage( string messageName, string name1, T1 value1, string name2, T2 value2, string name3, T3 value3, string name4, T4 value4 )
			{
				this.messageName = messageName;
				this.name1 = name1; this.name2 = name2; this.name3 = name3; this.name4 = name4; 
				this.value1 = value1; this.value2 = value2; this.value3 = value3; this.value4 = value4; 
			}

			void IMessage.Write( ICustomLogRecordBuilder recordBuilder, CustomLogRecordItem item )
			{
				recordBuilder.BeginWriteItem(item, new CustomLogRecordTextOptions(4, this.messageName));

				   
				recordBuilder.WriteCustomParameter( 0, this.name1, this.value1, CustomLogParameterOptions.SemanticParameter );

				
				recordBuilder.WriteCustomParameter( 1, this.name2, this.value2, CustomLogParameterOptions.SemanticParameter );

				
				recordBuilder.WriteCustomParameter( 2, this.name3, this.value3, CustomLogParameterOptions.SemanticParameter );

				
				recordBuilder.WriteCustomParameter( 3, this.name4, this.value4, CustomLogParameterOptions.SemanticParameter );

				
				recordBuilder.EndWriteItem(item);
				
			}

			/// <inheritdoc />
			public override string ToString() => DebugMessageFormatter.Format( this );
		}

		
	
	}



	partial class FormattedMessageBuilder
	{
		
		/// <summary>
        /// Creates a formatted string with 4 parameters.
        /// </summary>
        /// <param name="formattingString">The text of the log record, including parameters (e.g. <c>Opening {Path} file {ShareMode} sharing mode</c>).</param>
		/// <typeparam name="T1">Type of the first parameter.</typeparam>
		/// <param name="arg1">Value of the first parameter.</param>
			/// <typeparam name="T2">Type of the second parameter.</typeparam>
		/// <param name="arg2">Value of the second parameter.</param>
			/// <typeparam name="T3">Type of the third parameter.</typeparam>
		/// <param name="arg3">Value of the third parameter.</param>
			/// <typeparam name="T4">Type of the 4-th parameter.</typeparam>
		/// <param name="arg4">Value of the 4-th parameter.</param>
	        [MethodImpl(MethodImplOptions.AggressiveInlining)] // To avoid copying the struct.
	    public static FormattedMessage<T1, T2, T3, T4> Formatted<T1, T2, T3, T4>(  string formattingString, T1 arg1, T2 arg2, T3 arg3, T4 arg4 )
        {
            return new FormattedMessage<T1, T2, T3, T4>( formattingString, arg1, arg2, arg3, arg4 );
        }
	}

	partial class SemanticMessageBuilder
	{
		/// <summary>
        /// Create a semantic message with 4 parameters (using tuples).
        /// </summary>
        /// <param name="name">Name of the message.</param>
		/// <typeparam name="T1">Type of the first parameter value.</typeparam>
		/// <param name="parameterName1">Name of the first parameter.</param>
		/// <param name="parameterValue1">Name of the first parameter.</param>
			/// <typeparam name="T2">Type of the second parameter value.</typeparam>
		/// <param name="parameterName2">Name of the second parameter.</param>
		/// <param name="parameterValue2">Name of the second parameter.</param>
			/// <typeparam name="T3">Type of the third parameter value.</typeparam>
		/// <param name="parameterName3">Name of the third parameter.</param>
		/// <param name="parameterValue3">Name of the third parameter.</param>
			/// <typeparam name="T4">Type of the 4-th parameter value.</typeparam>
		/// <param name="parameterName4">Name of the 4-th parameter.</param>
		/// <param name="parameterValue4">Name of the 4-th parameter.</param>
	        [MethodImpl(MethodImplOptions.AggressiveInlining)] // To avoid copying the struct.
	    public static  SemanticMessage<T1, T2, T3, T4> Semantic<T1, T2, T3, T4>(  string name, string parameterName1, T1 parameterValue1, string parameterName2, T2 parameterValue2, string parameterName3, T3 parameterValue3, string parameterName4, T4 parameterValue4 )
        {
            return new SemanticMessage<T1, T2, T3, T4>( name, parameterName1, parameterValue1, parameterName2, parameterValue2, parameterName3, parameterValue3, parameterName4, parameterValue4 );
        }

		/// <summary>
        /// Create a semantic message with 4 parameters.
        /// </summary>
        /// <param name="name">Name of the message.</param>
		/// <typeparam name="T1">Type of the first parameter value.</typeparam>
		/// <param name="parameter1">Name and value of the first parameter wrapped as a tuple.</param>
			/// <typeparam name="T2">Type of the second parameter value.</typeparam>
		/// <param name="parameter2">Name and value of the second parameter wrapped as a tuple.</param>
			/// <typeparam name="T3">Type of the third parameter value.</typeparam>
		/// <param name="parameter3">Name and value of the third parameter wrapped as a tuple.</param>
			/// <typeparam name="T4">Type of the 4-th parameter value.</typeparam>
		/// <param name="parameter4">Name and value of the 4-th parameter wrapped as a tuple.</param>
	        [MethodImpl(MethodImplOptions.AggressiveInlining)] // To avoid copying the struct.
	    public static  SemanticMessage<T1, T2, T3, T4> Semantic<T1, T2, T3, T4>(  string name, in (string Name,T1 Value) parameter1, in (string Name,T2 Value) parameter2, in (string Name,T3 Value) parameter3, in (string Name,T4 Value) parameter4 )
        {
            return new SemanticMessage<T1, T2, T3, T4>( name, parameter1.Name, parameter1.Value, parameter2.Name, parameter2.Value, parameter3.Name, parameter3.Value, parameter4.Name, parameter4.Value );
        }
	}
	
	namespace Custom.Messages
	{
		/// <summary>
		/// Encapsulates a text message with 5 parameters. Use the <see cref="FormattedMessageBuilder"/> class to create an instance of this type.
		/// </summary>
	    [SuppressMessage("Microsoft.Performance","CA1815", Justification = "Equal is not a use case")]
	    [SuppressMessage("Microsoft.Design","CA1005:AvoidExcessiveParametersOnGenericTypes", Justification = "Recommended to use 'var' keyword.")]
		public readonly struct FormattedMessage<T1, T2, T3, T4, T5> : IMessage
		{
			private readonly string formattingString;
			
			private readonly T1 arg1; private readonly T2 arg2; private readonly T3 arg3; private readonly T4 arg4; private readonly T5 arg5; 

			
			[MethodImpl(MethodImplOptions.AggressiveInlining)] // To avoid copying the struct.
			internal FormattedMessage( string formattingString, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5 )
			{
				this.formattingString = formattingString;
				this.arg1 = arg1; this.arg2 = arg2; this.arg3 = arg3; this.arg4 = arg4; this.arg5 = arg5; 
			}

			void IMessage.Write( ICustomLogRecordBuilder recordBuilder, CustomLogRecordItem item )
			{
				recordBuilder.BeginWriteItem(item, new CustomLogRecordTextOptions(5, null));
    
				FormattingStringParser parser = new FormattingStringParser( this.formattingString );
				ArraySegment<char> parameter;

				   
				recordBuilder.WriteCustomString( parser.GetNextSubstring() );
				parameter = parser.GetNextParameter();

				if ( parameter.Array == null )
				{
					throw new InvalidFormattingStringException("The formatting string must have exactly 5 parameters.");
				}

				recordBuilder.WriteCustomParameter( 0, parameter, arg1, CustomLogParameterOptions.FormattedStringParameter );

				
				recordBuilder.WriteCustomString( parser.GetNextSubstring() );
				parameter = parser.GetNextParameter();

				if ( parameter.Array == null )
				{
					throw new InvalidFormattingStringException("The formatting string must have exactly 5 parameters.");
				}

				recordBuilder.WriteCustomParameter( 1, parameter, arg2, CustomLogParameterOptions.FormattedStringParameter );

				
				recordBuilder.WriteCustomString( parser.GetNextSubstring() );
				parameter = parser.GetNextParameter();

				if ( parameter.Array == null )
				{
					throw new InvalidFormattingStringException("The formatting string must have exactly 5 parameters.");
				}

				recordBuilder.WriteCustomParameter( 2, parameter, arg3, CustomLogParameterOptions.FormattedStringParameter );

				
				recordBuilder.WriteCustomString( parser.GetNextSubstring() );
				parameter = parser.GetNextParameter();

				if ( parameter.Array == null )
				{
					throw new InvalidFormattingStringException("The formatting string must have exactly 5 parameters.");
				}

				recordBuilder.WriteCustomParameter( 3, parameter, arg4, CustomLogParameterOptions.FormattedStringParameter );

				
				recordBuilder.WriteCustomString( parser.GetNextSubstring() );
				parameter = parser.GetNextParameter();

				if ( parameter.Array == null )
				{
					throw new InvalidFormattingStringException("The formatting string must have exactly 5 parameters.");
				}

				recordBuilder.WriteCustomParameter( 4, parameter, arg5, CustomLogParameterOptions.FormattedStringParameter );

				
				recordBuilder.WriteCustomString( parser.GetNextSubstring() );

				if ( parser.GetNextParameter().Array != null )
				{
					throw new InvalidFormattingStringException("The formatting string must have exactly 5 parameters.");
				}

				recordBuilder.EndWriteItem(item);


			}

			/// <inheritdoc />
			public override string ToString() => DebugMessageFormatter.Format( this );
		}

		 /// <summary>
		/// Encapsulates a semantic message with a 5 number of parameters. Use the <see cref="SemanticMessageBuilder"/> class to create an instance of this type.
		/// </summary>
		[SuppressMessage("Microsoft.Performance","CA1815", Justification = "Equal is not a use case")]
		[SuppressMessage("Microsoft.Design","CA1005:AvoidExcessiveParametersOnGenericTypes", Justification = "Recommended to use 'var' keyword.")]
		public readonly struct SemanticMessage<T1, T2, T3, T4, T5> : IMessage
		{
			private readonly string messageName;
			
			private readonly string name1; private readonly string name2; private readonly string name3; private readonly string name4; private readonly string name5; 
			private readonly T1 value1; private readonly T2 value2; private readonly T3 value3; private readonly T4 value4; private readonly T5 value5; 

			[MethodImpl(MethodImplOptions.AggressiveInlining)] // To avoid copying the struct.
			internal SemanticMessage( string messageName, string name1, T1 value1, string name2, T2 value2, string name3, T3 value3, string name4, T4 value4, string name5, T5 value5 )
			{
				this.messageName = messageName;
				this.name1 = name1; this.name2 = name2; this.name3 = name3; this.name4 = name4; this.name5 = name5; 
				this.value1 = value1; this.value2 = value2; this.value3 = value3; this.value4 = value4; this.value5 = value5; 
			}

			void IMessage.Write( ICustomLogRecordBuilder recordBuilder, CustomLogRecordItem item )
			{
				recordBuilder.BeginWriteItem(item, new CustomLogRecordTextOptions(5, this.messageName));

				   
				recordBuilder.WriteCustomParameter( 0, this.name1, this.value1, CustomLogParameterOptions.SemanticParameter );

				
				recordBuilder.WriteCustomParameter( 1, this.name2, this.value2, CustomLogParameterOptions.SemanticParameter );

				
				recordBuilder.WriteCustomParameter( 2, this.name3, this.value3, CustomLogParameterOptions.SemanticParameter );

				
				recordBuilder.WriteCustomParameter( 3, this.name4, this.value4, CustomLogParameterOptions.SemanticParameter );

				
				recordBuilder.WriteCustomParameter( 4, this.name5, this.value5, CustomLogParameterOptions.SemanticParameter );

				
				recordBuilder.EndWriteItem(item);
				
			}

			/// <inheritdoc />
			public override string ToString() => DebugMessageFormatter.Format( this );
		}

		
	
	}



	partial class FormattedMessageBuilder
	{
		
		/// <summary>
        /// Creates a formatted string with 5 parameters.
        /// </summary>
        /// <param name="formattingString">The text of the log record, including parameters (e.g. <c>Opening {Path} file {ShareMode} sharing mode</c>).</param>
		/// <typeparam name="T1">Type of the first parameter.</typeparam>
		/// <param name="arg1">Value of the first parameter.</param>
			/// <typeparam name="T2">Type of the second parameter.</typeparam>
		/// <param name="arg2">Value of the second parameter.</param>
			/// <typeparam name="T3">Type of the third parameter.</typeparam>
		/// <param name="arg3">Value of the third parameter.</param>
			/// <typeparam name="T4">Type of the 4-th parameter.</typeparam>
		/// <param name="arg4">Value of the 4-th parameter.</param>
			/// <typeparam name="T5">Type of the 5-th parameter.</typeparam>
		/// <param name="arg5">Value of the 5-th parameter.</param>
	        [MethodImpl(MethodImplOptions.AggressiveInlining)] // To avoid copying the struct.
	    public static FormattedMessage<T1, T2, T3, T4, T5> Formatted<T1, T2, T3, T4, T5>(  string formattingString, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5 )
        {
            return new FormattedMessage<T1, T2, T3, T4, T5>( formattingString, arg1, arg2, arg3, arg4, arg5 );
        }
	}

	partial class SemanticMessageBuilder
	{
		/// <summary>
        /// Create a semantic message with 5 parameters (using tuples).
        /// </summary>
        /// <param name="name">Name of the message.</param>
		/// <typeparam name="T1">Type of the first parameter value.</typeparam>
		/// <param name="parameterName1">Name of the first parameter.</param>
		/// <param name="parameterValue1">Name of the first parameter.</param>
			/// <typeparam name="T2">Type of the second parameter value.</typeparam>
		/// <param name="parameterName2">Name of the second parameter.</param>
		/// <param name="parameterValue2">Name of the second parameter.</param>
			/// <typeparam name="T3">Type of the third parameter value.</typeparam>
		/// <param name="parameterName3">Name of the third parameter.</param>
		/// <param name="parameterValue3">Name of the third parameter.</param>
			/// <typeparam name="T4">Type of the 4-th parameter value.</typeparam>
		/// <param name="parameterName4">Name of the 4-th parameter.</param>
		/// <param name="parameterValue4">Name of the 4-th parameter.</param>
			/// <typeparam name="T5">Type of the 5-th parameter value.</typeparam>
		/// <param name="parameterName5">Name of the 5-th parameter.</param>
		/// <param name="parameterValue5">Name of the 5-th parameter.</param>
	        [MethodImpl(MethodImplOptions.AggressiveInlining)] // To avoid copying the struct.
	    public static  SemanticMessage<T1, T2, T3, T4, T5> Semantic<T1, T2, T3, T4, T5>(  string name, string parameterName1, T1 parameterValue1, string parameterName2, T2 parameterValue2, string parameterName3, T3 parameterValue3, string parameterName4, T4 parameterValue4, string parameterName5, T5 parameterValue5 )
        {
            return new SemanticMessage<T1, T2, T3, T4, T5>( name, parameterName1, parameterValue1, parameterName2, parameterValue2, parameterName3, parameterValue3, parameterName4, parameterValue4, parameterName5, parameterValue5 );
        }

		/// <summary>
        /// Create a semantic message with 5 parameters.
        /// </summary>
        /// <param name="name">Name of the message.</param>
		/// <typeparam name="T1">Type of the first parameter value.</typeparam>
		/// <param name="parameter1">Name and value of the first parameter wrapped as a tuple.</param>
			/// <typeparam name="T2">Type of the second parameter value.</typeparam>
		/// <param name="parameter2">Name and value of the second parameter wrapped as a tuple.</param>
			/// <typeparam name="T3">Type of the third parameter value.</typeparam>
		/// <param name="parameter3">Name and value of the third parameter wrapped as a tuple.</param>
			/// <typeparam name="T4">Type of the 4-th parameter value.</typeparam>
		/// <param name="parameter4">Name and value of the 4-th parameter wrapped as a tuple.</param>
			/// <typeparam name="T5">Type of the 5-th parameter value.</typeparam>
		/// <param name="parameter5">Name and value of the 5-th parameter wrapped as a tuple.</param>
	        [MethodImpl(MethodImplOptions.AggressiveInlining)] // To avoid copying the struct.
	    public static  SemanticMessage<T1, T2, T3, T4, T5> Semantic<T1, T2, T3, T4, T5>(  string name, in (string Name,T1 Value) parameter1, in (string Name,T2 Value) parameter2, in (string Name,T3 Value) parameter3, in (string Name,T4 Value) parameter4, in (string Name,T5 Value) parameter5 )
        {
            return new SemanticMessage<T1, T2, T3, T4, T5>( name, parameter1.Name, parameter1.Value, parameter2.Name, parameter2.Value, parameter3.Name, parameter3.Value, parameter4.Name, parameter4.Value, parameter5.Name, parameter5.Value );
        }
	}
	
	namespace Custom.Messages
	{
		/// <summary>
		/// Encapsulates a text message with 6 parameters. Use the <see cref="FormattedMessageBuilder"/> class to create an instance of this type.
		/// </summary>
	    [SuppressMessage("Microsoft.Performance","CA1815", Justification = "Equal is not a use case")]
	    [SuppressMessage("Microsoft.Design","CA1005:AvoidExcessiveParametersOnGenericTypes", Justification = "Recommended to use 'var' keyword.")]
		public readonly struct FormattedMessage<T1, T2, T3, T4, T5, T6> : IMessage
		{
			private readonly string formattingString;
			
			private readonly T1 arg1; private readonly T2 arg2; private readonly T3 arg3; private readonly T4 arg4; private readonly T5 arg5; private readonly T6 arg6; 

			
			[MethodImpl(MethodImplOptions.AggressiveInlining)] // To avoid copying the struct.
			internal FormattedMessage( string formattingString, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6 )
			{
				this.formattingString = formattingString;
				this.arg1 = arg1; this.arg2 = arg2; this.arg3 = arg3; this.arg4 = arg4; this.arg5 = arg5; this.arg6 = arg6; 
			}

			void IMessage.Write( ICustomLogRecordBuilder recordBuilder, CustomLogRecordItem item )
			{
				recordBuilder.BeginWriteItem(item, new CustomLogRecordTextOptions(6, null));
    
				FormattingStringParser parser = new FormattingStringParser( this.formattingString );
				ArraySegment<char> parameter;

				   
				recordBuilder.WriteCustomString( parser.GetNextSubstring() );
				parameter = parser.GetNextParameter();

				if ( parameter.Array == null )
				{
					throw new InvalidFormattingStringException("The formatting string must have exactly 6 parameters.");
				}

				recordBuilder.WriteCustomParameter( 0, parameter, arg1, CustomLogParameterOptions.FormattedStringParameter );

				
				recordBuilder.WriteCustomString( parser.GetNextSubstring() );
				parameter = parser.GetNextParameter();

				if ( parameter.Array == null )
				{
					throw new InvalidFormattingStringException("The formatting string must have exactly 6 parameters.");
				}

				recordBuilder.WriteCustomParameter( 1, parameter, arg2, CustomLogParameterOptions.FormattedStringParameter );

				
				recordBuilder.WriteCustomString( parser.GetNextSubstring() );
				parameter = parser.GetNextParameter();

				if ( parameter.Array == null )
				{
					throw new InvalidFormattingStringException("The formatting string must have exactly 6 parameters.");
				}

				recordBuilder.WriteCustomParameter( 2, parameter, arg3, CustomLogParameterOptions.FormattedStringParameter );

				
				recordBuilder.WriteCustomString( parser.GetNextSubstring() );
				parameter = parser.GetNextParameter();

				if ( parameter.Array == null )
				{
					throw new InvalidFormattingStringException("The formatting string must have exactly 6 parameters.");
				}

				recordBuilder.WriteCustomParameter( 3, parameter, arg4, CustomLogParameterOptions.FormattedStringParameter );

				
				recordBuilder.WriteCustomString( parser.GetNextSubstring() );
				parameter = parser.GetNextParameter();

				if ( parameter.Array == null )
				{
					throw new InvalidFormattingStringException("The formatting string must have exactly 6 parameters.");
				}

				recordBuilder.WriteCustomParameter( 4, parameter, arg5, CustomLogParameterOptions.FormattedStringParameter );

				
				recordBuilder.WriteCustomString( parser.GetNextSubstring() );
				parameter = parser.GetNextParameter();

				if ( parameter.Array == null )
				{
					throw new InvalidFormattingStringException("The formatting string must have exactly 6 parameters.");
				}

				recordBuilder.WriteCustomParameter( 5, parameter, arg6, CustomLogParameterOptions.FormattedStringParameter );

				
				recordBuilder.WriteCustomString( parser.GetNextSubstring() );

				if ( parser.GetNextParameter().Array != null )
				{
					throw new InvalidFormattingStringException("The formatting string must have exactly 6 parameters.");
				}

				recordBuilder.EndWriteItem(item);


			}

			/// <inheritdoc />
			public override string ToString() => DebugMessageFormatter.Format( this );
		}

		 /// <summary>
		/// Encapsulates a semantic message with a 6 number of parameters. Use the <see cref="SemanticMessageBuilder"/> class to create an instance of this type.
		/// </summary>
		[SuppressMessage("Microsoft.Performance","CA1815", Justification = "Equal is not a use case")]
		[SuppressMessage("Microsoft.Design","CA1005:AvoidExcessiveParametersOnGenericTypes", Justification = "Recommended to use 'var' keyword.")]
		public readonly struct SemanticMessage<T1, T2, T3, T4, T5, T6> : IMessage
		{
			private readonly string messageName;
			
			private readonly string name1; private readonly string name2; private readonly string name3; private readonly string name4; private readonly string name5; private readonly string name6; 
			private readonly T1 value1; private readonly T2 value2; private readonly T3 value3; private readonly T4 value4; private readonly T5 value5; private readonly T6 value6; 

			[MethodImpl(MethodImplOptions.AggressiveInlining)] // To avoid copying the struct.
			internal SemanticMessage( string messageName, string name1, T1 value1, string name2, T2 value2, string name3, T3 value3, string name4, T4 value4, string name5, T5 value5, string name6, T6 value6 )
			{
				this.messageName = messageName;
				this.name1 = name1; this.name2 = name2; this.name3 = name3; this.name4 = name4; this.name5 = name5; this.name6 = name6; 
				this.value1 = value1; this.value2 = value2; this.value3 = value3; this.value4 = value4; this.value5 = value5; this.value6 = value6; 
			}

			void IMessage.Write( ICustomLogRecordBuilder recordBuilder, CustomLogRecordItem item )
			{
				recordBuilder.BeginWriteItem(item, new CustomLogRecordTextOptions(6, this.messageName));

				   
				recordBuilder.WriteCustomParameter( 0, this.name1, this.value1, CustomLogParameterOptions.SemanticParameter );

				
				recordBuilder.WriteCustomParameter( 1, this.name2, this.value2, CustomLogParameterOptions.SemanticParameter );

				
				recordBuilder.WriteCustomParameter( 2, this.name3, this.value3, CustomLogParameterOptions.SemanticParameter );

				
				recordBuilder.WriteCustomParameter( 3, this.name4, this.value4, CustomLogParameterOptions.SemanticParameter );

				
				recordBuilder.WriteCustomParameter( 4, this.name5, this.value5, CustomLogParameterOptions.SemanticParameter );

				
				recordBuilder.WriteCustomParameter( 5, this.name6, this.value6, CustomLogParameterOptions.SemanticParameter );

				
				recordBuilder.EndWriteItem(item);
				
			}

			/// <inheritdoc />
			public override string ToString() => DebugMessageFormatter.Format( this );
		}

		
	
	}



	partial class FormattedMessageBuilder
	{
		
		/// <summary>
        /// Creates a formatted string with 6 parameters.
        /// </summary>
        /// <param name="formattingString">The text of the log record, including parameters (e.g. <c>Opening {Path} file {ShareMode} sharing mode</c>).</param>
		/// <typeparam name="T1">Type of the first parameter.</typeparam>
		/// <param name="arg1">Value of the first parameter.</param>
			/// <typeparam name="T2">Type of the second parameter.</typeparam>
		/// <param name="arg2">Value of the second parameter.</param>
			/// <typeparam name="T3">Type of the third parameter.</typeparam>
		/// <param name="arg3">Value of the third parameter.</param>
			/// <typeparam name="T4">Type of the 4-th parameter.</typeparam>
		/// <param name="arg4">Value of the 4-th parameter.</param>
			/// <typeparam name="T5">Type of the 5-th parameter.</typeparam>
		/// <param name="arg5">Value of the 5-th parameter.</param>
			/// <typeparam name="T6">Type of the 6-th parameter.</typeparam>
		/// <param name="arg6">Value of the 6-th parameter.</param>
	        [MethodImpl(MethodImplOptions.AggressiveInlining)] // To avoid copying the struct.
	    public static FormattedMessage<T1, T2, T3, T4, T5, T6> Formatted<T1, T2, T3, T4, T5, T6>(  string formattingString, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6 )
        {
            return new FormattedMessage<T1, T2, T3, T4, T5, T6>( formattingString, arg1, arg2, arg3, arg4, arg5, arg6 );
        }
	}

	partial class SemanticMessageBuilder
	{
		/// <summary>
        /// Create a semantic message with 6 parameters (using tuples).
        /// </summary>
        /// <param name="name">Name of the message.</param>
		/// <typeparam name="T1">Type of the first parameter value.</typeparam>
		/// <param name="parameterName1">Name of the first parameter.</param>
		/// <param name="parameterValue1">Name of the first parameter.</param>
			/// <typeparam name="T2">Type of the second parameter value.</typeparam>
		/// <param name="parameterName2">Name of the second parameter.</param>
		/// <param name="parameterValue2">Name of the second parameter.</param>
			/// <typeparam name="T3">Type of the third parameter value.</typeparam>
		/// <param name="parameterName3">Name of the third parameter.</param>
		/// <param name="parameterValue3">Name of the third parameter.</param>
			/// <typeparam name="T4">Type of the 4-th parameter value.</typeparam>
		/// <param name="parameterName4">Name of the 4-th parameter.</param>
		/// <param name="parameterValue4">Name of the 4-th parameter.</param>
			/// <typeparam name="T5">Type of the 5-th parameter value.</typeparam>
		/// <param name="parameterName5">Name of the 5-th parameter.</param>
		/// <param name="parameterValue5">Name of the 5-th parameter.</param>
			/// <typeparam name="T6">Type of the 6-th parameter value.</typeparam>
		/// <param name="parameterName6">Name of the 6-th parameter.</param>
		/// <param name="parameterValue6">Name of the 6-th parameter.</param>
	        [MethodImpl(MethodImplOptions.AggressiveInlining)] // To avoid copying the struct.
	    public static  SemanticMessage<T1, T2, T3, T4, T5, T6> Semantic<T1, T2, T3, T4, T5, T6>(  string name, string parameterName1, T1 parameterValue1, string parameterName2, T2 parameterValue2, string parameterName3, T3 parameterValue3, string parameterName4, T4 parameterValue4, string parameterName5, T5 parameterValue5, string parameterName6, T6 parameterValue6 )
        {
            return new SemanticMessage<T1, T2, T3, T4, T5, T6>( name, parameterName1, parameterValue1, parameterName2, parameterValue2, parameterName3, parameterValue3, parameterName4, parameterValue4, parameterName5, parameterValue5, parameterName6, parameterValue6 );
        }

		/// <summary>
        /// Create a semantic message with 6 parameters.
        /// </summary>
        /// <param name="name">Name of the message.</param>
		/// <typeparam name="T1">Type of the first parameter value.</typeparam>
		/// <param name="parameter1">Name and value of the first parameter wrapped as a tuple.</param>
			/// <typeparam name="T2">Type of the second parameter value.</typeparam>
		/// <param name="parameter2">Name and value of the second parameter wrapped as a tuple.</param>
			/// <typeparam name="T3">Type of the third parameter value.</typeparam>
		/// <param name="parameter3">Name and value of the third parameter wrapped as a tuple.</param>
			/// <typeparam name="T4">Type of the 4-th parameter value.</typeparam>
		/// <param name="parameter4">Name and value of the 4-th parameter wrapped as a tuple.</param>
			/// <typeparam name="T5">Type of the 5-th parameter value.</typeparam>
		/// <param name="parameter5">Name and value of the 5-th parameter wrapped as a tuple.</param>
			/// <typeparam name="T6">Type of the 6-th parameter value.</typeparam>
		/// <param name="parameter6">Name and value of the 6-th parameter wrapped as a tuple.</param>
	        [MethodImpl(MethodImplOptions.AggressiveInlining)] // To avoid copying the struct.
	    public static  SemanticMessage<T1, T2, T3, T4, T5, T6> Semantic<T1, T2, T3, T4, T5, T6>(  string name, in (string Name,T1 Value) parameter1, in (string Name,T2 Value) parameter2, in (string Name,T3 Value) parameter3, in (string Name,T4 Value) parameter4, in (string Name,T5 Value) parameter5, in (string Name,T6 Value) parameter6 )
        {
            return new SemanticMessage<T1, T2, T3, T4, T5, T6>( name, parameter1.Name, parameter1.Value, parameter2.Name, parameter2.Value, parameter3.Name, parameter3.Value, parameter4.Name, parameter4.Value, parameter5.Name, parameter5.Value, parameter6.Name, parameter6.Value );
        }
	}
	
	namespace Custom.Messages
	{
		/// <summary>
		/// Encapsulates a text message with 7 parameters. Use the <see cref="FormattedMessageBuilder"/> class to create an instance of this type.
		/// </summary>
	    [SuppressMessage("Microsoft.Performance","CA1815", Justification = "Equal is not a use case")]
	    [SuppressMessage("Microsoft.Design","CA1005:AvoidExcessiveParametersOnGenericTypes", Justification = "Recommended to use 'var' keyword.")]
		public readonly struct FormattedMessage<T1, T2, T3, T4, T5, T6, T7> : IMessage
		{
			private readonly string formattingString;
			
			private readonly T1 arg1; private readonly T2 arg2; private readonly T3 arg3; private readonly T4 arg4; private readonly T5 arg5; private readonly T6 arg6; private readonly T7 arg7; 

			
			[MethodImpl(MethodImplOptions.AggressiveInlining)] // To avoid copying the struct.
			internal FormattedMessage( string formattingString, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7 )
			{
				this.formattingString = formattingString;
				this.arg1 = arg1; this.arg2 = arg2; this.arg3 = arg3; this.arg4 = arg4; this.arg5 = arg5; this.arg6 = arg6; this.arg7 = arg7; 
			}

			void IMessage.Write( ICustomLogRecordBuilder recordBuilder, CustomLogRecordItem item )
			{
				recordBuilder.BeginWriteItem(item, new CustomLogRecordTextOptions(7, null));
    
				FormattingStringParser parser = new FormattingStringParser( this.formattingString );
				ArraySegment<char> parameter;

				   
				recordBuilder.WriteCustomString( parser.GetNextSubstring() );
				parameter = parser.GetNextParameter();

				if ( parameter.Array == null )
				{
					throw new InvalidFormattingStringException("The formatting string must have exactly 7 parameters.");
				}

				recordBuilder.WriteCustomParameter( 0, parameter, arg1, CustomLogParameterOptions.FormattedStringParameter );

				
				recordBuilder.WriteCustomString( parser.GetNextSubstring() );
				parameter = parser.GetNextParameter();

				if ( parameter.Array == null )
				{
					throw new InvalidFormattingStringException("The formatting string must have exactly 7 parameters.");
				}

				recordBuilder.WriteCustomParameter( 1, parameter, arg2, CustomLogParameterOptions.FormattedStringParameter );

				
				recordBuilder.WriteCustomString( parser.GetNextSubstring() );
				parameter = parser.GetNextParameter();

				if ( parameter.Array == null )
				{
					throw new InvalidFormattingStringException("The formatting string must have exactly 7 parameters.");
				}

				recordBuilder.WriteCustomParameter( 2, parameter, arg3, CustomLogParameterOptions.FormattedStringParameter );

				
				recordBuilder.WriteCustomString( parser.GetNextSubstring() );
				parameter = parser.GetNextParameter();

				if ( parameter.Array == null )
				{
					throw new InvalidFormattingStringException("The formatting string must have exactly 7 parameters.");
				}

				recordBuilder.WriteCustomParameter( 3, parameter, arg4, CustomLogParameterOptions.FormattedStringParameter );

				
				recordBuilder.WriteCustomString( parser.GetNextSubstring() );
				parameter = parser.GetNextParameter();

				if ( parameter.Array == null )
				{
					throw new InvalidFormattingStringException("The formatting string must have exactly 7 parameters.");
				}

				recordBuilder.WriteCustomParameter( 4, parameter, arg5, CustomLogParameterOptions.FormattedStringParameter );

				
				recordBuilder.WriteCustomString( parser.GetNextSubstring() );
				parameter = parser.GetNextParameter();

				if ( parameter.Array == null )
				{
					throw new InvalidFormattingStringException("The formatting string must have exactly 7 parameters.");
				}

				recordBuilder.WriteCustomParameter( 5, parameter, arg6, CustomLogParameterOptions.FormattedStringParameter );

				
				recordBuilder.WriteCustomString( parser.GetNextSubstring() );
				parameter = parser.GetNextParameter();

				if ( parameter.Array == null )
				{
					throw new InvalidFormattingStringException("The formatting string must have exactly 7 parameters.");
				}

				recordBuilder.WriteCustomParameter( 6, parameter, arg7, CustomLogParameterOptions.FormattedStringParameter );

				
				recordBuilder.WriteCustomString( parser.GetNextSubstring() );

				if ( parser.GetNextParameter().Array != null )
				{
					throw new InvalidFormattingStringException("The formatting string must have exactly 7 parameters.");
				}

				recordBuilder.EndWriteItem(item);


			}

			/// <inheritdoc />
			public override string ToString() => DebugMessageFormatter.Format( this );
		}

		 /// <summary>
		/// Encapsulates a semantic message with a 7 number of parameters. Use the <see cref="SemanticMessageBuilder"/> class to create an instance of this type.
		/// </summary>
		[SuppressMessage("Microsoft.Performance","CA1815", Justification = "Equal is not a use case")]
		[SuppressMessage("Microsoft.Design","CA1005:AvoidExcessiveParametersOnGenericTypes", Justification = "Recommended to use 'var' keyword.")]
		public readonly struct SemanticMessage<T1, T2, T3, T4, T5, T6, T7> : IMessage
		{
			private readonly string messageName;
			
			private readonly string name1; private readonly string name2; private readonly string name3; private readonly string name4; private readonly string name5; private readonly string name6; private readonly string name7; 
			private readonly T1 value1; private readonly T2 value2; private readonly T3 value3; private readonly T4 value4; private readonly T5 value5; private readonly T6 value6; private readonly T7 value7; 

			[MethodImpl(MethodImplOptions.AggressiveInlining)] // To avoid copying the struct.
			internal SemanticMessage( string messageName, string name1, T1 value1, string name2, T2 value2, string name3, T3 value3, string name4, T4 value4, string name5, T5 value5, string name6, T6 value6, string name7, T7 value7 )
			{
				this.messageName = messageName;
				this.name1 = name1; this.name2 = name2; this.name3 = name3; this.name4 = name4; this.name5 = name5; this.name6 = name6; this.name7 = name7; 
				this.value1 = value1; this.value2 = value2; this.value3 = value3; this.value4 = value4; this.value5 = value5; this.value6 = value6; this.value7 = value7; 
			}

			void IMessage.Write( ICustomLogRecordBuilder recordBuilder, CustomLogRecordItem item )
			{
				recordBuilder.BeginWriteItem(item, new CustomLogRecordTextOptions(7, this.messageName));

				   
				recordBuilder.WriteCustomParameter( 0, this.name1, this.value1, CustomLogParameterOptions.SemanticParameter );

				
				recordBuilder.WriteCustomParameter( 1, this.name2, this.value2, CustomLogParameterOptions.SemanticParameter );

				
				recordBuilder.WriteCustomParameter( 2, this.name3, this.value3, CustomLogParameterOptions.SemanticParameter );

				
				recordBuilder.WriteCustomParameter( 3, this.name4, this.value4, CustomLogParameterOptions.SemanticParameter );

				
				recordBuilder.WriteCustomParameter( 4, this.name5, this.value5, CustomLogParameterOptions.SemanticParameter );

				
				recordBuilder.WriteCustomParameter( 5, this.name6, this.value6, CustomLogParameterOptions.SemanticParameter );

				
				recordBuilder.WriteCustomParameter( 6, this.name7, this.value7, CustomLogParameterOptions.SemanticParameter );

				
				recordBuilder.EndWriteItem(item);
				
			}

			/// <inheritdoc />
			public override string ToString() => DebugMessageFormatter.Format( this );
		}

		
	
	}



	partial class FormattedMessageBuilder
	{
		
		/// <summary>
        /// Creates a formatted string with 7 parameters.
        /// </summary>
        /// <param name="formattingString">The text of the log record, including parameters (e.g. <c>Opening {Path} file {ShareMode} sharing mode</c>).</param>
		/// <typeparam name="T1">Type of the first parameter.</typeparam>
		/// <param name="arg1">Value of the first parameter.</param>
			/// <typeparam name="T2">Type of the second parameter.</typeparam>
		/// <param name="arg2">Value of the second parameter.</param>
			/// <typeparam name="T3">Type of the third parameter.</typeparam>
		/// <param name="arg3">Value of the third parameter.</param>
			/// <typeparam name="T4">Type of the 4-th parameter.</typeparam>
		/// <param name="arg4">Value of the 4-th parameter.</param>
			/// <typeparam name="T5">Type of the 5-th parameter.</typeparam>
		/// <param name="arg5">Value of the 5-th parameter.</param>
			/// <typeparam name="T6">Type of the 6-th parameter.</typeparam>
		/// <param name="arg6">Value of the 6-th parameter.</param>
			/// <typeparam name="T7">Type of the 7-th parameter.</typeparam>
		/// <param name="arg7">Value of the 7-th parameter.</param>
	        [MethodImpl(MethodImplOptions.AggressiveInlining)] // To avoid copying the struct.
	    public static FormattedMessage<T1, T2, T3, T4, T5, T6, T7> Formatted<T1, T2, T3, T4, T5, T6, T7>(  string formattingString, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7 )
        {
            return new FormattedMessage<T1, T2, T3, T4, T5, T6, T7>( formattingString, arg1, arg2, arg3, arg4, arg5, arg6, arg7 );
        }
	}

	partial class SemanticMessageBuilder
	{
		/// <summary>
        /// Create a semantic message with 7 parameters (using tuples).
        /// </summary>
        /// <param name="name">Name of the message.</param>
		/// <typeparam name="T1">Type of the first parameter value.</typeparam>
		/// <param name="parameterName1">Name of the first parameter.</param>
		/// <param name="parameterValue1">Name of the first parameter.</param>
			/// <typeparam name="T2">Type of the second parameter value.</typeparam>
		/// <param name="parameterName2">Name of the second parameter.</param>
		/// <param name="parameterValue2">Name of the second parameter.</param>
			/// <typeparam name="T3">Type of the third parameter value.</typeparam>
		/// <param name="parameterName3">Name of the third parameter.</param>
		/// <param name="parameterValue3">Name of the third parameter.</param>
			/// <typeparam name="T4">Type of the 4-th parameter value.</typeparam>
		/// <param name="parameterName4">Name of the 4-th parameter.</param>
		/// <param name="parameterValue4">Name of the 4-th parameter.</param>
			/// <typeparam name="T5">Type of the 5-th parameter value.</typeparam>
		/// <param name="parameterName5">Name of the 5-th parameter.</param>
		/// <param name="parameterValue5">Name of the 5-th parameter.</param>
			/// <typeparam name="T6">Type of the 6-th parameter value.</typeparam>
		/// <param name="parameterName6">Name of the 6-th parameter.</param>
		/// <param name="parameterValue6">Name of the 6-th parameter.</param>
			/// <typeparam name="T7">Type of the 7-th parameter value.</typeparam>
		/// <param name="parameterName7">Name of the 7-th parameter.</param>
		/// <param name="parameterValue7">Name of the 7-th parameter.</param>
	        [MethodImpl(MethodImplOptions.AggressiveInlining)] // To avoid copying the struct.
	    public static  SemanticMessage<T1, T2, T3, T4, T5, T6, T7> Semantic<T1, T2, T3, T4, T5, T6, T7>(  string name, string parameterName1, T1 parameterValue1, string parameterName2, T2 parameterValue2, string parameterName3, T3 parameterValue3, string parameterName4, T4 parameterValue4, string parameterName5, T5 parameterValue5, string parameterName6, T6 parameterValue6, string parameterName7, T7 parameterValue7 )
        {
            return new SemanticMessage<T1, T2, T3, T4, T5, T6, T7>( name, parameterName1, parameterValue1, parameterName2, parameterValue2, parameterName3, parameterValue3, parameterName4, parameterValue4, parameterName5, parameterValue5, parameterName6, parameterValue6, parameterName7, parameterValue7 );
        }

		/// <summary>
        /// Create a semantic message with 7 parameters.
        /// </summary>
        /// <param name="name">Name of the message.</param>
		/// <typeparam name="T1">Type of the first parameter value.</typeparam>
		/// <param name="parameter1">Name and value of the first parameter wrapped as a tuple.</param>
			/// <typeparam name="T2">Type of the second parameter value.</typeparam>
		/// <param name="parameter2">Name and value of the second parameter wrapped as a tuple.</param>
			/// <typeparam name="T3">Type of the third parameter value.</typeparam>
		/// <param name="parameter3">Name and value of the third parameter wrapped as a tuple.</param>
			/// <typeparam name="T4">Type of the 4-th parameter value.</typeparam>
		/// <param name="parameter4">Name and value of the 4-th parameter wrapped as a tuple.</param>
			/// <typeparam name="T5">Type of the 5-th parameter value.</typeparam>
		/// <param name="parameter5">Name and value of the 5-th parameter wrapped as a tuple.</param>
			/// <typeparam name="T6">Type of the 6-th parameter value.</typeparam>
		/// <param name="parameter6">Name and value of the 6-th parameter wrapped as a tuple.</param>
			/// <typeparam name="T7">Type of the 7-th parameter value.</typeparam>
		/// <param name="parameter7">Name and value of the 7-th parameter wrapped as a tuple.</param>
	        [MethodImpl(MethodImplOptions.AggressiveInlining)] // To avoid copying the struct.
	    public static  SemanticMessage<T1, T2, T3, T4, T5, T6, T7> Semantic<T1, T2, T3, T4, T5, T6, T7>(  string name, in (string Name,T1 Value) parameter1, in (string Name,T2 Value) parameter2, in (string Name,T3 Value) parameter3, in (string Name,T4 Value) parameter4, in (string Name,T5 Value) parameter5, in (string Name,T6 Value) parameter6, in (string Name,T7 Value) parameter7 )
        {
            return new SemanticMessage<T1, T2, T3, T4, T5, T6, T7>( name, parameter1.Name, parameter1.Value, parameter2.Name, parameter2.Value, parameter3.Name, parameter3.Value, parameter4.Name, parameter4.Value, parameter5.Name, parameter5.Value, parameter6.Name, parameter6.Value, parameter7.Name, parameter7.Value );
        }
	}
	
	namespace Custom.Messages
	{
		/// <summary>
		/// Encapsulates a text message with 8 parameters. Use the <see cref="FormattedMessageBuilder"/> class to create an instance of this type.
		/// </summary>
	    [SuppressMessage("Microsoft.Performance","CA1815", Justification = "Equal is not a use case")]
	    [SuppressMessage("Microsoft.Design","CA1005:AvoidExcessiveParametersOnGenericTypes", Justification = "Recommended to use 'var' keyword.")]
		public readonly struct FormattedMessage<T1, T2, T3, T4, T5, T6, T7, T8> : IMessage
		{
			private readonly string formattingString;
			
			private readonly T1 arg1; private readonly T2 arg2; private readonly T3 arg3; private readonly T4 arg4; private readonly T5 arg5; private readonly T6 arg6; private readonly T7 arg7; private readonly T8 arg8; 

			
			[MethodImpl(MethodImplOptions.AggressiveInlining)] // To avoid copying the struct.
			internal FormattedMessage( string formattingString, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8 )
			{
				this.formattingString = formattingString;
				this.arg1 = arg1; this.arg2 = arg2; this.arg3 = arg3; this.arg4 = arg4; this.arg5 = arg5; this.arg6 = arg6; this.arg7 = arg7; this.arg8 = arg8; 
			}

			void IMessage.Write( ICustomLogRecordBuilder recordBuilder, CustomLogRecordItem item )
			{
				recordBuilder.BeginWriteItem(item, new CustomLogRecordTextOptions(8, null));
    
				FormattingStringParser parser = new FormattingStringParser( this.formattingString );
				ArraySegment<char> parameter;

				   
				recordBuilder.WriteCustomString( parser.GetNextSubstring() );
				parameter = parser.GetNextParameter();

				if ( parameter.Array == null )
				{
					throw new InvalidFormattingStringException("The formatting string must have exactly 8 parameters.");
				}

				recordBuilder.WriteCustomParameter( 0, parameter, arg1, CustomLogParameterOptions.FormattedStringParameter );

				
				recordBuilder.WriteCustomString( parser.GetNextSubstring() );
				parameter = parser.GetNextParameter();

				if ( parameter.Array == null )
				{
					throw new InvalidFormattingStringException("The formatting string must have exactly 8 parameters.");
				}

				recordBuilder.WriteCustomParameter( 1, parameter, arg2, CustomLogParameterOptions.FormattedStringParameter );

				
				recordBuilder.WriteCustomString( parser.GetNextSubstring() );
				parameter = parser.GetNextParameter();

				if ( parameter.Array == null )
				{
					throw new InvalidFormattingStringException("The formatting string must have exactly 8 parameters.");
				}

				recordBuilder.WriteCustomParameter( 2, parameter, arg3, CustomLogParameterOptions.FormattedStringParameter );

				
				recordBuilder.WriteCustomString( parser.GetNextSubstring() );
				parameter = parser.GetNextParameter();

				if ( parameter.Array == null )
				{
					throw new InvalidFormattingStringException("The formatting string must have exactly 8 parameters.");
				}

				recordBuilder.WriteCustomParameter( 3, parameter, arg4, CustomLogParameterOptions.FormattedStringParameter );

				
				recordBuilder.WriteCustomString( parser.GetNextSubstring() );
				parameter = parser.GetNextParameter();

				if ( parameter.Array == null )
				{
					throw new InvalidFormattingStringException("The formatting string must have exactly 8 parameters.");
				}

				recordBuilder.WriteCustomParameter( 4, parameter, arg5, CustomLogParameterOptions.FormattedStringParameter );

				
				recordBuilder.WriteCustomString( parser.GetNextSubstring() );
				parameter = parser.GetNextParameter();

				if ( parameter.Array == null )
				{
					throw new InvalidFormattingStringException("The formatting string must have exactly 8 parameters.");
				}

				recordBuilder.WriteCustomParameter( 5, parameter, arg6, CustomLogParameterOptions.FormattedStringParameter );

				
				recordBuilder.WriteCustomString( parser.GetNextSubstring() );
				parameter = parser.GetNextParameter();

				if ( parameter.Array == null )
				{
					throw new InvalidFormattingStringException("The formatting string must have exactly 8 parameters.");
				}

				recordBuilder.WriteCustomParameter( 6, parameter, arg7, CustomLogParameterOptions.FormattedStringParameter );

				
				recordBuilder.WriteCustomString( parser.GetNextSubstring() );
				parameter = parser.GetNextParameter();

				if ( parameter.Array == null )
				{
					throw new InvalidFormattingStringException("The formatting string must have exactly 8 parameters.");
				}

				recordBuilder.WriteCustomParameter( 7, parameter, arg8, CustomLogParameterOptions.FormattedStringParameter );

				
				recordBuilder.WriteCustomString( parser.GetNextSubstring() );

				if ( parser.GetNextParameter().Array != null )
				{
					throw new InvalidFormattingStringException("The formatting string must have exactly 8 parameters.");
				}

				recordBuilder.EndWriteItem(item);


			}

			/// <inheritdoc />
			public override string ToString() => DebugMessageFormatter.Format( this );
		}

		 /// <summary>
		/// Encapsulates a semantic message with a 8 number of parameters. Use the <see cref="SemanticMessageBuilder"/> class to create an instance of this type.
		/// </summary>
		[SuppressMessage("Microsoft.Performance","CA1815", Justification = "Equal is not a use case")]
		[SuppressMessage("Microsoft.Design","CA1005:AvoidExcessiveParametersOnGenericTypes", Justification = "Recommended to use 'var' keyword.")]
		public readonly struct SemanticMessage<T1, T2, T3, T4, T5, T6, T7, T8> : IMessage
		{
			private readonly string messageName;
			
			private readonly string name1; private readonly string name2; private readonly string name3; private readonly string name4; private readonly string name5; private readonly string name6; private readonly string name7; private readonly string name8; 
			private readonly T1 value1; private readonly T2 value2; private readonly T3 value3; private readonly T4 value4; private readonly T5 value5; private readonly T6 value6; private readonly T7 value7; private readonly T8 value8; 

			[MethodImpl(MethodImplOptions.AggressiveInlining)] // To avoid copying the struct.
			internal SemanticMessage( string messageName, string name1, T1 value1, string name2, T2 value2, string name3, T3 value3, string name4, T4 value4, string name5, T5 value5, string name6, T6 value6, string name7, T7 value7, string name8, T8 value8 )
			{
				this.messageName = messageName;
				this.name1 = name1; this.name2 = name2; this.name3 = name3; this.name4 = name4; this.name5 = name5; this.name6 = name6; this.name7 = name7; this.name8 = name8; 
				this.value1 = value1; this.value2 = value2; this.value3 = value3; this.value4 = value4; this.value5 = value5; this.value6 = value6; this.value7 = value7; this.value8 = value8; 
			}

			void IMessage.Write( ICustomLogRecordBuilder recordBuilder, CustomLogRecordItem item )
			{
				recordBuilder.BeginWriteItem(item, new CustomLogRecordTextOptions(8, this.messageName));

				   
				recordBuilder.WriteCustomParameter( 0, this.name1, this.value1, CustomLogParameterOptions.SemanticParameter );

				
				recordBuilder.WriteCustomParameter( 1, this.name2, this.value2, CustomLogParameterOptions.SemanticParameter );

				
				recordBuilder.WriteCustomParameter( 2, this.name3, this.value3, CustomLogParameterOptions.SemanticParameter );

				
				recordBuilder.WriteCustomParameter( 3, this.name4, this.value4, CustomLogParameterOptions.SemanticParameter );

				
				recordBuilder.WriteCustomParameter( 4, this.name5, this.value5, CustomLogParameterOptions.SemanticParameter );

				
				recordBuilder.WriteCustomParameter( 5, this.name6, this.value6, CustomLogParameterOptions.SemanticParameter );

				
				recordBuilder.WriteCustomParameter( 6, this.name7, this.value7, CustomLogParameterOptions.SemanticParameter );

				
				recordBuilder.WriteCustomParameter( 7, this.name8, this.value8, CustomLogParameterOptions.SemanticParameter );

				
				recordBuilder.EndWriteItem(item);
				
			}

			/// <inheritdoc />
			public override string ToString() => DebugMessageFormatter.Format( this );
		}

		
	
	}



	partial class FormattedMessageBuilder
	{
		
		/// <summary>
        /// Creates a formatted string with 8 parameters.
        /// </summary>
        /// <param name="formattingString">The text of the log record, including parameters (e.g. <c>Opening {Path} file {ShareMode} sharing mode</c>).</param>
		/// <typeparam name="T1">Type of the first parameter.</typeparam>
		/// <param name="arg1">Value of the first parameter.</param>
			/// <typeparam name="T2">Type of the second parameter.</typeparam>
		/// <param name="arg2">Value of the second parameter.</param>
			/// <typeparam name="T3">Type of the third parameter.</typeparam>
		/// <param name="arg3">Value of the third parameter.</param>
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
	        [MethodImpl(MethodImplOptions.AggressiveInlining)] // To avoid copying the struct.
	    public static FormattedMessage<T1, T2, T3, T4, T5, T6, T7, T8> Formatted<T1, T2, T3, T4, T5, T6, T7, T8>(  string formattingString, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8 )
        {
            return new FormattedMessage<T1, T2, T3, T4, T5, T6, T7, T8>( formattingString, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8 );
        }
	}

	partial class SemanticMessageBuilder
	{
		/// <summary>
        /// Create a semantic message with 8 parameters (using tuples).
        /// </summary>
        /// <param name="name">Name of the message.</param>
		/// <typeparam name="T1">Type of the first parameter value.</typeparam>
		/// <param name="parameterName1">Name of the first parameter.</param>
		/// <param name="parameterValue1">Name of the first parameter.</param>
			/// <typeparam name="T2">Type of the second parameter value.</typeparam>
		/// <param name="parameterName2">Name of the second parameter.</param>
		/// <param name="parameterValue2">Name of the second parameter.</param>
			/// <typeparam name="T3">Type of the third parameter value.</typeparam>
		/// <param name="parameterName3">Name of the third parameter.</param>
		/// <param name="parameterValue3">Name of the third parameter.</param>
			/// <typeparam name="T4">Type of the 4-th parameter value.</typeparam>
		/// <param name="parameterName4">Name of the 4-th parameter.</param>
		/// <param name="parameterValue4">Name of the 4-th parameter.</param>
			/// <typeparam name="T5">Type of the 5-th parameter value.</typeparam>
		/// <param name="parameterName5">Name of the 5-th parameter.</param>
		/// <param name="parameterValue5">Name of the 5-th parameter.</param>
			/// <typeparam name="T6">Type of the 6-th parameter value.</typeparam>
		/// <param name="parameterName6">Name of the 6-th parameter.</param>
		/// <param name="parameterValue6">Name of the 6-th parameter.</param>
			/// <typeparam name="T7">Type of the 7-th parameter value.</typeparam>
		/// <param name="parameterName7">Name of the 7-th parameter.</param>
		/// <param name="parameterValue7">Name of the 7-th parameter.</param>
			/// <typeparam name="T8">Type of the 8-th parameter value.</typeparam>
		/// <param name="parameterName8">Name of the 8-th parameter.</param>
		/// <param name="parameterValue8">Name of the 8-th parameter.</param>
	        [MethodImpl(MethodImplOptions.AggressiveInlining)] // To avoid copying the struct.
	    public static  SemanticMessage<T1, T2, T3, T4, T5, T6, T7, T8> Semantic<T1, T2, T3, T4, T5, T6, T7, T8>(  string name, string parameterName1, T1 parameterValue1, string parameterName2, T2 parameterValue2, string parameterName3, T3 parameterValue3, string parameterName4, T4 parameterValue4, string parameterName5, T5 parameterValue5, string parameterName6, T6 parameterValue6, string parameterName7, T7 parameterValue7, string parameterName8, T8 parameterValue8 )
        {
            return new SemanticMessage<T1, T2, T3, T4, T5, T6, T7, T8>( name, parameterName1, parameterValue1, parameterName2, parameterValue2, parameterName3, parameterValue3, parameterName4, parameterValue4, parameterName5, parameterValue5, parameterName6, parameterValue6, parameterName7, parameterValue7, parameterName8, parameterValue8 );
        }

		/// <summary>
        /// Create a semantic message with 8 parameters.
        /// </summary>
        /// <param name="name">Name of the message.</param>
		/// <typeparam name="T1">Type of the first parameter value.</typeparam>
		/// <param name="parameter1">Name and value of the first parameter wrapped as a tuple.</param>
			/// <typeparam name="T2">Type of the second parameter value.</typeparam>
		/// <param name="parameter2">Name and value of the second parameter wrapped as a tuple.</param>
			/// <typeparam name="T3">Type of the third parameter value.</typeparam>
		/// <param name="parameter3">Name and value of the third parameter wrapped as a tuple.</param>
			/// <typeparam name="T4">Type of the 4-th parameter value.</typeparam>
		/// <param name="parameter4">Name and value of the 4-th parameter wrapped as a tuple.</param>
			/// <typeparam name="T5">Type of the 5-th parameter value.</typeparam>
		/// <param name="parameter5">Name and value of the 5-th parameter wrapped as a tuple.</param>
			/// <typeparam name="T6">Type of the 6-th parameter value.</typeparam>
		/// <param name="parameter6">Name and value of the 6-th parameter wrapped as a tuple.</param>
			/// <typeparam name="T7">Type of the 7-th parameter value.</typeparam>
		/// <param name="parameter7">Name and value of the 7-th parameter wrapped as a tuple.</param>
			/// <typeparam name="T8">Type of the 8-th parameter value.</typeparam>
		/// <param name="parameter8">Name and value of the 8-th parameter wrapped as a tuple.</param>
	        [MethodImpl(MethodImplOptions.AggressiveInlining)] // To avoid copying the struct.
	    public static  SemanticMessage<T1, T2, T3, T4, T5, T6, T7, T8> Semantic<T1, T2, T3, T4, T5, T6, T7, T8>(  string name, in (string Name,T1 Value) parameter1, in (string Name,T2 Value) parameter2, in (string Name,T3 Value) parameter3, in (string Name,T4 Value) parameter4, in (string Name,T5 Value) parameter5, in (string Name,T6 Value) parameter6, in (string Name,T7 Value) parameter7, in (string Name,T8 Value) parameter8 )
        {
            return new SemanticMessage<T1, T2, T3, T4, T5, T6, T7, T8>( name, parameter1.Name, parameter1.Value, parameter2.Name, parameter2.Value, parameter3.Name, parameter3.Value, parameter4.Name, parameter4.Value, parameter5.Name, parameter5.Value, parameter6.Name, parameter6.Value, parameter7.Name, parameter7.Value, parameter8.Name, parameter8.Value );
        }
	}
	
	namespace Custom.Messages
	{
		/// <summary>
		/// Encapsulates a text message with 9 parameters. Use the <see cref="FormattedMessageBuilder"/> class to create an instance of this type.
		/// </summary>
	    [SuppressMessage("Microsoft.Performance","CA1815", Justification = "Equal is not a use case")]
	    [SuppressMessage("Microsoft.Design","CA1005:AvoidExcessiveParametersOnGenericTypes", Justification = "Recommended to use 'var' keyword.")]
		public readonly struct FormattedMessage<T1, T2, T3, T4, T5, T6, T7, T8, T9> : IMessage
		{
			private readonly string formattingString;
			
			private readonly T1 arg1; private readonly T2 arg2; private readonly T3 arg3; private readonly T4 arg4; private readonly T5 arg5; private readonly T6 arg6; private readonly T7 arg7; private readonly T8 arg8; private readonly T9 arg9; 

			
			[MethodImpl(MethodImplOptions.AggressiveInlining)] // To avoid copying the struct.
			internal FormattedMessage( string formattingString, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9 )
			{
				this.formattingString = formattingString;
				this.arg1 = arg1; this.arg2 = arg2; this.arg3 = arg3; this.arg4 = arg4; this.arg5 = arg5; this.arg6 = arg6; this.arg7 = arg7; this.arg8 = arg8; this.arg9 = arg9; 
			}

			void IMessage.Write( ICustomLogRecordBuilder recordBuilder, CustomLogRecordItem item )
			{
				recordBuilder.BeginWriteItem(item, new CustomLogRecordTextOptions(9, null));
    
				FormattingStringParser parser = new FormattingStringParser( this.formattingString );
				ArraySegment<char> parameter;

				   
				recordBuilder.WriteCustomString( parser.GetNextSubstring() );
				parameter = parser.GetNextParameter();

				if ( parameter.Array == null )
				{
					throw new InvalidFormattingStringException("The formatting string must have exactly 9 parameters.");
				}

				recordBuilder.WriteCustomParameter( 0, parameter, arg1, CustomLogParameterOptions.FormattedStringParameter );

				
				recordBuilder.WriteCustomString( parser.GetNextSubstring() );
				parameter = parser.GetNextParameter();

				if ( parameter.Array == null )
				{
					throw new InvalidFormattingStringException("The formatting string must have exactly 9 parameters.");
				}

				recordBuilder.WriteCustomParameter( 1, parameter, arg2, CustomLogParameterOptions.FormattedStringParameter );

				
				recordBuilder.WriteCustomString( parser.GetNextSubstring() );
				parameter = parser.GetNextParameter();

				if ( parameter.Array == null )
				{
					throw new InvalidFormattingStringException("The formatting string must have exactly 9 parameters.");
				}

				recordBuilder.WriteCustomParameter( 2, parameter, arg3, CustomLogParameterOptions.FormattedStringParameter );

				
				recordBuilder.WriteCustomString( parser.GetNextSubstring() );
				parameter = parser.GetNextParameter();

				if ( parameter.Array == null )
				{
					throw new InvalidFormattingStringException("The formatting string must have exactly 9 parameters.");
				}

				recordBuilder.WriteCustomParameter( 3, parameter, arg4, CustomLogParameterOptions.FormattedStringParameter );

				
				recordBuilder.WriteCustomString( parser.GetNextSubstring() );
				parameter = parser.GetNextParameter();

				if ( parameter.Array == null )
				{
					throw new InvalidFormattingStringException("The formatting string must have exactly 9 parameters.");
				}

				recordBuilder.WriteCustomParameter( 4, parameter, arg5, CustomLogParameterOptions.FormattedStringParameter );

				
				recordBuilder.WriteCustomString( parser.GetNextSubstring() );
				parameter = parser.GetNextParameter();

				if ( parameter.Array == null )
				{
					throw new InvalidFormattingStringException("The formatting string must have exactly 9 parameters.");
				}

				recordBuilder.WriteCustomParameter( 5, parameter, arg6, CustomLogParameterOptions.FormattedStringParameter );

				
				recordBuilder.WriteCustomString( parser.GetNextSubstring() );
				parameter = parser.GetNextParameter();

				if ( parameter.Array == null )
				{
					throw new InvalidFormattingStringException("The formatting string must have exactly 9 parameters.");
				}

				recordBuilder.WriteCustomParameter( 6, parameter, arg7, CustomLogParameterOptions.FormattedStringParameter );

				
				recordBuilder.WriteCustomString( parser.GetNextSubstring() );
				parameter = parser.GetNextParameter();

				if ( parameter.Array == null )
				{
					throw new InvalidFormattingStringException("The formatting string must have exactly 9 parameters.");
				}

				recordBuilder.WriteCustomParameter( 7, parameter, arg8, CustomLogParameterOptions.FormattedStringParameter );

				
				recordBuilder.WriteCustomString( parser.GetNextSubstring() );
				parameter = parser.GetNextParameter();

				if ( parameter.Array == null )
				{
					throw new InvalidFormattingStringException("The formatting string must have exactly 9 parameters.");
				}

				recordBuilder.WriteCustomParameter( 8, parameter, arg9, CustomLogParameterOptions.FormattedStringParameter );

				
				recordBuilder.WriteCustomString( parser.GetNextSubstring() );

				if ( parser.GetNextParameter().Array != null )
				{
					throw new InvalidFormattingStringException("The formatting string must have exactly 9 parameters.");
				}

				recordBuilder.EndWriteItem(item);


			}

			/// <inheritdoc />
			public override string ToString() => DebugMessageFormatter.Format( this );
		}

		 /// <summary>
		/// Encapsulates a semantic message with a 9 number of parameters. Use the <see cref="SemanticMessageBuilder"/> class to create an instance of this type.
		/// </summary>
		[SuppressMessage("Microsoft.Performance","CA1815", Justification = "Equal is not a use case")]
		[SuppressMessage("Microsoft.Design","CA1005:AvoidExcessiveParametersOnGenericTypes", Justification = "Recommended to use 'var' keyword.")]
		public readonly struct SemanticMessage<T1, T2, T3, T4, T5, T6, T7, T8, T9> : IMessage
		{
			private readonly string messageName;
			
			private readonly string name1; private readonly string name2; private readonly string name3; private readonly string name4; private readonly string name5; private readonly string name6; private readonly string name7; private readonly string name8; private readonly string name9; 
			private readonly T1 value1; private readonly T2 value2; private readonly T3 value3; private readonly T4 value4; private readonly T5 value5; private readonly T6 value6; private readonly T7 value7; private readonly T8 value8; private readonly T9 value9; 

			[MethodImpl(MethodImplOptions.AggressiveInlining)] // To avoid copying the struct.
			internal SemanticMessage( string messageName, string name1, T1 value1, string name2, T2 value2, string name3, T3 value3, string name4, T4 value4, string name5, T5 value5, string name6, T6 value6, string name7, T7 value7, string name8, T8 value8, string name9, T9 value9 )
			{
				this.messageName = messageName;
				this.name1 = name1; this.name2 = name2; this.name3 = name3; this.name4 = name4; this.name5 = name5; this.name6 = name6; this.name7 = name7; this.name8 = name8; this.name9 = name9; 
				this.value1 = value1; this.value2 = value2; this.value3 = value3; this.value4 = value4; this.value5 = value5; this.value6 = value6; this.value7 = value7; this.value8 = value8; this.value9 = value9; 
			}

			void IMessage.Write( ICustomLogRecordBuilder recordBuilder, CustomLogRecordItem item )
			{
				recordBuilder.BeginWriteItem(item, new CustomLogRecordTextOptions(9, this.messageName));

				   
				recordBuilder.WriteCustomParameter( 0, this.name1, this.value1, CustomLogParameterOptions.SemanticParameter );

				
				recordBuilder.WriteCustomParameter( 1, this.name2, this.value2, CustomLogParameterOptions.SemanticParameter );

				
				recordBuilder.WriteCustomParameter( 2, this.name3, this.value3, CustomLogParameterOptions.SemanticParameter );

				
				recordBuilder.WriteCustomParameter( 3, this.name4, this.value4, CustomLogParameterOptions.SemanticParameter );

				
				recordBuilder.WriteCustomParameter( 4, this.name5, this.value5, CustomLogParameterOptions.SemanticParameter );

				
				recordBuilder.WriteCustomParameter( 5, this.name6, this.value6, CustomLogParameterOptions.SemanticParameter );

				
				recordBuilder.WriteCustomParameter( 6, this.name7, this.value7, CustomLogParameterOptions.SemanticParameter );

				
				recordBuilder.WriteCustomParameter( 7, this.name8, this.value8, CustomLogParameterOptions.SemanticParameter );

				
				recordBuilder.WriteCustomParameter( 8, this.name9, this.value9, CustomLogParameterOptions.SemanticParameter );

				
				recordBuilder.EndWriteItem(item);
				
			}

			/// <inheritdoc />
			public override string ToString() => DebugMessageFormatter.Format( this );
		}

		
	
	}



	partial class FormattedMessageBuilder
	{
		
		/// <summary>
        /// Creates a formatted string with 9 parameters.
        /// </summary>
        /// <param name="formattingString">The text of the log record, including parameters (e.g. <c>Opening {Path} file {ShareMode} sharing mode</c>).</param>
		/// <typeparam name="T1">Type of the first parameter.</typeparam>
		/// <param name="arg1">Value of the first parameter.</param>
			/// <typeparam name="T2">Type of the second parameter.</typeparam>
		/// <param name="arg2">Value of the second parameter.</param>
			/// <typeparam name="T3">Type of the third parameter.</typeparam>
		/// <param name="arg3">Value of the third parameter.</param>
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
	        [MethodImpl(MethodImplOptions.AggressiveInlining)] // To avoid copying the struct.
	    public static FormattedMessage<T1, T2, T3, T4, T5, T6, T7, T8, T9> Formatted<T1, T2, T3, T4, T5, T6, T7, T8, T9>(  string formattingString, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9 )
        {
            return new FormattedMessage<T1, T2, T3, T4, T5, T6, T7, T8, T9>( formattingString, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9 );
        }
	}

	partial class SemanticMessageBuilder
	{
		/// <summary>
        /// Create a semantic message with 9 parameters (using tuples).
        /// </summary>
        /// <param name="name">Name of the message.</param>
		/// <typeparam name="T1">Type of the first parameter value.</typeparam>
		/// <param name="parameterName1">Name of the first parameter.</param>
		/// <param name="parameterValue1">Name of the first parameter.</param>
			/// <typeparam name="T2">Type of the second parameter value.</typeparam>
		/// <param name="parameterName2">Name of the second parameter.</param>
		/// <param name="parameterValue2">Name of the second parameter.</param>
			/// <typeparam name="T3">Type of the third parameter value.</typeparam>
		/// <param name="parameterName3">Name of the third parameter.</param>
		/// <param name="parameterValue3">Name of the third parameter.</param>
			/// <typeparam name="T4">Type of the 4-th parameter value.</typeparam>
		/// <param name="parameterName4">Name of the 4-th parameter.</param>
		/// <param name="parameterValue4">Name of the 4-th parameter.</param>
			/// <typeparam name="T5">Type of the 5-th parameter value.</typeparam>
		/// <param name="parameterName5">Name of the 5-th parameter.</param>
		/// <param name="parameterValue5">Name of the 5-th parameter.</param>
			/// <typeparam name="T6">Type of the 6-th parameter value.</typeparam>
		/// <param name="parameterName6">Name of the 6-th parameter.</param>
		/// <param name="parameterValue6">Name of the 6-th parameter.</param>
			/// <typeparam name="T7">Type of the 7-th parameter value.</typeparam>
		/// <param name="parameterName7">Name of the 7-th parameter.</param>
		/// <param name="parameterValue7">Name of the 7-th parameter.</param>
			/// <typeparam name="T8">Type of the 8-th parameter value.</typeparam>
		/// <param name="parameterName8">Name of the 8-th parameter.</param>
		/// <param name="parameterValue8">Name of the 8-th parameter.</param>
			/// <typeparam name="T9">Type of the 9-th parameter value.</typeparam>
		/// <param name="parameterName9">Name of the 9-th parameter.</param>
		/// <param name="parameterValue9">Name of the 9-th parameter.</param>
	        [MethodImpl(MethodImplOptions.AggressiveInlining)] // To avoid copying the struct.
	    public static  SemanticMessage<T1, T2, T3, T4, T5, T6, T7, T8, T9> Semantic<T1, T2, T3, T4, T5, T6, T7, T8, T9>(  string name, string parameterName1, T1 parameterValue1, string parameterName2, T2 parameterValue2, string parameterName3, T3 parameterValue3, string parameterName4, T4 parameterValue4, string parameterName5, T5 parameterValue5, string parameterName6, T6 parameterValue6, string parameterName7, T7 parameterValue7, string parameterName8, T8 parameterValue8, string parameterName9, T9 parameterValue9 )
        {
            return new SemanticMessage<T1, T2, T3, T4, T5, T6, T7, T8, T9>( name, parameterName1, parameterValue1, parameterName2, parameterValue2, parameterName3, parameterValue3, parameterName4, parameterValue4, parameterName5, parameterValue5, parameterName6, parameterValue6, parameterName7, parameterValue7, parameterName8, parameterValue8, parameterName9, parameterValue9 );
        }

		/// <summary>
        /// Create a semantic message with 9 parameters.
        /// </summary>
        /// <param name="name">Name of the message.</param>
		/// <typeparam name="T1">Type of the first parameter value.</typeparam>
		/// <param name="parameter1">Name and value of the first parameter wrapped as a tuple.</param>
			/// <typeparam name="T2">Type of the second parameter value.</typeparam>
		/// <param name="parameter2">Name and value of the second parameter wrapped as a tuple.</param>
			/// <typeparam name="T3">Type of the third parameter value.</typeparam>
		/// <param name="parameter3">Name and value of the third parameter wrapped as a tuple.</param>
			/// <typeparam name="T4">Type of the 4-th parameter value.</typeparam>
		/// <param name="parameter4">Name and value of the 4-th parameter wrapped as a tuple.</param>
			/// <typeparam name="T5">Type of the 5-th parameter value.</typeparam>
		/// <param name="parameter5">Name and value of the 5-th parameter wrapped as a tuple.</param>
			/// <typeparam name="T6">Type of the 6-th parameter value.</typeparam>
		/// <param name="parameter6">Name and value of the 6-th parameter wrapped as a tuple.</param>
			/// <typeparam name="T7">Type of the 7-th parameter value.</typeparam>
		/// <param name="parameter7">Name and value of the 7-th parameter wrapped as a tuple.</param>
			/// <typeparam name="T8">Type of the 8-th parameter value.</typeparam>
		/// <param name="parameter8">Name and value of the 8-th parameter wrapped as a tuple.</param>
			/// <typeparam name="T9">Type of the 9-th parameter value.</typeparam>
		/// <param name="parameter9">Name and value of the 9-th parameter wrapped as a tuple.</param>
	        [MethodImpl(MethodImplOptions.AggressiveInlining)] // To avoid copying the struct.
	    public static  SemanticMessage<T1, T2, T3, T4, T5, T6, T7, T8, T9> Semantic<T1, T2, T3, T4, T5, T6, T7, T8, T9>(  string name, in (string Name,T1 Value) parameter1, in (string Name,T2 Value) parameter2, in (string Name,T3 Value) parameter3, in (string Name,T4 Value) parameter4, in (string Name,T5 Value) parameter5, in (string Name,T6 Value) parameter6, in (string Name,T7 Value) parameter7, in (string Name,T8 Value) parameter8, in (string Name,T9 Value) parameter9 )
        {
            return new SemanticMessage<T1, T2, T3, T4, T5, T6, T7, T8, T9>( name, parameter1.Name, parameter1.Value, parameter2.Name, parameter2.Value, parameter3.Name, parameter3.Value, parameter4.Name, parameter4.Value, parameter5.Name, parameter5.Value, parameter6.Name, parameter6.Value, parameter7.Name, parameter7.Value, parameter8.Name, parameter8.Value, parameter9.Name, parameter9.Value );
        }
	}
	
	namespace Custom.Messages
	{
		/// <summary>
		/// Encapsulates a text message with 10 parameters. Use the <see cref="FormattedMessageBuilder"/> class to create an instance of this type.
		/// </summary>
	    [SuppressMessage("Microsoft.Performance","CA1815", Justification = "Equal is not a use case")]
	    [SuppressMessage("Microsoft.Design","CA1005:AvoidExcessiveParametersOnGenericTypes", Justification = "Recommended to use 'var' keyword.")]
		public readonly struct FormattedMessage<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> : IMessage
		{
			private readonly string formattingString;
			
			private readonly T1 arg1; private readonly T2 arg2; private readonly T3 arg3; private readonly T4 arg4; private readonly T5 arg5; private readonly T6 arg6; private readonly T7 arg7; private readonly T8 arg8; private readonly T9 arg9; private readonly T10 arg10; 

			
			[MethodImpl(MethodImplOptions.AggressiveInlining)] // To avoid copying the struct.
			internal FormattedMessage( string formattingString, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10 )
			{
				this.formattingString = formattingString;
				this.arg1 = arg1; this.arg2 = arg2; this.arg3 = arg3; this.arg4 = arg4; this.arg5 = arg5; this.arg6 = arg6; this.arg7 = arg7; this.arg8 = arg8; this.arg9 = arg9; this.arg10 = arg10; 
			}

			void IMessage.Write( ICustomLogRecordBuilder recordBuilder, CustomLogRecordItem item )
			{
				recordBuilder.BeginWriteItem(item, new CustomLogRecordTextOptions(10, null));
    
				FormattingStringParser parser = new FormattingStringParser( this.formattingString );
				ArraySegment<char> parameter;

				   
				recordBuilder.WriteCustomString( parser.GetNextSubstring() );
				parameter = parser.GetNextParameter();

				if ( parameter.Array == null )
				{
					throw new InvalidFormattingStringException("The formatting string must have exactly 10 parameters.");
				}

				recordBuilder.WriteCustomParameter( 0, parameter, arg1, CustomLogParameterOptions.FormattedStringParameter );

				
				recordBuilder.WriteCustomString( parser.GetNextSubstring() );
				parameter = parser.GetNextParameter();

				if ( parameter.Array == null )
				{
					throw new InvalidFormattingStringException("The formatting string must have exactly 10 parameters.");
				}

				recordBuilder.WriteCustomParameter( 1, parameter, arg2, CustomLogParameterOptions.FormattedStringParameter );

				
				recordBuilder.WriteCustomString( parser.GetNextSubstring() );
				parameter = parser.GetNextParameter();

				if ( parameter.Array == null )
				{
					throw new InvalidFormattingStringException("The formatting string must have exactly 10 parameters.");
				}

				recordBuilder.WriteCustomParameter( 2, parameter, arg3, CustomLogParameterOptions.FormattedStringParameter );

				
				recordBuilder.WriteCustomString( parser.GetNextSubstring() );
				parameter = parser.GetNextParameter();

				if ( parameter.Array == null )
				{
					throw new InvalidFormattingStringException("The formatting string must have exactly 10 parameters.");
				}

				recordBuilder.WriteCustomParameter( 3, parameter, arg4, CustomLogParameterOptions.FormattedStringParameter );

				
				recordBuilder.WriteCustomString( parser.GetNextSubstring() );
				parameter = parser.GetNextParameter();

				if ( parameter.Array == null )
				{
					throw new InvalidFormattingStringException("The formatting string must have exactly 10 parameters.");
				}

				recordBuilder.WriteCustomParameter( 4, parameter, arg5, CustomLogParameterOptions.FormattedStringParameter );

				
				recordBuilder.WriteCustomString( parser.GetNextSubstring() );
				parameter = parser.GetNextParameter();

				if ( parameter.Array == null )
				{
					throw new InvalidFormattingStringException("The formatting string must have exactly 10 parameters.");
				}

				recordBuilder.WriteCustomParameter( 5, parameter, arg6, CustomLogParameterOptions.FormattedStringParameter );

				
				recordBuilder.WriteCustomString( parser.GetNextSubstring() );
				parameter = parser.GetNextParameter();

				if ( parameter.Array == null )
				{
					throw new InvalidFormattingStringException("The formatting string must have exactly 10 parameters.");
				}

				recordBuilder.WriteCustomParameter( 6, parameter, arg7, CustomLogParameterOptions.FormattedStringParameter );

				
				recordBuilder.WriteCustomString( parser.GetNextSubstring() );
				parameter = parser.GetNextParameter();

				if ( parameter.Array == null )
				{
					throw new InvalidFormattingStringException("The formatting string must have exactly 10 parameters.");
				}

				recordBuilder.WriteCustomParameter( 7, parameter, arg8, CustomLogParameterOptions.FormattedStringParameter );

				
				recordBuilder.WriteCustomString( parser.GetNextSubstring() );
				parameter = parser.GetNextParameter();

				if ( parameter.Array == null )
				{
					throw new InvalidFormattingStringException("The formatting string must have exactly 10 parameters.");
				}

				recordBuilder.WriteCustomParameter( 8, parameter, arg9, CustomLogParameterOptions.FormattedStringParameter );

				
				recordBuilder.WriteCustomString( parser.GetNextSubstring() );
				parameter = parser.GetNextParameter();

				if ( parameter.Array == null )
				{
					throw new InvalidFormattingStringException("The formatting string must have exactly 10 parameters.");
				}

				recordBuilder.WriteCustomParameter( 9, parameter, arg10, CustomLogParameterOptions.FormattedStringParameter );

				
				recordBuilder.WriteCustomString( parser.GetNextSubstring() );

				if ( parser.GetNextParameter().Array != null )
				{
					throw new InvalidFormattingStringException("The formatting string must have exactly 10 parameters.");
				}

				recordBuilder.EndWriteItem(item);


			}

			/// <inheritdoc />
			public override string ToString() => DebugMessageFormatter.Format( this );
		}

		 /// <summary>
		/// Encapsulates a semantic message with a 10 number of parameters. Use the <see cref="SemanticMessageBuilder"/> class to create an instance of this type.
		/// </summary>
		[SuppressMessage("Microsoft.Performance","CA1815", Justification = "Equal is not a use case")]
		[SuppressMessage("Microsoft.Design","CA1005:AvoidExcessiveParametersOnGenericTypes", Justification = "Recommended to use 'var' keyword.")]
		public readonly struct SemanticMessage<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> : IMessage
		{
			private readonly string messageName;
			
			private readonly string name1; private readonly string name2; private readonly string name3; private readonly string name4; private readonly string name5; private readonly string name6; private readonly string name7; private readonly string name8; private readonly string name9; private readonly string name10; 
			private readonly T1 value1; private readonly T2 value2; private readonly T3 value3; private readonly T4 value4; private readonly T5 value5; private readonly T6 value6; private readonly T7 value7; private readonly T8 value8; private readonly T9 value9; private readonly T10 value10; 

			[MethodImpl(MethodImplOptions.AggressiveInlining)] // To avoid copying the struct.
			internal SemanticMessage( string messageName, string name1, T1 value1, string name2, T2 value2, string name3, T3 value3, string name4, T4 value4, string name5, T5 value5, string name6, T6 value6, string name7, T7 value7, string name8, T8 value8, string name9, T9 value9, string name10, T10 value10 )
			{
				this.messageName = messageName;
				this.name1 = name1; this.name2 = name2; this.name3 = name3; this.name4 = name4; this.name5 = name5; this.name6 = name6; this.name7 = name7; this.name8 = name8; this.name9 = name9; this.name10 = name10; 
				this.value1 = value1; this.value2 = value2; this.value3 = value3; this.value4 = value4; this.value5 = value5; this.value6 = value6; this.value7 = value7; this.value8 = value8; this.value9 = value9; this.value10 = value10; 
			}

			void IMessage.Write( ICustomLogRecordBuilder recordBuilder, CustomLogRecordItem item )
			{
				recordBuilder.BeginWriteItem(item, new CustomLogRecordTextOptions(10, this.messageName));

				   
				recordBuilder.WriteCustomParameter( 0, this.name1, this.value1, CustomLogParameterOptions.SemanticParameter );

				
				recordBuilder.WriteCustomParameter( 1, this.name2, this.value2, CustomLogParameterOptions.SemanticParameter );

				
				recordBuilder.WriteCustomParameter( 2, this.name3, this.value3, CustomLogParameterOptions.SemanticParameter );

				
				recordBuilder.WriteCustomParameter( 3, this.name4, this.value4, CustomLogParameterOptions.SemanticParameter );

				
				recordBuilder.WriteCustomParameter( 4, this.name5, this.value5, CustomLogParameterOptions.SemanticParameter );

				
				recordBuilder.WriteCustomParameter( 5, this.name6, this.value6, CustomLogParameterOptions.SemanticParameter );

				
				recordBuilder.WriteCustomParameter( 6, this.name7, this.value7, CustomLogParameterOptions.SemanticParameter );

				
				recordBuilder.WriteCustomParameter( 7, this.name8, this.value8, CustomLogParameterOptions.SemanticParameter );

				
				recordBuilder.WriteCustomParameter( 8, this.name9, this.value9, CustomLogParameterOptions.SemanticParameter );

				
				recordBuilder.WriteCustomParameter( 9, this.name10, this.value10, CustomLogParameterOptions.SemanticParameter );

				
				recordBuilder.EndWriteItem(item);
				
			}

			/// <inheritdoc />
			public override string ToString() => DebugMessageFormatter.Format( this );
		}

		
	
	}



	partial class FormattedMessageBuilder
	{
		
		/// <summary>
        /// Creates a formatted string with 10 parameters.
        /// </summary>
        /// <param name="formattingString">The text of the log record, including parameters (e.g. <c>Opening {Path} file {ShareMode} sharing mode</c>).</param>
		/// <typeparam name="T1">Type of the first parameter.</typeparam>
		/// <param name="arg1">Value of the first parameter.</param>
			/// <typeparam name="T2">Type of the second parameter.</typeparam>
		/// <param name="arg2">Value of the second parameter.</param>
			/// <typeparam name="T3">Type of the third parameter.</typeparam>
		/// <param name="arg3">Value of the third parameter.</param>
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
	        [MethodImpl(MethodImplOptions.AggressiveInlining)] // To avoid copying the struct.
	    public static FormattedMessage<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> Formatted<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(  string formattingString, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10 )
        {
            return new FormattedMessage<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>( formattingString, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10 );
        }
	}

	partial class SemanticMessageBuilder
	{
		/// <summary>
        /// Create a semantic message with 10 parameters (using tuples).
        /// </summary>
        /// <param name="name">Name of the message.</param>
		/// <typeparam name="T1">Type of the first parameter value.</typeparam>
		/// <param name="parameterName1">Name of the first parameter.</param>
		/// <param name="parameterValue1">Name of the first parameter.</param>
			/// <typeparam name="T2">Type of the second parameter value.</typeparam>
		/// <param name="parameterName2">Name of the second parameter.</param>
		/// <param name="parameterValue2">Name of the second parameter.</param>
			/// <typeparam name="T3">Type of the third parameter value.</typeparam>
		/// <param name="parameterName3">Name of the third parameter.</param>
		/// <param name="parameterValue3">Name of the third parameter.</param>
			/// <typeparam name="T4">Type of the 4-th parameter value.</typeparam>
		/// <param name="parameterName4">Name of the 4-th parameter.</param>
		/// <param name="parameterValue4">Name of the 4-th parameter.</param>
			/// <typeparam name="T5">Type of the 5-th parameter value.</typeparam>
		/// <param name="parameterName5">Name of the 5-th parameter.</param>
		/// <param name="parameterValue5">Name of the 5-th parameter.</param>
			/// <typeparam name="T6">Type of the 6-th parameter value.</typeparam>
		/// <param name="parameterName6">Name of the 6-th parameter.</param>
		/// <param name="parameterValue6">Name of the 6-th parameter.</param>
			/// <typeparam name="T7">Type of the 7-th parameter value.</typeparam>
		/// <param name="parameterName7">Name of the 7-th parameter.</param>
		/// <param name="parameterValue7">Name of the 7-th parameter.</param>
			/// <typeparam name="T8">Type of the 8-th parameter value.</typeparam>
		/// <param name="parameterName8">Name of the 8-th parameter.</param>
		/// <param name="parameterValue8">Name of the 8-th parameter.</param>
			/// <typeparam name="T9">Type of the 9-th parameter value.</typeparam>
		/// <param name="parameterName9">Name of the 9-th parameter.</param>
		/// <param name="parameterValue9">Name of the 9-th parameter.</param>
			/// <typeparam name="T10">Type of the 10-th parameter value.</typeparam>
		/// <param name="parameterName10">Name of the 10-th parameter.</param>
		/// <param name="parameterValue10">Name of the 10-th parameter.</param>
	        [MethodImpl(MethodImplOptions.AggressiveInlining)] // To avoid copying the struct.
	    public static  SemanticMessage<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> Semantic<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(  string name, string parameterName1, T1 parameterValue1, string parameterName2, T2 parameterValue2, string parameterName3, T3 parameterValue3, string parameterName4, T4 parameterValue4, string parameterName5, T5 parameterValue5, string parameterName6, T6 parameterValue6, string parameterName7, T7 parameterValue7, string parameterName8, T8 parameterValue8, string parameterName9, T9 parameterValue9, string parameterName10, T10 parameterValue10 )
        {
            return new SemanticMessage<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>( name, parameterName1, parameterValue1, parameterName2, parameterValue2, parameterName3, parameterValue3, parameterName4, parameterValue4, parameterName5, parameterValue5, parameterName6, parameterValue6, parameterName7, parameterValue7, parameterName8, parameterValue8, parameterName9, parameterValue9, parameterName10, parameterValue10 );
        }

		/// <summary>
        /// Create a semantic message with 10 parameters.
        /// </summary>
        /// <param name="name">Name of the message.</param>
		/// <typeparam name="T1">Type of the first parameter value.</typeparam>
		/// <param name="parameter1">Name and value of the first parameter wrapped as a tuple.</param>
			/// <typeparam name="T2">Type of the second parameter value.</typeparam>
		/// <param name="parameter2">Name and value of the second parameter wrapped as a tuple.</param>
			/// <typeparam name="T3">Type of the third parameter value.</typeparam>
		/// <param name="parameter3">Name and value of the third parameter wrapped as a tuple.</param>
			/// <typeparam name="T4">Type of the 4-th parameter value.</typeparam>
		/// <param name="parameter4">Name and value of the 4-th parameter wrapped as a tuple.</param>
			/// <typeparam name="T5">Type of the 5-th parameter value.</typeparam>
		/// <param name="parameter5">Name and value of the 5-th parameter wrapped as a tuple.</param>
			/// <typeparam name="T6">Type of the 6-th parameter value.</typeparam>
		/// <param name="parameter6">Name and value of the 6-th parameter wrapped as a tuple.</param>
			/// <typeparam name="T7">Type of the 7-th parameter value.</typeparam>
		/// <param name="parameter7">Name and value of the 7-th parameter wrapped as a tuple.</param>
			/// <typeparam name="T8">Type of the 8-th parameter value.</typeparam>
		/// <param name="parameter8">Name and value of the 8-th parameter wrapped as a tuple.</param>
			/// <typeparam name="T9">Type of the 9-th parameter value.</typeparam>
		/// <param name="parameter9">Name and value of the 9-th parameter wrapped as a tuple.</param>
			/// <typeparam name="T10">Type of the 10-th parameter value.</typeparam>
		/// <param name="parameter10">Name and value of the 10-th parameter wrapped as a tuple.</param>
	        [MethodImpl(MethodImplOptions.AggressiveInlining)] // To avoid copying the struct.
	    public static  SemanticMessage<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> Semantic<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(  string name, in (string Name,T1 Value) parameter1, in (string Name,T2 Value) parameter2, in (string Name,T3 Value) parameter3, in (string Name,T4 Value) parameter4, in (string Name,T5 Value) parameter5, in (string Name,T6 Value) parameter6, in (string Name,T7 Value) parameter7, in (string Name,T8 Value) parameter8, in (string Name,T9 Value) parameter9, in (string Name,T10 Value) parameter10 )
        {
            return new SemanticMessage<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>( name, parameter1.Name, parameter1.Value, parameter2.Name, parameter2.Value, parameter3.Name, parameter3.Value, parameter4.Name, parameter4.Value, parameter5.Name, parameter5.Value, parameter6.Name, parameter6.Value, parameter7.Name, parameter7.Value, parameter8.Name, parameter8.Value, parameter9.Name, parameter9.Value, parameter10.Name, parameter10.Value );
        }
	}
	




	
}


