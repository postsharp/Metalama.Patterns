// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

// To avoid confusion due to some identical type names, this file is concerned only with Roslyn types.
// Do not add usings for the namespaces Metalama.Framework.Code, Metalama.Framework.Engine.CodeModel or
// related namespaces.

using Metalama.Framework.Aspects;

namespace Metalama.Patterns.NotifyPropertyChanged.Implementation.DependencyAnalysis;

internal static partial class DependencyGraph
{
    [CompileTime]
    private sealed class ForkedGatherIdentifiersContext : GatherIdentifiersContext, IDisposable
    {
        private readonly ForkItem _forkItem;
        private readonly RootGatherIdentifiersContext _rootContext;

        public ForkedGatherIdentifiersContext( RootGatherIdentifiersContext rootContext, IReadOnlyCollection<SymbolRecord>? parentSymbols, ForkItem forkItem )
            : base( parentSymbols )
        {
            this._rootContext = rootContext;            
            this._forkItem = forkItem;
        }

        public override bool IsRoot => false;

        private void PopAndJoin()
        {
            this.ThrowIfJoined();

            if ( this.HasUnjoinedForks )
            {
                throw new InvalidOperationException( "The current " + nameof( GatherIdentifiersContext ) + " has unjoined forks, so canot be joined." );
            }

            this.RootContext.Manager.Pop( this );
            this._forkItem.IsJoined = true;
        }

        protected override RootGatherIdentifiersContext RootContext => this._rootContext;

        protected override void ThrowIfJoined()
        {
            if ( this._forkItem?.IsJoined == true )
            {
                throw new NotSupportedException( "The " + nameof( GatherIdentifiersContext ) + " fork has already been rejoined to its parent and cannot be modified directly." );
            }
        }

        public override void Dispose()
        {
            this.PopAndJoin();
        }
    }
}