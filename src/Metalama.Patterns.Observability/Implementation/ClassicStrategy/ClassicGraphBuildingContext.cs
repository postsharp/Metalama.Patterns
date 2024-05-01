// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Aspects;
using Metalama.Framework.Code;
using Metalama.Framework.Diagnostics;
using Metalama.Framework.Engine.CodeModel;
using Metalama.Framework.Engine.Diagnostics;
using Metalama.Patterns.Observability.Implementation.DependencyAnalysis;
using Metalama.Patterns.Observability.Options;
using Microsoft.CodeAnalysis;

namespace Metalama.Patterns.Observability.Implementation.ClassicStrategy;

[CompileTime]
internal sealed class ClassicGraphBuildingContext : IGraphBuildingContext
{
    private readonly ClassicObservabilityStrategyImpl _strategy;

    public ClassicGraphBuildingContext( ClassicObservabilityStrategyImpl strategy )
    {
        this._strategy = strategy;
    }

    public bool HasReportedErrors { get; private set; }

    public bool MustIgnoreUnsupportedDependencies( ISymbol symbol )
    {
        var decl = this._strategy.CurrentType.Compilation.GetDeclaration( symbol );

        var options = decl switch
        {
            ICompilation compilation => compilation.Enhancements().GetOptions<DependencyAnalysisOptions>(),
            INamespace ns => ns.Enhancements().GetOptions<DependencyAnalysisOptions>(),
            INamedType type => type.Enhancements().GetOptions<DependencyAnalysisOptions>(),
            IMember member => member.Enhancements().GetOptions<DependencyAnalysisOptions>(),
            _ => throw new NotImplementedException()
        };

        return options.IgnoreUnsupportedDependencies == true;
    }

    public bool IsAutoPropertyOrField( ISymbol symbol )
        => this._strategy.CurrentType.Compilation.GetDeclaration( symbol ) is IFieldOrProperty { IsAutoPropertyOrField: true };

    public void ReportDiagnostic( IDiagnostic diagnostic, Location? location = null )
    {
        this.HasReportedErrors |= diagnostic.Definition.Severity == Severity.Error;
        this._strategy.AspectBuilder.Diagnostics.Report( diagnostic, location.ToDiagnosticLocation() );
    }

    public bool TreatAsImplementingInpc( ITypeSymbol type )
    {
        var typeDecl = this._strategy.CurrentType.Compilation.GetDeclaration( type ) as IType;

        return typeDecl != null && this._strategy.InpcInstrumentationKindLookup.Get( typeDecl ) != InpcInstrumentationKind.None;
    }

    public bool HasInheritedOnChildPropertyChangedPropertyPath( string dottedPropertyPath )
        => this._strategy.HasInheritedOnChildPropertyChangedPropertyPath( dottedPropertyPath );

    public bool HasInheritedOnObservablePropertyChangedProperty( string dottedPropertyPath )
        => this._strategy.HasInheritedOnObservablePropertyChangedProperty( dottedPropertyPath );

    public InpcInstrumentationKind GetInpcInstrumentationKind( INamedType type ) => this._strategy.GetInpcInstrumentationKind( type );
}