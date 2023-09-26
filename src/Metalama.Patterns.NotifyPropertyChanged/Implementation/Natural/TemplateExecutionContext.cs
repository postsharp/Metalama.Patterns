﻿// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Aspects;
using Metalama.Framework.Code;
using Metalama.Patterns.NotifyPropertyChanged.Options;

namespace Metalama.Patterns.NotifyPropertyChanged.Implementation.Natural;

/*
 * NB: TemplateExecutionContext members must not hold any reference to the IAspectBuilder<> passed
 * to BuildAspect. Members must be immutable, with the exception of cached lookups. Lazy behaviour 
 * should be avoided.
 */

/// <summary>
/// Shared context for template execution.
/// </summary>
[CompileTime]
internal sealed record TemplateExecutionContext(
    Elements Elements,
    ClassicImplementationStrategyOptions Options,
    NotifyPropertyChangedOptions CommonOptions,
    InpcInstrumentationKindLookup InpcInstrumentationKindLookup,
    IReadOnlyDependencyGraphNode DependencyGraph,
    IReadOnlyDeferredDeclaration<IMethod> OnUnmonitoredObservablePropertyChangedMethod,
    IReadOnlyCertainDeferredDeclaration<IMethod> OnPropertyChangedMethod,
    IReadOnlyCertainDeferredDeclaration<IMethod> OnChildPropertyChangedMethod,
    IMethod? BaseOnPropertyChangedMethod,
    IMethod? BaseOnChildPropertyChangedMethod,
    IMethod? BaseOnUnmonitoredObservablePropertyChangedMethod )
{
}