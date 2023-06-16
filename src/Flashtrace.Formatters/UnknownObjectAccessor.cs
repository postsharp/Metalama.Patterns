// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using System.Collections;
using System.Collections.Concurrent;
using System.Linq.Expressions;
using System.Reflection;

namespace Flashtrace.Formatters;

/// <summary>
/// A convenience wrapper for instances of anonymous types (or any unknown type) that exposes properties.
/// </summary>
public readonly struct UnknownObjectAccessor : IEquatable<UnknownObjectAccessor>, IEnumerable<KeyValuePair<string, object>>
{
    private readonly AccessorType? _type;
    private readonly object? _instance;

    private UnknownObjectAccessor( object? instance )
    {
        this._instance = instance;
        if ( instance == null )
        {
            this._type = null;
        }
        else
        {
            this._type = AccessorType.GetInstance( instance.GetType() );
        }
        
    }

    private UnknownObjectAccessor( AccessorType? type, object? instance )
    {
        this._type = type;
        this._instance = instance;
    }

    /// <summary>
    /// Gets a delegate to a factory method that returns a <see cref="UnknownObjectAccessor"/> for an object of a type given as a generic parameter.
    /// </summary>
    /// <typeparam name="T">Type of the objects to be wrapped. It must be the final type (cannot be <c>object</c>, for instance).</typeparam>
    /// <returns>A delegate that takes the object as input and returns its <see cref="UnknownObjectAccessor"/>.</returns>
    public static Func<T, UnknownObjectAccessor> GetFactory<T>()
    {
        AccessorType type = AccessorType.GetInstance( typeof( T ) );
        return o => new UnknownObjectAccessor( type, o );
    }

    /// <summary>
    /// Gets a delegate to a factory method that returns a <see cref="UnknownObjectAccessor"/> for an object of a type given as a generic parameter.
    /// </summary>
    /// <param name="type">Type of the objects to be wrapped. It must be the final type (cannot be <c>object</c>, for instance).</param>
    /// <returns>A delegate that takes the object as input and returns its <see cref="UnknownObjectAccessor"/>.</returns>
    public static Func<object, UnknownObjectAccessor> GetFactory( Type type )
    {
        AccessorType accessorType = AccessorType.GetInstance( type );
        return o => new UnknownObjectAccessor( accessorType, o );

    }

    /// <summary>
    /// Gets an <see cref="UnknownObjectAccessor"/> for a specific object.
    /// </summary>
    /// <param name="value">The wrapped object.</param>
    /// <returns>A wrapper for <paramref name="value"/>.</returns>
    public static UnknownObjectAccessor GetInstance( object value ) => new UnknownObjectAccessor( value );

    /// <summary>
    /// Gets the value of a named property for the current <see cref="UnknownObjectAccessor"/>.
    /// </summary>
    /// <typeparam name="T">Type of the property value.</typeparam>
    /// <param name="name">Property name.</param>
    /// <param name="value">Returns the property value if a property named <paramref name="name"/> exists and its value can be cast to <typeparamref name="T"/>, otherwise <c>default</c>.</param>
    /// <returns><c>true</c> if the property value if a property named <paramref name="name"/> exists and its value can be cast to <typeparamref name="T"/>, otherwise <c>false</c>.</returns>
    public bool TryGetProperty<T>( string name, out T value )
    {
        if ( this._type == null || this._instance == null )
        {
            value = default!;
            return false;
        }

        if ( this._type.TryGetProperty( this._instance, name, out var weaklyTypedValue ) )
        {
            if ( weaklyTypedValue is T typedValue )
            {
                value = typedValue;
                return true;
            }
        }

        value = default!;
        return false;
    }

    /// <summary>
    /// Gets the value of a named property for an arbitrary object.
    /// </summary>
    /// <typeparam name="T">Type of the property value.</typeparam>
    /// <param name="instance">The object whose property has to be returned.</param>
    /// <param name="name">Property name.</param>
    /// <param name="value">Returns the property value if a property named <paramref name="name"/> exists and its value can be cast to <typeparamref name="T"/>, otherwise <c>default</c>.</param>
    /// <returns><c>true</c> if the property value if a property named <paramref name="name"/> exists and its value can be cast to <typeparamref name="T"/>, otherwise <c>false</c>.</returns>

    public static bool TryGetProperty<T>( object instance, string name, out T value ) => UnknownObjectAccessor.GetInstance( instance ).TryGetProperty( name, out value );

    /// <summary>
    /// Converts the wrapped object to an array of name-value tuples.
    /// </summary>
    /// <returns></returns>
    public IReadOnlyList<(string name, object value)> ToTuples() => this._type.ToTuples( this._instance );

    /// <inheritdoc/>
    public override bool Equals( object obj ) => obj is UnknownObjectAccessor other && this.Equals( other );

    /// <inheritdoc/>
    public override int GetHashCode()
    {
        return this._instance == null ? 0 : this._instance.GetHashCode();
    }

    /// <inheritdoc/>
    public bool Equals( UnknownObjectAccessor other )
    {
        return this._instance == other._instance;
    }

    /// <summary>
    /// Returns an <see cref="Enumerator"/>, which enumerates properties of the current <see cref="UnknownObjectAccessor"/>
    /// as a set of <c>KeyValuePair&lt;string, object&gt;</c>.
    /// </summary>
    /// <returns></returns>
    public Enumerator GetEnumerator() => new Enumerator( this );

    IEnumerator<KeyValuePair<string, object>> IEnumerable<KeyValuePair<string, object>>.GetEnumerator() => this.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

    /// <summary>
    /// Invokes the <see cref="IUnknownObjectPropertyVisitor{TState}.Visit{TValue}(string, TValue, ref TState)"/> method of a given visitor
    /// for each property of the current <see cref="UnknownObjectAccessor"/>.
    /// </summary>
    /// <typeparam name="TState">The type of the opaque state passed to the  <see cref="IUnknownObjectPropertyVisitor{TState}.Visit{TValue}(string, TValue, ref TState)"/>
    /// method.</typeparam>
    /// <param name="visitor">The visitor.</param>
    /// <param name="state">The opaque state passed to the  <see cref="IUnknownObjectPropertyVisitor{TState}.Visit{TValue}(string, TValue, ref TState)"/> method.</param>
    public void VisitProperties<TState>( IUnknownObjectPropertyVisitor<TState> visitor, ref TState state )
    {
        if ( this._instance == null )
            return;

        this._type.VisitProperties( this._instance, visitor, ref state );
    }


    /// <summary>
    /// Determines if two <see cref="UnknownObjectAccessor"/> are equal.
    /// </summary>
    /// <param name="left"></param>
    /// <param name="right"></param>
    /// <returns></returns>
    public static bool operator ==( UnknownObjectAccessor left, UnknownObjectAccessor right )
    {
        return left.Equals( right );
    }

    /// <summary>
    /// Determines if two <see cref="UnknownObjectAccessor"/> are different.
    /// </summary>
    /// <param name="left"></param>
    /// <param name="right"></param>
    /// <returns></returns>
    public static bool operator !=( UnknownObjectAccessor left, UnknownObjectAccessor right )
    {
        return !(left == right);
    }

    /// <summary>
    /// A value type that implements <c>IEnumerator&lt;KeyValuePair&lt;lt;string, object&gt;gt;</c>
    /// </summary>
    public struct Enumerator : IEnumerator<KeyValuePair<string, object>>
    {
        private readonly object instance;
        private Dictionary<string, Func<object, object>>.Enumerator enumerator;

        internal Enumerator( in UnknownObjectAccessor accessor )
        {
            this.instance = accessor._instance;
            this.enumerator = accessor._type.GetAccessorEnumerator();
        }

        /// <inheritdoc/>
        public KeyValuePair<string, object> Current => new KeyValuePair<string, object>( this.enumerator.Current.Key, this.enumerator.Current.Value( this.instance ) );

        object IEnumerator.Current => this.Current;

        /// <inheritdoc/>
        public void Dispose() => this.enumerator.Dispose();

        /// <inheritdoc/>
        public bool MoveNext() => this.enumerator.MoveNext();

        /// <inheritdoc/>
        public void Reset() => throw new NotSupportedException();

    }

    internal static IEnumerable<PropertyInfo> GetProperties( Type type )
    {
        foreach ( PropertyInfo property in type.GetRuntimeProperties() )
        {
            MethodInfo getter = property.GetMethod;
            if ( getter != null && !getter.IsStatic && getter.IsPublic && getter.GetParameters().Length == 0 )
            {
                yield return property;
            }
        }
    }


    private class AccessorType
    {
        private static readonly ConcurrentDictionary<Type, AccessorType> instances = new ConcurrentDictionary<Type, AccessorType>();
        private readonly Dictionary<string, Func<object, object>> accessors;
        private readonly ConcurrentDictionary<Type, object> visitorListCache = new ConcurrentDictionary<Type, object>();
        private readonly Type type;

        private AccessorType( Type type )
        {
            this.accessors = new Dictionary<string, Func<object, object>>();

            foreach ( PropertyInfo property in GetProperties(type ))
            {
                MethodInfo getter = property.GetMethod;
                ParameterExpression param = Expression.Parameter( typeof( object ) );
                UnaryExpression expression = Expression.Convert( Expression.Property( Expression.Convert( param, type ), property ), typeof( object ) );
                Expression<Func<object, object>> lambda = Expression.Lambda<Func<object, object>>( expression, param );
                this.accessors.Add( property.Name, lambda.Compile() );
            }

            this.type = type;
        }

        public IReadOnlyList<(string, object)> ToTuples( object value )
        {
            List<ValueTuple<string, object>> properties = new List<(string, object)>( this.accessors.Count );

            foreach ( KeyValuePair<string, Func<object, object>> accessor in this.accessors )
            {
                properties.Add( (accessor.Key, accessor.Value( value )) );
            }

            return properties;
        }

        internal Dictionary<string, Func<object, object>>.Enumerator GetAccessorEnumerator() => this.accessors.GetEnumerator();

        public static AccessorType GetInstance( Type type ) => instances.GetOrAdd( type, t => new AccessorType( t ) );

        public bool TryGetProperty( object o, string name, out object value )
        {
            if ( this.accessors.TryGetValue( name, out Func<object, object> func ) )
            {
                value = func( o );
                return true;
            }
            else
            {
                value = null;
                return false;
            }
        }

        public void VisitProperties<TState>( object o, IUnknownObjectPropertyVisitor<TState> visitor,  ref TState state )
        {
            Dictionary<string, VisitDelegate<TState>> actions = (Dictionary<string, VisitDelegate<TState>>) this.visitorListCache.GetOrAdd( typeof( TState ), _ => this.GetVisitors<TState>() );
            foreach ( KeyValuePair<string, VisitDelegate<TState>> action in actions )
            {
                if ( visitor.MustVisit( action.Key, ref state ) )
                {
                    try
                    {
                    
                            action.Value( visitor, o, ref state );
                    
                    }
                    catch
                    {

                    }
                }
            }
        }

        private Dictionary<string,VisitDelegate<TState>> GetVisitors<TState>()
        {
            Dictionary<string, VisitDelegate<TState>> list = new Dictionary<string, VisitDelegate<TState>>();

            MethodInfo visitMethod = typeof( IUnknownObjectPropertyVisitor<TState> ).GetMethod( "Visit" );
            foreach ( PropertyInfo property in this.type.GetRuntimeProperties() )
            {
                MethodInfo getter = property.GetMethod;
                if ( getter != null && !getter.IsStatic && getter.IsPublic &&  getter.GetParameters().Length == 0 )
                {

                    ParameterExpression visitorParameter = Expression.Parameter( typeof( IUnknownObjectPropertyVisitor<TState> ) );
                    ParameterExpression objectParameter = Expression.Parameter( typeof( object ) );
                    ParameterExpression stateParameter = Expression.Parameter( typeof( TState ).MakeByRefType() );

                    MemberExpression getValue = Expression.Property( Expression.Convert( objectParameter, this.type ), property );
                    MethodCallExpression callExpression = Expression.Call( visitorParameter, visitMethod.MakeGenericMethod( property.PropertyType ), Expression.Constant( property.Name ), getValue, stateParameter );

                    Expression<VisitDelegate<TState>> lambda = Expression.Lambda<VisitDelegate<TState>>( callExpression, visitorParameter, objectParameter, stateParameter );
                    list.Add( property.Name, lambda.Compile() );
                }
            }

            return list;
        }

        private delegate void VisitDelegate<TState>( IUnknownObjectPropertyVisitor<TState> visitor, object instance, ref TState state );
    }
}