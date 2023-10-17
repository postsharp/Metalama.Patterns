// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Aspects;
using Metalama.Framework.Code;
using Metalama.Patterns.Observability.Metadata;
using System.ComponentModel;

namespace Metalama.Patterns.Observability.Implementation;

[CompileTime]
internal class Assets
{
    public Assets()
    {
        this.INotifyPropertyChanged = (INamedType) TypeFactory.GetType( typeof(INotifyPropertyChanged) );
        this.PropertyChangedEventOfINotifyPropertyChanged = this.INotifyPropertyChanged.Events.First();
        this.NullableINotifyPropertyChanged = this.INotifyPropertyChanged.ToNullableType();
        this.PropertyChangedEventHandler = (INamedType) TypeFactory.GetType( typeof(PropertyChangedEventHandler) );
        this.NullablePropertyChangedEventHandler = this.PropertyChangedEventHandler.ToNullableType();
        this.IgnoreAutoChangeNotificationAttribute = (INamedType) TypeFactory.GetType( typeof(NonObservableAttribute) );
        this.EqualityComparerOfT = (INamedType) TypeFactory.GetType( typeof(EqualityComparer<>) );
        this.OnChildPropertyChangedMethodAttribute = (INamedType) TypeFactory.GetType( typeof(OnChildPropertyChangedMethodAttribute) );

        this.OnUnmonitoredObservablePropertyChangedMethodAttribute =
            (INamedType) TypeFactory.GetType( typeof(OnUnmonitoredObservablePropertyChangedMethodAttribute) );
    }

    public INamedType OnChildPropertyChangedMethodAttribute { get; }

    public INamedType OnUnmonitoredObservablePropertyChangedMethodAttribute { get; }

    // ReSharper disable once InconsistentNaming
    public INamedType INotifyPropertyChanged { get; }

    public INamedType NullableINotifyPropertyChanged { get; }

    public IEvent PropertyChangedEventOfINotifyPropertyChanged { get; }

    private INamedType PropertyChangedEventHandler { get; }

    public INamedType NullablePropertyChangedEventHandler { get; }

    public INamedType IgnoreAutoChangeNotificationAttribute { get; }

    public INamedType EqualityComparerOfT { get; }

    /// <summary>
    /// Gets the <see cref="IProperty"/> for property <c>EqualityComparer<paramref name="type"/>>.Default</c>.
    /// </summary>
    public IProperty GetDefaultEqualityComparerForType( IType type )
        => this.EqualityComparerOfT.WithTypeArguments( type ).Properties.Single( p => p.Name == "Default" );
}