// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using JetBrains.Annotations;
using System.Diagnostics;

namespace Flashtrace.Contexts;

/// <summary>
/// Describes the caller of a logging method.
/// </summary>
[PublicAPI]
public struct CallerInfo
{
    private Type? _sourceType;

    /// <summary>
    /// Initializes a new instance of the <see cref="CallerInfo"/> struct specifying the source type as a <see cref="RuntimeTypeHandle"/>.
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
        this.SourceTypeToken = sourceTypeToken;
        this.MethodName = methodName;
        this.SourceLineInfo = new SourceLineInfo( file, line, column );
        this._sourceType = null;
        this.Attributes = attributes;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CallerInfo"/> struct specifying the source type as a <see cref="Type"/>.
    /// </summary>
    /// <param name="sourceType"><see cref="Type"/> of the calling type.</param>
    /// <param name="methodName">Name of the calling method.</param>
    /// <param name="file">Path of the source code of the calling code.</param>
    /// <param name="line">Line in <paramref name="file"/> of the caller.</param>
    /// <param name="column">Column in <paramref name="file"/> of the caller.</param>
    /// <param name="attributes">Attributes.</param>
    [DebuggerStepThrough]
    public CallerInfo( Type sourceType, string methodName, string? file, int line, int column, CallerAttributes attributes )
    {
        this._sourceType = sourceType;
        this.MethodName = methodName;
        this.SourceTypeToken = default;
        this.SourceLineInfo = new SourceLineInfo( file, line, column );
        this.Attributes = attributes;
    }

    /// <summary>
    /// Gets the caller attributes.
    /// </summary>
    public CallerAttributes Attributes { get; private set; }

    /// <summary>
    /// Gets a value indicating whether the caller is an <c>async</c> method.
    /// </summary>
    public bool IsAsync => (this.Attributes & CallerAttributes.IsAsync) != 0;

    /// <summary>
    /// Gets the source <see cref="Type"/>.
    /// </summary>
    public Type? SourceType
    {
        get
        {
            // As per MS documentation, Type.GetTypeFromHandle will return null if the argument "is null", despite the return type not being marked as Type?.
            if ( this._sourceType == null && this.SourceTypeToken.Value != IntPtr.Zero )
            {
                this._sourceType = Type.GetTypeFromHandle( this.SourceTypeToken );
            }

            return this._sourceType;
        }
    }

    /// <summary>
    /// Gets the <see cref="RuntimeTypeHandle"/> of the caller <see cref="Type"/>.
    /// </summary>
    public RuntimeTypeHandle SourceTypeToken { get; }

    /// <summary>
    /// Gets the name of the caller method.
    /// </summary>
    public string? MethodName { get; private set; }

    /// <summary>
    /// Gets the <see cref="SourceLineInfo"/> of the caller.
    /// </summary>
    public SourceLineInfo SourceLineInfo { get; private set; }

    /// <summary>
    /// Gets a value indicating whether the current <see cref="CallerInfo"/> is null.
    /// </summary>
    public bool IsNull => this.MethodName == null && this.SourceLineInfo.IsNull;

    internal static CallerInfo Null;

    /// <inheritdoc />
    public override string ToString()
    {
        if ( this.IsNull )
        {
            return "null";
        }

        var useTypeName = this.SourceType?.FullName ?? this.SourceType?.Name ?? "<unknown>";
        var useMethodName = this.MethodName ?? "<unknown>";

        if ( this.SourceLineInfo.IsNull )
        {
            return useTypeName + "." + useMethodName;
        }
        else
        {
            return useTypeName + "." + useMethodName + " at " + this.SourceLineInfo.File + ", " + this.SourceLineInfo.Line;
        }
    }

    /// <summary>
    /// Gets a <see cref="CallerInfo"/> of the caller by performing a stack walk (using <see cref="StackFrame"/>).
    /// </summary>
    /// <param name="skipFrames">The number of stack frames to skip.</param>
    /// <returns> A <see cref="CallerInfo"/> for the caller (skipping the specified number of stack frames), or <c>default</c> if the platform does not support the <see cref="StackFrame"/> class.</returns>
    public static CallerInfo GetDynamic( int skipFrames )
    {
        // ReSharper code cleanup wants ';;', but this leads to a missing space warning which must be disabled.
#pragma warning disable SA1002
        for ( var i = skipFrames + 1;; i++ )
#pragma warning restore SA1002
        {
            var frame = new StackFrame( i, true );

            var method = frame.GetMethod();

            if ( method == null )
            {
                // We reach the bottom of the stack.
                return default;
            }

            if ( method.DeclaringType == null )
            {
                continue;
            }

            // TODO: Do we want to automagically skip any Flashtrace or Metalama stack frames here?

            return new CallerInfo(
                method.DeclaringType,
                method.Name,
                frame.GetFileName(),
                frame.GetFileLineNumber(),
                frame.GetFileColumnNumber(),
                CallerAttributes.None );
        }
    }
}