// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Aspects;
using Metalama.Framework.Code;

namespace Metalama.Patterns.Xaml.Implementation.NamingConvention;

[CompileTime]
internal readonly struct InspectedDeclaration
{
    internal InspectedDeclaration( IDeclaration declaration, bool isValid, string? category )
    {
        this.Declaration = declaration;
        this.IsValid = isValid;
        this.Category = category;
    }

    public IDeclaration Declaration { get; }

    public bool IsValid { get; }

    public string? Category { get; }
}
