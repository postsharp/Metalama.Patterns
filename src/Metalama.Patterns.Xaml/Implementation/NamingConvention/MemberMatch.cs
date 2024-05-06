// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Aspects;
using Metalama.Framework.Code;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;

namespace Metalama.Patterns.Xaml.Implementation.NamingConvention;

/// <summary>
/// Describes the naming convention match associated with a particular declaration.
/// </summary>
/// <remarks>
/// A single <see cref="NamingConventionMatch"/> may contain several <see cref="MemberMatch{TDeclaration}"/> properties,
/// one for each category of declaration that needs to be matched, such as "can execute" and "validate".
/// </remarks>
/// <typeparam name="TMember"></typeparam>
[CompileTime]
internal sealed class MemberMatch<TMember, TKind> : IMemberMatch
    where TMember : class, IMemberOrNamedType
    where TKind : struct
{
    public static MemberMatch<TMember, TKind> Success( TMember declaration, TKind kind )
        => new( MemberMatchOutcome.Success, declaration ?? throw new ArgumentNullException( nameof(declaration) ), kind: kind );

    public static MemberMatch<TMember, TKind> Success( TKind kind ) => new( MemberMatchOutcome.Success, null, kind: kind );

    public static MemberMatch<TMember, TKind> Conflict( TMember conflictingDeclaration ) => new( MemberMatchOutcome.Conflict, conflictingDeclaration );

    public static MemberMatch<TMember, TKind> Ambiguous() => new( MemberMatchOutcome.Ambiguous );

    public static MemberMatch<TMember, TKind> NotFound( ImmutableArray<string> candidateNames )
        => new( MemberMatchOutcome.NotFound, candidateNames: candidateNames );

    public static MemberMatch<TMember, TKind> NotFound( string candidateName )
        => new( MemberMatchOutcome.NotFound, candidateNames: ImmutableArray.Create( candidateName ) );

    public static MemberMatch<TMember, TKind> NotFound() => new( MemberMatchOutcome.NotFound, candidateNames: ImmutableArray<string>.Empty );

    public static MemberMatch<TMember, TKind> Invalid() => new( MemberMatchOutcome.Invalid );

    private MemberMatch( MemberMatchOutcome? outcome, TMember? member = null, ImmutableArray<string> candidateNames = default, TKind kind = default )
    {
        this.Member = member;
        this.Kind = kind;
        this.Outcome = outcome;
        this.CandidateNames = candidateNames;
    }

    public MemberMatchOutcome? Outcome { get; }

    public TMember? Member { get; }

    public TKind Kind { get; }

    IMemberOrNamedType? IMemberMatch.Member => this.Member;

    [MemberNotNullWhen( true, nameof(CandidateNames) )]
    public bool HasCandidateNames => this.CandidateNames != null;

    public ImmutableArray<string> CandidateNames { get; }

    public MemberMatch<TBaseDeclaration, TKind> Cast<TBaseDeclaration>()
        where TBaseDeclaration : class, IMemberOrNamedType
        => new( this.Outcome, (TBaseDeclaration?) (IDeclaration?) this.Member, this.CandidateNames );
}