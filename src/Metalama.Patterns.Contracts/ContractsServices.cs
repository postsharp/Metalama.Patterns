// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

namespace Metalama.Patterns.Contracts;

/// <summary>
/// This class provides services supporting Contract aspects. By adding your own classes to the <see cref="ExceptionFactory"/> chain, you can change the way the
/// exceptions are created when a contract is broken. By plugging into the <see cref="ContractLocalizedTextProvider"/> chain, you can change the way the exception messages
/// are generated. See the documentation for the classes for more details: <see cref="ContractLocalizedTextProvider"/>, <see cref="ContractExceptionFactory"/>.
/// </summary>
public class ContractsServices
{
    /// <summary>
    /// The default instance of <see cref="ContractsServices"/>.
    /// </summary>
    public static readonly ContractsServices Default;

    /// <summary>
    /// The default exception factory is kept for the handling of the obsolete exception creation methods in LocationContractAttribute.
    /// </summary>
    internal static readonly ContractExceptionFactory DefaultExceptionFactory;        

    static ContractsServices()
    {
        DefaultExceptionFactory = new DefaultContractExceptionFactory( null );
        Default = new ContractsServices();
    }

    private ContractsServices()
    {
    }

    private volatile ContractLocalizedTextProvider _localizedTextProvider = new(null);

    /// <summary>
    /// Gets or sets the head of the ContractLocalizedTextProvider responsibility chain.
    /// </summary>
    public ContractLocalizedTextProvider LocalizedTextProvider
    {
        get => this._localizedTextProvider;
        set => this._localizedTextProvider = value ?? throw new ArgumentNullException( nameof(value) );
    }

    private volatile ContractExceptionFactory _exceptionFactory = DefaultExceptionFactory;

    internal void ResetExceptionFactory() => this.ExceptionFactory = DefaultExceptionFactory;

    /// <summary>
    /// Gets or sets the head of the ContractExceptionFactory responsibility chain.
    /// </summary>
    public ContractExceptionFactory ExceptionFactory
    {
        get => this._exceptionFactory;
        set => this._exceptionFactory = value ?? throw new ArgumentNullException( nameof(value) );
    }
}