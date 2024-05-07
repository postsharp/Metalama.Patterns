﻿// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Aspects;

namespace Metalama.Patterns.Xaml.Implementation.NamingConvention;

[CompileTime]
internal readonly record struct InspectedNamingConventionMatch<TMatch>( TMatch Match, IEnumerable<InspectedMember> InspectedDeclarations )
    where TMatch : NamingConventionMatch;