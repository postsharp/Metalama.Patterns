// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Aspects;
using Metalama.Framework.Code;

namespace Metalama.Patterns.Xaml.Implementation.NamingConvention;

[CompileTime]
public readonly struct InspectedDeclarationsAdder
{
    private readonly ICollection<InspectedDeclaration> _inspectedDeclarations;

    internal InspectedDeclarationsAdder( ICollection<InspectedDeclaration> inspectedDeclarations )
    {
        this._inspectedDeclarations = inspectedDeclarations;
    }

    public void Add( IDeclaration declaration, bool isValid, string? category = null )
    {
        this._inspectedDeclarations.Add( new InspectedDeclaration( declaration, isValid, category ) );
    }
}