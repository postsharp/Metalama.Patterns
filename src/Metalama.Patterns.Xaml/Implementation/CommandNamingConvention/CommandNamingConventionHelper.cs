// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Aspects;
using Metalama.Framework.Code;
using Metalama.Patterns.Xaml.Implementation.NamingConvention;

namespace Metalama.Patterns.Xaml.Implementation.CommandNamingConvention;

[CompileTime]
internal static class CommandNamingConventionHelper
{
    public static CommandNamingConventionMatch Match<TContextImpl, TCanExecutePredicate>(
        INamingConvention namingConvention,
        IMethod executeMethod,
        in TContextImpl context,
        string commandPropertyName,
        in TCanExecutePredicate canExecutePredicate,
        bool considerMethod = true,
        bool considerProperty = true,
        bool requireCanExecuteMatch = false )
        where TContextImpl : ICommandNamingMatchContext
        where TCanExecutePredicate : INameMatchPredicate
    {
        IMethod? selectedCandidateCanExecuteMethod = null;
        DeclarationMatchOutcome? canExecuteMethodMatchOutcome = null;

        if ( considerMethod )
        {
            List<IMethod>? candidateCanExecuteMethods = null;

            foreach ( var method in executeMethod.DeclaringType.Methods )
            {
                if ( canExecutePredicate.IsMatch( method.Name ) )
                {
                    (candidateCanExecuteMethods ??= new()).Add( method );
                }
            }

            List<IMethod>? eligibleCanExecuteMethods = null;

            if ( candidateCanExecuteMethods is { Count: > 0 } )
            {
                foreach ( var method in candidateCanExecuteMethods )
                {
                    if ( context.IsValidCanExecuteMethod( method ) )
                    {
                        (eligibleCanExecuteMethods ??= new()).Add( method );
                    }
                }
            }

            canExecuteMethodMatchOutcome = DeclarationMatchOutcome.NotFound;

            switch ( eligibleCanExecuteMethods?.Count ?? 0 )
            {
                case 0:
                    if ( candidateCanExecuteMethods is { Count: > 0 } )
                    {
                        canExecuteMethodMatchOutcome = DeclarationMatchOutcome.Invalid;
                    }
                    break;

                case 1:
                    selectedCandidateCanExecuteMethod = eligibleCanExecuteMethods![0];
                    canExecuteMethodMatchOutcome = DeclarationMatchOutcome.Success;
                    break;

                case > 1:
                    canExecuteMethodMatchOutcome = DeclarationMatchOutcome.Ambiguous;
                    break;
            }
        }

        IProperty? selectedCandidateCanExecuteProperty = null;
        DeclarationMatchOutcome? canExecutePropertyMatchOutcome = null;

        if ( considerProperty )
        {
            List<IProperty>? candiateCanExecuteProperties = null;

            foreach ( var property in executeMethod.DeclaringType.Properties )
            {
                if ( canExecutePredicate.IsMatch( property.Name ) )
                {
                    (candiateCanExecuteProperties ??= new()).Add( property );
                }
            }

            List<IProperty>? eligibleCanExecuteProperties = null;

            if ( candiateCanExecuteProperties is { Count: > 0 } )
            {
                foreach ( var property in candiateCanExecuteProperties )
                {
                    if ( context.IsValidCanExecuteProperty( property ) )
                    {
                        (eligibleCanExecuteProperties ??= new()).Add( property );
                    }
                }
            }

            canExecutePropertyMatchOutcome = DeclarationMatchOutcome.NotFound;

            switch ( eligibleCanExecuteProperties?.Count ?? 0 )
            {
                case 0:
                    if ( candiateCanExecuteProperties is { Count: > 0 } )
                    {
                        canExecutePropertyMatchOutcome = DeclarationMatchOutcome.Invalid;
                    }
                    break;

                case 1:
                    selectedCandidateCanExecuteProperty = eligibleCanExecuteProperties![0];
                    canExecutePropertyMatchOutcome = DeclarationMatchOutcome.Success;
                    break;

                case > 1:
                    canExecutePropertyMatchOutcome = DeclarationMatchOutcome.Ambiguous;
                    break;
            }
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
            return new CommandNamingConventionMatch( namingConvention, commandPropertyName, DeclarationMatch<IMember>.NotFound( canExecutePredicate ), requireCanExecuteMatch );
        }
    }
}