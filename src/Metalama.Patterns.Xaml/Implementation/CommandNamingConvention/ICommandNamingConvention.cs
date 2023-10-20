// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Aspects;
using Metalama.Framework.Code;
using Metalama.Framework.Serialization;
using Metalama.Patterns.Xaml.Implementation.NamingConvention;

namespace Metalama.Patterns.Xaml.Implementation.CommandNamingConvention;

[CompileTime]
public interface ICommandNamingConvention : INamingConvention<IMethod, ICommandNamingMatchContext, CommandNamingConventionMatch>, ICompileTimeSerializable, IEquatable<ICommandNamingConvention>
{
}