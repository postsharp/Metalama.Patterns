// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Aspects;
using Metalama.Framework.Code;

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
public readonly struct DeclarationMatch<TDeclaration>
    where TDeclaration : class, IDeclaration
{
    public static DeclarationMatch<TDeclaration> Success( TDeclaration declaration )
        => new DeclarationMatch<TDeclaration>( DeclarationMatchOutcome.Success, declaration ?? throw new ArgumentNullException( nameof( declaration ) ) );

    public static DeclarationMatch<TDeclaration> Ambiguous( params string[] candidateNames )
        => new DeclarationMatch<TDeclaration>( DeclarationMatchOutcome.Ambiguous, candidateNames: candidateNames );

    public static DeclarationMatch<TDeclaration> NotFound( params string[] candidateNames )
        => new DeclarationMatch<TDeclaration>( DeclarationMatchOutcome.NotFound, candidateNames: candidateNames );

    public static DeclarationMatch<TDeclaration> Invalid()
        => new DeclarationMatch<TDeclaration>( DeclarationMatchOutcome.Invalid );

    internal DeclarationMatch( DeclarationMatchOutcome outcome, TDeclaration? declaration = null, IEnumerable<string>? candidateNames = null )
    {
        this.Declaration = declaration;
        this.Outcome = outcome;
        this.CandidateNames = candidateNames;
    }

    public DeclarationMatchOutcome? Outcome { get; }

    public TDeclaration? Declaration { get; }

    public IEnumerable<string>? CandidateNames { get; }
}