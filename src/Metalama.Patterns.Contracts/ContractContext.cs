// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Aspects;
using Metalama.Framework.Code;

namespace Metalama.Patterns.Contracts;

[CompileTime]
public sealed class ContractContext
{
    public IMetaTarget Target { get; }

    public ContractContext( IMetaTarget target )
    {
        this.Target = target;
        this.Options = target.Declaration.GetContractOptions();
    }

    public ContractOptions Options { get; }

    public IType TargetType => ((IHasType) this.Target.Declaration).Type;

    public string TargetDisplayName
        => this.Target.Declaration.DeclarationKind switch
        {
            DeclarationKind.Parameter when this.Target.Parameter.IsReturnParameter => "return value",
            DeclarationKind.Parameter => $"'{this.Target.Parameter.Name}' parameter",
            DeclarationKind.Property => $"'{this.Target.Property.Name}' property",
            DeclarationKind.Field => $"'{this.Target.Field.Name}' field",
            DeclarationKind.Indexer => "indexer",
            _ => throw new ArgumentOutOfRangeException(
                nameof(this.Target) + "." + nameof(this.Target.Declaration) + "." +
                nameof(this.Target.Declaration.DeclarationKind) )
        };

    public string TargetParameterName
        => this.Target.Declaration.DeclarationKind switch
        {
            DeclarationKind.Parameter => this.Target.Parameter.Name,
            DeclarationKind.Property or DeclarationKind.Field or DeclarationKind.Indexer => "value",
            _ => throw new ArgumentOutOfRangeException(
                nameof(this.Target) + "." + nameof(this.Target.Declaration) + "." +
                nameof(this.Target.Declaration.DeclarationKind) )
        };
}