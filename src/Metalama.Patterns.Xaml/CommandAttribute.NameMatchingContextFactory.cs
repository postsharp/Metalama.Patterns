// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Aspects;
using Metalama.Patterns.Xaml.Implementation.NamingConvention;

namespace Metalama.Patterns.Xaml;

public sealed partial class CommandAttribute
{
    [CompileTime]
    private readonly struct NameMatchingContextFactory : INamingConventionMatchContextFactory<NameMatchingContext>
    {
        public NameMatchingContext Create( in InspectedDeclarationsAdder inspectedDeclarations )
            => new NameMatchingContext() { InspectedDeclarations = inspectedDeclarations };
    }
}