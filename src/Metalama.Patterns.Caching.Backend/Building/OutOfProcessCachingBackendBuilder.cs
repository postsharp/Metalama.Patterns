// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

namespace Metalama.Patterns.Caching.Building;

/// <summary>
/// A base class for <see cref="CachingBackendBuilder"/> representing an out-of-process cache. These back-ends can be further
/// enhanced with non-blocking modifiers or an in-memory L1 layer through the <see cref="CachingBackendFactory.NonBlocking"/>
/// or <see cref="CachingBackendFactory.WithLocalLayer(OutOfProcessCachingBackendBuilder)"/>
/// methods. 
/// </summary>
public abstract class OutOfProcessCachingBackendBuilder : ConcreteCachingBackendBuilder { }