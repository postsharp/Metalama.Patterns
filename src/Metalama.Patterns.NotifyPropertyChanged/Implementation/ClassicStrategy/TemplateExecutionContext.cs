// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Aspects;
using Metalama.Framework.Code;
using Metalama.Patterns.NotifyPropertyChanged.Options;

namespace Metalama.Patterns.NotifyPropertyChanged.Implementation.ClassicStrategy;

/*
 * NB: TemplateExecutionContext members must not hold any reference to the IAspectBuilder<> passed
 * to BuildAspect. Members must be immutable, with the exception of cached computed values. Lazy behaviour
 * should be avoided.
 */

/// <summary>
/// Immutable context for template execution.
/// </summary>
[CompileTime]
internal sealed record TemplateExecutionContext(
    NotifyPropertyChangedOptions CommonOptions,
    ClassicImplementationStrategyOptions Options,
    INamedType TargetType,
    Assets Assets,
    InpcInstrumentationKindLookup InpcInstrumentationKindLookup,
    IReadOnlyClassicProcessingNode DependencyGraph,
    IMethod? OnUnmonitoredObservablePropertyChangedMethod,
    IMethod OnPropertyChangedMethod,
    IMethod OnChildPropertyChangedMethod,
    IMethod? BaseOnPropertyChangedMethod,
    IMethod? BaseOnChildPropertyChangedMethod,
    IMethod? BaseOnUnmonitoredObservablePropertyChangedMethod );