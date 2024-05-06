// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Aspects;
using Metalama.Framework.Serialization;

namespace Metalama.Patterns.Xaml.Implementation.NamingConvention;

[CompileTime]
internal interface INamingConvention : ICompileTimeSerializable
{
    string Name { get; }
}

[CompileTime]
internal interface INamingConvention<in TArguments, out TMatch> : INamingConvention
    where TMatch : NamingConventionMatch
{
    TMatch Match( TArguments arguments, InspectedMemberAdder inspectedMember );
}