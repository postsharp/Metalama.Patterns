// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Fabrics;
using Metalama.Patterns.Xaml.Options;

namespace Metalama.Patterns.Xaml.AspectTests.CommandTests.NamingConvention.Regex;

internal class NsFabric : NamespaceFabric
{
    public override void AmendNamespace( INamespaceAmender amender )
    {
        amender.Outbound.ConfigureCommand(
            b => b.ApplyRegexNamingConvention(
                "rx1-key",
                "rx1-name",
                "^MakeIt(?<" + CommandOptionsBuilder.Names.CommandNameGroup + ">.+)$",
                "The" + CommandOptionsBuilder.Names.CommandNameToken + "Command",
                "(CanIt" + CommandOptionsBuilder.Names.CommandNameToken + ")|(" + CommandOptionsBuilder.Names.CommandNameToken + "ItCan)" ) );
    }
}

// <target>
internal class Regex
{
    [Command]
    private void MakeItBeep() { }

    // Matches regex naming convention:    
    private bool CanItBeep() => true;

    // Not matched by regex naming convention, so should not be ambiguous:
    private bool CanExecuteBeep() => true;

    [Command]
    private void MakeItUseTheForce() { }

    // Also matches regex naming convention:    
    private bool UseTheForceItCan() => true;

    // Does not match regex naming convention, fallthrough to default naming convention:
    [Command]
    private void ExecuteFoo() { }

    private bool CanExecuteFoo() => true;

    // Not matched by default naming convention, so should not be ambiguous:
    private bool ItCanFoo() => true;
}