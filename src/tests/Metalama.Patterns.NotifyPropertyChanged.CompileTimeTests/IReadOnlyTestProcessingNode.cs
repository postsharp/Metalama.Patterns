// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Aspects;
using Metalama.Patterns.NotifyPropertyChanged.Implementation;
using Metalama.Patterns.NotifyPropertyChanged.Implementation.Graph;

namespace Metalama.Patterns.NotifyPropertyChanged.CompileTimeTests;

[CompileTime]
internal interface IReadOnlyTestProcessingNode : IReadOnlyProcessingNode, INode<IReadOnlyTestProcessingNode> { }
