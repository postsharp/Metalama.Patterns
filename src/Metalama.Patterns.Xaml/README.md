# Metalama.Patterns.Xaml

The WPF conventions regarding field naming and so on are described [here](https://learn.microsoft.com/en-us/dotnet/desktop/wpf/properties/custom-dependency-properties?view=netdesktop-7.00).
 
For an informative discussion of how default value handling was implemented in PostSharp, see [here](https://postsharp.tpondemand.com/entity/15285-dependency-properties-propertymetadatadefaultvalue-is-not-set).

## Default Values with [DependencyProperty]

The Metalama implementation differs from the PostSharp implementation with the intention of being more in the spirit of Metalama.

If the target property has an initializer, this is used as both `PropertyMetadata.DefaultValue` and as the initial value. An initial value can also be set explictly in the constructor of the declaring type of the target property, which will happen after the value from the initializer has been applied.
