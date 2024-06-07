// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Aspects;

namespace Metalama.Patterns.Xaml.Implementation.NamingConvention;

[CompileTime]
internal static class NamingConventionEvaluator
{
    public static INamingConventionEvaluationResult<TMatch> Evaluate<TArguments, TMatch>(
        IEnumerable<INamingConvention<TArguments, TMatch>> namingConventions,
        TArguments arguments )
        where TMatch : NamingConventionMatch
    {
        var e = new Evaluator<TArguments, TMatch>();

        foreach ( var nc in namingConventions )
        {
            if ( e.Evaluate( nc, arguments ) )
            {
                break;
            }
        }

        e.Finish();

        return e;
    }

    public static INamingConventionEvaluationResult<TMatch> Evaluate<TArguments, TMatch>(
        INamingConvention<TArguments, TMatch> namingConvention,
        TArguments arguments )
        where TMatch : NamingConventionMatch
    {
        var e = new Evaluator<TArguments, TMatch>();
        e.Evaluate( namingConvention, arguments );
        e.Finish();

        return e;
    }

    [CompileTime]
    private sealed class Evaluator<TArguments, TMatch> : INamingConventionEvaluationResult<TMatch>
        where TMatch : NamingConventionMatch
    {
        private List<InspectedMember> _inspectedDeclarations = new();
        private List<(TMatch Match, int InspectedDeclarationsStartIndex, int InspectedDeclarationsEndIndex)>? _unsuccessfulMatchDetails;

        public bool Success => this.SuccessfulMatch != null;

        public InspectedNamingConventionMatch<TMatch>? SuccessfulMatch { get; private set; }

        public IEnumerable<InspectedNamingConventionMatch<TMatch>>? UnsuccessfulMatches { get; private set; }

        public bool Evaluate( INamingConvention<TArguments, TMatch> namingConvention, in TArguments arguments )
        {
            var firstInspectedIndex = this._inspectedDeclarations.Count;

            var match = namingConvention.Match( arguments, this._inspectedDeclarations.Add );

            if ( match.Success )
            {
                this.SuccessfulMatch = new InspectedNamingConventionMatch<TMatch>(
                    match,
                    this.GetInspectedDeclarationsSlice( firstInspectedIndex, this._inspectedDeclarations.Count, true ) );

                return true;
            }
            else
            {
                (this._unsuccessfulMatchDetails ??= new List<(TMatch Match, int InspectedDeclarationsStartIndex, int InspectedDeclarationsEndIndex)>()).Add(
                    (match, firstInspectedIndex, this._inspectedDeclarations.Count) );

                return false;
            }
        }

        private IEnumerable<InspectedMember> GetInspectedDeclarationsSlice( int startIndex, int endIndex, bool isSuccess )
        {
            if ( startIndex == endIndex )
            {
                return Array.Empty<InspectedMember>();
            }

            if ( isSuccess && startIndex == 0 && endIndex == this._inspectedDeclarations.Count )
            {
                return this._inspectedDeclarations;
            }

            return this._inspectedDeclarations.Skip( startIndex ).Take( endIndex - startIndex );
        }

        public void Finish()
        {
            if ( this.Success )
            {
                this._unsuccessfulMatchDetails = null;
                this._inspectedDeclarations = null!;

                return;
            }

            if ( this._unsuccessfulMatchDetails != null )
            {
                this.UnsuccessfulMatches =
                    this._unsuccessfulMatchDetails
                        .Select(
                            m => new InspectedNamingConventionMatch<TMatch>(
                                m.Match,
                                this.GetInspectedDeclarationsSlice( m.InspectedDeclarationsStartIndex, m.InspectedDeclarationsEndIndex, false ) ) );
            }
        }
    }
}