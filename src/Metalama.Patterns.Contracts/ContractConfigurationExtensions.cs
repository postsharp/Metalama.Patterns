// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Aspects;
using Metalama.Framework.Code;

namespace Metalama.Patterns.Contracts;

[CompileTime]
public static class ContractConfigurationExtensions
{
    public static void ConfigureContracts( this IAspectReceiver<ICompilation> receiver, ContractOptions options ) => receiver.SetOptions( options );

    public static void ConfigureContracts( this IAspectReceiver<INamespace> receiver, ContractOptions options ) => receiver.SetOptions( options );

    public static void ConfigureContracts( this IAspectReceiver<INamedType> receiver, ContractOptions options ) => receiver.SetOptions( options );

    public static void ConfigureContracts( this IAspectReceiver<IFieldOrPropertyOrIndexer> receiver, ContractOptions options )
        => receiver.SetOptions( options );

    public static void ConfigureContracts( this IAspectReceiver<IMethod> receiver, ContractOptions options ) => receiver.SetOptions( options );

    public static void ConfigureContracts( this IAspectReceiver<IParameter> receiver, ContractOptions options ) => receiver.SetOptions( options );
}