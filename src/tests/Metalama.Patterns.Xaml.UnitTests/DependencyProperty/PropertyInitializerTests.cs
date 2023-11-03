// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using FluentAssertions;
using Metalama.Patterns.Xaml.UnitTests.Assets.DependencyPropertyNS;
using Xunit;

// ReSharper disable once CheckNamespace
namespace Metalama.Patterns.Xaml.UnitTests.DependencyPropertyNS;

public sealed class PropertyInitializerTests
{
    [Fact]
    public void DefaultConfiguration()
    {
        PropertyInitializerTestClass.DefaultConfigurationProperty.DefaultMetadata.DefaultValue
            .Should()
            .Be( 42 );

        PropertyInitializerTestClass.DefaultConfigurationInitializerCallCount.Should().Be( 1 );
        var instance = new PropertyInitializerTestClass();
        PropertyInitializerTestClass.DefaultConfigurationInitializerCallCount.Should().Be( 1 );
        instance.DefaultConfiguration.Should().Be( 42 );
    }

    [Fact]
    public void NotDefaultNotInitial()
    {
        PropertyInitializerTestClass.NotDefaultNotInitialProperty.DefaultMetadata.DefaultValue
            .Should()
            .Be( 0 );

        PropertyInitializerTestClass.NotDefaultNotInitialInitializerCallCount.Should().Be( 0 );
        var instance = new PropertyInitializerTestClass();
        PropertyInitializerTestClass.NotDefaultNotInitialInitializerCallCount.Should().Be( 0 );
        instance.NotDefaultNotInitial.Should().Be( 0 );
    }

    [Fact]
    public void InitialOnly()
    {
        PropertyInitializerTestClass.InitialOnlyProperty.DefaultMetadata.DefaultValue.Should().Be( 0 );
        var callCount = PropertyInitializerTestClass.InitialOnlyInitializerCallCount;
        var instance = new PropertyInitializerTestClass();
        PropertyInitializerTestClass.InitialOnlyInitializerCallCount.Should().Be( callCount + 1 );
        instance.InitialOnly.Should().Be( 42 );
    }

    [Fact]
    public void DefaultAndInitial()
    {
        PropertyInitializerTestClass.DefaultAndInitialProperty.DefaultMetadata.DefaultValue.Should().Be( 42 );
        var callCount = PropertyInitializerTestClass.DefaultAndInitialInitializerCallCount;
        var instance = new PropertyInitializerTestClass();
        PropertyInitializerTestClass.DefaultAndInitialInitializerCallCount.Should().Be( callCount + 1 );
        instance.DefaultAndInitial.Should().Be( 42 );
    }
}