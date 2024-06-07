// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Fabrics;
using Metalama.Patterns.Xaml.Configuration;
using System.Windows;

namespace Metalama.Patterns.Xaml.AspectTests.DependencyPropertyTests.Diagnostics.MultipleNamingConventionsNoMatch;

internal class NsFabric : NamespaceFabric
{
    public override void AmendNamespace( INamespaceAmender amender )
    {
        amender.ConfigureDependencyProperty(
            b =>
            {
                b.AddNamingConvention(
                    new DependencyPropertyNamingConvention( "rx1" )
                    {
                        OnPropertyChangedPattern = "{Name}" + "Rx1Ed", OnPropertyChangingPattern = "{Name}" + "Rx1Ing"
                    } );

                b.AddNamingConvention(
                    new DependencyPropertyNamingConvention( "rx2" )
                    {
                        OnPropertyChangedPattern = "{Name}" + "Rx2Ed", OnPropertyChangingPattern = "{Name}" + "Rx2Ing"
                    } );

                b.AddNamingConvention(
                    new DependencyPropertyNamingConvention( "rx3" )
                    {
                        OnPropertyChangedPattern = "{Name}" + "Rx3Ed", OnPropertyChangingPattern = "{Name}" + "Rx3Ing"
                    } );

                b.RemoveNamingConvention( CommandOptionsBuilder.DefaultNamingConventionName );
            } );
    }
}

// <target>
internal class MultipleNamingConventionsNoMatch : DependencyObject
{
    [DependencyProperty]
    public int Foo { get; set; }

    // Convention `rx1-name` => changing ambiguous, changed ok

    private void FooRx1Ing() { }

    private void FooRx1Ing( int v ) { }

    private void FooRx1Ed() { }

    // Convention `rx2-name` => changing invalid

    private string? FooRx2Ing( List<int> v ) => null;

    // Convention `rx3-name` => changing missing
}