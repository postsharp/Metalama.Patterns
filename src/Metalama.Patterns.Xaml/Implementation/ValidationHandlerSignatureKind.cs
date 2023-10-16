// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Aspects;

namespace Metalama.Patterns.Xaml.Implementation;

[CompileTime]
internal enum ValidationHandlerSignatureKind
{
    NotFound,
    Ambiguous,
    Invalid,
    InstanceValue,
    InstanceDependencyPropertyAndValue,
    StaticValue,
    StaticDependencyPropertyAndValue,
    StaticDependencyPropertyAndInstanceAndValue,
    StaticInstanceAndValue
}