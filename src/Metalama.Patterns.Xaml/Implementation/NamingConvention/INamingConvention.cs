// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Aspects;

namespace Metalama.Patterns.Xaml.Implementation.NamingConvention;

[CompileTime]
internal interface INamingConvention
{
    string DiagnosticName { get; }
}

[CompileTime]
internal interface INamingConvention<TArguments, TMatch> : INamingConvention
    where TMatch : INamingConventionMatch
{
    TMatch Match( TArguments arguments, InspectedDeclarationsAdder inspectedDeclarations );
}