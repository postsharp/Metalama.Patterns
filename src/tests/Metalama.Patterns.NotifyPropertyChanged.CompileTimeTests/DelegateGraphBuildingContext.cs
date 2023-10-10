// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Diagnostics;
using Metalama.Patterns.NotifyPropertyChanged.Implementation.DependencyAnalysis;
using Microsoft.CodeAnalysis;

namespace Metalama.Patterns.NotifyPropertyChanged.CompileTimeTests;

public sealed class DelegateGraphBuildingContext : DependencyGraph.IGraphBuildingContext
{
    private readonly Func<ISymbol, bool>? _isConfiguredAsSafeToCall;
    private readonly Action<IDiagnostic, Location?>? _reportDiagnostic;
    private readonly Func<ITypeSymbol, bool>? _treatAsImplementingInpc;

    public DelegateGraphBuildingContext(
        Func<ISymbol, bool>? isConfiguredAsSafeToCall = null,
        Action<IDiagnostic, Location?>? reportDiagnostic = null,
        Func<ITypeSymbol, bool>? treatAsImplementingInpc = null )
    {
        this._isConfiguredAsSafeToCall = isConfiguredAsSafeToCall;
        this._reportDiagnostic = reportDiagnostic;
        this._treatAsImplementingInpc = treatAsImplementingInpc;
    }

    public bool IsConfiguredAsSafe( ISymbol symbol ) => this._isConfiguredAsSafeToCall?.Invoke( symbol ) ?? false;

    public void ReportDiagnostic( IDiagnostic diagnostic, Location? location = null ) => this._reportDiagnostic?.Invoke( diagnostic, location );

    public bool TreatAsImplementingInpc( ITypeSymbol type ) => this._treatAsImplementingInpc?.Invoke( type ) ?? false;
}