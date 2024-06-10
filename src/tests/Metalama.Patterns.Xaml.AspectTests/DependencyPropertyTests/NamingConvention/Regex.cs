﻿// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Fabrics;
using Metalama.Patterns.Xaml.Configuration;
using System.Windows;

namespace Metalama.Patterns.Xaml.AspectTests.DependencyPropertyTests.NamingConvention.Regex;

internal class NsFabric : NamespaceFabric
{
    public override void AmendNamespace( INamespaceAmender amender )
    {
        amender.ConfigureDependencyProperty(
            b => b.AddNamingConvention(
                new DependencyPropertyNamingConvention( "rx1" )
                {
                    PropertyNamePattern = "^Yoda(?<Name>.+)$",
                    OnPropertyChangingPattern = "^(Do|Make){Name}Changing$",
                    OnPropertyChangedPattern = "^(Do|Make){Name}Changed$",
                    ValidatePattern = "^Is{Name}Valid",
                    RegistrationFieldName = "The{Name}PropertyItIs"
                } ) );
    }
}

// <target>
internal class Regex : DependencyObject
{
    [DependencyProperty]
    public int Foo { get; set; }

    private void OnFooChanging() { }

    private void OnFooChanged() { }

    private void ValidateFoo( int v ) { }

    [DependencyProperty]
    public string YodaFoo { get; set; }

    private void DoFooChanging() { }

    private void MakeFooChanged( string a, string b ) { }

    private void IsFooValid( string s ) { }
}