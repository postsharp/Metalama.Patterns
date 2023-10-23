// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Aspects;
using Metalama.Framework.Code;

namespace Metalama.Patterns.Xaml.Implementation.NamingConvention;

[CompileTime]
public interface IDeclarationMatchVisitor
{
    void Visit<TDeclaration>( in DeclarationMatch<TDeclaration> match, bool isRequired, IReadOnlyList<string> applicableCategories )
        where TDeclaration : class, IDeclaration;
}