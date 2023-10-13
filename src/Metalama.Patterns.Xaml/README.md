# Metalama.Patterns.Xaml

The WPF conventions regarding field naming and so on are described [here](https://learn.microsoft.com/en-us/dotnet/desktop/wpf/properties/custom-dependency-properties?view=netdesktop-7.00).
 
For an informative discussion of how default value handling was implemented in PostSharp, see [here](https://postsharp.tpondemand.com/entity/15285-dependency-properties-propertymetadatadefaultvalue-is-not-set).

## Default Values with [DependencyProperty]

The Metalama implementation differs from the PostSharp implementation with the intention of being more in the spirit of Metalama.

If the target property has an initializer, this is used as `PropertyMetadata.DefaultValue` when `DependencyPropertyOptions.InitializerProvidesDefaultValue` is `true` (the default) and as the initial value when `DependencyPropertyOptions.InitializerProvidesInitialValue` is `true` (the default). 

## Valid PropertyChanging and PropertyChanged Method Signatures

The following signatures originate from the PostSharp implementation:

* `static void OnFooChanged()`
* `static void OnFooChanged(DependencyProperty property)`
* `static void OnFooChanged(TDeclaringType instance)`
* `static void OnFooChanged(DependencyProperty property, TDeclaringType instance)`
* `void OnFooChanged()`
* `void OnFooChanged(DependencyProperty property)`

where TDeclaringType is the declaring type of the target property, or DependencyObject, or object.

The following signatures are inspired by `ObservableProperty` from the Windows Community Toolkit:

* `void OnFooChanging(value_type value)`
* `void OnFooChanged(value_type value)`
* `void OnFooChanged(value_type oldValue, value_type newValue)`
* `void OnFooChanging<T>(T value)`
* `void OnFooChanged<T>(T value)`
* `void OnFooChanged<T>(T oldValue, T newValue)`

where `value_type` is any type assignable from the actual type of the target property.

Note that `oldValue` does not appear to fit the purely DependencyProperty-backed GetValue/SetValue/callbacks model for OnChanging, so `void OnNameChanging(value_type? oldValue, value_type? newValue)` is not supported.