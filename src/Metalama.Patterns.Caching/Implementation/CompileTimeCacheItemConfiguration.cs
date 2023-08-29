// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Aspects;
using Metalama.Framework.Code;
using Metalama.Framework.Code.DeclarationBuilders;

namespace Metalama.Patterns.Caching.Implementation;

/// <summary>
/// Configuration of a <see cref="CacheItem"/>.
/// </summary>
[CompileTime]
internal sealed class CompileTimeCacheItemConfiguration
{
    // This class is near-identical to CacheItemConfiguration, but can be used exclusively at compile time.
    // It's not practical to make CacheItemConfiguration [RunTimeOrCompileTime] because this
    // then spreads the [RunTimeOrCompileTime] requirement a wide body of code.

    public CompileTimeCacheItemConfiguration() { }

    public string? ProfileName { get; set; }

    public bool? AutoReload { get; set; }

    public TimeSpan? AbsoluteExpiration { get; set; }

    public TimeSpan? SlidingExpiration { get; set; }

    public CacheItemPriority? Priority { get; set; }

    public bool? IgnoreThisParameter { get; set; }

    public bool? UseDependencyInjection { get; set; }

    public void ApplyFallback( CompileTimeCacheItemConfiguration fallback )
    {
        this.AutoReload ??= fallback.AutoReload;
        this.AbsoluteExpiration ??= fallback.AbsoluteExpiration;
        this.SlidingExpiration ??= fallback.SlidingExpiration;
        this.Priority ??= fallback.Priority;
        this.ProfileName ??= fallback.ProfileName;
        this.IgnoreThisParameter ??= fallback.IgnoreThisParameter;
        this.UseDependencyInjection ??= fallback.UseDependencyInjection;
    }

    /// <summary>
    /// Applies the effective configuration of the method by applying fallback configuration from <see cref="CachingConfigurationAttribute"/>
    /// attributes on ancestor types and the declaring assembly.
    /// </summary>
    public void ApplyEffectiveConfiguration( IMethod method )
    {
        var configurationFromAttributes = CompileTimeCacheConfigurationHelper.GetConfigurationFromAttributes( method );
        this.ApplyFallback( configurationFromAttributes );
    }

    // ReSharper disable once UnusedMember.Global
    internal CompileTimeCacheItemConfiguration Clone() => (CompileTimeCacheItemConfiguration) this.MemberwiseClone();

    /// <summary>
    /// Initializes a new instance of the <see cref="CompileTimeCacheItemConfiguration"/> class from the given
    /// custom attribute which must have construction semantics equivalent to <see cref="CachingConfigurationAttribute"/>
    /// and <see cref="CacheAttribute"/> (which both have the same construction semantics).
    /// </summary>
    public CompileTimeCacheItemConfiguration( IAttribute attribute )
    {
        if ( attribute == null )
        {
            throw new ArgumentNullException( nameof(attribute) );
        }

        if ( !(attribute.Type.Is( typeof(CacheAttribute) ) || attribute.Type.Is( typeof(CachingConfigurationAttribute) )) )
        {
            throw new ArgumentOutOfRangeException(
                nameof(attribute),
                "Must be a " + nameof(CacheAttribute) + " or a " + nameof(CachingConfigurationAttribute) + "." );
        }

        try
        {
            if ( attribute.TryGetNamedArgument( nameof(ICachingConfigurationAttribute.ProfileName), out var profileName ) )
            {
                this.ProfileName = (string?) profileName.Value;
            }

            if ( attribute.TryGetNamedArgument( nameof(ICachingConfigurationAttribute.AutoReload), out var autoReload ) )
            {
                this.AutoReload = (bool) autoReload.Value!;
            }

            if ( attribute.TryGetNamedArgument( nameof(ICachingConfigurationAttribute.AbsoluteExpiration), out var absoluteExpiration ) )
            {
                this.AbsoluteExpiration = TimeSpan.FromMinutes( (double) absoluteExpiration.Value! );
            }

            if ( attribute.TryGetNamedArgument( nameof(ICachingConfigurationAttribute.SlidingExpiration), out var slidingExpiration ) )
            {
                this.SlidingExpiration = TimeSpan.FromMinutes( (double) slidingExpiration.Value! );
            }

            if ( attribute.TryGetNamedArgument( nameof(ICachingConfigurationAttribute.Priority), out var priority ) )
            {
                this.Priority = (CacheItemPriority) priority.Value!;
            }

            if ( attribute.TryGetNamedArgument( nameof(ICachingConfigurationAttribute.IgnoreThisParameter), out var ignoreThisParameter ) )
            {
                this.IgnoreThisParameter = (bool) ignoreThisParameter.Value!;
            }

            if ( attribute.TryGetNamedArgument( nameof(ICachingConfigurationAttribute.UseDependencyInjection), out var useDependencyInjection ) )
            {
                this.UseDependencyInjection = (bool) useDependencyInjection.Value!;
            }
        }
        catch ( Exception e )
        {
            throw new CachingAssertionFailedException( "The construction semantics of the attribute are not as expected.", e );
        }
    }

    public AttributeConstruction ToAttributeConstruction()
    {
        var args = new Dictionary<string, object?>();

        if ( this.AbsoluteExpiration.HasValue )
        {
            args[nameof(CachingConfigurationAttribute.AbsoluteExpiration)] = this.AbsoluteExpiration.Value.TotalMinutes;
        }

        if ( this.AutoReload.HasValue )
        {
            args[nameof(CachingConfigurationAttribute.AutoReload)] = this.AutoReload;
        }

        if ( this.IgnoreThisParameter.HasValue )
        {
            args[nameof(CachingConfigurationAttribute.IgnoreThisParameter)] = this.IgnoreThisParameter;
        }

        if ( this.Priority.HasValue )
        {
            args[nameof(CachingConfigurationAttribute.Priority)] = this.Priority;
        }

        if ( this.SlidingExpiration.HasValue )
        {
            args[nameof(CachingConfigurationAttribute.SlidingExpiration)] = this.SlidingExpiration.Value.TotalMinutes;
        }

        if ( this.ProfileName != null )
        {
            args[nameof(CachingConfigurationAttribute.ProfileName)] = this.ProfileName;
        }

        if ( this.UseDependencyInjection != null )
        {
            args[nameof(CachingConfigurationAttribute.UseDependencyInjection)] = this.UseDependencyInjection;
        }

        return AttributeConstruction.Create( typeof(CacheAttribute), namedArguments: args.ToList() );
    }
}