// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Aspects;
using Metalama.Framework.Code;
using Metalama.Framework.Serialization;
using Metalama.Patterns.Xaml.Implementation.NamingConvention;
using System.Text.RegularExpressions;

namespace Metalama.Patterns.Xaml.Implementation.DependencyPropertyNamingConvention;

[CompileTime]
internal sealed class RegexDependencyPropertyNamingConvention : IDependencyPropertyNamingConvention
{
    public const string NameGroup = "Name";
    public const string NameToken = "$Name$";

    private readonly string? _matchName;
    private readonly string? _registrationFieldPattern;
    private readonly string? _matchPropertyChanging;
    private readonly string? _matchPropertyChanged;
    private readonly string? _matchValidate;
    private readonly bool _requirePropertyChangingMatch;
    private readonly bool _requirePropertyChangedMatch;
    private readonly bool _requireValidateMatch;

    [NonCompileTimeSerialized]
    private Regex? _matchNameRegex;

    /// <summary>
    /// Initializes a new instance of the <see cref="RegexDependencyPropertyNamingConvention"/> class.
    /// </summary>
    /// <param name="diagnosticName">A short name describing the convention, used when reporting diagnostics.</param>
    /// <param name="matchName">
    /// A regex match expression that will be evaluated against the name of the target property of the <see cref="DependencyPropertyAttribute"/> aspect. 
    /// The expression should yield a match group named <see cref="NameGroup"/>. If <paramref name="matchName"/> is <see langword="null"/>,
    /// the name of the target property is used.
    /// </param>
    /// <param name="registrationFieldPattern">
    /// The name of the registration field to be introduced. A string in which the substring <see cref="NameToken"/> will be replaced with
    /// the name as determined according to <paramref name="matchName"/>. If <paramref name="registrationFieldPattern"/> is <see langword="null"/>,
    /// the name produced by <see cref="DefaultDependencyPropertyNamingConvention.GetRegistrationFieldNameFromPropertyName(string)"/> is used.
    /// </param>
    /// <param name="matchPropertyChanging">
    /// <para>
    /// A regex match expression that will be evauluated against method names to identify candidate property-changing methods. 
    /// All occurences of the substring <see cref="NameToken"/> will be replaced with the name
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
    /// All occurences of the substring <see cref="NameToken"/> will be replaced with the name
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
    /// All occurences of the substring <see cref="NameToken"/> will be replaced with the name
    /// detemined according to <paramref name="matchName"/> before the expression is evaluated.
    /// </para>
    /// <para>
    /// If <paramref name="matchPropertyChanged"/> is <see langword="null"/>, the name produced by 
    /// <see cref="DefaultDependencyPropertyNamingConvention.GetValidateMethodNameFromPropertyName(string)"/> is matched.
    /// </para>
    /// </param>
    /// <param name="requirePropertyChangingMatch">
    /// If <see langword="true"/> (the default), a matching valid unambiguous property-changing method must be found for a match to be considered successful.
    /// </param>
    /// <param name="requirePropertyChangedMatch">
    /// If <see langword="true"/> (the default), a matching valid unambiguous property-changed method must be found for a match to be considered successful.
    /// </param>
    /// <param name="requireValidateMatch">
    /// If <see langword="true"/> (the default), a matching valid unambiguous validate method must be found for a match to be considered successful.
    /// </param>
    public RegexDependencyPropertyNamingConvention( 
        string diagnosticName,
        string? matchName, // get name from target property name, default is the raw property name.
        string? registrationFieldPattern, // eg "$Name$Property", else GetRegistrationFieldNameFromPropertyName
        string? matchPropertyChanging,
        string? matchPropertyChanged,
        string? matchValidate,
        bool requirePropertyChangingMatch = false,
        bool requirePropertyChangedMatch = false,
        bool requireValidateMatch = false )
    {
        if ( string.IsNullOrWhiteSpace( diagnosticName ) )
        {
            throw new ArgumentException( "Must not be null, empty or only white space.", nameof( diagnosticName ) );
        }

        this.DiagnosticName = diagnosticName;
        this._matchName = matchName;
        this._registrationFieldPattern = registrationFieldPattern;
        this._matchPropertyChanging = matchPropertyChanging;
        this._matchPropertyChanged = matchPropertyChanged;
        this._matchValidate = matchValidate;
        this._requirePropertyChangingMatch = requirePropertyChangingMatch;
        this._requirePropertyChangedMatch = requirePropertyChangedMatch;
        this._requireValidateMatch = requireValidateMatch;
    }

    public string DiagnosticName { get; }

    public DependencyPropertyNamingConventionMatch Match( IProperty targetProperty, InspectedDeclarationsAdder inspectedDeclarations )
    {
        string? propertyName = null;

        if ( this._matchName != null )
        {
            this._matchNameRegex ??= new Regex( this._matchName );

            var m = this._matchNameRegex.Match( targetProperty.Name );

            if ( m.Success )
            {
                var g = m.Groups[NameGroup];
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

        if ( string.IsNullOrWhiteSpace( propertyName))
        {
            return new DependencyPropertyNamingConventionMatch(
                this,
                null,
                null,
                DeclarationMatch<IMemberOrNamedType>.Invalid(),
                DeclarationMatch<IMethod>.NotFound(),
                DeclarationMatch<IMethod>.NotFound(),
                DeclarationMatch<IMethod>.NotFound(),
                ChangeHandlerSignatureKind.Invalid,
                ChangeHandlerSignatureKind.Invalid,
                ValidationHandlerSignatureKind.Invalid,
                this._requirePropertyChangingMatch,
                this._requirePropertyChangedMatch,
                this._requireValidateMatch );
        }

#if NETCOREAPP
#pragma warning disable CA1307 // Specify StringComparison for clarity
#endif
        var registrationFieldName = this._registrationFieldPattern != null 
            ? this._registrationFieldPattern.Replace( NameToken, propertyName ) 
            : DefaultDependencyPropertyNamingConvention.GetRegistrationFieldNameFromPropertyName( propertyName );

        var matchPropertyChanging = this._matchPropertyChanging?.Replace( NameToken, propertyName );
        
        var matchPropertyChanged = this._matchPropertyChanged?.Replace( NameToken, propertyName );
        
        var matchValidate = this._matchValidate?.Replace( NameToken, propertyName );
#if NETCOREAPP
#pragma warning restore CA1307 // Specify StringComparison for clarity
#endif

        return DependencyPropertyNamingConventionHelper.Match(
            this,
            targetProperty,
            inspectedDeclarations,
            propertyName,
            registrationFieldName,
            new StringOrRegexNameMatchPredicate( 
                matchPropertyChanging == null ? DefaultDependencyPropertyNamingConvention.GetPropertyChangingMethodNameFromPropertyName( propertyName ) : null,
                matchPropertyChanging != null ? new Regex(matchPropertyChanging) : null ),
            new StringOrRegexNameMatchPredicate(
                matchPropertyChanged == null ? DefaultDependencyPropertyNamingConvention.GetPropertyChangedMethodNameFromPropertyName( propertyName ) : null,
                matchPropertyChanged != null ? new Regex( matchPropertyChanged ) : null ),
            new StringOrRegexNameMatchPredicate(
                matchValidate == null ? DefaultDependencyPropertyNamingConvention.GetValidateMethodNameFromPropertyName( propertyName ) : null,
                matchValidate != null ? new Regex( matchValidate ) : null ),
            this._requirePropertyChangingMatch,
            this._requirePropertyChangedMatch,
            this._requireValidateMatch );
    }    
}