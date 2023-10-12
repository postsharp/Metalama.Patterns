// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Aspects;

namespace Metalama.Patterns.Xaml;

public sealed partial class DependencyPropertyAttribute
{
    /* PostSharp accepted handler signatures:

            static void OnPropertyNameChanged()
            static void OnPropertyNameChanged(DependencyProperty property)
            static void OnPropertyNameChanged(TDeclaringType instance)
            static void OnPropertyNameChanged(DependencyProperty property, TDeclaringType instance)
            void OnPropertyNameChanged()
            void OnPropertyNameChanged(DependencyProperty property)

            where TDeclaringType is: declaring type || DependencyObject || object

        Windows Community Toolkit "ObservableProperty" style handler signatures:

          Supported here (instance only):
            void OnNameChanging(string? value);
            void OnNameChanged(string? value);
            void OnNameChanged(string? oldValue, string? newValue);                

          Not supported here, 'oldValue' does not appear to fit the purely DependencyProperty-backed GetValue/SetValue/callbacks model for OnChanging.
            void OnNameChanging(string? oldValue, string? newValue);
    */

    [CompileTime]
    private enum ChangeHandlerParametersKind
    {
        Invalid,
        None,
        Value,
        OldValueAndNewValue,
        DependencyProperty,
        StaticNone,
        StaticDependencyProperty,
        StaticInstance,
        StaticDependencyPropertyAndInstance
    }
}