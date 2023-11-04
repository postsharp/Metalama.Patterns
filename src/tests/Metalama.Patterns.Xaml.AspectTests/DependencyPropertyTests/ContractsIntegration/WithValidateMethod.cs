// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

// @Include(../../_TrimAttribute.cs)
// @IgnoredDiagnostic(LAMA5206)

using Metalama.Patterns.Contracts;
using System.Windows;

namespace Metalama.Patterns.Xaml.AspectTests.DependencyPropertyTests.ContractsIntegration;

internal class WithValidateMethod : DependencyObject
{
    [DependencyProperty]
    [Trim]
    public string Name { get; set; }

    private bool ValidateName( string name ) => name.Length > 3;
}