// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Aspects;

namespace Metalama.Patterns.Contracts;

[CompileTime]
internal record InequalityAmbiguity( InequatilyStrictness DefaultStrictness, string NewName1, string NewName2 );