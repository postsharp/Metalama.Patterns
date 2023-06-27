// Copyright (c) SharpCrafters s.r.o. This file is not open source. It is released under a commercial
// source-available license. Please see the LICENSE.md file in the repository root for details.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using PostSharp.Aspects;
using PostSharp.Aspects.Dependencies;
using PostSharp.Extensibility;
using PostSharp.Patterns.Caching.Implementation;
using PostSharp.Patterns.Diagnostics;
using PostSharp.Serialization;

namespace PostSharp.Patterns.Caching
{
    /// <summary>
    /// Custom attribute that, when applied on a method, causes an invocation of this method to remove from
    /// the cache the result of invocations of other given methods with the same parameter values. 
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1019:DefineAccessorsForAttributeArguments")]
    [MulticastAttributeUsage( MulticastTargets.Method, AllowMultiple = true )]
    [Metric("UsedFeatures", "Patterns.Caching.InvalidateCache")]
    [ProvideAspectRole( "CacheInvalidation" )]
    [AspectRoleDependency(AspectDependencyAction.Order, AspectDependencyPosition.After, StandardRoles.Validation)]
    [AspectRoleDependency(AspectDependencyAction.Order, AspectDependencyPosition.After, StandardRoles.Tracing)]
    [LinesOfCodeAvoided(1)]
    [PSerializable]
    public sealed class InvalidateCacheAttribute : MethodInterceptionAspect
    {
       
        [PNonSerialized]
        private Type invalidatedMethodsDeclaringType;

        [PNonSerialized]
        private readonly string[] invalidatedMethodNames;

        [PNonSerialized]
        private MethodInfo targetMethod;

        [PNonSerialized]
        private LogSource logger;

        // ReSharper disable once FieldCanBeMadeReadOnly.Local
        private List<InvalidatedMethodInfo> invalidatedMethods = new List<InvalidatedMethodInfo>();

        /// <summary>
        /// Determines whether the current <see cref="InvalidateCacheAttribute"/> can match several overloads of the methods.
        /// The default value is <c>false</c>, which means that an error will be emitted if the current <see cref="InvalidateCacheAttribute"/> matches
        /// several methods of the same name.
        /// </summary>
        public bool AllowMultipleOverloads { get; set; }

        /// <summary>
        /// Initializes a new <see cref="InvalidateCacheAttribute"/> that invalidates method of the same type as the
        /// type to which the current <see cref="InvalidateCacheAttribute"/> aspect is being applied.
        /// </summary>
        /// <param name="methodNames">A list of names of methods to invalidate. All parameters of these methods (except those marked 
        /// with <see cref="NotCacheKeyAttribute"/>) must have a parameter of the same name and compatible type in the target
        /// method of the current <see cref="InvalidateCacheAttribute"/> aspect.</param>
        public InvalidateCacheAttribute( params string[] methodNames ) : this( null, methodNames )
        {
        }

        /// <summary>
        /// Initializes a new <see cref="InvalidateCacheAttribute"/> that invalidates method of a different type than the
        /// type to which the current <see cref="InvalidateCacheAttribute"/> aspect is being applied.
        /// </summary>
        /// <param name="declaringType">The type containing the methods to invalidate.</param>
        /// <param name="methodNames">A list of names of methods to invalidate. All parameters of these methods (except those marked 
        /// with <see cref="NotCacheKeyAttribute"/>) must have a parameter of the same name and compatible type in the target
        /// method of the current <see cref="InvalidateCacheAttribute"/> aspect.</param>

        public InvalidateCacheAttribute( Type declaringType, params string[] methodNames )
        {
            this.invalidatedMethodsDeclaringType = declaringType;
            this.invalidatedMethodNames = methodNames;
        }

        /// <exclude/>
        public override bool CompileTimeValidate( MethodBase method )
        {
            if ( this.invalidatedMethodNames == null || this.invalidatedMethodNames.Length == 0 )
            {
                CachingMessageSource.Instance.Write( method, SeverityType.Error, "CAC007", method );
            }

            IAspectRepositoryService aspectRepositoryService = PostSharpEnvironment.CurrentProject.GetService<IAspectRepositoryService>();
            IWeavingSymbolsService weavingSymbolService = PostSharpEnvironment.CurrentProject.GetService<IWeavingSymbolsService>();
            IFormattingService formattingService = PostSharpEnvironment.CurrentProject.GetService<IFormattingService>();

            // We need to initialize ourselves after all aspects have been discovered and after CacheAspect has been initialized.
            aspectRepositoryService.AspectDiscoveryCompleted +=
                ( sender, args ) => this.PostInitialization( (MethodInfo) method,
                                                             aspectRepositoryService, weavingSymbolService, formattingService );

            return true;
        }

        private void PostInitialization( MethodInfo invalidatingMethod,
                                         IAspectRepositoryService aspectRepositoryService,
                                         IWeavingSymbolsService weavingSymbolService,
                                         IFormattingService formattingService )
        {
            if ( this.invalidatedMethodsDeclaringType == null )
            {
                this.invalidatedMethodsDeclaringType = invalidatingMethod.DeclaringType;
            }

            foreach ( string methodName in this.invalidatedMethodNames )
            {
                if ( string.IsNullOrEmpty( methodName ) )
                {
                    CachingMessageSource.Instance.Write( invalidatingMethod, SeverityType.Error, "CAC006", invalidatingMethod );
                    return;
                }
            }

            ParameterInfo[] invalidatingMethodParameters = invalidatingMethod.GetParameters();

            MethodInfo[] methods = this.invalidatedMethodsDeclaringType.GetMethods(
                BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public );

            DictionaryOfLists<string, MethodMatchingError> matchingErrorsDictionary = new DictionaryOfLists<string, MethodMatchingError>();

            DictionaryOfLists<string, InvalidatedMethodInfo> invalidatedMethodsDictionary =
                new DictionaryOfLists<string, InvalidatedMethodInfo>();

            foreach ( MethodInfo invalidatedMethod in methods )
            {
                if ( Array.IndexOf( this.invalidatedMethodNames, invalidatedMethod.Name ) < 0 )
                {
                    continue;
                }

                // Ensure the method is actually cached.
                ICacheAspect cacheAspect = (ICacheAspect)
                    aspectRepositoryService.GetAspectInstances( invalidatedMethod )
                                           .SingleOrDefault( aspectInstance => aspectInstance.Aspect is ICacheAspect )?.Aspect;
                if ( cacheAspect == null )
                {
                    matchingErrorsDictionary.Add(
                        invalidatedMethod.Name,
                        new MethodMatchingError( invalidatingMethod,
                                                 "CAC002",
                                                 invalidatingMethod,
                                                 invalidatedMethod ) );

                    continue;
                }

                // Check that the 'this' parameter is compatible.
                if ( !invalidatedMethod.IsStatic && !cacheAspect.BuildTimeConfiguration.IgnoreThisParameter.GetValueOrDefault() &&
                     (invalidatingMethod.IsStatic || !invalidatedMethod.DeclaringType.IsAssignableFrom( invalidatingMethod.DeclaringType )) )
                {
                    matchingErrorsDictionary.Add(
                        invalidatedMethod.Name,
                        new MethodMatchingError( invalidatingMethod, "CAC003",
                                                 invalidatingMethod,
                                                 invalidatedMethod,
                                                 invalidatedMethod.DeclaringType,
                                                 invalidatingMethod.DeclaringType ) );

                    continue;
                }

                // Match parameters.
                ParameterInfo[] invalidatedMethodParameters = invalidatedMethod.GetParameters();

                InvalidatedMethodInfo invalidatedMethodInfo = new InvalidatedMethodInfo( invalidatedMethod );

                bool allParametersMatching = true;

                for ( int i = 0; i < invalidatedMethodParameters.Length; i++ )
                {
                    ParameterInfo invalidatedMethodsParameter = invalidatedMethodParameters[i];
                    if ( invalidatedMethodsParameter.IsDefined( typeof(NotCacheKeyAttribute) ) )
                    {
                        continue;
                    }

                    // Match parameter by name.
                    ParameterInfo invalidatingMethodParameter =
                        invalidatingMethodParameters.SingleOrDefault( p => p.Name == invalidatedMethodsParameter.Name );

                    if ( invalidatingMethodParameter == null )
                    {
                        matchingErrorsDictionary.Add(
                            invalidatedMethod.Name,
                            new MethodMatchingError( invalidatingMethod,
                                                     "CAC004",
                                                     invalidatingMethod,
                                                     invalidatedMethod,
                                                     invalidatedMethodsParameter.Name ) );
                        allParametersMatching = false;
                        continue;
                    }

                    // Check that the type is compatible.
                    if ( !invalidatedMethodsParameter.ParameterType.IsAssignableFrom( invalidatingMethodParameter.ParameterType ) )
                    {
                        matchingErrorsDictionary.Add(
                            invalidatedMethod.Name,
                            new MethodMatchingError( invalidatingMethod,
                                                     "CAC005",
                                                     invalidatingMethod,
                                                     invalidatedMethod,
                                                     invalidatedMethodsParameter.Name ) );
                        allParametersMatching = false;
                        continue;
                    }

                    invalidatedMethodInfo.ParameterMap[i] = invalidatingMethodParameter.Position;
                }

                if ( !allParametersMatching )
                {
                    continue;
                }

                invalidatedMethodsDictionary.Add( invalidatedMethod.Name, invalidatedMethodInfo );
            }

            foreach ( string invalidatedMethodName in this.invalidatedMethodNames )
            {
                List<InvalidatedMethodInfo> invalidatedOverloads;

                if ( !invalidatedMethodsDictionary.TryGetList( invalidatedMethodName, out invalidatedOverloads ) || invalidatedOverloads.Count == 0 )
                {
                    List<MethodMatchingError> errors;

                    if ( matchingErrorsDictionary.TryGetList( invalidatedMethodName, out errors ) )
                    {
                        // There were errors, but the method of the given name exists
                        foreach ( MethodMatchingError error in errors )
                        {
                            error.Write();
                        }
                    }
                    else
                    {
                        // The method of the given name does not exist
                        CachingMessageSource.Instance.Write( invalidatingMethod, SeverityType.Error, "CAC008",
                                                             invalidatingMethod,
                                                             invalidatedMethodName,
                                                             this.invalidatedMethodsDeclaringType );
                    }

                    continue;
                }

                if ( !this.AllowMultipleOverloads && invalidatedOverloads.Count > 1 )
                {
                    CachingMessageSource.Instance.Write( invalidatingMethod, SeverityType.Error, "CAC009",
                                                         invalidatingMethod,
                                                         invalidatedMethodName,
                                                         this.invalidatedMethodsDeclaringType );

                    continue;
                }

                foreach ( InvalidatedMethodInfo invalidatedMethodInfo in invalidatedOverloads )
                {
                    weavingSymbolService.PushAnnotation( invalidatedMethodInfo.Method,
                                                         description: formattingService.Format( CultureInfo.InvariantCulture, "The method is invalidated by {0}.", invalidatingMethod ) );

                    weavingSymbolService.PushAnnotation( invalidatingMethod,
                                                         description: formattingService.Format(CultureInfo.InvariantCulture, "The method invalidates {0}.", invalidatedMethodInfo.Method ) );
                }

                this.invalidatedMethods.AddRange( invalidatedOverloads );
            }
        }

        /// <exclude />
        public override void RuntimeInitialize( MethodBase method )
        {
            this.targetMethod = (MethodInfo) method;
        }

        private LogSource GetLogger()
        {
            return this.logger ?? (this.logger = LogSourceFactory.ForRole3( LoggingRoles.Caching ).GetLogSource( this.targetMethod.DeclaringType ));
        }

        /// <exclude/>
        public override void OnInvoke( MethodInterceptionArgs args )
        {
            using ( var activity = this.GetLogger().Default.OpenActivity( FormattedMessageBuilder.Formatted( "Processing invalidation by method {Method}", this.targetMethod ) ) )
            {
                try
                {
                    base.OnInvoke( args );

                    foreach ( InvalidatedMethodInfo invalidatedMethod in this.invalidatedMethods )
                    {
                        object[] mappedArguments = MapArguments( invalidatedMethod, args.Arguments );
                        object instance = MapInstance( invalidatedMethod, args.Instance );

                        CachingServices.Invalidation.Invalidate( invalidatedMethod.Method, instance, mappedArguments );
                    }

                    activity.SetSuccess();
                }
                catch ( Exception e )
                {
                    activity.SetException( e );
                    throw;
                }
            }
        }

        /// <exclude/>
        public override async Task OnInvokeAsync( MethodInterceptionArgs args )
        {
            using ( var activity = this.GetLogger().Default.OpenActivity(  FormattedMessageBuilder.Formatted( "Processing invalidation by async method {Method}", this.targetMethod ) ) )
            {
                try
                {

                    Task t = base.OnInvokeAsync( args );

                    if ( !t.IsCompleted )
                    {
                        activity.Suspend();
                        try
                        {
                            await t;
                        }
                        finally
                        {
                            activity.Resume();
                        }
                    }

                    List<Task> tasks = new List<Task>( this.invalidatedMethods.Count );
                    foreach ( InvalidatedMethodInfo invalidatedMethod in this.invalidatedMethods )
                    {
                        object[] mappedArguments = MapArguments( invalidatedMethod, args.Arguments );
                        object instance = MapInstance( invalidatedMethod, args.Instance );

                        tasks.Add( CachingServices.Invalidation.InvalidateAsync( invalidatedMethod.Method, instance, mappedArguments ) );
                    }

                    t = Task.WhenAll( tasks );

                    if ( !t.IsCompleted )
                    {
                        activity.Suspend();
                        try
                        {
                            await t;
                        }
                        finally
                        {
                            activity.Resume();
                        }
                    }

                    activity.SetSuccess();
                }
                catch ( Exception e )
                {
                    activity.SetException( e );
                    throw;
                }
            }
        }

        private static object MapInstance( InvalidatedMethodInfo invalidatedMethod, object instance )
        {
            return invalidatedMethod.Method.IsStatic ? null : instance;
        }

        private static object[] MapArguments( InvalidatedMethodInfo invalidatedMethod, Arguments arguments )
        {
            object[] mappedArguments = new object[invalidatedMethod.ParameterMap.Length];

            for ( int i = 0; i < mappedArguments.Length; i++ )
            {
                int mappedArgumentPosition = invalidatedMethod.ParameterMap[i];
                if ( mappedArgumentPosition >= 0 )
                {
                    mappedArguments[i] = arguments[mappedArgumentPosition];
                }
            }

            return mappedArguments;
        }

        private class MethodMatchingError
        {
            public MethodInfo Method { get; }

            public string Code { get; }

            public object[] Arguments { get; }

            public MethodMatchingError(MethodInfo method, string code, params object[] arguments)
            {
                this.Method = method;
                this.Code = code;
                this.Arguments = arguments;
            }

            public void Write()
            {
                CachingMessageSource.Instance.Write( this.Method, SeverityType.Error, this.Code, this.Arguments );
            }
        }

        private class DictionaryOfLists<TKey, TValue> : IEnumerable<KeyValuePair<TKey, List<TValue>>>
        {
            private readonly Dictionary<TKey, List<TValue>> collectionsDictionary = new Dictionary<TKey, List<TValue>>();

            public void Add(TKey methodName, TValue value)
            {
                List<TValue> list;

                if (!this.collectionsDictionary.TryGetValue(methodName, out list))
                {
                    list = new List<TValue>();
                    this.collectionsDictionary.Add(methodName, list);
                }

                list.Add(value);
            }

            public bool TryGetList(TKey key, out List<TValue> list)
            {
                return this.collectionsDictionary.TryGetValue(key, out list);
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return this.GetEnumerator();
            }

            public IEnumerator<KeyValuePair<TKey, List<TValue>>> GetEnumerator()
            {
                return this.collectionsDictionary.GetEnumerator();
            }
        }


        [PSerializable]
        private class InvalidatedMethodInfo
        {
            public InvalidatedMethodInfo( MethodInfo method )
            {
                this.Method = method;
                this.ParameterMap = new int[method.GetParameters().Length];
                for ( int i = 0; i < this.ParameterMap.Length; i++ )
                {
                    this.ParameterMap[i] = -1;
                }
            }

            // ReSharper disable FieldCanBeMadeReadOnly.Local
            public MethodInfo Method;            
            public int[] ParameterMap;
            // ReSharper restore FieldCanBeMadeReadOnly.Local
        }
    }
}
