// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Aspects;
using Metalama.Patterns.Xaml.Implementation.NamingConvention;

namespace Metalama.Patterns.Xaml.Implementation;

internal sealed partial class DependencyPropertyAspectBuilder
{
    [CompileTime]
    private sealed class DiagnosticReporter : BaseDiagnosticReporter
    {
        public DiagnosticReporter( IAspectBuilder builder ) : base( builder ) { }

        protected override string GetInvalidityReason( in InspectedMember inspectedMember ) => " Refer to documentation for supported method signatures.";

        protected override string GetTargetDeclarationDescription() => "[DependencyProperty] property ";
    }
}