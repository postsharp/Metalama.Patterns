// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Aspects;
using Metalama.Framework.Code;
using Metalama.Framework.Diagnostics;
using Metalama.Framework.Engine.CodeModel;
using Metalama.Patterns.Observability.Options;
using Microsoft.CodeAnalysis;
using SpecialType = Microsoft.CodeAnalysis.SpecialType;
using TypeKind = Microsoft.CodeAnalysis.TypeKind;

namespace Metalama.Patterns.Observability.Implementation.DependencyAnalysis;

[CompileTime]
internal abstract class GraphBuildingContext
{
    private readonly ICompilation _compilation;

    protected GraphBuildingContext( ICompilation compilation )
    {
        this._compilation = compilation;
    }

    public abstract void ReportDiagnostic( IDiagnostic diagnostic, Location? location = default );

    public abstract bool TreatAsImplementingInpc( ITypeSymbol type );

    public virtual bool CanIgnoreUnobservableExpressions( ISymbol symbol )
        => this.GetDependencyAnalysisOptions( symbol ).SuppressWarnings == true;

    protected virtual DependencyAnalysisOptions GetDependencyAnalysisOptions( ISymbol symbol )
    {
        var decl = this._compilation.GetDeclaration( symbol );

        var options = decl switch
        {
            ICompilation compilation => compilation.Enhancements().GetOptions<DependencyAnalysisOptions>(),
            INamespace ns => ns.Enhancements().GetOptions<DependencyAnalysisOptions>(),
            INamedType type => type.Enhancements().GetOptions<DependencyAnalysisOptions>(),
            IMember member => member.Enhancements().GetOptions<DependencyAnalysisOptions>(),
            _ => DependencyAnalysisOptions.Default
        };

        return options;
    }

    public bool IsAutoPropertyOrField( ISymbol symbol ) => this._compilation.GetDeclaration( symbol ) is IFieldOrProperty { IsAutoPropertyOrField: true };

    public ObservabilityContract? GetObservabilityContract( ISymbol symbol ) => this.GetDependencyAnalysisOptions( symbol ).ObservabilityContract;

    public bool IsConstant( IFieldSymbol field ) => this.IsConstantMember( field );

    public bool IsConstant( IPropertySymbol property ) => this.IsConstantMember( property );

    public bool IsConstant( IMethodSymbol method ) => this.IsConstantMember( method );

    private bool IsConstantMember( ISymbol symbol )
    {
        // Options take precedence over hard-written rules.
        // We have a simple catch-call observability contract at the moment, which conveniently makes all required guarantees.
        var hasContract = this.GetDependencyAnalysisOptions( symbol ).ObservabilityContract != null;

        if ( hasContract )
        {
            return true;
        }

        if ( symbol.Kind is SymbolKind.Property or SymbolKind.Field or SymbolKind.Method )
        {
            return this.IsConstant( symbol.ContainingType );
        }
        else
        {
            return false;
        }
    }

    public bool IsConstant( ITypeSymbol type )
    {
        return type is {
            SpecialType: SpecialType.System_Boolean or
            SpecialType.System_Byte or
            SpecialType.System_Char or
            SpecialType.System_DateTime or
            SpecialType.System_Decimal or
            SpecialType.System_Double or
            SpecialType.System_Int16 or
            SpecialType.System_Int32 or
            SpecialType.System_Int64 or
            SpecialType.System_SByte or
            SpecialType.System_Single or
            SpecialType.System_String or
            SpecialType.System_UInt16 or
            SpecialType.System_UInt32 or
            SpecialType.System_UInt64
        } or { IsValueType: true, ContainingNamespace.Name: "System" } or { TypeKind: TypeKind.Enum or TypeKind.Delegate };
    }
}