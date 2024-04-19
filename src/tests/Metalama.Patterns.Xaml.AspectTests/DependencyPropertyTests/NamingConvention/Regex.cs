// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Fabrics;
using Metalama.Patterns.Xaml.Options;
using System.Windows;

namespace Metalama.Patterns.Xaml.AspectTests.DependencyPropertyTests.NamingConvention.Regex;

internal class NsFabric : NamespaceFabric
{
    public override void AmendNamespace( INamespaceAmender amender )
    {
        amender.ConfigureDependencyProperty(
            b => b.ApplyRegexNamingConvention(
                "rx1-key",
                "rx1-name",
                "^Yoda(?<Name>.+)$",
                "The$Name$PropertyItIs",
                "^(Do|Make)$Name$Changing$",
                "^(Do|Make)$Name$Changed$",
                "^Is$Name$Valid" ) );
    }
}

// <target>
internal class Regex : DependencyObject
{
    [DependencyProperty]
    public int Foo { get; set; }

    private void OnFooChanging() { }

    private void OnFooChanged() { }

    private bool ValidateFoo( int v ) => true;

    [DependencyProperty]
    public string YodaFoo { get; set; }

    private void DoFooChanging() { }

    private void MakeFooChanged( string a, string b ) { }

    private bool IsFooValid( string s ) => true;
}