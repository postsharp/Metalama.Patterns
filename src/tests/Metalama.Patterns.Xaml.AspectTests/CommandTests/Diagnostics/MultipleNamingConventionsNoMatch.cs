// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Fabrics;
using Metalama.Patterns.Xaml.Configuration;

namespace Metalama.Patterns.Xaml.AspectTests.CommandTests.Diagnostics.MultipleNamingConventionsNoMatch;

internal class NsFabric : NamespaceFabric
{
    public override void AmendNamespace( INamespaceAmender amender )
    {
        amender.ConfigureCommand(
            b =>
            {
                b.AddNamingConvention(
                    new CommandNamingConvention( "rx1" )
                    {
                        CommandNamePattern = "^Execute(?<CommandName>.+)$",
                        CommandPropertyName = "{CommandName}CommandRx1",
                        CanExecutePattern = ["CanRx1{CommandName}"]
                    } );

                b.AddNamingConvention(
                    new CommandNamingConvention( "rx2" )
                    {
                        CommandNamePattern = "^Execute(?<CommandName>.+)$",
                        CommandPropertyName = "{CommandName}CommandRx2",
                        CanExecutePattern = ["CanRx2{CommandName}"]
                    } );

                b.AddNamingConvention(
                    new CommandNamingConvention( "rx3" )
                    {
                        CommandNamePattern = "^Execute(?<CommandName>.+)$",
                        CommandPropertyName = "{CommandName}CommandRx3",
                        CanExecutePattern = ["CanRx3{CommandName}"]
                    } );

                b.RemoveNamingConvention( CommandOptionsBuilder.DefaultNamingConventionName );
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