// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using JetBrains.Annotations;
using Metalama.Framework.Aspects;
using Metalama.Framework.Options;
using Metalama.Patterns.Xaml.Implementation.DependencyPropertyNamingConvention;
using Metalama.Patterns.Xaml.Implementation.NamingConvention;
using System.Windows;

namespace Metalama.Patterns.Xaml.Options;

[PublicAPI]
[CompileTime]
public sealed class DependencyPropertyOptionsBuilder
{
    private DependencyPropertyOptions _options = new();

    /// <summary>
    /// Gets the key of the default naming convention.
    /// </summary>
    public static string DefaultNamingConventionKey => DefaultDependencyPropertyNamingConvention.RegistrationKey;

    public static class Names
    {
        public const string NameGroup = RegexDependencyPropertyNamingConvention.NameGroup;
        public const string NameToken = RegexDependencyPropertyNamingConvention.NameToken;
    }

    /// <summary>
    /// Gets or sets a value indicating whether the property should be registered as a read-only property.
    /// </summary>
    public bool? IsReadOnly { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the property initializer (if present) should be used to set the initial value of the <see cref="DependencyProperty"/>
    /// in the instance constructor of the declaring class of the target property. The default is <see langword="false"/>.
    /// </summary>
    public bool? InitializerProvidesInitialValue { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the property initializer (if present) should be used to for <see cref="PropertyMetadata.DefaultValue"/>.
    /// The default is <see langword="true"/>.
    /// </summary>
    public bool? InitializerProvidesDefaultValue { get; set; }

    /// <summary>
    /// Adds or updates a naming convention identified by the given <paramref name="key"/>.
    /// </summary>
    /// <param name="key">A unique identifier used as a key in the keyed collection of naming conventions.</param>
    /// <param name="diagnosticName">A short name describing the convention, used when reporting diagnostics.</param>
    /// <param name="matchName">
    /// A regex match expression that will be evaluated against the name of the target property of the <see cref="DependencyPropertyAttribute"/> aspect. 
    /// The expression should yield a match group named <see cref="Names.NameGroup"/>. If <paramref name="matchName"/> is <see langword="null"/>,
    /// the name of the target property is used.
    /// </param>
    /// <param name="registrationFieldPattern">
    /// The name of the registration field to be introduced. A string in which the substring <see cref="Names.NameToken"/> will be replaced with
    /// the name as determined according to <paramref name="matchName"/>. If <paramref name="registrationFieldPattern"/> is <see langword="null"/>,
    /// the name produced by <see cref="DefaultDependencyPropertyNamingConvention.GetRegistrationFieldNameFromPropertyName(string)"/> is used.
    /// </param>
    /// <param name="matchPropertyChanging">
    /// <para>
    /// A regex match expression that will be evauluated against method names to identify candidate property-changing methods. 
    /// All occurences of the substring <see cref="Names.NameToken"/> will be replaced with the name
    /// detemined according to <paramref name="matchName"/> before the expression is evaluated.
    /// </para>
    /// <para>
    /// If <paramref name="matchPropertyChanging"/> is <see langword="null"/>, the name produced by 
    /// <see cref="DefaultDependencyPropertyNamingConvention.GetPropertyChangingMethodNameFromPropertyName(string)"/> is matched.
    /// </para>
    /// </param>
    /// <param name="matchPropertyChanged">
    /// <para>
    /// A regex match expression that will be evauluated against method names to identify candidate property-changed methods. 
    /// All occurences of the substring <see cref="Names.NameToken"/> will be replaced with the name
    /// detemined according to <paramref name="matchName"/> before the expression is evaluated.
    /// </para>
    /// <para>
    /// If <paramref name="matchPropertyChanged"/> is <see langword="null"/>, the name produced by 
    /// <see cref="DefaultDependencyPropertyNamingConvention.GetPropertyChangedMethodNameFromPropertyName(string)"/> is matched.
    /// </para>
    /// </param>
    /// <param name="matchValidate">
    /// <para>
    /// A regex match expression that will be evauluated against method names to identify candidate validate methods. 
    /// All occurences of the substring <see cref="Names.NameToken"/> will be replaced with the name
    /// detemined according to <paramref name="matchName"/> before the expression is evaluated.
    /// </para>
    /// <para>
    /// If <paramref name="matchPropertyChanged"/> is <see langword="null"/>, the name produced by 
    /// <see cref="DefaultDependencyPropertyNamingConvention.GetValidateMethodNameFromPropertyName(string)"/> is matched.
    /// </para>
    /// </param>
    /// <param name="priority">
    /// The priority of the naming convention. The default priority is 0. The system-registered default naming convention has priority 1000. Naming conventions are
    /// matched in ascending priority. The first successful match is used.
    /// </param>
    /// <param name="requirePropertyChangingMatch">
    /// If <see langword="true"/>, a matching valid unambiguous property-changing method must be found for a match to be considered successful.
    /// The default value is <see langword="true"/> when <paramref name="matchPropertyChanging"/> is specified, otherwise <see langword="false"/>.
    /// </param>
    /// <param name="requirePropertyChangedMatch">
    /// If <see langword="true"/>, a matching valid unambiguous property-changed method must be found for a match to be considered successful.
    /// The default value is <see langword="true"/> when <paramref name="matchPropertyChanged"/> is specified, otherwise <see langword="false"/>.
    /// </param>
    /// <param name="requireValidateMatch">
    /// If <see langword="true"/>, a matching valid unambiguous validate method must be found for a match to be considered successful.
    /// The default value is <see langword="true"/> when <paramref name="matchValidate"/> is specified, otherwise <see langword="false"/>.
    /// </param>
    public void ApplyRegexNamingConvention(
        string key,
        string diagnosticName,
        string? matchName,
        string? registrationFieldPattern,
        string? matchPropertyChanging,
        string? matchPropertyChanged,
        string? matchValidate,
        int priority = 0,
        bool? requirePropertyChangingMatch = null,
        bool? requirePropertyChangedMatch = null,
        bool? requireValidateMatch = null )
    {
        if ( key == DefaultDependencyPropertyNamingConvention.RegistrationKey )
        {
            throw new InvalidOperationException( "The default naming convention cannot be modified." );
        }

        this._options = this._options with
        {
            NamingConventionRegistrations =
            this._options.NamingConventionRegistrations.AddOrApplyChanges(
                new NamingConventionRegistration<IDependencyPropertyNamingConvention>(
                    key,
                    new RegexDependencyPropertyNamingConvention(
                        diagnosticName,
                        matchName,
                        registrationFieldPattern,
                        matchPropertyChanging,
                        matchPropertyChanged,
                        matchValidate,
                        requirePropertyChangingMatch ?? matchPropertyChanging != null,
                        requirePropertyChangedMatch ?? matchPropertyChanged != null,
                        requireValidateMatch ?? matchValidate != null ),
                    priority ) )
        };
    }

    public void SetNamingConventionPriority( string key, int priority )
    {
        this._options = this._options with
        {
            NamingConventionRegistrations =
            this._options.NamingConventionRegistrations.AddOrApplyChanges(
                new NamingConventionRegistration<IDependencyPropertyNamingConvention>( key, null, priority ) )
        };
    }

    public void RemoveNamingConvention( string key )
    {
        this._options = this._options with
        {
            NamingConventionRegistrations =
            this._options.NamingConventionRegistrations.Remove( key )
        };
    }

    /// <summary>
    /// Resets naming convention registrations to the default state, removing any user-registered naming conventions.
    /// </summary>
    public void ResetNamingConventions()
    {
        this._options = this._options with
        {
            NamingConventionRegistrations =
            this._options.NamingConventionRegistrations
                .ApplyChanges( IncrementalKeyedCollection.Clear<string, NamingConventionRegistration<IDependencyPropertyNamingConvention>>(), default )
                .AddOrApplyChanges( DependencyPropertyOptions.DefaultNamingConventionRegistrations() )
        };
    }

    internal DependencyPropertyOptions Build() => this._options;
}