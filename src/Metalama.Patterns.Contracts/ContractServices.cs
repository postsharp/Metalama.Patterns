// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

namespace Metalama.Patterns.Contracts;

/// <summary>
/// This class provides services supporting Contract aspects. By adding your own classes to the <see cref="ExceptionFactory"/> chain, you can change the way the
/// exceptions are created when a contract is broken. By plugging into the <see cref="ContractLocalizedTextProvider"/> chain, you can change the way the exception messages
/// are generated. See the documentation for the classes for more details: <see cref="ContractLocalizedTextProvider"/>, <see cref="ContractExceptionFactory"/>.
/// </summary>
public static class ContractServices
{
    private static volatile ContractLocalizedTextProvider _localizedTextProvider = new(null);

    /// <summary>
    /// Gets or sets the head of the ContractLocalizedTextProvider responsibility chain.
    /// </summary>
    public static ContractLocalizedTextProvider LocalizedTextProvider
    {
        get => _localizedTextProvider;
        set => _localizedTextProvider = value ?? throw new ArgumentNullException( nameof(value) );
    }

    /// <summary>
    /// The default exception factory is kept for the handling of the obsolete exception creation methods in LocationContractAttribute.
    /// </summary>
    internal static readonly ContractExceptionFactory DefaultExceptionFactory =
        new DefaultContractExceptionFactory( null );

    private static volatile ContractExceptionFactory _exceptionFactory = DefaultExceptionFactory;

    internal static void ResetExceptionFactory() => ExceptionFactory = DefaultExceptionFactory;

    /// <summary>
    /// Gets or sets the head of the ContractExceptionFactory responsibility chain.
    /// </summary>
    public static ContractExceptionFactory ExceptionFactory
    {
        get => _exceptionFactory;
        set => _exceptionFactory = value ?? throw new ArgumentNullException( nameof(value) );
    }
}