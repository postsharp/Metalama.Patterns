// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Fabrics;
using Metalama.Patterns.Xaml.Options;

namespace Metalama.Patterns.Xaml.AspectTests.CommandTests.Diagnostics.MultipleNamingConventionsNoMatch;

internal class NsFabric : NamespaceFabric
{
    public override void AmendNamespace( INamespaceAmender amender )
    {
        amender.Outbound.ConfigureCommand(
            b =>
            {
                b.ApplyRegexNamingConvention(
                    "rx1-key",
                    "rx1-name",
                    "^Execute(?<" + CommandOptionsBuilder.Names.CommandNameGroup + ">.+)$",
                    CommandOptionsBuilder.Names.CommandNameToken + "CommandRx1",
                    "CanRx1" + CommandOptionsBuilder.Names.CommandNameToken,
                    1 );

                b.ApplyRegexNamingConvention(
                    "rx2-key",
                    "rx2-name",
                    "^Execute(?<" + CommandOptionsBuilder.Names.CommandNameGroup + ">.+)$",
                    CommandOptionsBuilder.Names.CommandNameToken + "CommandRx2",
                    "CanRx2" + CommandOptionsBuilder.Names.CommandNameToken,
                    2 );

                b.ApplyRegexNamingConvention(
                    "rx3-key",
                    "rx3-name",
                    "^Execute(?<" + CommandOptionsBuilder.Names.CommandNameGroup + ">.+)$",
                    CommandOptionsBuilder.Names.CommandNameToken + "CommandRx3",
                    "(CanRx3" + CommandOptionsBuilder.Names.CommandNameToken + ")|(CanRx3" + CommandOptionsBuilder.Names.CommandNameToken + "Property)",
                    3 );

                b.RemoveNamingConvention( CommandOptionsBuilder.DefaultNamingConventionKey );
            } );
    }
}

// <target>
internal class MultipleNamingConventionsNoMatch
{
    [Command]
    private void ExecuteFoo() { }

    // Convention `rx1-name` => ambiguous methods
    private bool CanRx1Foo() => true;

    private bool CanRx1Foo( int v ) => true;

    // Convention `rx2-name` => invalid methods
    private void CanRx2Foo() { }

    private void CanRx2Foo( int v ) { }

    // Convention `rx3-name` => ambiguous method or property
    private bool CanRx3Foo() => true;

    private bool CanRx3FooProperty => true;
}