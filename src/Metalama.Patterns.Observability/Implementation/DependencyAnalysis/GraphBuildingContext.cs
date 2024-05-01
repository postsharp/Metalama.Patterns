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

    public virtual bool CanIgnoreUnobservableExpressions( IPropertySymbol symbol )
        => this.GetDependencyAnalysisOptions( symbol ).IgnoreUnobservableExpressions == true;

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

    public bool IsDeeplyImmutableType( ITypeSymbol type )
    {
        // Options take precedence over hard-written rules.
        var fromOptions = this.GetDependencyAnalysisOptions( type ).IsDeeplyImmutableType;

        if ( fromOptions != null )
        {
            return fromOptions.Value;
        }

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