// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Aspects;
using Metalama.Framework.Code;

namespace Metalama.Patterns.Xaml.Implementation.NamingConvention;

[CompileTime]
internal readonly struct InspectedMemberAdder
{
    private readonly ICollection<InspectedMember> _inspectedDeclarations;

    internal InspectedMemberAdder( ICollection<InspectedMember> inspectedDeclarations )
    {
        this._inspectedDeclarations = inspectedDeclarations;
    }

    public void Add( IMemberOrNamedType member, bool isValid, string? category = null )
        => this._inspectedDeclarations.Add( new InspectedMember( member, isValid, category ) );
}