// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using JetBrains.Annotations;
using System.ComponentModel;

namespace Metalama.Patterns.Observability;

/// <summary>
/// Indicates that the <c>OnChildPropertyChanged</c> and <c>OnObservablePropertyChanged</c> is invoked for the specified properties.
/// </summary>
[PublicAPI]
[EditorBrowsable( EditorBrowsableState.Never )]
[AttributeUsage( AttributeTargets.Method )]
public sealed class InvokedForPropertiesAttribute : Attribute
{
    // ReSharper disable once UnusedParameter.Local
    /// <summary>
    /// Initializes a new instance of the <see cref="InvokedForPropertiesAttribute"/> class.
    /// </summary>
    /// <param name="parentPropertyPaths">
    /// The parent property paths which the declaring class monitors for reference and child property changes,
    /// and for which  <c>OnChildPropertyChanged</c> or <c>OnObservablePropertyChanged</c>  will be called.
    /// </param>
    public InvokedForPropertiesAttribute( params string[] parentPropertyPaths ) { }
}