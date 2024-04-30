// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Aspects;
using Metalama.Patterns.Observability.Implementation;
using Metalama.Patterns.Observability.Implementation.DependencyAnalysis;

namespace Metalama.Patterns.Observability.CompileTimeTests;

// ReSharper disable once RedundantTypeDeclarationBody
[CompileTime]
internal interface IReadOnlyTestProcessingNode : IReadOnlyProcessingNode, IDependencyNode<IReadOnlyTestProcessingNode> { }