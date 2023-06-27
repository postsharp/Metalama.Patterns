// Copyright (c) SharpCrafters s.r.o. This file is not open source. It is released under a commercial
// source-available license. Please see the LICENSE.md file in the repository root for details.

using System;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using PostSharp.Aspects;
using PostSharp.Aspects.Dependencies;
using PostSharp.Extensibility;
using PostSharp.Patterns.Caching.Implementation;
using PostSharp.Patterns.Diagnostics;
using PostSharp.Serialization;
using PostSharp.Patterns.Diagnostics.Contexts;
using static PostSharp.Patterns.Diagnostics.FormattedMessageBuilder;

namespace PostSharp.Patterns.Caching
{
    /// <summary>
    /// Custom attribute that, when applied on a method, causes the return value of the method to be cached
    /// for the specific list of arguments passed to this method call.
    /// </summary>
    /// <remarks>
    /// <para>There are several ways to configure the behavior of the <see cref="CacheAttribute"/> aspect: you can set the properties of the
    /// <see cref="CacheAttribute"/> class, such as <see cref="AbsoluteExpiration"/> or <see cref="SlidingExpiration"/>. You can
    /// add the <see cref="CacheConfigurationAttribute"/> custom attribute to the declaring type, a base type, or the declaring assembly.
    /// Finally, you can define a profile by setting the <see cref="ProfileName"/> property and configure the profile at run time
    /// by accessing the <see cref="CachingServices.Profiles"/> collection of the <see cref="CachingServices"/> class.</para>
    /// <para>Use the <see cref="NotCacheKeyAttribute"/> custom attribute to exclude a parameter from being a part of the cache key.</para>
    /// <para>To invalidate a cached method, see <see cref="InvalidateCacheAttribute"/> and <see cref="CachingServices.Invalidation"/>.</para>
    /// </remarks>
    [Metric("UsedFeatures", "Patterns.Caching.Cache")]
    [ProvideAspectRole(StandardRoles.Caching)]
    [AspectRoleDependency(AspectDependencyAction.Order, AspectDependencyPosition.After, StandardRoles.Validation)]
    [AspectRoleDependency(AspectDependencyAction.Order, AspectDependencyPosition.After, StandardRoles.Tracing)]
    [LinesOfCodeAvoided(3)]
    [PSerializable]
    [MulticastAttributeUsage(PersistMetaData = true)]
    public sealed class CacheAttribute : MethodInterceptionAspect, ICacheAspect
    {
        private CacheItemConfiguration configuration = new CacheItemConfiguration();

        [PNonSerialized]
        private SpinLock initializeLock;

        private static readonly CachingProfile disabledProfile = new CachingProfile("Disabled") { IsEnabled = false };

        [PNonSerialized]
        private CachingProfile profile;

        [PNonSerialized]
        private int profileRevision;

        [PNonSerialized]
        private LogSource logger;

        [PNonSerialized]
        private CacheItemConfiguration mergedConfiguration;

        [PNonSerialized]
        private MethodInfo targetMethod;

        /// <summary>
        /// Gets or sets the name of the <see cref="CachingProfile"/> that contains the configuration of the current <see cref="CacheAttribute"/>.
        /// </summary>
        public string ProfileName
        {
            get { return this.configuration.ProfileName; }
            set { this.configuration.ProfileName = value; }
        }

        /// <summary>
        /// Determines whether the method calls are automatically reloaded (by re-evaluating the target method with the same arguments)
        /// when the cache item is removed from the cache.
        /// </summary>
        public bool AutoReload
        {
            get { return this.configuration.AutoReload.GetValueOrDefault(); }
            set { this.configuration.AutoReload = value; }
        }

        /// <summary>
        /// Gets or sets the total duration, in minutes, during which the result of the current method is stored in cache. The absolute
        /// expiration time is counted from the moment the method is evaluated and cached.
        /// </summary>
        public double AbsoluteExpiration
        {
            get { return this.configuration.AbsoluteExpiration.GetValueOrDefault(TimeSpan.Zero).TotalMinutes; }
            set { this.configuration.AbsoluteExpiration = TimeSpan.FromMinutes( value ); }
        }

        /// <summary>
        /// Gets or sets the duration, in minutes, during which the result of the current method is stored in cache after it has been
        /// added to or accessed from the cache. The expiration is extended every time the value is accessed from the cache.
        /// </summary>
        public double SlidingExpiration
        {
            get { return this.configuration.SlidingExpiration.GetValueOrDefault(TimeSpan.Zero).TotalMinutes; }
            set { this.configuration.SlidingExpiration = TimeSpan.FromMinutes(value); }
        }

        /// <summary>
        /// Gets or sets the priority of the current method.
        /// </summary>
        public CacheItemPriority Priority
        {
            get { return this.configuration.Priority.GetValueOrDefault(CacheItemPriority.Default); }
            set { this.configuration.Priority = value; }
        }

        /// <summary>
        /// Determines whether the <c>this</c> instance should be a part of the cache key. The default value of this property is <c>false</c>,
        /// which means that by default the <c>this</c> instance is a part of the cache key.
        /// </summary>
        public bool IgnoreThisParameter
        {
            get { return this.configuration.IgnoreThisParameter.GetValueOrDefault(); }
            set { this.configuration.IgnoreThisParameter = value; }
        }

        /// <exclude />
        public override void CompileTimeInitialize( MethodBase method, AspectInfo aspectInfo )
        {
            this.configuration.ApplyFallback( BuildTimeCacheConfigurationManager.GetConfigurationFromAttributes( method ) );
        }


        /// <exclude />
        public override bool CompileTimeValidate( MethodBase method )
        {
            MethodInfo methodInfo = method as MethodInfo;
            if ( methodInfo == null || methodInfo.ReturnType == typeof(void) || methodInfo.ReturnType == typeof(Task) )
            {
                CachingMessageSource.Instance.Write( method, SeverityType.Error, "CAC001", method );
                return false;
            }
            if ( method.GetParameters().Any(parameter => parameter.ParameterType.IsByRef && !parameter.IsIn) )
            {
                CachingMessageSource.Instance.Write( method, SeverityType.Error, "CAC010", method );
                return false;
            }
            
            return true;
        }

        /// <exclude />
        public override void RuntimeInitialize( MethodBase method )
        {
            this.targetMethod = (MethodInfo) method;

            CacheAspectRepository.Add( (MethodInfo) method, this );
        }

        private CacheItemConfiguration MergedConfiguration
        {
            get
            {
                if ( this.profile == null || this.profileRevision < CachingServices.Profiles.RevisionNumber )
                {
                    bool initializeLockTaken = false;
                    try
                    {
                        this.initializeLock.Enter(ref initializeLockTaken);

                        if ( this.profile == null || this.profileRevision < CachingServices.Profiles.RevisionNumber )
                        {

                            string profileName = this.configuration.ProfileName ?? CachingProfile.DefaultName;

                            CachingProfile localProfile = CachingServices.Profiles[profileName];
                            
                            if (localProfile == null )
                            {
                                this.profile = disabledProfile;
                                this.GetLogger().Warning.Write( Formatted(
                                                        "The cache is incorrectly configured for method {Method}: there is no profile named {Profile}.",
                                                        this.targetMethod, profileName) );
                            }

                            this.mergedConfiguration = this.configuration.Clone();
                            this.mergedConfiguration.ApplyFallback(localProfile);

                            Thread.MemoryBarrier();

                            // Need to set this after setting mergedConfiguration to prevent data races.
                            this.profile = localProfile;
                            this.profileRevision = CachingServices.Profiles.RevisionNumber;
                        }
                    }
                    finally
                    {
                        if ( initializeLockTaken )
                            this.initializeLock.Exit();
                    }
                }

                return this.mergedConfiguration;
            }
        }

        private LogSource GetLogger()
        {
            if (this.logger == null )
            {
                this.logger = LogSourceFactory.ForRole3( LoggingRoles.Caching ).GetLogSource(this.targetMethod.DeclaringType);
            }

            return this.logger;
        }


        /// <exclude />
        public override void OnInvoke( MethodInterceptionArgs args )
        {
            LogSource logSource = this.GetLogger();
            using ( var activity = logSource.Default.OpenActivity( Formatted("Processing invocation of method {Method}", this.targetMethod ) ) )
            {
                try
                {
                    if ( !this.MergedConfiguration.IsEnabled.GetValueOrDefault() )
                    {
                        logSource.Debug.EnabledOrNull?.Write( Formatted("Ignoring the caching aspect because caching is disabled for this profile.") );

                        base.OnInvoke( args );
                    }
                    else
                    {
                        MethodInfo method = (MethodInfo) args.Method;

                        string methodKey = CachingServices.DefaultKeyBuilder.BuildMethodKey( method, args.Arguments, args.Instance );

                        logSource.Debug.EnabledOrNull?.Write( Formatted( "Key=\"{Key}\".", methodKey ) );

                        args.ReturnValue = CachingFrontend.GetOrAdd( method,
                                                                     methodKey, method.ReturnType, this.MergedConfiguration,
                                                                     new MethodEvaluationData( args ).GetValue,
                                                                     logSource );

                       
                    }

                    activity.SetSuccess();

                }
                catch ( Exception e ) 
                {
                    activity.SetException(e);
                    throw;
                }
            }
        }

        CacheItemConfiguration ICacheAspect.BuildTimeConfiguration => this.configuration;

        CacheItemConfiguration ICacheAspect.RunTimeConfiguration
        {
            get
            {
                throw new NotSupportedException();
            }
        }



        /// <exclude />
        public override async Task OnInvokeAsync( MethodInterceptionArgs args )
        {
            CallerInfo callerInfo = CallerInfo.Async;

            LogSource logSource = this.GetLogger();
            using ( var activity = logSource.Default.OpenActivity( Formatted("Processing invocation of async method {Method}", this.targetMethod ), default, ref callerInfo ) )
            {
                try
                {
                    if ( !this.MergedConfiguration.IsEnabled.GetValueOrDefault() )
                    {
                        logSource.Debug.EnabledOrNull?.Write( Formatted("Ignoring the caching aspect because caching is disabled for this profile.") );

                        Task task = base.OnInvokeAsync( args );

                        if ( !task.IsCompleted )
                        {
                            // We need to call LogActivity.Suspend and LogActivity.Resume manually because we're in an aspect,
                            // and the await instrumentation policy is not applied.
                            activity.Suspend();
                            try
                            {
                                await task;
                            }
                            finally
                            {
                                activity.Resume();
                            }
                        }
                    }
                    else
                    {
                        MethodInfo method = (MethodInfo) args.Method;

                        string methodKey = CachingServices.DefaultKeyBuilder.BuildMethodKey( method, args.Arguments, args.Instance );

                        logSource.Debug.EnabledOrNull?.Write( Formatted(  "Key=\"{Key}\".", methodKey ) );

                        // TODO: Pass a CancellationToken.

                        Task<object> task = CachingFrontend.GetOrAddAsync(method,
                                                                                methodKey, method.ReturnType.GetGenericArguments()[0], this.MergedConfiguration,
                                                                                new MethodEvaluationData(args).GetValueAsync,
                                                                                logSource, CancellationToken.None);
                        if (!task.IsCompleted)
                        {
                            // We need to call LogActivity.Suspend and LogActivity.Resume manually because we're in an aspect,
                            // and the await instrumentation policy is not applied.
                            activity.Suspend();
                            try
                            {
                                await task;
                            }
                            finally
                            {
                                activity.Resume();
                            }
                        }

                        args.ReturnValue = task.Result;
                    }

                    activity.SetSuccess();
                }
                catch ( Exception e)
                {
                    activity.SetException(e);
                    throw;
                }
            }
        }

        private class MethodEvaluationData
        {
            private object instance;
            private readonly Arguments arguments;
            private readonly IMethodBinding methodBinding;
            private readonly IAsyncMethodBinding asyncMethodBinding;

            public MethodEvaluationData( MethodInterceptionArgs args )
            {
                this.methodBinding = args.Binding;
                if ( args.IsAsync )
                {
                    this.asyncMethodBinding = args.AsyncBinding;
                }
                this.arguments = args.Arguments;
                this.instance = args.Instance;
            }

            public object GetValue()
            {
                return this.methodBinding.Invoke( ref this.instance, this.arguments );
            }

            public async Task<object> GetValueAsync()
            {
                return await this.asyncMethodBinding.InvokeAsync(ref this.instance, this.arguments);
            }
        }
    }
}