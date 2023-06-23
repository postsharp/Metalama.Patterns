// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

// ReSharper disable All
#pragma warning disable SA1649
#pragma warning disable SA1402

namespace Flashtrace;

// TODO: [FT-PreReview] Resolve usages, remove.
[AttributeUsage( AttributeTargets.All )]
internal sealed class ExplicitCrossPackageInternalAttribute : Attribute { }

// TODO: [FT-PreReview] Resolve usages, remove.
[AttributeUsage( AttributeTargets.All )]
internal sealed class InternalImplementAttribute : Attribute { }

// TODO: [FT-PreReview] Resolve usages, remove.
[AttributeUsage( AttributeTargets.All )]
internal sealed class RequiredAttribute : Attribute { }