// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using JetBrains.Annotations;
using Metalama.Framework.Aspects;
using Metalama.Framework.Code;
using System.Diagnostics.CodeAnalysis;

namespace Metalama.Patterns.Xaml.Implementation.NamingConvention;

/// <summary>
/// Describes the naming convention match associated with a particular declaration.
/// </summary>
/// <remarks>
/// A single <see cref="INamingConventionMatch"/> may contain several <see cref="DeclarationMatch{TDeclaration}"/> properties,
/// one for each category of declaration that needs to be matched, such as "can execute" and "validate".
/// </remarks>
/// <typeparam name="TDeclaration"></typeparam>
[CompileTime]
internal readonly struct DeclarationMatch<TDeclaration>
    where TDeclaration : class, IDeclaration
{
    public static DeclarationMatch<TDeclaration> Success( TDeclaration declaration )
        => new( DeclarationMatchOutcome.Success, declaration ?? throw new ArgumentNullException( nameof(declaration) ) );

    public static DeclarationMatch<TDeclaration> SuccessOrConflict( TDeclaration? conflictingDeclaration )
        => new( conflictingDeclaration == null ? DeclarationMatchOutcome.Success : DeclarationMatchOutcome.Conflict, conflictingDeclaration );

    public static DeclarationMatch<TDeclaration> Ambiguous() => new( DeclarationMatchOutcome.Ambiguous );

    public static DeclarationMatch<TDeclaration> NotFound<TNameMatchPredicate>( in TNameMatchPredicate predicate )
        where TNameMatchPredicate : INameMatchPredicate
    {
        predicate.GetCandidateNames( out var singleValue, out var collection );

        return new DeclarationMatch<TDeclaration>( DeclarationMatchOutcome.NotFound, candidateNames: (object?) collection ?? singleValue );
    }

    public static DeclarationMatch<TDeclaration> NotFound( string candidateName ) => new( DeclarationMatchOutcome.NotFound, candidateNames: candidateName );

    public static DeclarationMatch<TDeclaration> NotFound( IEnumerable<string>? candidateNames = null )
        => new( DeclarationMatchOutcome.NotFound, candidateNames: candidateNames );

    public static DeclarationMatch<TDeclaration> Invalid() => new( DeclarationMatchOutcome.Invalid );

    public static DeclarationMatch<TDeclaration> FromOutcome( DeclarationMatchOutcome outcome, TDeclaration? declaration = null, string? candidateName = null )
        => new( outcome, declaration, candidateName );

    public static DeclarationMatch<TDeclaration> FromOutcome( DeclarationMatchOutcome outcome, TDeclaration? declaration, IEnumerable<string>? candidateNames )
        => new( outcome, declaration, candidateNames );

    private readonly object? _candidateNames;

    private DeclarationMatch( DeclarationMatchOutcome outcome, TDeclaration? declaration = null, object? candidateNames = null )
    {
        this.Declaration = declaration;
        this.Outcome = outcome;
        this._candidateNames = candidateNames;
    }

    private DeclarationMatch( DeclarationMatchOutcome? outcome, TDeclaration? declaration, object? candidateNames )
    {
        this.Declaration = declaration;
        this.Outcome = outcome;
        this._candidateNames = candidateNames;
    }

    public DeclarationMatchOutcome? Outcome { get; }

    public TDeclaration? Declaration { get; }

    [MemberNotNullWhen( true, nameof(CandidateNames) )]
    public bool HasCandidateNames => this._candidateNames != null;

    public IEnumerable<string>? CandidateNames
        => this._candidateNames == null ? null : this._candidateNames as IEnumerable<string> ?? new[] { (string) this._candidateNames };

    public DeclarationMatch<TBaseDeclaration> ForDeclarationType<TBaseDeclaration>()
        where TBaseDeclaration : class, IDeclaration
        => new( this.Outcome, (TBaseDeclaration?) (IDeclaration?) this.Declaration, this._candidateNames );
}