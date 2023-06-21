// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

namespace Flashtrace
{
    // TODO: Resolve usages, remove.
    [AttributeUsage( AttributeTargets.All )]
    internal sealed class ExplicitCrossPackageInternalAttribute : Attribute { }

    // TODO: Resolve usages, remove.
    [AttributeUsage( AttributeTargets.All )] 
    internal sealed class InternalImplementAttribute : Attribute { }

    // TODO: Resolve usages, remove.
    [AttributeUsage( AttributeTargets.All )] 
    internal sealed class RequiredAttribute : Attribute { }
}
