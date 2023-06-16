// Copyright (c) SharpCrafters s.r.o. This file is not open source. It is released under a commercial
// source-available license. Please see the LICENSE.md file in the repository root for details.

using System;
using System.Reflection;
using PostSharp.Patterns.Diagnostics;
using PostSharp.Patterns.Threading.Primitives;
using PostSharp.Patterns.Utilities;
using PostSharp.Reflection;

namespace PostSharp.Patterns.Formatters
{
    /// <summary>
    /// Allows to get and register formatters for a specific type.
    /// </summary>
    /// <typeparam name="TRole">Marker type used to differentiate FormattingServices for different purposes (e.g. caching or logging).</typeparam>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes")]
    public abstract class FormatterRepository<TRole> where TRole : FormattingRole, new()
    {
        internal static readonly FormattingRole Role = new TRole();

        private static readonly CovariantTypeExtensionFactory<IFormatter> formatterFactory;
        private static readonly CovariantTypeExtensionFactory<IFormatter> dynamicFormatterFactory;
        private static readonly PortableConcurrentDictionary<Type, TypeExtensionInfo<IFormatter>> dynamicFormatterCache =
            new PortableConcurrentDictionary<Type, TypeExtensionInfo<IFormatter>>();
        private static readonly LogSource logger;


        /// <summary>
        /// Initializes a new <see cref="FormatterRepository{TRole}"/>.
        /// </summary>
        protected FormatterRepository()
        {

        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline")]
        static FormatterRepository()
        {
            logger = LogSourceFactory.ForRole3( Role.LoggingRole ).GetLogSource( typeof(FormatterRepository<TRole>));

            formatterFactory = new CovariantTypeExtensionFactory<IFormatter>(typeof(IFormatter<>), typeof(FormatterConverter<,>), typeof(TRole), logger);
            dynamicFormatterFactory = new CovariantTypeExtensionFactory<IFormatter>(typeof(IFormatter<>), typeof(FormatterConverter<,>), typeof(TRole), logger);
            
            RegisterDefaultFormatters();
        }

        private static void RegisterDefaultFormatters()
        {
            Register(typeof(string), StringFormatter.Instance);
            Register(typeof(char), CharFormatter.Instance);
            Register(typeof(bool), BooleanFormatter.Instance);
            Register(typeof(byte), ByteFormatter.Instance);
            Register(typeof(sbyte), SByteFormatter.Instance);
            Register(typeof(ushort), UInt16Formatter.Instance);
            Register(typeof(short), Int16Formatter.Instance);
            Register(typeof(uint), UInt32Formatter.Instance);
            Register(typeof(int), Int32Formatter.Instance);
            Register(typeof(ulong), UInt64Formatter.Instance);
            Register(typeof(long), Int64Formatter.Instance);
            Register(typeof(float), new DefaultFormatter<TRole, float>());
            Register(typeof(double), new DefaultFormatter<TRole, double>());
            Register(typeof(decimal), new DefaultFormatter<TRole, decimal>());
            Register(typeof(UIntPtr), new DefaultFormatter<TRole, UIntPtr>());
            Register(typeof(IntPtr), new DefaultFormatter<TRole, IntPtr>());
            Register(typeof(DateTime), new DefaultFormatter<TRole, DateTime>());
            Register(typeof(DateTimeOffset), new DefaultFormatter<TRole, DateTimeOffset>());
            Register(typeof(Guid), new DefaultFormatter<TRole, Guid>());
            Register(typeof(TimeSpan), new DefaultFormatter<TRole, TimeSpan>());
            Register(typeof(IFormattable), new FormattableFormatter<IFormattable, TRole>());
            Register(typeof(Nullable<>), typeof(NullableFormatter<,>));
            Register(typeof(Type), TypeFormatter.Instance);
            Register(typeof(MethodBase), MethodFormatter.Instance);
        }


        /// <summary>
        /// Requests that formatters for parameters of a given type will be resolved according
        /// to the type of the parameter value, not to the type of the parameter itself. Interfaces, abstract classes and the <see cref="object"/> class are
        /// always resolved dynamically.
        /// </summary>
        /// <typeparam name="T">Type of the parameter.</typeparam>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
        public static void SetDynamic<T>() where T : class
        {
            formatterFactory.RegisterTypeExtension( typeof(T), new DynamicFormatter<T, TRole>() );
        }

        /// <summary>
        /// Registers the given <paramref name="formatter"/> for the type <typeparamref name="T"/>.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters")]
        public static void Register<T>(IFormatter<T> formatter)
        {
            if (formatter == null)
            {
                throw new ArgumentNullException(nameof(formatter));
            }

            Register(typeof(T), formatter);
        }

        /// <summary>
        /// Registers the given <paramref name="formatter"/> for the given <paramref name="targetType"/>.
        /// </summary>
        /// <remarks>The formatter will work for the given target type, and also for any type that inherits/implements the target type.</remarks>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes")]
        public static void Register(Type targetType, IFormatter formatter)
        {
            formatterFactory.RegisterTypeExtension(targetType, formatter);
            dynamicFormatterFactory.RegisterTypeExtension(targetType, formatter);
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
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes")]
        public static void Register(Type targetType, Type formatterType)
        {
            formatterFactory.RegisterTypeExtension(targetType, formatterType);
            dynamicFormatterFactory.RegisterTypeExtension(targetType, formatterType);
        }



        /// <summary>
        /// Returns the formatter for the type <typeparamref name="T"/>. 
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes")]
        public static IFormatter<T> Get<T>()
        {
            return FormatterCache<T>.GetInstance();
        }


        /// <summary>
        /// Returns a formatter for a specific object. This overload should be used when the type of the object
        /// is not known at build time because the type is non-sealed.
        /// </summary>
        /// <param name="objectType">Object type.</param>
        /// <returns>The formatter the object <paramref name="objectType"/>.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes")]
        public static IFormatter Get(Type objectType)
        {
            return dynamicFormatterCache.GetOrAdd(objectType, getFormatterFunc).Extension;
        }



        private static void UpdateFormatterCache(Type objectType, TypeExtensionInfo<IFormatter> newFormatterInfo)
        {
            while (true)
            {
                TypeExtensionInfo<IFormatter> oldFormatterInfo;
                if (!dynamicFormatterCache.TryGetValue(objectType, out oldFormatterInfo))
                {
                    dynamicFormatterCache.TryAdd(objectType, newFormatterInfo);
                    return;
                }

                if (!newFormatterInfo.ShouldOverwrite(oldFormatterInfo))
                {
                    return;
                }

                if (dynamicFormatterCache.TryUpdate(objectType, newFormatterInfo, oldFormatterInfo))
                {
                    return;
                }

            }

        }

        private static readonly Func<Type, TypeExtensionInfo<IFormatter>> getFormatterFunc = delegate (Type type)
        {
            return dynamicFormatterFactory.GetTypeExtension(type,newFormatterInfo => UpdateFormatterCache(type, newFormatterInfo), () => CreateDefaultFormatter(type));
        };


        /// <summary>
        /// Clears formatters, but doesn't reset registrations.
        /// </summary>
        /// <remarks>Used by unit tests to clear standard formatters.</remarks>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes")]
        public static void Reset()
        {
            formatterFactory.Clear();
            RegisterDefaultFormatters();
        }

        private static IFormatter CreateDefaultFormatter( Type type )
            => type.IsAnonymous() ? (IFormatter) new AnonymousTypeFormatter<TRole>( type ) :
            (IFormatter)
            typeof( DefaultFormatter<,> ).MakeGenericType( typeof( TRole ), type ).GetConstructor(Type.EmptyTypes).Invoke( null );

        private static IFormatter CreateDefaultFormatter<T>( )
           => typeof(T).IsAnonymous() ? (IFormatter) new AnonymousTypeFormatter<TRole>( typeof(T) ) :
           (IFormatter) new DefaultFormatter<TRole,T>();


        private static class FormatterCache<T>
        {
            private static IFormatter<T> formatter;
            private static TypeExtensionInfo<IFormatter> formatterInfo;

            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline")]
            static FormatterCache()
            {
                formatterInfo = formatterFactory.GetTypeExtension(typeof(T), UpdateStaticCacheCallback, CreateDefaultFormatterStatic);
                formatter = (IFormatter<T>)formatterInfo.Extension;
            }

            private static IFormatter CreateDefaultFormatterStatic()
            {
                if ( typeof( T ) == typeof( object ) || typeof( T ).IsInterface() || typeof( T ).IsAbstract() )
                {
                    return new DynamicFormatter<T, TRole>();
                }
                else if ( typeof(T).IsEnum() )
                {
                    return new EnumFormatter<T>();
                }
                else
                {
                    return CreateDefaultFormatter<T>( );
                }
            }

            private static void UpdateStaticCacheCallback(TypeExtensionInfo<IFormatter> typeExtensionInfo)
            {
                if (typeExtensionInfo.ShouldOverwrite(formatterInfo))
                {
                    formatter = (IFormatter<T>)formatterFactory.Convert(typeExtensionInfo.Extension, typeof(T));
                    formatterInfo = typeExtensionInfo;
                }
            }

            public static IFormatter<T> GetInstance()
            {
                return formatter;
            }

        }
    }
}