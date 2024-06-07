﻿// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Aspects;
using Metalama.Framework.Code;
using Metalama.Patterns.Xaml.Implementation.NamingConvention;

namespace Metalama.Patterns.Xaml.Implementation.CommandNamingConvention;

// ReSharper disable once RedundantTypeDeclarationBody
[CompileTime]
internal interface ICommandNamingConvention : INamingConvention<IMethod, CommandNamingConventionMatch> { }