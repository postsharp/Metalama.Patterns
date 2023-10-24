// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Aspects;
using Metalama.Framework.Code;

namespace Metalama.Patterns.Xaml.Implementation.CommandNamingConvention;

[CompileTime]
internal interface ICommandNamingMatchContext
{
    bool IsValidCanExecuteMethod( IMethod method );

    bool IsValidCanExecuteProperty( IProperty property );
}