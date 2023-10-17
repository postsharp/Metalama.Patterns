﻿// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Aspects;
using Metalama.Patterns.Observability.Implementation;
using Metalama.Patterns.Observability.Implementation.Graph;

namespace Metalama.Patterns.Observability.CompileTimeTests;

[CompileTime]
internal interface IReadOnlyTestProcessingNode : IReadOnlyProcessingNode, INode<IReadOnlyTestProcessingNode> { }