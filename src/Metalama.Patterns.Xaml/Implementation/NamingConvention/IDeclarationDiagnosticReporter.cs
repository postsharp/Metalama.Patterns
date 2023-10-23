// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Aspects;

namespace Metalama.Patterns.Xaml.Implementation.NamingConvention;

[CompileTime]
internal interface IDeclarationDiagnosticReporter
{
    void ReportAmbiguousDeclaration( INamingConvention namingConvention, in InspectedDeclaration inspectedDeclaration );

    void ReportInvalidDeclaration( INamingConvention namingConvention, in InspectedDeclaration inspectedDeclaration );

    void ReportNotFound( INamingConvention namingConvention, IEnumerable<string> candidateNames, IEnumerable<string> applicableCategories );
}