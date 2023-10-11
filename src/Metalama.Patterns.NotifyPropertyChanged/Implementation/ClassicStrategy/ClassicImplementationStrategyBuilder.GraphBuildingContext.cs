// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Aspects;
using Metalama.Framework.Code;
using Metalama.Framework.Diagnostics;
using Metalama.Framework.Engine.CodeModel;
using Metalama.Framework.Engine.Diagnostics;
using Metalama.Patterns.NotifyPropertyChanged.Implementation.DependencyAnalysis;
using Metalama.Patterns.NotifyPropertyChanged.Options;

namespace Metalama.Patterns.NotifyPropertyChanged.Implementation.ClassicStrategy;

internal sealed partial class ClassicImplementationStrategyBuilder
{
    [CompileTime]
    private sealed class GraphBuildingContext : DependencyGraph.IGraphBuildingContext
    {
        private readonly ClassicImplementationStrategyBuilder _strategyBuilder;

        public GraphBuildingContext( ClassicImplementationStrategyBuilder strategyBuilder )
        {
            this._strategyBuilder = strategyBuilder;
        }

        public bool HasReportedErrors { get; private set; }

        public bool IsConfiguredAsSafe( Microsoft.CodeAnalysis.ISymbol symbol )
        {
            var decl = this._strategyBuilder._builder.Target.Compilation.GetDeclaration( symbol );

            if ( decl == null )
            {
                return false;
            }

            var options = decl switch
            {
                ICompilation compilation => compilation.Enhancements().GetOptions<DependencyAnalysisOptions>(),
                INamespace ns => ns.Enhancements().GetOptions<DependencyAnalysisOptions>(),
                INamedType type => type.Enhancements().GetOptions<DependencyAnalysisOptions>(),
                IMember member => member.Enhancements().GetOptions<DependencyAnalysisOptions>(),
                _ => throw new NotImplementedException()
            };
            
            return options.IsSafe == true;
        }

        public void ReportDiagnostic( IDiagnostic diagnostic, Microsoft.CodeAnalysis.Location? location = null )
        {
            this.HasReportedErrors |= diagnostic.Definition.Severity == Severity.Error;
            this._strategyBuilder._builder.Diagnostics.Report( diagnostic, location.ToDiagnosticLocation() );
        }

        public bool TreatAsImplementingInpc( Microsoft.CodeAnalysis.ITypeSymbol type )
        {
            var typeDecl = this._strategyBuilder._builder.Target.Compilation.GetDeclaration( type ) as IType;

            return typeDecl != null && this._strategyBuilder._inpcInstrumentationKindLookup.Get( typeDecl ) != InpcInstrumentationKind.None;
        }
    }
}