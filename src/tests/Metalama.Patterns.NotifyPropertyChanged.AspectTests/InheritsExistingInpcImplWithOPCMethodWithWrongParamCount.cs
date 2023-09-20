// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

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