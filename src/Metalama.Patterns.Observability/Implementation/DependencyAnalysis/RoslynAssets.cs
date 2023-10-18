// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

// To avoid confusion due to some identical type names, this file is concerned only with Roslyn types.
// Do not add usings for the namespaces Metalama.Framework.Code, Metalama.Framework.Engine.CodeModel or
// related namespaces.

using Metalama.Framework.Aspects;
using Microsoft.CodeAnalysis;

namespace Metalama.Patterns.Observability.Implementation.DependencyAnalysis;

[CompileTime]
internal sealed class RoslynAssets
{
    public RoslynAssets( Compilation compilation )
    {
        this.TimeSpan = compilation.GetTypeByMetadataName( "System.TimeSpan" );
        this.DateTimeOffset = compilation.GetTypeByMetadataName( "System.DateTimeOffset" );
        this.DateOnly = compilation.GetTypeByMetadataName( "System.DateOnly" );
        this.TimeOnly = compilation.GetTypeByMetadataName( "System.TimeOnly" );
    }

    private HashSet<INamedTypeSymbol>? _nonSpecialPrimitiveTypes;

    private HashSet<INamedTypeSymbol> GetNonSpecialPrimitiveTypes()
    {
        var h = new HashSet<INamedTypeSymbol>( SymbolEqualityComparer.Default );

        AddIfNotNull( h, this.DateTimeOffset );
        AddIfNotNull( h, this.TimeSpan );
        AddIfNotNull( h, this.DateOnly );
        AddIfNotNull( h, this.TimeOnly );

        return h;

        static void AddIfNotNull( HashSet<INamedTypeSymbol> hashSet, INamedTypeSymbol? type )
        {
            if ( type != null )
            {
                hashSet.Add( type );
            }
        }
    }

    public bool IsNonSpecialPrimitiveType( ITypeSymbol type )
    {
        this._nonSpecialPrimitiveTypes ??= this.GetNonSpecialPrimitiveTypes();

        return type is INamedTypeSymbol namedType && this._nonSpecialPrimitiveTypes.Contains( namedType );            
    }

    public INamedTypeSymbol? DateTimeOffset { get; }

    public INamedTypeSymbol? TimeSpan { get; }

    public INamedTypeSymbol? DateOnly { get; }

    public INamedTypeSymbol? TimeOnly { get; }
}