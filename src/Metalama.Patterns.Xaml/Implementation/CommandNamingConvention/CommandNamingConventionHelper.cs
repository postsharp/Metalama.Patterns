// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Aspects;
using Metalama.Framework.Code;
using Metalama.Patterns.Xaml.Implementation.NamingConvention;

namespace Metalama.Patterns.Xaml.Implementation.CommandNamingConvention;

[CompileTime]
internal static class CommandNamingConventionHelper
{
    public static CommandNamingConventionMatch Match<TContextImpl>(
        INamingConvention namingConvention,
        IMethod executeMethod,
        TContextImpl context,
        string commandPropertyName,
        string canExecuteName,
        bool considerMethod = true,
        bool considerProperty = true )
        where TContextImpl : ICommandNamingMatchContext
    {
        IMethod? selectedCandidateCanExecuteMethod = null;
        DeclarationMatchOutcome? canExecuteMethodMatchOutcome = null;

        if ( considerMethod )
        {
            var candiateCanExecuteMethods = executeMethod.DeclaringType.Methods.OfName( canExecuteName ).ToList();

            var eligibleCanExecuteMethods = candiateCanExecuteMethods.Where( m => context.IsValidCanExecuteMethod( m ) ).ToList();

            canExecuteMethodMatchOutcome = DeclarationMatchOutcome.NotFound;

            switch ( eligibleCanExecuteMethods.Count )
            {
                case 0:
                    if ( candiateCanExecuteMethods.Count > 0 )
                    {
                        canExecuteMethodMatchOutcome = DeclarationMatchOutcome.Invalid;
                    }
                    break;

                case 1:
                    selectedCandidateCanExecuteMethod = eligibleCanExecuteMethods[0];
                    canExecuteMethodMatchOutcome = DeclarationMatchOutcome.Success;
                    break;

                case > 1:
                    canExecuteMethodMatchOutcome = DeclarationMatchOutcome.Ambiguous;
                    break;
            }
        }

        IProperty? candiateCanExecuteProperty = null;
        DeclarationMatchOutcome? canExecutePropertyMatchOutcome = null;

        if ( considerProperty )
        {
            candiateCanExecuteProperty = executeMethod.DeclaringType.Properties.OfName( canExecuteName ).SingleOrDefault();

            canExecutePropertyMatchOutcome = candiateCanExecuteProperty != null && context.IsValidCanExecuteProperty( candiateCanExecuteProperty )
                ? DeclarationMatchOutcome.Success
                : DeclarationMatchOutcome.NotFound;
        }
        
        if ( canExecuteMethodMatchOutcome == DeclarationMatchOutcome.Success && canExecutePropertyMatchOutcome == DeclarationMatchOutcome.Success )
        {
            return new CommandNamingConventionMatch( namingConvention, commandPropertyName, DeclarationMatch<IMember>.Ambiguous( canExecuteName ) );
        }
        else if ( canExecuteMethodMatchOutcome == DeclarationMatchOutcome.Success )
        {
            return new CommandNamingConventionMatch( namingConvention, commandPropertyName, DeclarationMatch<IMember>.Success( selectedCandidateCanExecuteMethod! ) );
        }
        else if ( canExecutePropertyMatchOutcome == DeclarationMatchOutcome.Success )
        {
            return new CommandNamingConventionMatch( namingConvention, commandPropertyName, DeclarationMatch<IMember>.Success( candiateCanExecuteProperty! ) );
        }
        else if ( canExecuteMethodMatchOutcome == DeclarationMatchOutcome.Ambiguous )
        {
            return new CommandNamingConventionMatch( namingConvention, commandPropertyName, DeclarationMatch<IMember>.Ambiguous( canExecuteName ) );
        }
        else if ( canExecuteMethodMatchOutcome == DeclarationMatchOutcome.Invalid || canExecutePropertyMatchOutcome == DeclarationMatchOutcome.Invalid )
        {
            return new CommandNamingConventionMatch( namingConvention, commandPropertyName, DeclarationMatch<IMember>.Invalid() );
        }
        else
        {
            return new CommandNamingConventionMatch( namingConvention, commandPropertyName, DeclarationMatch<IMember>.NotFound( canExecuteName ) );
        }
    }
}