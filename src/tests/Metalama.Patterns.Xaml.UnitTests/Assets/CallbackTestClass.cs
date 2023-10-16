// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using FluentAssertions;
using System.Runtime.CompilerServices;
using System.Windows;

// ReSharper disable UnusedMember.Local

namespace Metalama.Patterns.Xaml.UnitTests.Assets;

public sealed partial class CallbackTestClass : DependencyObject
{
    public sealed class ThreadContext
    {
        private static readonly ThreadLocal<ThreadContext> _current = new( () => new ThreadContext() );

        public static ThreadContext Current => _current.Value!;

        public void Reset( Func<int, bool>? onValidate = null )
        {
            this.Log.Clear();
            this.OnValidate = onValidate;
        }

        public List<string> Log { get; } = new();

        public Func<int, bool>? OnValidate { get; set; }
    }

    public string Id { get; } = Guid.NewGuid().ToString();

    private static void LogCall( string? suffix = null, [CallerMemberName] string? name = null )
    {
        ThreadContext.Current.Log.Add( suffix == null ? name! : $"{name}|{suffix}" );
    }

    private static bool Validate( int value )
    {
        var f = ThreadContext.Current.OnValidate;

        return f == null || f( value );
    }

    #region OnChanging and OnChanged

    [DependencyProperty]
    public int ImplicitInstanceDependencyProperty { get; set; }

    private void OnImplicitInstanceDependencyPropertyChanging( DependencyProperty dependencyProperty )
    {
        LogCall();
        dependencyProperty.Should().BeSameAs( ImplicitInstanceDependencyPropertyProperty );
    }

    private void OnImplicitInstanceDependencyPropertyChanged( DependencyProperty dependencyProperty )
    {
        LogCall();
        dependencyProperty.Should().BeSameAs( ImplicitInstanceDependencyPropertyProperty );
    }

    [DependencyProperty]
    public int ImplicitInstanceNoParameters { get; set; }

    private void OnImplicitInstanceNoParametersChanging()
    {
        LogCall();
    }

    private void OnImplicitInstanceNoParametersChanged()
    {
        LogCall();
    }

    [DependencyProperty]
    public int ImplicitInstanceValue { get; set; }

    private void OnImplicitInstanceValueChanging( int value )
    {
        LogCall( $"{value}" );
    }

    private void OnImplicitInstanceValueChanged( int value )
    {
        LogCall( $"{value}" );
    }

    [DependencyProperty]
    public int ImplicitInstanceOldValueNewValue { get; set; }

    private void OnImplicitInstanceOldValueNewValueChanged( int oldValue, int newValue )
    {
        LogCall( $"{oldValue}|{newValue}" );
    }

    [DependencyProperty]
    public int ImplicitStaticDependencyProperty { get; set; }

    private static void OnImplicitStaticDependencyPropertyChanging( DependencyProperty dependencyProperty )
    {
        LogCall();
        dependencyProperty.Should().BeSameAs( ImplicitStaticDependencyPropertyProperty );
    }

    private static void OnImplicitStaticDependencyPropertyChanged( DependencyProperty dependencyProperty )
    {
        LogCall();
        dependencyProperty.Should().BeSameAs( ImplicitStaticDependencyPropertyProperty );
    }

    [DependencyProperty]
    public int ImplicitStaticDependencyPropertyAndInstance { get; set; }

    private static void OnImplicitStaticDependencyPropertyAndInstanceChanging( DependencyProperty dependencyProperty, CallbackTestClass instance )
    {
        LogCall( instance.Id );
        dependencyProperty.Should().BeSameAs( ImplicitStaticDependencyPropertyAndInstanceProperty );
    }

    private static void OnImplicitStaticDependencyPropertyAndInstanceChanged( DependencyProperty dependencyProperty, CallbackTestClass instance )
    {
        LogCall( instance.Id );
        dependencyProperty.Should().BeSameAs( ImplicitStaticDependencyPropertyAndInstanceProperty );
    }

    [DependencyProperty]
    public int ImplicitStaticInstance { get; set; }

    private static void OnImplicitStaticInstanceChanging( CallbackTestClass instance )
    {
        LogCall( instance.Id );
    }

    private static void OnImplicitStaticInstanceChanged( CallbackTestClass instance )
    {
        LogCall( instance.Id );
    }

    [DependencyProperty]
    public int ImplicitStaticNoParameters { get; set; }

    private void OnImplicitStaticNoParametersChanging()
    {
        LogCall();
    }

    private void OnImplicitStaticNoParametersChanged()
    {
        LogCall();
    }

    #endregion

    #region Validate

    [DependencyProperty]
    public int ImplicitInstanceValidateDependencyPropertyAndValue { get; set; }

    private bool ValidateImplicitInstanceValidateDependencyPropertyAndValue( DependencyProperty dependencyProperty, int value )
    {
        LogCall( $"{value}" );
        dependencyProperty.Should().BeSameAs( ImplicitInstanceValidateDependencyPropertyAndValueProperty );

        return Validate( value );
    }

    private void OnImplicitInstanceValidateDependencyPropertyAndValueChanging()
    {
        LogCall();
    }

    private void OnImplicitInstanceValidateDependencyPropertyAndValueChanged()
    {
        LogCall();
    }

    [DependencyProperty]
    public int ImplicitInstanceValidateValue { get; set; }

    private bool ValidateImplicitInstanceValidateValue( int value )
    {
        LogCall( $"{value}" );

        return Validate( value );
    }

    private void OnImplicitInstanceValidateValueChanging()
    {
        LogCall();
    }

    private void OnImplicitInstanceValidateValueChanged()
    {
        LogCall();
    }

    [DependencyProperty]
    public int ImplicitStaticValidateDependencyPropertyAndInstanceAndValue { get; set; }

    private static bool ValidateImplicitStaticValidateDependencyPropertyAndInstanceAndValue(
        DependencyProperty dependencyProperty,
        CallbackTestClass instance,
        int value )
    {
        LogCall( $"{instance.Id}|{value}" );
        dependencyProperty.Should().BeSameAs( ImplicitStaticValidateDependencyPropertyAndInstanceAndValueProperty );

        return Validate( value );
    }

    private void OnImplicitStaticValidateDependencyPropertyAndInstanceAndValueChanging()
    {
        LogCall();
    }

    private void OnImplicitStaticValidateDependencyPropertyAndInstanceAndValueChanged()
    {
        LogCall();
    }

    [DependencyProperty]
    public int ImplicitStaticValidateDependencyPropertyAndValue { get; set; }

    private static bool ValidateImplicitStaticValidateDependencyPropertyAndValue( DependencyProperty dependencyProperty, int value )
    {
        LogCall( $"{value}" );
        dependencyProperty.Should().BeSameAs( ImplicitStaticValidateDependencyPropertyAndValueProperty );

        return Validate( value );
    }

    private void OnImplicitStaticValidateDependencyPropertyAndValueChanging()
    {
        LogCall();
    }

    private void OnImplicitStaticValidateDependencyPropertyAndValueChanged()
    {
        LogCall();
    }

    [DependencyProperty]
    public int ImplicitStaticValidateValue { get; set; }

    private bool ValidateImplicitStaticValidateValue( int value )
    {
        LogCall( $"{value}" );

        return Validate( value );
    }

    private void OnImplicitStaticValidateValueChanging()
    {
        LogCall();
    }

    private void OnImplicitStaticValidateValueChanged()
    {
        LogCall();
    }

    #endregion
}