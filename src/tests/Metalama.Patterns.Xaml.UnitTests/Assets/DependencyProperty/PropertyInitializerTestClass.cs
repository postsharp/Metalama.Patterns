﻿// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using System.Windows;

namespace Metalama.Patterns.Xaml.UnitTests.Assets.DependencyPropertyNS;

#pragma warning disable LAMA5206

public sealed partial class PropertyInitializerTestClass : DependencyObject
{
    public static int DefaultConfigurationInitializerCallCount { get; private set; }

    private static int DefaultConfigurationInitializer()
    {
        ++DefaultConfigurationInitializerCallCount;

        return 42;
    }

    // ReSharper disable once AutoPropertyCanBeMadeGetOnly.Global
    [DependencyProperty]
    public int DefaultConfiguration { get; set; } = DefaultConfigurationInitializer();

    public static int NotDefaultNotInitialInitializerCallCount { get; private set; }

    private static int NotDefaultNotInitialInitializer()
    {
        ++NotDefaultNotInitialInitializerCallCount;

        return 42;
    }

    [DependencyProperty( InitializerProvidesDefaultValue = false, InitializerProvidesInitialValue = false )]

    // ReSharper disable once AutoPropertyCanBeMadeGetOnly.Global
#pragma warning disable LAMA5200 // Initializer will not be used.
    public int NotDefaultNotInitial { get; set; } = NotDefaultNotInitialInitializer();
#pragma warning restore LAMA5200 // Initializer will not be used.

    public static int InitialOnlyInitializerCallCount { get; private set; }

    private static int InitialOnlyInitializer()
    {
        ++InitialOnlyInitializerCallCount;

        return 42;
    }

    // ReSharper disable once AutoPropertyCanBeMadeGetOnly.Global
    [DependencyProperty( InitializerProvidesDefaultValue = false, InitializerProvidesInitialValue = true )]
    public int InitialOnly { get; set; } = InitialOnlyInitializer();

    public static int DefaultAndInitialInitializerCallCount { get; private set; }

    private static int DefaultAndInitialInitializer()
    {
        ++DefaultAndInitialInitializerCallCount;

        return 42;
    }

    // ReSharper disable once AutoPropertyCanBeMadeGetOnly.Global
    [DependencyProperty( InitializerProvidesDefaultValue = true, InitializerProvidesInitialValue = true )]
    public int DefaultAndInitial { get; set; } = DefaultAndInitialInitializer();
}