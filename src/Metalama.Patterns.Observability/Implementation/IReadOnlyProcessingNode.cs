// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Aspects;
using Metalama.Framework.Code;

namespace Metalama.Patterns.Observability.Implementation;

[CompileTime]
internal interface IReadOnlyProcessingNode
{
    /// <summary>
    /// Gets the Metalama <see cref="IFieldOrProperty"/> for the node.
    /// </summary>
    IFieldOrProperty FieldOrProperty { get; }

    /// <summary>
    /// Gets the name of the field or property for the node.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Gets a property path like "A1" or "A1.B1.C1".
    /// </summary>
    string DottedPropertyPath { get; }

    /// <summary>
    /// Gets a property path like "A1" or "A1B1C1".
    /// </summary>
    string ContiguousPropertyPath { get; }

    string ToString( string? format );

    string ToString( object? highlight, string? format = null );
}