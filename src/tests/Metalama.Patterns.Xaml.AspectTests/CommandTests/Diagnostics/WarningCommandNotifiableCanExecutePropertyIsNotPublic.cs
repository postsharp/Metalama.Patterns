// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

// @RemoveOutputCode

using System.ComponentModel;
using System.Windows.Input;

namespace Metalama.Patterns.Xaml.AspectTests.CommandTests.Diagnostics;

public class WarningCommandNotifiableCanExecutePropertyIsNotPublic : INotifyPropertyChanged
{
    [Command]
    public ICommand PrivateCanExecuteCommand { get; }

    private void ExecutePrivateCanExecute() { }

    private bool CanExecutePrivateCanExecute => true;

    public event PropertyChangedEventHandler? PropertyChanged;
}