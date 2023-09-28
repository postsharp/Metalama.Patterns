// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Aspects;
using Metalama.Framework.Code;

namespace Metalama.Patterns.NotifyPropertyChanged.Implementation.ClassicStrategy;

[CompileTime]
internal interface IClassicProcessingNodeInitializationHelper
{
    InpcInstrumentationKind GetInpcInstrumentationKind( IType type );

    InpcBaseHandling DetermineInpcBaseHandlingForNode( ClassicProcessingNode node );
}