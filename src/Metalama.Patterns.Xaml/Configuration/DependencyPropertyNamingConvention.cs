// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Aspects;
using Metalama.Framework.Code;
using Metalama.Framework.Serialization;
using Metalama.Patterns.Xaml.Implementation;
using Metalama.Patterns.Xaml.Implementation.DependencyPropertyNamingConvention;
using Metalama.Patterns.Xaml.Implementation.NamingConvention;
using System.Text.RegularExpressions;

namespace Metalama.Patterns.Xaml.Configuration;

// Prevent netframework-only false positives

#if NETFRAMEWORK
#pragma warning disable CS8604 // Possible null reference argument.
#endif

#pragma warning disable SA1623

[CompileTime]
public sealed record DependencyPropertyNamingConvention : IDependencyPropertyNamingConvention
{
    /// <summary>
    /// Gets or sets the regular expression pattern that will be evaluated against the name of the target property of the <see cref="DependencyPropertyAttribute"/> aspect. 
    /// The expression should yield a match group named <c>Name</c>. If <see cref="PropertyNamePattern"/> is not specified, the name of the target property is used
    /// as the name of the dependency property.
    /// </summary>
    public string? PropertyNamePattern { get; init; }

    /// <summary>
    /// Gets or sets the name of the registration field to be introduced. The substring <c>{Name}</c> will be replaced with
    /// the name as determined according to <see cref="PropertyNamePattern"/>. The default value is  <c>{Name}Property</c>.
    /// </summary>
    public string? RegistrationFieldName { get; init; }

    /// <summary>
    /// Gets or sets a regular expression pattern used to identify a method invoked <i>before</i> the property is changed. 
    /// All occurrences of the substring <c>{Name}</c> will be replaced with the name
    /// determined according to <see cref="PropertyNamePattern"/> before the expression is evaluated. The default value is <c>On{Name}Changing</c>.
    /// </summary>
    public string? OnPropertyChangingPattern { get; init; }

    /// <summary>
    /// Gets or sets a regular expression pattern used to identify a method invoked <i>after</i> the property has changed. 
    /// All occurrences of the substring <c>{Name}</c> will be replaced with the name
    /// determined according to <see cref="PropertyNamePattern"/> before the expression is evaluated. The default value is <c>On{Name}Changed</c>.
    /// </summary>
    public string? OnPropertyChangedPattern { get; init; }

    /// <summary>
    /// Gets or sets a regular expression used to identify the method called before the property is changed to perform validation. 
    /// All occurrences of the substring <c>{Name}</c>  will be replaced with the name
    /// determined according to <see cref="PropertyNamePattern"/> before the expression is evaluated. The default value is <c>^Validate{Name}$</c>.
    /// </summary>
    public string? ValidatePattern { get; init; }

    /// <summary>
    /// Gets or sets a value indicating whether the <c>OChanging</c> method is required. The default value of this property
    /// is <c>true</c> if a value is provided for <see cref="OnPropertyChangingPattern"/>, otherwise <c>false</c>.
    /// </summary>
    public bool? IsOnPropertyChangingRequired { get; init; }

    /// <summary>
    /// Gets or sets a value indicating whether the <c>OnChanged</c> method is required. The default value of this property
    /// is <c>true</c> if a value is provided for <see cref="OnPropertyChangedPattern"/>, otherwise <c>false</c>.
    /// </summary>
    public bool? IsOnPropertyChangedRequired { get; init; }

    /// <summary>
    /// Gets or sets a value indicating whether the <c>Validate</c> method is required. The default value of this property
    /// is <c>true</c> if a value is provided for <see cref="ValidatePattern"/>, otherwise <c>false</c>.
    /// </summary>
    public bool? IsValidateRequired { get; init; }

    [NonCompileTimeSerialized]
    private Regex? _matchNameRegex;

    public DependencyPropertyNamingConvention( string name )
    {
        this.Name = name;
    }

    public string Name { get; }

    DependencyPropertyNamingConventionMatch INamingConvention<IProperty, DependencyPropertyNamingConventionMatch>.Match(
        IProperty targetProperty,
        Action<InspectedMember> addInspectedMember )
    {
        string? propertyName = null;

        if ( this.PropertyNamePattern != null )
        {
            this._matchNameRegex ??= new Regex( this.PropertyNamePattern );

            var m = this._matchNameRegex.Match( targetProperty.Name );

            if ( m.Success )
            {
                var g = m.Groups["Name"];

                if ( g.Success )
                {
                    propertyName = g.Value;
                }
            }
        }
        else
        {
            propertyName = targetProperty.Name;
        }

        if ( string.IsNullOrWhiteSpace( propertyName ) )
        {
            return new DependencyPropertyNamingConventionMatch(
                this,
                null,
                null,
                MemberMatch<IMemberOrNamedType, DefaultMatchKind>.Invalid(),
                MemberMatch<IMethod, ChangeHandlerSignatureKind>.NotFound(),
                MemberMatch<IMethod, ChangeHandlerSignatureKind>.NotFound(),
                MemberMatch<IMethod, ValidationHandlerSignatureKind>.NotFound(),
                this.IsOnPropertyChangingRequired.GetValueOrDefault( this.OnPropertyChangingPattern != null ),
                this.IsOnPropertyChangedRequired.GetValueOrDefault( this.OnPropertyChangedPattern != null ),
                this.IsValidateRequired.GetValueOrDefault( this.ValidatePattern != null ) );
        }

#if NETCOREAPP
#pragma warning disable CA1307 // Specify StringComparison for clarity
#endif
        var registrationFieldName = this.RegistrationFieldName != null
            ? this.RegistrationFieldName.Replace( "{Name}", propertyName )
            : DefaultDependencyPropertyNamingConvention.GetRegistrationFieldNameFromPropertyName( propertyName );

        var matchPropertyChanging = this.OnPropertyChangingPattern?.Replace( "{Name}", propertyName );

        var matchPropertyChanged = this.OnPropertyChangedPattern?.Replace( "{Name}", propertyName );

        var matchValidate = this.ValidatePattern?.Replace( "{Name}", propertyName );
#if NETCOREAPP
#pragma warning restore CA1307 // Specify StringComparison for clarity
#endif

        var propertyChangingPredicate = matchPropertyChanging == null
            ? (INameMatchPredicate) new StringNameMatchPredicate(
                DefaultDependencyPropertyNamingConvention.GetPropertyChangingMethodNameFromPropertyName( propertyName ) )
            : new RegexNameMatchPredicate( new Regex( matchPropertyChanging ) );

        var propertyChangedPredicate = matchPropertyChanged == null
            ? (INameMatchPredicate) new StringNameMatchPredicate(
                DefaultDependencyPropertyNamingConvention.GetPropertyChangedMethodNameFromPropertyName( propertyName ) )
            : new RegexNameMatchPredicate( new Regex( matchPropertyChanged ) );

        var validatePredicate = matchValidate == null
            ? (INameMatchPredicate) new StringNameMatchPredicate(
                DefaultDependencyPropertyNamingConvention.GetValidateMethodNameFromPropertyName( propertyName ) )
            : new RegexNameMatchPredicate( new Regex( matchValidate ) );

        return DependencyPropertyNamingConventionMatcher.Match(
            this,
            targetProperty,
            addInspectedMember,
            propertyName,
            registrationFieldName,
            propertyChangingPredicate,
            propertyChangedPredicate,
            validatePredicate,
            this.IsOnPropertyChangingRequired.GetValueOrDefault( this.OnPropertyChangingPattern != null ),
            this.IsOnPropertyChangedRequired.GetValueOrDefault( this.OnPropertyChangedPattern != null ),
            this.IsValidateRequired.GetValueOrDefault( this.ValidatePattern != null ) );
    }
}