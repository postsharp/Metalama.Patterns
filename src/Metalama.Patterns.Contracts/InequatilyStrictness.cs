// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Aspects;

namespace Metalama.Patterns.Contracts;

/// <summary>
/// Enumerates the meanings of inequalities of unspecified strictness such as <see cref="GreaterThanAttribute"/>, <see cref="LessThanAttribute"/>,
/// <see cref="PositiveAttribute"/> and <see cref="NegativeAttribute"/>.
/// </summary>
[CompileTime]
public enum InequatilyStrictness
{
    /// <summary>
    /// Inequalities of unspecified strictness are interpreted as strict, i.e. <c>[GreaterThan(5)]</c> corresponds to <c>x &gt; 5</c>,
    /// <c>[LessThan(5)]</c> corresponds to <c>x &lt; 5</c>, <c>[Positive]</c> corresponds to <c>x &gt; 0</c> and
    /// <c>[Negative]</c> corresponds to <c>x &lt; 0</c>. This is the default convention starting from Metalama 2024.2.
    /// </summary>
    Strict,

    /// <summary>
    /// Inequalities of unspecified strictness are interpreted as non-strict, i.e. <c>[GreaterThan(5)]</c> corresponds to <c>x &gt;= 5</c>,
    /// <c>[LessThan(5)]</c> corresponds to <c>x &lt;= 5</c>, <c>[Positive]</c> corresponds to <c>x &gt;= 0</c> and
    /// <c>[Negative]</c> corresponds to <c>x &lt;= 0</c>. This was the default convention in PostSharp and in Metalama prior to version 2024.2.
    /// </summary>
    NonStrict
}