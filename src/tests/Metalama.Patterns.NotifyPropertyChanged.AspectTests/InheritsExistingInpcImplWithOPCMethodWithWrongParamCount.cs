// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

#pragma warning disable SA1402 // File may only contain a single type
#pragma warning disable SA1649 // File name should match first type name

using System.ComponentModel;

namespace Metalama.Patterns.NotifyPropertyChanged.AspectTests;

public class ExistingInpcImplWithOPCMethodWithWrongParamCount : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged( string propertyName, int propertyIdx ) { }
}

// <target>
[NotifyPropertyChanged]
public partial class InheritsExistingInpcImplWithOPCMethodWithWrongParamCount : ExistingInpcImplWithOPCMethodWithWrongParamCount
{
}