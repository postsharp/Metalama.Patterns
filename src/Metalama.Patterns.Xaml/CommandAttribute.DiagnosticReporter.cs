// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Aspects;
using Metalama.Framework.Code;
using Metalama.Patterns.Xaml.Implementation;
using Metalama.Patterns.Xaml.Implementation.NamingConvention;
using System.Net.NetworkInformation;
using System.Text;

namespace Metalama.Patterns.Xaml;

public sealed partial class CommandAttribute
{
    [CompileTime]
    private readonly struct DiagnosticReporter : IDeclarationDiagnosticReporter
    {
        public IAspectBuilder<IMethod> Builder { get; init; }

        public void ReportAmbiguousDeclaration( INamingConvention namingConvention, in InspectedDeclaration inspectedDeclaration )
        {
            this.Builder.Diagnostics.Report(
                Diagnostics.WarningValidCandidateDeclarationIsAmbiguous.WithArguments(
                    (
                    inspectedDeclaration.Declaration.DeclarationKind,
                    inspectedDeclaration.Category,
                    "[Command] method ",
                    this.Builder.Target,
                    namingConvention.DiagnosticName
                    ) ),
                inspectedDeclaration.Declaration );
        }

        public void ReportInvalidDeclaration( INamingConvention namingConvention, in InspectedDeclaration inspectedDeclaration )
        {
            this.Builder.Diagnostics.Report(
                Diagnostics.WarningInvalidCandidateDeclarationSignature.WithArguments(
                    (
                    inspectedDeclaration.Declaration.DeclarationKind,
                    inspectedDeclaration.Category,
                    "[Command] method ",
                    this.Builder.Target,
                    namingConvention.DiagnosticName,
                    inspectedDeclaration.Declaration.DeclarationKind == DeclarationKind.Property
                        ? " The property must be of type bool and have a getter."
                        : " The method must not be generic, must return bool and may optionally have a single parameter of any type, but which must not be a ref or out parameter."
                    ) ),
                inspectedDeclaration.Declaration );
        }

        public void ReportNotFound( INamingConvention namingConvention, IEnumerable<string> candidateNames, IEnumerable<string> applicableCategories )
        {
            this.Builder.Diagnostics.Report(
                Diagnostics.WarningCandidateNamesNotFound.WithArguments(
                    (
                        applicableCategories.PrettyList( " or " ),
                        namingConvention.DiagnosticName,
                        candidateNames.PrettyList( " or " )
                    ) ) );
        }
    }
}