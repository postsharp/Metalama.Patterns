// Copyright (c) SharpCrafters s.r.o. This file is not open source. It is released under a commercial
// source-available license. Please see the LICENSE.md file in the repository root for details.

using System;
using System.Diagnostics;
using System.Reflection;

namespace PostSharp.Patterns.Diagnostics.Contexts
{
    /// <summary>
    /// Represents information about the caller of the <see cref="Logger"/> class.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1815:OverrideEqualsAndOperatorEqualsOnValueTypes")]
    public struct CallerInfo
    {

        private Type sourceType;

        /// <summary>
        /// Initializes a new <see cref="CallerInfo"/>, and uses as a <see cref="RuntimeTypeHandle"/> to specify the source type.
        /// </summary>
        /// <param name="sourceTypeToken"><see cref="RuntimeTypeHandle"/> of the calling type.</param>
        /// <param name="methodName">Name of the calling method.</param>
        /// <param name="file">Path of the source code of the calling code.</param>
        /// <param name="line">Line in <paramref name="file"/> of the caller.</param>
        /// <param name="column">Column in <paramref name="file"/> of the caller.</param>
        /// <param name="attributes">Attributes.</param>
        [DebuggerStepThrough]
        public CallerInfo( RuntimeTypeHandle sourceTypeToken, string methodName, string file, int line, int column, CallerAttributes attributes )
        {
            this.SourceTypeToken= sourceTypeToken;
            this.MethodName = methodName;
            this.SourceLineInfo = new SourceLineInfo(file, line, column);
            this.sourceType = null;
            this.Attributes = attributes;
        }

        /// <summary>
        /// Initializes a new <see cref="CallerInfo"/>, and uses as a <see cref="Type"/> to specify the source type.
        /// </summary>
        /// <param name="sourceType"><see cref="Type"/> of the calling type.</param>
        /// <param name="methodName">Name of the calling method.</param>
        /// <param name="file">Path of the source code of the calling code.</param>
        /// <param name="line">Line in <paramref name="file"/> of the caller.</param>
        /// <param name="column">Column in <paramref name="file"/> of the caller.</param>
        /// <param name="attributes">Attributes.</param>
        [DebuggerStepThrough]
        public CallerInfo(Type sourceType, string methodName, string file, int line, int column, CallerAttributes attributes )
        {
            this.sourceType = sourceType;
            this.MethodName = methodName;
            this.SourceTypeToken = default(RuntimeTypeHandle);
            this.SourceLineInfo = new SourceLineInfo(file, line, column);
            this.Attributes = attributes;
        }

        /// <summary>
        /// Gets the caller attributes.
        /// </summary>
        public CallerAttributes Attributes { get; private set; }

        /// <summary>
        /// Determines whether the caller is an <c>async</c> method.
        /// </summary>
        public bool IsAsync => (this.Attributes & CallerAttributes.IsAsync) != 0;

        /// <summary>
        /// Gets the source <see cref="Type"/>.
        /// </summary>
        public Type SourceType
        {
            get
            {
                if ( this.sourceType == null )
                {
                    this.sourceType = Type.GetTypeFromHandle( this.SourceTypeToken );
                }

                return this.sourceType;
            }
        }

        /// <summary>
        /// Gets the <see cref="RuntimeTypeHandle"/> of the caller <see cref="Type"/>.
        /// </summary>
        public RuntimeTypeHandle SourceTypeToken { get; }

        /// <summary>
        /// Gets the name of the caller method.
        /// </summary>
        public string MethodName { get; private set; }

        /// <summary>
        /// Gets the <see cref="SourceLineInfo"/> of the caller.
        /// </summary>
        public SourceLineInfo SourceLineInfo { get; private set; }

        


        /// <summary>
        /// Determines whether the current <see cref="CallerInfo"/> is null.
        /// </summary>
        public bool IsNull => this.MethodName == null && this.SourceLineInfo.IsNull;

        [ExplicitCrossPackageInternal]
        internal static CallerInfo Null;

        [ExplicitCrossPackageInternal]
        internal static CallerInfo Async => new CallerInfo { Attributes = CallerAttributes.IsAsync };



        /// <inheritdoc />
        public override string ToString()
        {
            if ( this.IsNull ) return "null";

            if (this.SourceLineInfo.IsNull)
                return this.SourceType.FullName + "." + this.MethodName;
            else
                return this.SourceType.FullName + "." + this.MethodName + " at " + this.SourceLineInfo.File + ", " + this.SourceLineInfo.Line;
            

        }




#pragma warning disable CS1574 // XML comment has cref attribute that could not be resolved
        /// <summary>
        /// Gets a <see cref="CallerInfo"/> of the caller by performing a stack walk (using <see cref="StackFrame"/>).
        /// </summary>
        /// <param name="skipFrames">The number of stack frames to skip.</param>
        /// <returns> A <see cref="CallerInfo"/> for the caller (skipping the specified number of stack frames), or <c>default</c> if the platform does not support the <see cref="StackFrame"/> class.</returns>
#pragma warning restore CS1574 // XML comment has cref attribute that could not be resolved
#pragma warning disable CA1801 // Unused parameter.
        public static CallerInfo GetDynamic(int skipFrames)
#pragma warning restore CA1801
        {
#if STACK_FRAME
            for ( int i = skipFrames + 1;  ; i++ )
            {
                StackFrame frame = new StackFrame( i, true);

                MethodBase method = frame.GetMethod();

                if ( method == null )
                {
                    // We reach the bottom of the stack.
                    return default;
                }

                if (method.DeclaringType == null)
                {
                    continue;
                }

                if ( (method.DeclaringType.Namespace != null && method.DeclaringType.Namespace.StartsWith( "PostSharp.Aspects", StringComparison.Ordinal )) ||
                     string.Equals( method.DeclaringType.Namespace, "PostSharp.Patterns.Diagnostics.ThreadingInstrumentation", StringComparison.Ordinal ) )
                {
                    continue;
                }

                return new CallerInfo(method.DeclaringType, method.Name, frame.GetFileName(), frame.GetFileLineNumber(), frame.GetFileColumnNumber(), CallerAttributes.None);
            }
#else
            return default;
#endif
      

        }
    }
}
