// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Fabrics;
using Metalama.Patterns.Observability.Options;

namespace Metalama.Patterns.Observability.AspectTests.ConfigureDiagnosticCommentsByFabric;

public class Fabric : NamespaceFabric
{
    public override void AmendNamespace( INamespaceAmender amender )
    {
        amender.Outbound.ConfigureObservability( b => b.DiagnosticCommentVerbosity = 1 );
    }
}

// <target>
[Observable]
public class ConfigureDiagnosticCommentsByFabric { }