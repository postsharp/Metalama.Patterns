// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Aspects;

namespace Metalama.Patterns.Xaml.Implementation.NamingConvention;

[CompileTime]
public interface INamingConvention
{
    string DiagnosticName { get; }
}

[CompileTime]
public interface INamingConvention<TArguments, TContext, TMatch> : INamingConvention
    where TMatch : INamingConventionMatch
{
    TMatch Match<TContextImpl>( in TArguments arguments, in TContextImpl context )
        where TContextImpl : TContext;
}