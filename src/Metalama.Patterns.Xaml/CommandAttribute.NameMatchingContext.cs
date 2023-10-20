// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Aspects;
using Metalama.Framework.Code;
using Metalama.Patterns.Xaml.Implementation.CommandNamingConvention;
using Metalama.Patterns.Xaml.Implementation.NamingConvention;

namespace Metalama.Patterns.Xaml;

public sealed partial class CommandAttribute
{
    [CompileTime]
    private readonly struct NameMatchingContext : ICommandNamingMatchContext
    {
        public InspectedDeclarationsAdder InspectedDeclarations { get; init; }

        public bool IsValidCanExecuteMethod( IMethod method )
        {
            var isValid = CommandAttribute.IsValidCanExecuteMethod( method );
            this.InspectedDeclarations.Add( method, isValid, _canExecuteMethodCategory );
            return isValid;
        }

        public bool IsValidCanExecuteProperty( IProperty property )
        {
            var isValid = CommandAttribute.IsValidCanExecuteProperty( property );
            this.InspectedDeclarations.Add( property, isValid, _canExecutePropertyCategory );
            return isValid;
        }
    }
}