// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using System.Globalization;

namespace Flashtrace.Formatters;

[Obsolete("Maybe not required post-TRole?", true)]
internal class TypeExtensionFactory<T> 
    where T : class
{
    private readonly Type _genericInterfaceType;
    private readonly Type _converterType;
    private readonly Dictionary<Type, T> _instances = new Dictionary<Type, T>();
    private readonly Dictionary<Type, Type> _genericExtensionTypes = new Dictionary<Type, Type>();
    
    private readonly Dictionary<Type, TypeExtensionCacheUpdateCallback<T>> _cacheInvalidationCallbacks =
        new Dictionary<Type, TypeExtensionCacheUpdateCallback<T>>();

    public TypeExtensionFactory( Type genericInterfaceType, Type converterType )
    {
        this._genericInterfaceType = genericInterfaceType;
        this._converterType = converterType;
    }

    public void RegisterTypeExtension(Type targetType, T typeExtension)
    {
        if (targetType == null)
        {
            throw new ArgumentNullException(nameof( targetType ));
        }

        if (typeExtension == null)
        {
            throw new ArgumentNullException(nameof( typeExtension ));
        }

        if (targetType.IsGenericTypeDefinition)
        {
            throw new ArgumentException(
                "When a specific type extension is supplied, target type has to be a concrete type, not a generic type definition.",
                nameof( targetType ));
        }

        var foundGenericBase = this.FindInterfaceGenericInstance(typeExtension.GetType());
        if ( foundGenericBase != null )
        {
            var foundGenericArgument = foundGenericBase.GetGenericArguments().Single();
            // IsAssignableFrom says that T can be assigned to Nullable<T>, but we don't support that case
            if ( !foundGenericArgument.IsAssignableFrom( targetType ) || Nullable.GetUnderlyingType( foundGenericArgument ) == targetType )
            {
                throw new ArgumentException( string.Format( CultureInfo.InvariantCulture, "{0} is not a type extension for the type {1}.", typeExtension.GetType(), targetType ) );
            }
        }

        lock ( this._instances)
        {
            this._instances[targetType] = typeExtension;
            var typeExtensionInfo = new TypeExtensionInfo<T>(typeExtension, targetType, false);

            foreach (var kvp in this._cacheInvalidationCallbacks)
            {
                // IsAssignableFrom says that T can be assigned to Nullable<T>, but we don't support that case
                if (targetType.IsAssignableFrom(kvp.Key) && kvp.Key != Nullable.GetUnderlyingType(targetType))
                {
                    kvp.Value(typeExtensionInfo);
                }
            }
        }
    }

    public void RegisterTypeExtension(Type targetType, Type typeExtensionType)
    {
        if (targetType == null)
        {
            throw new ArgumentNullException(nameof( targetType ));
        }

        if (typeExtensionType == null)
        {
            throw new ArgumentNullException(nameof( typeExtensionType ));
        }

        if (!typeExtensionType.IsGenericTypeDefinition)
        {
            if (targetType.IsGenericTypeDefinition)
            {
                throw new ArgumentException("When type extension type is not generic, target type cannot be generic.");
            }

            this.RegisterTypeExtension(targetType, (T)Activator.CreateInstance(typeExtensionType));
            return;
        }

        var foundBase = this.FindInterfaceGenericInstance(typeExtensionType);
        if (foundBase == null)
        {
            throw new ArgumentException( string.Format(CultureInfo.InvariantCulture, "{0} type has to inherit from {1}.", typeExtensionType, this._genericInterfaceType ), nameof( typeExtensionType ));
        }

        if (!targetType.IsGenericTypeDefinition && targetType != typeof(Array))
        {
            throw new ArgumentException("When a type extension is generic, target type has to be generic.");
        }

        var targetTypeParametersCount = targetType == typeof(Array) ? 1 : targetType.GetGenericArguments().Length;
        var extensionTypeParametersCount = typeExtensionType.GetGenericArguments().Length;
        var hasKindMarkerTypeParameter = this.HasRoleTypeParameter(typeExtensionType);

        if (!((targetTypeParametersCount == extensionTypeParametersCount && !hasKindMarkerTypeParameter) ||
              (targetTypeParametersCount == extensionTypeParametersCount - 1 && hasKindMarkerTypeParameter)))
        {

            throw new ArgumentException("The number of generic parameters of target type and extension type are not compatible.");
        }

        lock ( this._instances)
        {
            this._genericExtensionTypes[targetType] = typeExtensionType;

            foreach (var kvp in this._cacheInvalidationCallbacks)
            {
                this.TryInitialize(targetType, typeExtensionType, kvp.Key, kvp.Value);
            }
        }
    }

    private void TryInitialize(
        Type typeExtensionTargetType, Type typeExtensionType, Type registrationType, TypeExtensionCacheUpdateCallback<T> cacheUpdateCallback)
    {
        foreach (var assignableType in this.GetAssignableTypes(registrationType))
        {
            var genericAssignableTypeDefinition = GetGenericTypeDefinition(assignableType);

            if (genericAssignableTypeDefinition == typeExtensionTargetType)
            {
                var genericArguments = GetGenericArguments(assignableType);

                var genericTypeExtensionType = this.MakeGenericExtensionType(typeExtensionType, genericArguments);

                var extension = (T)Activator.CreateInstance(genericTypeExtensionType);

                var typeExtensionInfo = new TypeExtensionInfo<T>(
                    extension, MakeGenericType(typeExtensionTargetType, genericArguments), true
                    );

                cacheUpdateCallback(typeExtensionInfo);
            }
        }
    }

    private T? GetExtensionCore(Type objectType, out bool isExtensionGeneric, out Type targetType, Action<Exception>? onExceptionWhileCreatingTypeExtension )
    {
        T? bestExtension = null;
        targetType = typeof(object);
        Func<T>? bestExtensionFactory = null;
        isExtensionGeneric = false;

        foreach (var assignableType in this.GetAssignableTypes(objectType))
        {
            T typeExtension;
            if ( this._instances.TryGetValue(assignableType, out typeExtension))
            {
                if (ShouldOverwrite(assignableType, false, targetType, isExtensionGeneric))
                {
                    bestExtension = typeExtension;
                    targetType = assignableType;
                    bestExtensionFactory = null;
                    isExtensionGeneric = false;
                }
            }

            var assignableGenericTypeDefinition = GetGenericTypeDefinition(assignableType);
            if (assignableGenericTypeDefinition != null)
            {
                Type extensionType;
                if ( this._genericExtensionTypes.TryGetValue(assignableGenericTypeDefinition, out extensionType))
                {
                    var genericArguments = GetGenericArguments(assignableType);
                    var genericExtensionType = this.MakeGenericExtensionType(extensionType, genericArguments);

                    if (ShouldOverwrite(assignableType, true, targetType, isExtensionGeneric))
                    {
                        bestExtension = null;
                        targetType = assignableType;
                        bestExtensionFactory =
                            () => (T)Activator.CreateInstance(genericExtensionType);
                        isExtensionGeneric = true;
                    }
                }
            }
        }

        if (bestExtension == null && bestExtensionFactory != null )
        {
            try
            {
                return bestExtensionFactory();
            }
            catch (Exception e)
            {
                onExceptionWhileCreatingTypeExtension?.Invoke( e );

                // TODO: Review, confirm replacement with onExceptionWhileCreatingTypeExtension.
                // this.logger.Error.Write( FormattedMessageBuilder.Formatted(  "Exception while creating a type extension for {ObjectType}.", objectType  ), e);

                // Extension constructor threw, so remove it from the dictionary and try again
                var targetGenericTypeDefinition = GetGenericTypeDefinition(targetType);
                this._genericExtensionTypes.Remove(targetGenericTypeDefinition);
                return this.GetExtensionCore( objectType, out isExtensionGeneric, out targetType, onExceptionWhileCreatingTypeExtension );
            }
        }

        return bestExtension;
    }

    public void Clear()
    {
        lock ( this._instances)
        {
            this._instances.Clear();
            this._genericExtensionTypes.Clear();
        }
    }    

    public TypeExtensionInfo<T> GetTypeExtension(Type objectType, TypeExtensionCacheUpdateCallback<T> cacheUpdateCallback, Func<T> createDefault, Action<Exception>? onExceptionWhileCreatingTypeExtension = null )
    {
        lock ( this._instances)
        {
            bool isGeneric;
            Type targetType;

            var bestTypeExtension = this.GetExtensionCore(objectType, out isGeneric, out targetType, onExceptionWhileCreatingTypeExtension) ?? createDefault();

            if (cacheUpdateCallback != null)
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

            bestTypeExtension = this.Convert(bestTypeExtension, objectType);

            return new TypeExtensionInfo<T>(bestTypeExtension, targetType, isGeneric);
        }
    }    

    public T? Convert(T o, Type targetObjectType)
    {
        if ( o == null )
        {
            return null;
        }

        var targetType = this._genericInterfaceType.MakeGenericType( targetObjectType );

        if(targetType.IsAssignableFrom(o.GetType()))
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

        return (T) Activator.CreateInstance(wrapperType, o);
    }

    /// <remarks>This logic is supposed to mimic overload resolution in C#.</remarks>
    public static bool ShouldOverwrite(Type newExtensionTargetType, bool isNewGeneric, Type oldExtensionTargetType, bool isOldGeneric)
    {
        // new is more specific or equal
        if (oldExtensionTargetType.IsAssignableFrom(newExtensionTargetType))
        {
            // new is more specific
            if (newExtensionTargetType != oldExtensionTargetType)
            {
                return true;
            }

            // types are equal, less generic wins
            if (isNewGeneric && !isOldGeneric)
            {
                return false;
            }

            return true;
        }

        // old is more specific
        if (newExtensionTargetType.IsAssignableFrom(oldExtensionTargetType))
        {
            return false;
        }

        // the two are not comparable
        return true;
    }

    // TODO: Remove this method?
    /// <summary>
    /// Checks whether the first type parameter is constrained to be <see cref="FormattingRole"/>.
    /// </summary>
    [Obsolete("Type-based roles are deprecated.", true)]
    private bool HasRoleTypeParameter(Type typeExtensionType)
    {
#if true
        throw new NotSupportedException();
#else
        if (this._roleType == null)
        {
            return false;
        }

        // TODO: Abstract from FormattingRole.
        foreach (var constraint in typeExtensionType.GetGenericArguments()[0].GetGenericParameterConstraints())
        {
            var type = constraint;

            do
            {
                // This is quite ugly, but...
                if (type.Name.EndsWith("Role", StringComparison.Ordinal))
                {
                    return true;
                }

                type = type.BaseType;
            } while (type != null);
        }

        return false;
#endif
    }

    // TODO: Clean up.
    private Type MakeGenericExtensionType(Type extensionType, Type[] genericArguments)
    {
#if false
        if ( this.HasRoleTypeParameter(extensionType))
        {
            // newGenericArguments is genericArguments with typeof(TKind) prepended
            var newGenericArguments = new Type[genericArguments.Length + 1];
            newGenericArguments[0] = this._roleType;
            Array.Copy(genericArguments, 0, newGenericArguments, 1, genericArguments.Length);

            genericArguments = newGenericArguments;
        }
#endif

        return extensionType.MakeGenericType(genericArguments);
    }


    private static Type? GetGenericTypeDefinition( Type type )
    {
        if ( type.IsGenericType )
        {
            return type.GetGenericTypeDefinition();
        }

        if ( type.IsArray )
        {
            return typeof( Array );
        }

        return null;
    }

    private static Type MakeGenericType( Type type, Type[] typeArguments )
    {
        if ( type.IsGenericTypeDefinition )
        {
            return type.MakeGenericType( typeArguments );
        }

        if ( type == typeof( Array ) )
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
            return new[] { type.GetElementType() };
        }

        return Array.Empty<Type>();
    }

    protected virtual IEnumerable<Type> GetAssignableTypes( Type type )
    {
        return new[] {type};
    }

    private Type? FindInterfaceGenericInstance(Type derivedType)
    {
        IList<Type> interfaces = derivedType.GetInterfaces().Where( type => type.IsGenericType && type.GetGenericTypeDefinition() == this._genericInterfaceType ).ToList();

        // TODO: Have a specific exception type.
        if ( interfaces.Count > 1 )
        {
            throw new ArgumentException( string.Format(CultureInfo.InvariantCulture, "The type {0} cannot implement several generic instances of the {1} interface.", derivedType, this._genericInterfaceType));
        }

        if ( interfaces.Count == 0 )
        {
            return null;
        }

        return interfaces[0];
    }
}