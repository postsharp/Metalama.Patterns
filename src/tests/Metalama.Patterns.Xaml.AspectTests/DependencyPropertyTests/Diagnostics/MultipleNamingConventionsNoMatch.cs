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
                b.ApplyRegexNamingConvention(
                    "rx1-key",
                    "rx1-name",
                    null,
                    null,
                    DependencyPropertyOptionsBuilder.Names.NameToken + "Rx1Ing",
                    DependencyPropertyOptionsBuilder.Names.NameToken + "Rx1Ed",
                    null,
                    1 );

                b.ApplyRegexNamingConvention(
                    "rx2-key",
                    "rx2-name",
                    null,
                    null,
                    DependencyPropertyOptionsBuilder.Names.NameToken + "Rx2Ing",
                    null,
                    null,
                    2 );

                b.ApplyRegexNamingConvention(
                    "rx3-key",
                    "rx3-name",
                    null,
                    null,
                    DependencyPropertyOptionsBuilder.Names.NameToken + "Rx3Ing",
                    null,
                    null,
                    3 );

                b.RemoveNamingConvention( CommandOptionsBuilder.DefaultNamingConventionKey );
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