// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Aspects;
using Metalama.Framework.Code;
using System.Diagnostics.CodeAnalysis;

namespace Metalama.Patterns.Xaml.Implementation.NamingConvention;

/// <summary>
/// Describes the naming convention match associated with a particular declaration.
/// </summary>
/// <remarks>
/// A single <see cref="NamingConventionMatch"/> may contain several <see cref="MemberMatch{TDeclaration}"/> properties,
/// one for each category of declaration that needs to be matched, such as "can execute" and "validate".
/// </remarks>
/// <typeparam name="TDeclaration"></typeparam>
[CompileTime]
internal class MemberMatch<TDeclaration> : IMemberMatch
    where TDeclaration : class, IMemberOrNamedType
{
    public static MemberMatch<TDeclaration> Success( TDeclaration declaration )
        => new( MemberMatchOutcome.Success, declaration ?? throw new ArgumentNullException( nameof(declaration) ) );

    public static MemberMatch<TDeclaration> SuccessOrConflict( TDeclaration? conflictingDeclaration )
        => new( conflictingDeclaration == null ? MemberMatchOutcome.Success : MemberMatchOutcome.Conflict, conflictingDeclaration );

    public static MemberMatch<TDeclaration> Ambiguous() => new( MemberMatchOutcome.Ambiguous );

    public static MemberMatch<TDeclaration> NotFound<TNameMatchPredicate>( in TNameMatchPredicate predicate )
        where TNameMatchPredicate : INameMatchPredicate
    {
        predicate.GetCandidateNames( out var singleValue, out var collection );

        return new MemberMatch<TDeclaration>( MemberMatchOutcome.NotFound, candidateNames: (object?) collection ?? singleValue );
    }

    public static MemberMatch<TDeclaration> NotFound( string candidateName ) => new( MemberMatchOutcome.NotFound, candidateNames: candidateName );

    public static MemberMatch<TDeclaration> NotFound( IEnumerable<string>? candidateNames = null )
        => new( MemberMatchOutcome.NotFound, candidateNames: candidateNames );

    public static MemberMatch<TDeclaration> Invalid() => new( MemberMatchOutcome.Invalid );

    public static MemberMatch<TDeclaration> FromOutcome( MemberMatchOutcome outcome, TDeclaration? declaration = null, string? candidateName = null )
        => new( outcome, declaration, candidateName );

    public static MemberMatch<TDeclaration> FromOutcome( MemberMatchOutcome outcome, TDeclaration? declaration, IEnumerable<string>? candidateNames )
        => new( outcome, declaration, candidateNames );

    private readonly object? _candidateNames;

    private MemberMatch( MemberMatchOutcome outcome, TDeclaration? member = null, object? candidateNames = null )
    {
        this.Member = member;
        this.Outcome = outcome;
        this._candidateNames = candidateNames;
    }

    private MemberMatch( MemberMatchOutcome? outcome, TDeclaration? member, object? candidateNames )
    {
        this.Member = member;
        this.Outcome = outcome;
        this._candidateNames = candidateNames;
    }

    public MemberMatchOutcome? Outcome { get; }

    public TDeclaration? Member { get; }

    IMemberOrNamedType? IMemberMatch.Member => this.Member;

    [MemberNotNullWhen( true, nameof(CandidateNames) )]
    public bool HasCandidateNames => this._candidateNames != null;

    public IEnumerable<string>? CandidateNames
        => this._candidateNames == null ? null : this._candidateNames as IEnumerable<string> ?? new[] { (string) this._candidateNames };

    public MemberMatch<TBaseDeclaration> Cast<TBaseDeclaration>()
        where TBaseDeclaration : class, IMemberOrNamedType
        => new( this.Outcome, (TBaseDeclaration?) (IDeclaration?) this.Member, this._candidateNames );
}