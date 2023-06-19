// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using System.Reflection;
using Xunit;
using Xunit.Abstractions;

namespace Flashtrace.Formatters.UnitTests.Formatters;

public abstract class FormattersTestsBase
{
    /// <summary>
    /// Gets a new instance of <see cref="FormatterRepository"/>.
    /// </summary>
    protected static FormatterRepository GetNewRepository() => new FormatterRepository( TestRole.Instance );

    /// <summary>
    /// Gets a shared instance of <see cref="FormatterRepository"/>.
    /// </summary>
    protected FormatterRepository DefaultRepository { get; } = GetNewRepository();
}
