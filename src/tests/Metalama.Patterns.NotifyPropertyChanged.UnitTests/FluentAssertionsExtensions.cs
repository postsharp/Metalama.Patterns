// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using FluentAssertions;
using FluentAssertions.Collections;
using System.Diagnostics;

namespace Metalama.Patterns.NotifyPropertyChanged.UnitTests;

[DebuggerNonUserCode]
public static class FluentAssertionsExtensions
{
    public static AndConstraint<GenericCollectionAssertions<TExpectation>> BeEquivalentTo<TExpectation>( this GenericCollectionAssertions<IEnumerable<TExpectation>, TExpectation, GenericCollectionAssertions<TExpectation>> assertions, params TExpectation[] expectation )
    {
        return assertions.BeEquivalentTo( expectation );
    }
}