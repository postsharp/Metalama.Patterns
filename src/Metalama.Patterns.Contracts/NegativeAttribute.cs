// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Code;
using Metalama.Framework.Diagnostics;

namespace Metalama.Patterns.Contracts;

/// <summary>
/// Custom attribute that, when added to a field, property or parameter, throws
/// an <see cref="ArgumentOutOfRangeException"/> if the target is assigned a value
/// greater than zero.
/// </summary>
/// <remarks>
///     <para>Null values are accepted and do not throw an exception.
/// </para>
/// <para>Error message is identified by <see cref="ContractLocalizedTextProvider.LessThanErrorMessage"/>.</para>
/// </remarks>
public class NegativeAttribute : LessThanAttribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="NegativeAttribute"/> class.
    /// </summary>
    public NegativeAttribute() : base( 0 ) { }

    private static readonly DiagnosticDefinition<(IDeclaration, string)> _rangeCannotBeApplied =
        CreateCannotBeAppliedDiagosticDefinition( "LAMA5003", nameof(NegativeAttribute) );

    /// <inheritdoc/>
    protected override DiagnosticDefinition<(IDeclaration Declaration, string TargetBasicType)> GetCannotBeAppliedDiagosticDefinition()
        => _rangeCannotBeApplied;
}