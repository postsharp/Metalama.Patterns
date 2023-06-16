// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

namespace Flashtrace.Formatters;

/// <summary>
/// Appends the description of an object into an <see cref="UnsafeStringBuilder"/>. Weakly-typed variant of <see cref="IFormatter{T}"/>.
/// </summary>
public interface IFormatter
{
    /// <summary>
    /// Gets the <see cref="IFormatterRepository"/> which current <see cref="IFormatter"/> uses to get formatters for other types.
    /// </summary>
    IFormatterRepository Repository { get; }

    /// <summary>
    /// Appends the description of an object into given <see cref="UnsafeStringBuilder"/> (weakly-typed variant).
    /// </summary>
    /// <param name="stringBuilder">The target <see cref="UnsafeStringBuilder"/>.</param>
    /// <param name="value">The value to be formatted.</param>
    void Write( UnsafeStringBuilder stringBuilder, object? value );

    /// <summary>
    /// Gets the formatter attributes.
    /// </summary>
    FormatterAttributes Attributes { get; }
}

/// <summary>
/// An interface that implementations of <see cref="IFormattable"/> can optionally implement to support options.
/// </summary>
public interface IOptionAwareFormatter : IFormatter
{
    /// <summary>
    /// Returns a copy of the current formatter, but for different options.
    /// </summary>
    /// <param name="options">The new options.</param>
    /// <returns>A copy of the current formatter with the new <paramref name="options"/>.</returns>
    /// <remarks>
    /// It is essential for performance that the implementation respects a semi-singleton pattern, i.e. to keep a single instance of the formatter
    /// for each single distinct value of <see cref="FormattingOptions"/>.
    /// </remarks> 
    IOptionAwareFormatter WithOptions( FormattingOptions options );
}

/// <summary>
/// Attributes of an <see cref="IFormatter"/>.
/// </summary>
[Flags]
public enum FormatterAttributes
{
    /// <summary>
    /// Default.
    /// </summary>
    None = 0,

    /// <summary>
    /// A normal (custom) formatter.
    /// </summary>
    Normal = 1,

    /// <summary>
    /// A dynamic formatter, which resolves to another formatter according to the type of the value, not the type of the location.
    /// </summary>
    Dynamic = 2,

    /// <summary>
    /// A converter.
    /// </summary>
    Converter = 4,

    /// <summary>
    /// A default formatter, using <see cref="object.ToString"/>.
    /// </summary>
    Default = 8,
}

/// <summary>
/// Options that influence the formatting of an object by an <see cref="IOptionAwareFormatter"/>.
/// </summary>
/// <remarks>
/// <para>
/// This class can be extended by implementations of custom back-end.
/// </para>
/// <para>
/// It is essential for performance that the implementation respects a semi-singleton pattern, i.e. to keep a single instance of distinct value.
/// </para>
/// </remarks>
public class FormattingOptions
{
    /// <summary>
    /// Gets the default <see cref="FormattingOptions"/>.
    /// </summary>
    public static FormattingOptions Default { get; } = new FormattingOptions( false );

    /// <summary>
    /// Gets the <see cref="FormattingOptions"/> indicating that string should not be quoted.
    /// </summary>
    public static FormattingOptions Unquoted { get; } = new FormattingOptions( true );

    /// <summary>
    /// Initializes a new instance of <see cref="FormattingOptions"/> by copying all values from another <see cref="FormattingOptions"/>.
    /// </summary>
    /// <param name="prototype">The <see cref="FormattingOptions"/> instance whose values have to be copied.</param>
    protected FormattingOptions( FormattingOptions prototype )
    {
        this.RequiresUnquotedStrings = prototype.RequiresUnquotedStrings;
    }

    private FormattingOptions( bool unquotedStrings )
    {
        this.RequiresUnquotedStrings = unquotedStrings;
    }
    
    /// <summary>
    /// Gets a value indicating whether the formatters should not use quotation when formatting strings.
    /// </summary>
    public bool RequiresUnquotedStrings { get; }
}

/// <summary>
/// Extension methods for the <see cref="IFormatter"/> interface.
/// </summary>
public static class FormatterExtensions
{
    /// <summary>
    /// Returns a copy of the current formatter, but for different options.
    /// </summary>
    /// <param name="formatter">The original formatter.</param>
    /// <param name="options">The new options.</param>
    /// <returns>A copy of the current formatter with the new <paramref name="options"/>.</returns>
    public static IFormatter<T> WithOptions<T>( this IFormatter<T> formatter, FormattingOptions options )
    {
        if ( formatter is IOptionAwareFormatter optionAwareFormatter )
        {
            return (IFormatter<T>) optionAwareFormatter.WithOptions( options );
        }
        else
        {
            return formatter;
        }
    }

    /// <summary>
    /// Returns a copy of the current formatter, but for different options.
    /// </summary>
    /// <param name="formatter">The original formatter.</param>
    /// <param name="options">The new options.</param>
    /// <returns>A copy of the current formatter with the new <paramref name="options"/>.</returns>
    public static IFormatter WithOptions( this IFormatter formatter, FormattingOptions options )
    {
        if ( formatter is IOptionAwareFormatter optionAwareFormatter )
        {
            return optionAwareFormatter.WithOptions( options ?? FormattingOptions.Default );
        }
        else
        {
            return formatter;
        }
    }
}

/// <summary>
/// Appends the description of an object into an <see cref="UnsafeStringBuilder"/>. Strongly-typed variant of <see cref="IFormatter"/>.
/// </summary>
/// <typeparam name="T">Type of values that can be formatted.</typeparam>
public interface IFormatter<in T> : IFormatter
{
    /// <summary>
    /// Appends the description of an object into given <see cref="UnsafeStringBuilder"/> (weakly-typed variant).
    /// </summary>
    /// <param name="stringBuilder">The target <see cref="UnsafeStringBuilder"/>.</param>
    /// <param name="value">The value to be formatted.</param>
    void Write( UnsafeStringBuilder stringBuilder, T? value );
}

/// <summary>
/// Base implementation of the <see cref="IFormatter{T}"/> interface.
/// </summary>
/// <typeparam name="T"></typeparam>
public abstract class Formatter<T> : IFormatter<T>, IOptionAwareFormatter
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Formatter{T}"/> class using the specified <see cref="IFormatterRepository"/>
    /// to access formatters for other types.
    /// </summary>
    protected Formatter( IFormatterRepository repository ) 
    {
        this.Repository = repository ?? throw new ArgumentNullException( nameof( repository ) );
    }
    
    public IFormatterRepository Repository { get; }

    /// <inheritdoc />  
    public void Write( UnsafeStringBuilder stringBuilder, object? value )
    {
        this.Write( stringBuilder, (T?) value );
    }

    /// <inheritdoc />
    public abstract void Write( UnsafeStringBuilder stringBuilder, T? value );

    /// <inheritdoc />
    public virtual IOptionAwareFormatter WithOptions( FormattingOptions options ) => this;
    
    IOptionAwareFormatter IOptionAwareFormatter.WithOptions( FormattingOptions options ) => this.WithOptions( options );
    
    /// <inheritdoc />
    public virtual FormatterAttributes Attributes => FormatterAttributes.Normal;

}