// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Aspects;
using Metalama.Framework.Code;
using Metalama.Patterns.Xaml.Implementation.NamingConvention;

namespace Metalama.Patterns.Xaml.Implementation.CommandNamingConvention;

[CompileTime]
internal static class CommandNamingConventionHelper
{
    public static CommandNamingConventionMatch Match<TMatchCanExecuteNamePredicate>(
        INamingConvention namingConvention,
        IMethod executeMethod,
        InspectedDeclarationsAdder inspectedDeclarations,
        string commandPropertyName,
        in TMatchCanExecuteNamePredicate matchCanExecuteNamePredicate,
        bool considerMethod = true,
        bool considerProperty = true,
        bool requireCanExecuteMatch = false )
        where TMatchCanExecuteNamePredicate : INameMatchPredicate
    {
        IMethod? selectedCandidateCanExecuteMethod = null;
        DeclarationMatchOutcome? canExecuteMethodMatchOutcome = null;

        if ( considerMethod )
        {
            (canExecuteMethodMatchOutcome, selectedCandidateCanExecuteMethod) =
                executeMethod.DeclaringType.Methods.FindValidMatchingDeclaration( matchCanExecuteNamePredicate, CommandAttribute.IsValidCanExecuteMethodDelegate, inspectedDeclarations );
        }

        IProperty? selectedCandidateCanExecuteProperty = null;
        DeclarationMatchOutcome? canExecutePropertyMatchOutcome = null;

        if ( considerProperty )
        {
            (canExecutePropertyMatchOutcome, selectedCandidateCanExecuteProperty) =
                executeMethod.DeclaringType.Properties.FindValidMatchingDeclaration( matchCanExecuteNamePredicate, CommandAttribute.IsValidCanExecutePropertyDelegate, inspectedDeclarations );
        }

        if ( canExecuteMethodMatchOutcome == DeclarationMatchOutcome.Success && canExecutePropertyMatchOutcome == DeclarationMatchOutcome.Success )
        {
            return new CommandNamingConventionMatch( namingConvention, commandPropertyName, DeclarationMatch<IMember>.Ambiguous(), requireCanExecuteMatch );
        }
        else if ( canExecuteMethodMatchOutcome == DeclarationMatchOutcome.Success )
        {
            return new CommandNamingConventionMatch( namingConvention, commandPropertyName, DeclarationMatch<IMember>.Success( selectedCandidateCanExecuteMethod! ), requireCanExecuteMatch );
        }
        else if ( canExecutePropertyMatchOutcome == DeclarationMatchOutcome.Success )
        {
            return new CommandNamingConventionMatch( namingConvention, commandPropertyName, DeclarationMatch<IMember>.Success( selectedCandidateCanExecuteProperty! ), requireCanExecuteMatch );
        }
        else if ( canExecuteMethodMatchOutcome == DeclarationMatchOutcome.Ambiguous )
        {
            return new CommandNamingConventionMatch( namingConvention, commandPropertyName, DeclarationMatch<IMember>.Ambiguous(), requireCanExecuteMatch );
        }
        else if ( canExecuteMethodMatchOutcome == DeclarationMatchOutcome.Invalid || canExecutePropertyMatchOutcome == DeclarationMatchOutcome.Invalid )
        {
            return new CommandNamingConventionMatch( namingConvention, commandPropertyName, DeclarationMatch<IMember>.Invalid(), requireCanExecuteMatch );
        }
        else
        {
            return new CommandNamingConventionMatch( namingConvention, commandPropertyName, DeclarationMatch<IMember>.NotFound( matchCanExecuteNamePredicate ), requireCanExecuteMatch );
        }
    }
}