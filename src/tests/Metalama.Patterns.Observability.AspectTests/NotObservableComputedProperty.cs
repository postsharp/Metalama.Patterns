// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using System;

namespace Metalama.Patterns.Observability.AspectTests.NotObservableComputedProperty;

[Observable]
public class DateTimeViewModel
{
    public DateTime DateTime { get; set; }

    [NotObservable]
    public double MinutesFromNow => (DateTime.Now - this.DateTime).TotalMinutes;
}