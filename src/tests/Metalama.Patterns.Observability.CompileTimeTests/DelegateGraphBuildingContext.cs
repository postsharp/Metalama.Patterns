// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Diagnostics;
using Metalama.Patterns.Observability.Implementation.DependencyAnalysis;
using Microsoft.CodeAnalysis;

namespace Metalama.Patterns.Observability.CompileTimeTests;

internal sealed class DelegateGraphBuildingContext : DependencyGraph.IGraphBuildingContext
{
    private readonly Func<ISymbol, bool>? _isConfiguredAsSafe;
    private readonly Action<IDiagnostic, Location?>? _reportDiagnostic;
    private readonly Func<ITypeSymbol, bool>? _treatAsImplementingInpc;
    private readonly Func<ISymbol, bool>? _isAutoPropertyOrField;

    /// <summary>
    /// Initializes a new instance of the <see cref="DelegateGraphBuildingContext"/> class.
    /// </summary>
    /// <param name="isConfiguredAsSafe">If <see langword="null"/>, <see cref="DependencyGraph.IGraphBuildingContext.IsConfiguredAsSafe(ISymbol)"/> will always return <see langword="false"/>.</param>
    /// <param name="reportDiagnostic"></param>
    /// <param name="treatAsImplementingInpc">If <see langword="null"/>, <see cref="DependencyGraph.IGraphBuildingContext.TreatAsImplementingInpc(ITypeSymbol)"/> will always return <see langword="false"/>.</param>
    /// <param name="isAutoPropertyOrField">If <see langword="null"/>, <see cref="DependencyGraph.IGraphBuildingContext.IsAutoPropertyOrField(ISymbol)"/> will return <see langword="true"/> for any field or property.</param>
    public DelegateGraphBuildingContext(
        Func<ISymbol, bool>? isConfiguredAsSafe = null,
        Action<IDiagnostic, Location?>? reportDiagnostic = null,
        Func<ITypeSymbol, bool>? treatAsImplementingInpc = null,
        Func<ISymbol, bool>? isAutoPropertyOrField = null )
    {
        this._isConfiguredAsSafe = isConfiguredAsSafe;
        this._reportDiagnostic = reportDiagnostic;
        this._treatAsImplementingInpc = treatAsImplementingInpc;
        this._isAutoPropertyOrField = isAutoPropertyOrField;
    }

    public bool IsAutoPropertyOrField( ISymbol symbol ) => this._isAutoPropertyOrField?.Invoke( symbol ) ?? symbol is { Kind: SymbolKind.Field or SymbolKind.Property };

    public bool IsConfiguredAsSafe( ISymbol symbol ) => this._isConfiguredAsSafe?.Invoke( symbol ) ?? false;

    public void ReportDiagnostic( IDiagnostic diagnostic, Location? location = null ) => this._reportDiagnostic?.Invoke( diagnostic, location );

    public bool TreatAsImplementingInpc( ITypeSymbol type ) => this._treatAsImplementingInpc?.Invoke( type ) ?? false;
}