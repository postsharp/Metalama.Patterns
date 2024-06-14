// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using JetBrains.Annotations;
using Metalama.Framework.Aspects;
using Metalama.Framework.Code;

namespace Metalama.Patterns.Contracts;

/// <summary>
/// Represents the context in which a <see cref="ContractBaseAttribute"/> is used,
/// i.e. its target declaration and <see cref="Options"/>.
/// </summary>
[CompileTime]
[PublicAPI]
public sealed class ContractContext
{
    private readonly IMetaTarget _target;

    public ContractContext( IMetaTarget target )
    {
        this._target = target;
        this.Options = target.Declaration.GetContractOptions();
    }

    /// <summary>
    /// Gets the declaration (parameter, field or property) validated by the contract.
    /// </summary>
    public IHasType TargetDeclaration => (IHasType) this._target.Declaration;

    /// <summary>
    /// Gets the application options.
    /// </summary>
    public ContractOptions Options { get; }

    /// <summary>
    /// Gets the type of the <see cref="TargetDeclaration"/>.
    /// </summary>
    public IType Type => this.TargetDeclaration.Type;

    /// <summary>
    /// Gets the display name of the <see cref="TargetDeclaration"/>.
    /// </summary>
    public string TargetDisplayName
        => this._target.Declaration.DeclarationKind switch
        {
            DeclarationKind.Parameter when this._target.Parameter.IsReturnParameter => "return value",
            DeclarationKind.Parameter => $"'{this._target.Parameter.Name}' parameter",
            DeclarationKind.Property => $"'{this._target.Property.Name}' property",
            DeclarationKind.Field => $"'{this._target.Field.Name}' field",
            DeclarationKind.Indexer => "indexer",
            _ => throw new ArgumentOutOfRangeException(
                nameof(this._target) + "." + nameof(this._target.Declaration) + "." +
                nameof(this._target.Declaration.DeclarationKind) )
        };

    /// <summary>
    /// Gets the name of the target parameter, or <c>value</c> if the target declaration is a property.
    /// </summary>
    public string TargetParameterName
        => this._target.Declaration.DeclarationKind switch
        {
            DeclarationKind.Parameter => this._target.Parameter.Name,
            DeclarationKind.Property or DeclarationKind.Field or DeclarationKind.Indexer => "value",
            _ => throw new ArgumentOutOfRangeException(
                nameof(this._target) + "." + nameof(this._target.Declaration) + "." +
                nameof(this._target.Declaration.DeclarationKind) )
        };
}