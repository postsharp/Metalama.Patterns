// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Aspects;
using Metalama.Framework.Code;
using Metalama.Patterns.Xaml.Implementation.NamingConvention;

namespace Metalama.Patterns.Xaml.Implementation.DependencyPropertyNamingConvention;

[CompileTime]
internal sealed class DefaultDependencyPropertyNamingConvention : IDependencyPropertyNamingConvention
{
    public static string RegistrationKey { get; } = "{5150E382-3376-44CF-A278-CC7E4C8E5361}";

    public string DiagnosticName => "default";

    public DependencyPropertyNamingConventionMatch Match( IProperty targetProperty, InspectedDeclarationsAdder inspectedDeclarations )
    {
        var propertyName = targetProperty.Name;

        return DependencyPropertyNamingConventionHelper.Match(
            this,
            targetProperty,
            inspectedDeclarations,
            propertyName,
            GetRegistrationFieldNameFromPropertyName( propertyName ),
            new StringNameMatchPredicate( GetPropertyChangingMethodNameFromPropertyName( propertyName ) ),
            new StringNameMatchPredicate( GetPropertyChangedMethodNameFromPropertyName( propertyName ) ),
            new StringNameMatchPredicate( GetValidateMethodNameFromPropertyName( propertyName ) ) );
    }

    internal static string GetRegistrationFieldNameFromPropertyName( string propertyName ) => $"{propertyName}Property";

    internal static string GetPropertyChangingMethodNameFromPropertyName( string propertyName ) => $"On{propertyName}Changing";

    internal static string GetPropertyChangedMethodNameFromPropertyName( string propertyName ) => $"On{propertyName}Changed";

    internal static string GetValidateMethodNameFromPropertyName( string propertyName ) => $"Validate{propertyName}";
}