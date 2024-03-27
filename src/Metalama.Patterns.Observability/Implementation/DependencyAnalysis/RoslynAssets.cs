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
    private readonly INamedTypeSymbol? _dateTimeOffset;
    private readonly INamedTypeSymbol? _timeSpan;
    private readonly INamedTypeSymbol? _dateOnly;
    private readonly INamedTypeSymbol? _timeOnly;

    public RoslynAssets( Compilation compilation )
    {
        this._timeSpan = compilation.GetTypeByMetadataName( "System.TimeSpan" );
        this._dateTimeOffset = compilation.GetTypeByMetadataName( "System.DateTimeOffset" );
        this._dateOnly = compilation.GetTypeByMetadataName( "System.DateOnly" );
        this._timeOnly = compilation.GetTypeByMetadataName( "System.TimeOnly" );
    }

    private HashSet<INamedTypeSymbol>? _nonSpecialPrimitiveTypes;

    private HashSet<INamedTypeSymbol> GetNonSpecialPrimitiveTypes()
    {
        var h = new HashSet<INamedTypeSymbol>( SymbolEqualityComparer.Default );

        AddIfNotNull( h, this._dateTimeOffset );
        AddIfNotNull( h, this._timeSpan );
        AddIfNotNull( h, this._dateOnly );
        AddIfNotNull( h, this._timeOnly );

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
}