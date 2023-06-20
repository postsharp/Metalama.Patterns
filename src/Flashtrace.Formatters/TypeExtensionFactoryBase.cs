// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace Flashtrace.Formatters;

// T is for example IFormatter

/// <summary>
/// The base class for type extension factories for types deriving or implementing <typeparamref name="T"/>.
/// </summary>
internal class TypeExtensionFactoryBase<T>
    where T : class
{
    private readonly Type _genericInterfaceType;
    private readonly Type _converterType;
    private readonly Dictionary<Type, T> _instances = new();
    private readonly Dictionary<Type, Type> _genericExtensionTypes = new();

    private readonly Dictionary<Type, TypeExtensionCacheUpdateCallback<T>> _cacheInvalidationCallbacks = new();

    public TypeExtensionFactoryBase( Type genericInterfaceType, Type converterType )
    {
        this._genericInterfaceType = genericInterfaceType;
        this._converterType = converterType;
    }

    public void RegisterTypeExtension( Type targetType, T typeExtension )
    {
        if ( targetType == null )
        {
            throw new ArgumentNullException( nameof(targetType) );
        }

        if ( typeExtension == null )
        {
            throw new ArgumentNullException( nameof(typeExtension) );
        }

        if ( targetType.IsGenericTypeDefinition )
        {
            throw new ArgumentException(
                "When a specific type extension is supplied, target type has to be a concrete type, not a generic type definition.",
                nameof(targetType) );
        }

        var foundGenericBase = this.FindInterfaceGenericInstance( typeExtension.GetType() );

        if ( foundGenericBase != null )
        {
            var foundGenericArgument = foundGenericBase.GetGenericArguments().Single();

            // IsAssignableFrom says that T can be assigned to Nullable<T>, but we don't support that case
            if ( !foundGenericArgument.IsAssignableFrom( targetType ) || Nullable.GetUnderlyingType( foundGenericArgument ) == targetType )
            {
                throw new ArgumentException(
                    string.Format( CultureInfo.InvariantCulture, "{0} is not a type extension for the type {1}.", typeExtension.GetType(), targetType ) );
            }
        }

        lock ( this._instances )
        {
            this._instances[targetType] = typeExtension;
            var typeExtensionInfo = new TypeExtensionInfo<T>( typeExtension, targetType, false );

            foreach ( var kvp in this._cacheInvalidationCallbacks )
            {
                // IsAssignableFrom says that T can be assigned to Nullable<T>, but we don't support that case
                if ( targetType.IsAssignableFrom( kvp.Key ) && kvp.Key != Nullable.GetUnderlyingType( targetType ) )
                {
                    kvp.Value( typeExtensionInfo );
                }
            }
        }
    }

    protected void RegisterTypeExtension( Type targetType, Type typeExtensionType, object?[]? constructorArgs )
    {
        if ( targetType == null )
        {
            throw new ArgumentNullException( nameof(targetType) );
        }

        if ( typeExtensionType == null )
        {
            throw new ArgumentNullException( nameof(typeExtensionType) );
        }

        if ( !typeExtensionType.IsGenericTypeDefinition )
        {
            if ( targetType.IsGenericTypeDefinition )
            {
                throw new ArgumentException( "When type extension type is not generic, target type cannot be generic." );
            }

            this.RegisterTypeExtension( targetType, (T) Activator.CreateInstance( typeExtensionType, constructorArgs )! );

            return;
        }

        if ( this.FindInterfaceGenericInstance( typeExtensionType ) == null )
        {
            throw new ArgumentException(
                string.Format( CultureInfo.InvariantCulture, "{0} type has to inherit from {1}.", typeExtensionType, this._genericInterfaceType ),
                nameof(typeExtensionType) );
        }

        if ( !targetType.IsGenericTypeDefinition && targetType != typeof(Array) )
        {
            throw new ArgumentException( "When a type extension is generic, target type has to be generic." );
        }

        var targetTypeParametersCount = targetType == typeof(Array) ? 1 : targetType.GetGenericArguments().Length;
        var extensionTypeParametersCount = typeExtensionType.GetGenericArguments().Length;

        if ( targetTypeParametersCount != extensionTypeParametersCount )
        {
            throw new ArgumentException( "The number of generic parameters of target type and extension type are not compatible." );
        }

        lock ( this._instances )
        {
            this._genericExtensionTypes[targetType] = typeExtensionType;

            foreach ( var kvp in this._cacheInvalidationCallbacks )
            {
                this.TryInitialize( targetType, typeExtensionType, kvp.Key, kvp.Value, constructorArgs );
            }
        }
    }

    private void TryInitialize(
        Type typeExtensionTargetType,
        Type typeExtensionType,
        Type registrationType,
        TypeExtensionCacheUpdateCallback<T> cacheUpdateCallback,
        object?[]? constructorArgs )
    {
        foreach ( var assignableType in this.GetAssignableTypes( registrationType ) )
        {
            var genericAssignableTypeDefinition = GetGenericTypeDefinition( assignableType );

            if ( genericAssignableTypeDefinition == typeExtensionTargetType )
            {
                var genericArguments = GetGenericArguments( assignableType );

                var genericTypeExtensionType = MakeGenericExtensionType( typeExtensionType, genericArguments );

                var extension = (T) Activator.CreateInstance( genericTypeExtensionType, constructorArgs )!;

                var typeExtensionInfo = new TypeExtensionInfo<T>( extension, MakeGenericType( typeExtensionTargetType, genericArguments ), true );

                cacheUpdateCallback( typeExtensionInfo );
            }
        }
    }

    // TODO: Review onExceptionWhileCreatingTypeExtension - this replaces a log message.

    private T? GetExtensionCore(
        Type objectType,
        object?[]? constructorArgs,
        out bool isExtensionGeneric,
        out Type targetType,
        Action<Exception>? onExceptionWhileCreatingTypeExtension )
    {
        T? bestExtension = null;
        targetType = typeof(object);
        Func<T>? bestExtensionFactory = null;
        isExtensionGeneric = false;

        foreach ( var assignableType in this.GetAssignableTypes( objectType ) )
        {
            if ( this._instances.TryGetValue( assignableType, out var typeExtension ) )
            {
                if ( ShouldOverwrite( assignableType, false, targetType, isExtensionGeneric ) )
                {
                    bestExtension = typeExtension;
                    targetType = assignableType;
                    bestExtensionFactory = null;
                    isExtensionGeneric = false;
                }
            }

            var assignableGenericTypeDefinition = GetGenericTypeDefinition( assignableType );

            if ( assignableGenericTypeDefinition != null )
            {
                if ( this._genericExtensionTypes.TryGetValue( assignableGenericTypeDefinition, out var extensionType ) )
                {
                    var genericArguments = GetGenericArguments( assignableType );
                    var genericExtensionType = MakeGenericExtensionType( extensionType, genericArguments );

                    if ( ShouldOverwrite( assignableType, true, targetType, isExtensionGeneric ) )
                    {
                        bestExtension = null;
                        targetType = assignableType;

                        bestExtensionFactory = () => (T) Activator.CreateInstance( genericExtensionType, constructorArgs )!;

                        isExtensionGeneric = true;
                    }
                }
            }
        }

        if ( bestExtension == null && bestExtensionFactory != null )
        {
            try
            {
                return bestExtensionFactory();
            }
            catch ( Exception e )
            {
                onExceptionWhileCreatingTypeExtension?.Invoke( e );

                // TODO: Review, confirm replacement with onExceptionWhileCreatingTypeExtension.
                // this.logger.Error.Write( FormattedMessageBuilder.Formatted(  "Exception while creating a type extension for {ObjectType}.", objectType  ), e);

                // Extension constructor threw, so remove it from the dictionary and try again
                var targetGenericTypeDefinition = GetGenericTypeDefinition( targetType );

                if ( targetGenericTypeDefinition != null )
                {
                    this._genericExtensionTypes.Remove( targetGenericTypeDefinition );
                }

                return this.GetExtensionCore( objectType, constructorArgs, out isExtensionGeneric, out targetType, onExceptionWhileCreatingTypeExtension );
            }
        }

        return bestExtension;
    }

    public void Clear()
    {
        lock ( this._instances )
        {
            this._instances.Clear();
            this._genericExtensionTypes.Clear();
        }
    }

    protected TypeExtensionInfo<T> GetTypeExtension(
        Type objectType,
        object?[]? constructorArgs,
        TypeExtensionCacheUpdateCallback<T>? cacheUpdateCallback,
        Func<T> createDefault,
        Action<Exception>? onExceptionWhileCreatingTypeExtension = null )
    {
        lock ( this._instances )
        {
            var bestTypeExtension =
                this.GetExtensionCore( objectType, constructorArgs, out var isGeneric, out var targetType, onExceptionWhileCreatingTypeExtension ) ??
                createDefault();

            if ( cacheUpdateCallback != null )
            {
                if ( this._cacheInvalidationCallbacks.TryGetValue( objectType, out var multicastDelegate ) )
                {
                    multicastDelegate += cacheUpdateCallback;
                    this._cacheInvalidationCallbacks[objectType] = multicastDelegate;
                }
                else
                {
                    this._cacheInvalidationCallbacks.Add( objectType, cacheUpdateCallback );
                }
            }

            bestTypeExtension = this.Convert( bestTypeExtension, objectType, constructorArgs );

            return new TypeExtensionInfo<T>( bestTypeExtension, targetType, isGeneric );
        }
    }

    [return: NotNullIfNotNull( nameof(o) )]
    protected T? Convert( T? o, Type targetObjectType, object?[]? additionalConstructorArgs )
    {
        if ( o == null )
        {
            return null;
        }

        var targetType = this._genericInterfaceType.MakeGenericType( targetObjectType );

        // ReSharper disable once UseMethodIsInstanceOfType : Don't want to risk any subtle change of logic.
        
        if ( targetType.IsAssignableFrom( o.GetType() ) )
        {
            return o;
        }

        var extensionType = this.FindInterfaceGenericInstance( o.GetType() );

        Type wrapperType;

        if ( extensionType != null )
        {
            var sourceType = extensionType.GetGenericArguments().Single();
            wrapperType = this._converterType.MakeGenericType( targetObjectType, sourceType );
        }
        else
        {
            // The formatter does not implement a strongly-typed interface.
            wrapperType = this._converterType.MakeGenericType( targetObjectType, typeof(object) );
        }

        object[] ctorArgs;

        if ( additionalConstructorArgs == null || additionalConstructorArgs.Length == 0 )
        {
            ctorArgs = new object[] { 0 };
        }
        else
        {
            ctorArgs = new object[additionalConstructorArgs.Length + 1];
            ctorArgs[0] = o;
            Array.Copy( additionalConstructorArgs, 0, ctorArgs, 1, additionalConstructorArgs.Length );
        }

        return (T) Activator.CreateInstance( wrapperType, ctorArgs )!;
    }

    /// <remarks>This logic is supposed to mimic overload resolution in C#.</remarks>
    public static bool ShouldOverwrite( Type newExtensionTargetType, bool isNewGeneric, Type oldExtensionTargetType, bool isOldGeneric )
    {
        // new is more specific or equal
        if ( oldExtensionTargetType.IsAssignableFrom( newExtensionTargetType ) )
        {
            // new is more specific
            if ( newExtensionTargetType != oldExtensionTargetType )
            {
                return true;
            }

            // types are equal, less generic wins
            if ( isNewGeneric && !isOldGeneric )
            {
                return false;
            }

            return true;
        }

        // old is more specific
        if ( newExtensionTargetType.IsAssignableFrom( oldExtensionTargetType ) )
        {
            return false;
        }

        // the two are not comparable
        return true;
    }

    private static Type MakeGenericExtensionType( Type extensionType, Type[] genericArguments )
    {
        return extensionType.MakeGenericType( genericArguments );
    }

    private static Type? GetGenericTypeDefinition( Type type )
    {
        if ( type.IsGenericType )
        {
            return type.GetGenericTypeDefinition();
        }

        if ( type.IsArray )
        {
            return typeof(Array);
        }

        return null;
    }

    private static Type MakeGenericType( Type type, Type[] typeArguments )
    {
        if ( type.IsGenericTypeDefinition )
        {
            return type.MakeGenericType( typeArguments );
        }

        if ( type == typeof(Array) )
        {
            return typeArguments.Single().MakeArrayType();
        }

        throw new InvalidOperationException();
    }

    private static Type[] GetGenericArguments( Type type )
    {
        if ( type.IsGenericType )
        {
            return type.GetGenericArguments();
        }

        if ( type.IsArray )
        {
            return new[] { type.GetElementType()! };
        }

        return Array.Empty<Type>();
    }

    protected virtual IEnumerable<Type> GetAssignableTypes( Type type )
    {
        return new[] { type };
    }

    private Type? FindInterfaceGenericInstance( Type derivedType )
    {
        IList<Type> interfaces = derivedType.GetInterfaces()
            .Where( type => type.IsGenericType && type.GetGenericTypeDefinition() == this._genericInterfaceType )
            .ToList();

        // TODO: Have a specific exception type.
        if ( interfaces.Count > 1 )
        {
            throw new ArgumentException(
                string.Format(
                    CultureInfo.InvariantCulture,
                    "The type {0} cannot implement several generic instances of the {1} interface.",
                    derivedType,
                    this._genericInterfaceType ) );
        }

        if ( interfaces.Count == 0 )
        {
            return null;
        }

        return interfaces[0];
    }
}