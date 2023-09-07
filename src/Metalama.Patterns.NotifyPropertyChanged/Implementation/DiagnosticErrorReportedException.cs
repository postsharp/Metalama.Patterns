// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Aspects;

namespace Metalama.Patterns.NotifyPropertyChanged.Implementation;

/// <summary>
/// An exception thrown when a diagnostic error has been reported and compile-time processing should be stopped.
/// </summary>
[CompileTime]
internal sealed class DiagnosticErrorReportedException : Exception
{
    public DiagnosticErrorReportedException() : base( "A diagnostic error has been reported." )
    {
    }
}