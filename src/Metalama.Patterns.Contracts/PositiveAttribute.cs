// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using JetBrains.Annotations;
using Metalama.Framework.Code;
using Metalama.Framework.Diagnostics;

namespace Metalama.Patterns.Contracts;

/// <summary>
/// Custom attribute that, when added to a field, property or parameter, throws
/// an <see cref="ArgumentOutOfRangeException"/> if the target is assigned a value
/// smaller than zero.
/// </summary>
/// <remarks>
///     <para>Null values are accepted and do not throw an exception.
/// </para>
/// <para>Error message is identified by <see cref="ContractTextProvider.GreaterThanErrorMessage"/>.</para>
/// </remarks>
[PublicAPI]
public class PositiveAttribute : GreaterThanAttribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PositiveAttribute"/> class.
    /// </summary>
    public PositiveAttribute() : base( 0 ) { }

    private static readonly DiagnosticDefinition<(IDeclaration, string)> _rangeCannotBeApplied =
        CreateCannotBeAppliedDiagnosticDefinition( "LAMA5004", nameof(PositiveAttribute) );

    /// <inheritdoc/>
    protected override DiagnosticDefinition<(IDeclaration Declaration, string TargetBasicType)> GetCannotBeAppliedDiagnosticDefinition()
        => _rangeCannotBeApplied;
}