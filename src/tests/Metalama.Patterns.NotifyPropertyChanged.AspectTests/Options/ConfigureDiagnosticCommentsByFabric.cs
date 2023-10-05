// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Fabrics;
using Metalama.Patterns.NotifyPropertyChanged.Options;

namespace Metalama.Patterns.NotifyPropertyChanged.AspectTests.ConfigureDiagnosticCommentsByFabric;

public class Fabric : NamespaceFabric
{
    public override void AmendNamespace( INamespaceAmender amender )
    {
        amender.Outbound.ConfigureNotifyPropertyChanged( b => b.DiagnosticCommentVerbosity = 1 );
    }
}

// <target>
[NotifyPropertyChanged]
public class ConfigureDiagnosticCommentsByFabric { }