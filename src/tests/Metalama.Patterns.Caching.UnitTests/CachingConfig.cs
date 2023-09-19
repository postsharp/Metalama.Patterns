// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Patterns.Caching.Aspects;

// Don't use dependency injection by default.
[assembly: CachingConfiguration( UseDependencyInjection = false )]