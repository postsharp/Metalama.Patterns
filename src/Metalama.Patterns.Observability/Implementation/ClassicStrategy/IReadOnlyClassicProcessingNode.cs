// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Aspects;
using Metalama.Framework.Code;
using Metalama.Patterns.Observability.Implementation.DependencyAnalysis;

namespace Metalama.Patterns.Observability.Implementation.ClassicStrategy;

[CompileTime]
internal interface IReadOnlyClassicProcessingNode : IReadOnlyProcessingNode, IDependencyNode<IReadOnlyClassicProcessingNode>
{
    /// <summary>
    /// Gets the <see cref="InpcInstrumentationKind"/> for the type of the field or property.
    /// </summary>
    InpcInstrumentationKind PropertyTypeInpcInstrumentationKind { get; }

    /// <summary>
    /// Gets a method like <c>void UpdateA2C2()</c>. 
    /// </summary>
    /// <remarks>
    /// In a given inheritance hierarchy, for a given property path, this method is defined in
    /// the type closest to the root type where a reference to a property of A2.C2 first occurs.
    /// The method is always private as it is only called by other members of the type where it
    /// is defined. The value of <see cref="UpdateMethod"/> is always explicitly set for all 
    /// nodes (regardless of whether the value is <see langword="null"/> or not), and accessing the value
    /// will throw if it has not been set. This provides defense against incorrect program design
    /// that could otherwise lead to subtle incorrect behaviour.
    /// </remarks>
    IDeferred<IMethod?> UpdateMethod { get; }

    /// <summary>
    /// Gets the non-null <see cref="UpdateMethod"/> declarations of the children of the current node.
    /// </summary>
    IReadOnlyCollection<IMethod> ChildUpdateMethods { get; }

    InpcBaseHandling InpcBaseHandling { get; }

    /// <summary>
    /// Gets the field like "PropertyChangedEventHandler? _onA2PropertyChangedHandler", if it has been introduced.
    /// </summary>
    IDeferred<IField> HandlerField { get; }

    /// <summary>
    /// Gets the field like "B? _lastA2", if it has been introduced.
    /// </summary>
    IDeferred<IField> LastValueField { get; }
}