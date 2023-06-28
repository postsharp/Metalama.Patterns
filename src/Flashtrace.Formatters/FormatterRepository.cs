// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using JetBrains.Annotations;
using System.Collections.Concurrent;
using System.Reflection;

namespace Flashtrace.Formatters;

/// <summary>
/// Allows to get and register formatters for a specific type.
/// </summary>
[PublicAPI]
public class FormatterRepository : IFormatterRepository
{
    private readonly CovariantTypeExtensionFactory<IFormatter, IFormatterRepository> _formatterFactory;
    private readonly CovariantTypeExtensionFactory<IFormatter, IFormatterRepository> _dynamicFormatterFactory;

    private readonly ConcurrentDictionary<Type, InvariantFormatterCacheEntry> _invariantFormatterCache = new();
    private readonly ConcurrentDictionary<Type, TypeExtensionInfo<IFormatter>> _dynamicFormatterCache = new();

    private readonly Func<Type, TypeExtensionInfo<IFormatter>> _getFormatterFunc;
    private readonly Func<Type, InvariantFormatterCacheEntry> _getInvariantFormatterFunc;

    public FormatterRepository( FormattingRole role, bool registerDefaultFormatters = true )
    {
        this.Role = role ?? throw new ArgumentNullException( nameof(role) );

        this._formatterFactory =
            new CovariantTypeExtensionFactory<IFormatter, IFormatterRepository>( typeof(IFormatter<>), typeof(FormatterConverter<,>), this );

        this._dynamicFormatterFactory =
            new CovariantTypeExtensionFactory<IFormatter, IFormatterRepository>( typeof(IFormatter<>), typeof(FormatterConverter<,>), this );

        this._getFormatterFunc = type =>
            this._dynamicFormatterFactory.GetTypeExtension(
                type,
                newFormatterInfo => this.UpdateFormatterCache( type, newFormatterInfo ),
                () => this.CreateDefaultFormatter( type ) );

        this._getInvariantFormatterFunc = type =>
            (InvariantFormatterCacheEntry) Activator.CreateInstance( typeof(InvariantFormatterCacheEntry<>).MakeGenericType( type ), this )!;

        if ( registerDefaultFormatters )
        {
            this.RegisterDefaultFormatters();
        }
    }

    protected void RegisterDefaultFormatters()
    {
        this.Register( typeof(string), new StringFormatter( this ) );
        this.Register( typeof(char), new CharFormatter( this ) );
        this.Register( typeof(bool), new BooleanFormatter( this ) );
        this.Register( typeof(byte), new ByteFormatter( this ) );
        this.Register( typeof(sbyte), new SByteFormatter( this ) );
        this.Register( typeof(ushort), new UInt16Formatter( this ) );
        this.Register( typeof(short), new Int16Formatter( this ) );
        this.Register( typeof(uint), new UInt32Formatter( this ) );
        this.Register( typeof(int), new Int32Formatter( this ) );
        this.Register( typeof(ulong), new UInt64Formatter( this ) );
        this.Register( typeof(long), new Int64Formatter( this ) );
        this.Register( typeof(float), new DefaultFormatter<float>( this ) );
        this.Register( typeof(double), new DefaultFormatter<double>( this ) );
        this.Register( typeof(decimal), new DefaultFormatter<decimal>( this ) );
        this.Register( typeof(UIntPtr), new DefaultFormatter<UIntPtr>( this ) );
        this.Register( typeof(IntPtr), new DefaultFormatter<IntPtr>( this ) );
        this.Register( typeof(DateTime), new DefaultFormatter<DateTime>( this ) );
        this.Register( typeof(DateTimeOffset), new DefaultFormatter<DateTimeOffset>( this ) );
        this.Register( typeof(Guid), new DefaultFormatter<Guid>( this ) );
        this.Register( typeof(TimeSpan), new DefaultFormatter<TimeSpan>( this ) );
        this.Register( typeof(IFormattable), new FormattableFormatter<IFormattable>( this ) );
        this.Register( typeof(Nullable<>), typeof(NullableFormatter<>) );
        this.Register( typeof(Type), new TypeFormatter( this ) );
        this.Register( typeof(MethodBase), new MethodInfoFormatter( this ) );
    }

    /// <inheritdoc />
    public FormattingRole Role { get; }

    /// <summary>
    /// Requests that formatters for parameters of a given type will be resolved according
    /// to the type of the parameter value, not to the type of the parameter itself. Interfaces, abstract classes and the <see cref="object"/> class are
    /// always resolved dynamically.
    /// </summary>
    /// <typeparam name="T">Type of the parameter.</typeparam>
    public void SetDynamic<T>()
        where T : class
    {
        this._formatterFactory.RegisterTypeExtension( typeof(T), new DynamicFormatter<T>( this ) );
    }

    /// <summary>
    /// Registers the given <paramref name="formatter"/> for the type <typeparamref name="T"/>.
    /// </summary>
    public void Register<T>( IFormatter<T> formatter )
    {
        if ( formatter == null )
        {
            throw new ArgumentNullException( nameof(formatter) );
        }

        this.Register( typeof(T), formatter );
    }

    /// <summary>
    /// Registers the given <paramref name="formatter"/> for the given <paramref name="targetType"/>.
    /// </summary>
    /// <remarks>The formatter will work for the given target type, and also for any type that inherits/implements the target type.</remarks>
    public void Register( Type targetType, IFormatter formatter )
    {
        this._formatterFactory.RegisterTypeExtension( targetType, formatter );
        this._dynamicFormatterFactory.RegisterTypeExtension( targetType, formatter );
    }

    /// <summary>
    /// Registers the given <paramref name="formatterType"/> for the given <paramref name="targetType"/>.
    /// </summary>
    /// <remarks>
    /// <para>Instances of <paramref name="formatterType"/> are going to be created using a parameterless constructor.</para>
    /// <para>When <paramref name="targetType"/> is generic, the registration applies to its generic instantiations
    /// (and types that inherit/implement them).</para>
    /// </remarks>
    /// <example>
    /// If you register a formatter as
    /// <c>FormattingServices.RegisterFormatter(typeof(IDictionary&lt;,&gt;), typeof(MyDictionaryFormatter&lt;,&gt;)</c>
    /// and then log a parameter of type <c>Dictionary&lt;int, string&gt;</c>, the formatter for that parameter will be created
    /// by calling <c>new MyDictionaryFormatter&lt;int, string&gt;</c>.
    /// </example>
    public void Register( Type targetType, Type formatterType )
    {
        this._formatterFactory.RegisterTypeExtension( targetType, formatterType );
        this._dynamicFormatterFactory.RegisterTypeExtension( targetType, formatterType );
    }

    /// <summary>
    /// Returns the formatter for the type <typeparamref name="T"/>. 
    /// </summary>
    public IFormatter<T> Get<T>() => (IFormatter<T>) this._invariantFormatterCache.GetOrAdd( typeof(T), this._getInvariantFormatterFunc ).GetInstance();

    /// <summary>
    /// Returns a formatter for a specific object. This overload should be used when the type of the object
    /// is not known at build time because the type is non-sealed.
    /// </summary>
    /// <param name="objectType">Object type.</param>
    /// <returns>The formatter the object <paramref name="objectType"/>.</returns>
    public IFormatter Get( Type objectType )
        => this._dynamicFormatterCache.GetOrAdd( objectType, this._getFormatterFunc ).Extension
           ?? throw new FormatterNotFoundException();
    
    /// <summary>
    /// Attempts to get the <see cref="IFormatter"/> for the specified <see cref="Type"/>.
    /// </summary>
    /// <param name="objectType">Object type.</param>
    /// <param name="formatter">The formatter for objects of the specified type, or null if the repository is unable to provide a formatter for the specified type.</param>
    /// <returns>true if the repository was able to provide a formatter for the specified type; otherwise, false.</returns>
    public bool TryGet( Type objectType, out IFormatter? formatter )
    {
        formatter = this._dynamicFormatterCache.GetOrAdd( objectType, this._getFormatterFunc ).Extension;

        return formatter != null;
    }
    
    private void UpdateFormatterCache( Type objectType, TypeExtensionInfo<IFormatter> newFormatterInfo )
    {
        while ( true )
        {
            if ( !this._dynamicFormatterCache.TryGetValue( objectType, out var oldFormatterInfo ) )
            {
                this._dynamicFormatterCache.TryAdd( objectType, newFormatterInfo );

                return;
            }

            if ( !newFormatterInfo.ShouldOverwrite( oldFormatterInfo ) )
            {
                return;
            }

            if ( this._dynamicFormatterCache.TryUpdate( objectType, newFormatterInfo, oldFormatterInfo ) )
            {
                return;
            }
        }
    }

    private IFormatter CreateDefaultFormatter( Type type )
        => type.IsAnonymous()
            ? new AnonymousTypeFormatter( this, type )
            : (IFormatter) Activator.CreateInstance(
                typeof(DefaultFormatter<>).MakeGenericType( type ),
                this )!;

    private IFormatter CreateDefaultFormatter<T>()
        => typeof(T).IsAnonymous()
            ? new AnonymousTypeFormatter( this, typeof(T) )
            : new DefaultFormatter<T>( this );

    private abstract class InvariantFormatterCacheEntry
    {
        public abstract IFormatter GetInstance();
    }

    private sealed class InvariantFormatterCacheEntry<T> : InvariantFormatterCacheEntry
    {
        private readonly FormatterRepository _parent;
        private IFormatter<T> _formatter;
        private TypeExtensionInfo<IFormatter> _formatterInfo;

        public InvariantFormatterCacheEntry( FormatterRepository parent )
        {
            this._parent = parent;
            this._formatterInfo = parent._formatterFactory.GetTypeExtension( typeof(T), this.UpdateCacheCallback, this.CreateDefaultFormatter );

            this._formatter = (IFormatter<T>) (this._formatterInfo.Extension
                                               ?? throw new FlashtraceFormattersAssertionFailedException( "null was not expected." ));
        }

        private IFormatter CreateDefaultFormatter()
        {
            if ( typeof(T) == typeof(object) || typeof(T).IsInterface || typeof(T).IsAbstract )
            {
                return new DynamicFormatter<T>( this._parent );
            }
            else if ( typeof(T).IsEnum )
            {
                return (IFormatter) Activator.CreateInstance( typeof(EnumFormatter<>).MakeGenericType( typeof(T) ), this._parent )!;
            }
            else
            {
                return this._parent.CreateDefaultFormatter<T>();
            }
        }

        private void UpdateCacheCallback( TypeExtensionInfo<IFormatter> typeExtensionInfo )
        {
            if ( typeExtensionInfo.ShouldOverwrite( this._formatterInfo ) )
            {
                this._formatter = (IFormatter<T>) (this._parent._formatterFactory.Convert( typeExtensionInfo.Extension, typeof(T) )
                                                   ?? throw new FlashtraceFormattersAssertionFailedException( "null was not expected." ));

                this._formatterInfo = typeExtensionInfo;
            }
        }

        public override IFormatter GetInstance() => this._formatter;
    }
}