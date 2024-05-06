﻿// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Aspects;
using Metalama.Framework.Code;

namespace Metalama.Patterns.Xaml.Implementation.NamingConvention;

[CompileTime]
internal interface IDiagnosticReporter
{
    void ReportNoNamingConventionMatched( IEnumerable<INamingConvention> namingConventionsTried );

    void ReportNoConfiguredNamingConventions();

    void ReportAmbiguousDeclaration( INamingConvention namingConvention, in InspectedMember inspectedMember, bool isRequired );

    void ReportInvalidDeclaration( INamingConvention namingConvention, in InspectedMember inspectedMember, bool isRequired );

    void ReportConflictingDeclaration(
        INamingConvention namingConvention,
        IDeclaration conflictingDeclaration,
        IEnumerable<string> applicableCategories,
        bool isRequired );

    void ReportDeclarationNotFound(
        INamingConvention namingConvention,
        IEnumerable<string> candidateNames,
        IEnumerable<string> applicableCategories,
        bool isRequired );
}