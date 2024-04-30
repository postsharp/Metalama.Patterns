// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Aspects;
using Metalama.Framework.Diagnostics;
using Microsoft.CodeAnalysis;

namespace Metalama.Patterns.Observability.Implementation.DependencyAnalysis;

[CompileTime]
internal interface IGraphBuildingContext
{
    void ReportDiagnostic( IDiagnostic diagnostic, Location? location = null );

    bool TreatAsImplementingInpc( ITypeSymbol type );

    /// <summary>
    /// Gets a value indicating if the given symbol is configured as safe for dependency analysis.
    /// </summary>
    /// <remarks>
    /// Configuration is by <see cref="IgnoreUnsupportedDependenciesAttribute"/> or <see cref="Options.DependencyAnalysisOptions"/>.
    /// </remarks>
    bool MustIgnoreUnsupportedDependencies( ISymbol symbol );

    bool IsAutoPropertyOrField( ISymbol symbol );
}