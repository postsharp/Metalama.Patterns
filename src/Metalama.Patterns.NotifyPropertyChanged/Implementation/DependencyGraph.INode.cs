// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Aspects;
using Metalama.Framework.Code;
using Microsoft.CodeAnalysis;

namespace Metalama.Patterns.NotifyPropertyChanged.Implementation;

internal static partial class DependencyGraph
{
    [CompileTime]
    public interface INode
    {
        INode GetOrAddChild( ISymbol childSymbol, ICompilation compilation );
        
        void AddReferencedBy( INode node );
    }
}