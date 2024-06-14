// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Aspects;
using Metalama.Framework.Code;

namespace Metalama.Patterns.Contracts;

/// <summary>
/// Fabric extension methods to configure the <c>Metalama.Patterns.Contracts</c> namespace.
/// </summary>
[CompileTime]
public static class ContractConfigurationExtensions
{
    /// <summary>
    /// Configures <see cref="ContractOptions"/> for the current <see cref="ICompilation"/>.
    /// </summary>
    public static void ConfigureContracts( this IAspectReceiver<ICompilation> receiver, ContractOptions options ) => receiver.SetOptions( options );

    /// <summary>
    /// Configures <see cref="ContractOptions"/> for the given <see cref="INamespace"/>.
    /// </summary>
    public static void ConfigureContracts( this IAspectReceiver<INamespace> receiver, ContractOptions options ) => receiver.SetOptions( options );

    /// <summary>
    /// Configures <see cref="ContractOptions"/> for the given <see cref="INamedType"/>.
    /// </summary>
    public static void ConfigureContracts( this IAspectReceiver<INamedType> receiver, ContractOptions options ) => receiver.SetOptions( options );

    /// <summary>
    /// Configures <see cref="ContractOptions"/> for the given <see cref="IFieldOrPropertyOrIndexer"/>.
    /// </summary>
    public static void ConfigureContracts( this IAspectReceiver<IFieldOrPropertyOrIndexer> receiver, ContractOptions options )
        => receiver.SetOptions( options );

    /// <summary>
    /// Configures <see cref="ContractOptions"/> for the given <see cref="IMethod"/>.
    /// </summary>
    public static void ConfigureContracts( this IAspectReceiver<IMethod> receiver, ContractOptions options ) => receiver.SetOptions( options );

    /// <summary>
    /// Configures <see cref="ContractOptions"/> for the given <see cref="IParameter"/>.
    /// </summary>
    public static void ConfigureContracts( this IAspectReceiver<IParameter> receiver, ContractOptions options ) => receiver.SetOptions( options );
}