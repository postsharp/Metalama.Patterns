// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using System.Diagnostics.CodeAnalysis;

namespace Flashtrace;

/// <summary>
/// Options for the <see cref="Exception"/> method.
/// </summary>
[SuppressMessage( "Microsoft.Performance", "CA1815:OverrideEqualsAndOperatorEqualsOnValueTypes" )]
public struct WriteMessageOptions
{
    /// <summary>
    /// Initializes a <see cref="WriteMessageOptions"/> and optionally specifies the properties given a CLR object.
    /// </summary>
    /// <param name="data">Optional. Specifies the properties of the <see cref="WriteMessageOptions"/>, typically specified as an instance of a well-known or anonymous CLR type.
    /// The resulting <see cref="LogEventData"/> will have the default <see cref="LogEventMetadata"/>, which means that all CLR properties will be exposed
    /// as logging properties unless they are annotated with <see cref="LoggingPropertyOptionsAttribute"/>.</param>
    public WriteMessageOptions( object data = null ) : this( LogEventData.Create( data ) ) { }

    /// <summary>
    /// Initializes a <see cref="WriteMessageOptions"/> and optionally specifies the properties given a <see cref="LogEventData"/>.
    /// </summary>
    /// <param name="data">Optional. Specifies the properties of the <see cref="WriteMessageOptions"/>. See <see cref="LogEventData"/>.</param>
    public WriteMessageOptions( in LogEventData data ) : this()
    {
        this.Data = data;
    }

    /// <summary>
    /// Specifies the properties of the <see cref="WriteMessageOptions"/>, typically specified as an instance of a well-known or anonymous CLR type.
    /// </summary>
    public LogEventData Data { get; set; }
}