// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

#pragma warning disable

using Metalama.Patterns.NotifyPropertyChanged;

namespace NpcExperiments.Exp5;

[NotifyPropertyChanged]
class A
{
    // Property is of a type that won't be part of a design-time partial compilation. This will produce a warning comment in diff preview.
    public Exp2.A OtherA { get; set; }

    public int Q => OtherA.A8;
}