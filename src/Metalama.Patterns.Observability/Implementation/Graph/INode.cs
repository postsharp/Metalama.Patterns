// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Aspects;

namespace Metalama.Patterns.Observability.Implementation.Graph;

// ReSharper disable once RedundantTypeDeclarationBody
[CompileTime]
internal interface INode<T> : IHasDepth, IHasChildren<T>, IHasParent<T>, IHasReferencedBy<T> { }