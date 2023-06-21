// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using JetBrains.Annotations;

namespace Flashtrace.Formatters;

/// <summary>
/// Describes a formatting role.
/// </summary>
[PublicAPI]
public class FormattingRole
{
    /// <summary>
    /// Initializes a new instance of the <see cref="FormattingRole"/> class.
    /// </summary>
    public FormattingRole( string name )
    {
        if ( string.IsNullOrWhiteSpace( name ) )
        {
            throw new ArgumentNullException( nameof(name) );
        }

        this.Name = name;
    }

    /// <summary>
    /// Gets the name of the <see cref="FormattingRole"/>.
    /// </summary>
    public string Name { get; }

    public override string ToString() => this.Name;
}