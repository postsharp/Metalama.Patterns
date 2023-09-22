// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

namespace Metalama.Patterns.NotifyPropertyChanged;

// TODO: Update comment ported from PS once feature is implemented.

/// <summary>
/// Custom attribute that, when applied on a property, prevents the dependency analysis algorithm used by the
/// <see cref="NotifyPropertyChangedAttribute"/> aspect from emitting errors when it encounters constructs
/// that it cannot analyze. 
/// </summary>
/// <remarks>
/// <para>Getters of properties annotated with <see cref="SafeForDependencyAnalysisAttribute"/> should typically
/// specify dependencies manually using the <c>Depends</c> class. Note that all dependencies that can be
/// found using code analysis will be taken into account. The <see cref="SafeForDependencyAnalysisAttribute"/> only
/// disables errors, but not the code analysis itself.</para>
/// </remarks>
[AttributeUsage( AttributeTargets.Property )]
public sealed class SafeForDependencyAnalysisAttribute : Attribute
{
    // TODO: Not sure why this attribute is not defined on methods too.
}