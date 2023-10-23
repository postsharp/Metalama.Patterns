// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Aspects;

namespace Metalama.Patterns.Xaml.Implementation.NamingConvention;

[CompileTime]
internal static class NamingConventionEvaluator<TContext, TContextImplementation>
    where TContextImplementation : TContext
{
    public static INamingConventionEvaluationResult<TMatch> Evaluate<TArguments, TMatch, TMatchContextFactory>( IEnumerable<INamingConvention<TArguments, TContext, TMatch>> namingConventions, TArguments arguments, in TMatchContextFactory matchContextFactory )
        where TMatch : INamingConventionMatch
        where TMatchContextFactory : INamingConventionMatchContextFactory<TContextImplementation>
    {
        var e = new Evaluator<TArguments, TMatch, TMatchContextFactory>();
        
        foreach ( var nc in namingConventions )
        {
            if ( e.Evaluate( nc, arguments, matchContextFactory ) )
            {
                break;
            }
        }

        e.Finish();

        return e;
    }

    public static INamingConventionEvaluationResult<TMatch> Evaluate<TArguments, TMatch, TMatchContextFactory>( INamingConvention<TArguments, TContext, TMatch> namingConvention, TArguments arguments, in TMatchContextFactory matchContextFactory )
        where TMatch : INamingConventionMatch
        where TMatchContextFactory : INamingConventionMatchContextFactory<TContextImplementation>
    {
        var e = new Evaluator<TArguments, TMatch, TMatchContextFactory>();
        e.Evaluate( namingConvention, arguments, matchContextFactory );
        e.Finish();
        
        return e;
    }

    [CompileTime]
    private sealed class Evaluator<TArguments, TMatch, TMatchContextFactory> : INamingConventionEvaluationResult<TMatch>
        where TMatch : INamingConventionMatch
        where TMatchContextFactory : INamingConventionMatchContextFactory<TContextImplementation>
    {        
        private List<InspectedDeclaration> _inspectedDeclarations = new();
        private List<(INamingConvention<TArguments, TContext, TMatch> NamingConvention, TMatch Match, int InspectedDeclarationsStartIndex, int InspectedDeclarationsEndIndex)>? _unsuccessfulMatchDetails;

        public bool Success => this.SuccessfulMatch != null;

        public TMatch? SuccessfulMatch { get; private set; }

        public IEnumerable<UnsuccesfulNamingConventionMatch<TMatch>>? UnsuccessfulMatches { get; private set;  }

        public bool Evaluate( INamingConvention<TArguments,TContext,TMatch> namingConvention, in TArguments arguments, in TMatchContextFactory matchContextFactory )
        {
            var firstInspectedIndex = this._inspectedDeclarations.Count;

            var nameMatchingContext = matchContextFactory.Create( new InspectedDeclarationsAdder( this._inspectedDeclarations ) );

            var match = namingConvention.Match( arguments, nameMatchingContext );

            if ( match.Success )
            {
                this.SuccessfulMatch = match;
                return true;
            }
            else
            {
                (this._unsuccessfulMatchDetails ??= new()).Add( (namingConvention, match, firstInspectedIndex, this._inspectedDeclarations.Count) );
                return false;
            }
        }

        public void Finish()
        {
            if ( this.Success )
            {
                // If finally successful, discard failure details.

                this._unsuccessfulMatchDetails = null;
                this._inspectedDeclarations = null!;

                return;
            }

            if ( this._unsuccessfulMatchDetails != null )
            {
                // Organize inspected declarations to be suitable for reporting diagnostics:

                this.UnsuccessfulMatches =
                    this._unsuccessfulMatchDetails
                    .Select( m => new UnsuccesfulNamingConventionMatch<TMatch>( /* m.NamingConvention, */ m.Match, m.InspectedDeclarationsStartIndex == m.InspectedDeclarationsEndIndex ? Array.Empty<InspectedDeclaration>() : this._inspectedDeclarations.Skip( m.InspectedDeclarationsStartIndex ).Take( m.InspectedDeclarationsEndIndex - m.InspectedDeclarationsStartIndex ) ) );
            }
        }
    }
}