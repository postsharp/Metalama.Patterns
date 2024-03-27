// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Aspects;
using Metalama.Framework.Code;
using Metalama.Patterns.Xaml.Implementation.NamingConvention;

namespace Metalama.Patterns.Xaml.Implementation.DependencyPropertyNamingConvention;

[CompileTime]
internal sealed class ExplicitDependencyPropertyNamingConvention : IDependencyPropertyNamingConvention
{
    private readonly string? _registrationFieldName;
    private readonly string? _propertyChangingMethodName;
    private readonly string? _propertyChangedMethodName;
    private readonly string? _validateMethodName;

    public ExplicitDependencyPropertyNamingConvention(
        string? registrationFieldName,
        string? propertyChangingMethodName,
        string? propertyChangedMethodName,
        string? validateMethodName )
    {
        this._registrationFieldName = registrationFieldName;
        this._propertyChangingMethodName = propertyChangingMethodName;
        this._propertyChangedMethodName = propertyChangedMethodName;
        this._validateMethodName = validateMethodName;
    }

    public string DiagnosticName => "explicitly-configured";

    public DependencyPropertyNamingConventionMatch Match( IProperty targetProperty, InspectedDeclarationsAdder inspectedDeclarations )
    {
        var propertyName = targetProperty.Name;

        return DependencyPropertyNamingConventionHelper.Match(
            this,
            targetProperty,
            inspectedDeclarations,
            propertyName,
            this._registrationFieldName ?? DefaultDependencyPropertyNamingConvention.GetRegistrationFieldNameFromPropertyName( propertyName ),
            new StringNameMatchPredicate(
                this._propertyChangingMethodName ?? DefaultDependencyPropertyNamingConvention.GetPropertyChangingMethodNameFromPropertyName( propertyName ) ),
            new StringNameMatchPredicate(
                this._propertyChangedMethodName ?? DefaultDependencyPropertyNamingConvention.GetPropertyChangedMethodNameFromPropertyName( propertyName ) ),
            new StringNameMatchPredicate(
                this._validateMethodName ?? DefaultDependencyPropertyNamingConvention.GetValidateMethodNameFromPropertyName( propertyName ) ),
            requirePropertyChangingMatch: this._propertyChangingMethodName != null,
            requirePropertyChangedMatch: this._propertyChangedMethodName != null,
            requireValidateMatch: this._validateMethodName != null );
    }
}