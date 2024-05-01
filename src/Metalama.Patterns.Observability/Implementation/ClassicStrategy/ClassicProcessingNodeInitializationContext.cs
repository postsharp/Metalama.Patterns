// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Aspects;
using Metalama.Framework.Code;

namespace Metalama.Patterns.Observability.Implementation.ClassicStrategy;

[CompileTime]
internal sealed record ClassicProcessingNodeInitializationContext( ICompilation Compilation, ClassicObservabilityStrategyImpl Helper )
    : ProcessingNodeInitializationContext( Compilation );