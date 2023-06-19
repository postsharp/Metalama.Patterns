// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Xunit.Abstractions;

namespace Flashtrace.Formatters.UnitTests.Formatters;

public abstract class FormattersTestsBase
{
    protected readonly ITestOutputHelper _logger;

    public FormattersTestsBase( ITestOutputHelper logger )
    {
        this._logger = logger;
    }

    /// <summary>
    /// Gets a new instance of <see cref="FormatterRepository"/>.
    /// </summary>
    protected static FormatterRepository GetNewRepository() => new FormatterRepository( TestRole.Instance );

    /// <summary>
    /// Gets a shared instance of <see cref="FormatterRepository"/>.
    /// </summary>
    protected FormatterRepository DefaultRepository { get; } = GetNewRepository();

    /// <summary>
    /// Formats a value using <see cref="DefaultRepository"/>.
    /// </summary>
    protected string FormatDefault<T>( T value )
        => this.Format<T>( this.DefaultRepository, value );

    /// <summary>
    /// Formats a value using the specified <see cref="IFormatterRepository"/>.
    /// </summary>
    protected string Format<T>( IFormatterRepository formatterRepository, T value )
    {
        var stringBuilder = new UnsafeStringBuilder( 1024 );
        formatterRepository.Get<T>()!.Write( stringBuilder, value );
        var result = stringBuilder.ToString();
        //this._logger?.WriteLine( "'" + value?.ToString() + "' => '" + result + "'" );
        return result;
    }
}