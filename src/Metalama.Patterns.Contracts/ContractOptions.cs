// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Code;
using Metalama.Framework.Options;

namespace Metalama.Patterns.Contracts;

public sealed class ContractOptions : IHierarchicalOptions<ICompilation>, IHierarchicalOptions<INamespace>, IHierarchicalOptions<INamedType>,
                                      IHierarchicalOptions<IFieldOrPropertyOrIndexer>,
                                      IHierarchicalOptions<IMethod>, IHierarchicalOptions<IParameter>
{
    public ContractTemplates? Templates { get; init; }

    public object ApplyChanges( object changes, in ApplyChangesContext context )
    {
        var other = (ContractOptions) changes;

        return new ContractOptions { Templates = other.Templates ?? this.Templates };
    }

    public IHierarchicalOptions GetDefaultOptions( OptionsInitializationContext context ) => new ContractOptions { Templates = new ContractTemplates() };
}