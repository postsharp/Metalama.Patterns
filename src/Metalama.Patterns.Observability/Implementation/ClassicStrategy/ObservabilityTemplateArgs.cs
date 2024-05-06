// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Aspects;
using Metalama.Framework.Code;
using Metalama.Patterns.Observability.Configuration;

namespace Metalama.Patterns.Observability.Implementation.ClassicStrategy;

/*
 * NB: ObservabilityTemplateArgs members must not hold any reference to the IAspectBuilder<> passed
 * to BuildAspect. Members must be immutable, with the exception of cached computed values. Lazy behaviour
 * should be avoided.
 */

// ReSharper disable once NotAccessedPositionalProperty.Global
/// <summary>
/// Immutable context for template execution.
/// </summary>
[CompileTime]
internal sealed record ObservabilityTemplateArgs(
    ObservabilityOptions CommonOptions,
    ClassicObservabilityStrategyOptions Options,
    INamedType TargetType,
    Assets Assets,
    InpcInstrumentationKindLookup InpcInstrumentationKindLookup,
    ClassicObservableTypeInfo ObservableTypeInfo,
    IMethod? OnObservablePropertyChangedMethod,
    IMethod OnPropertyChangedMethod,
    IMethod? OnChildPropertyChangedMethod,
    IMethod? BaseOnPropertyChangedMethod,
    IMethod? BaseOnChildPropertyChangedMethod,
    IMethod? BaseOnObservablePropertyChangedMethod );