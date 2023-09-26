// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Code;
using Metalama.Patterns.NotifyPropertyChanged.Metadata;
using System.ComponentModel;

namespace Metalama.Patterns.NotifyPropertyChanged.Implementation.Natural;

internal sealed partial class BuildAspectContext
{
    internal sealed class ElementsRecord
    {
        public ElementsRecord()
        {
            this.INotifyPropertyChanged = (INamedType) TypeFactory.GetType( typeof(INotifyPropertyChanged) );
            this.PropertyChangedEventOfINotifyPropertyChanged = this.INotifyPropertyChanged.Events.First();
            this.NullableINotifyPropertyChanged = this.INotifyPropertyChanged.ToNullableType();
            this.PropertyChangedEventHandler = (INamedType) TypeFactory.GetType( typeof(PropertyChangedEventHandler) );
            this.NullablePropertyChangedEventHandler = this.PropertyChangedEventHandler.ToNullableType();
            this.IgnoreAutoChangeNotificationAttribute = (INamedType) TypeFactory.GetType( typeof(IgnoreNotifyPropertyChangedAttribute) );
            this.EqualityComparerOfT = (INamedType) TypeFactory.GetType( typeof(EqualityComparer<>) );
            this.OnChildPropertyChangedMethodAttribute = (INamedType) TypeFactory.GetType( typeof(OnChildPropertyChangedMethodAttribute) );
            this.OnUnmonitoredObservablePropertyChangedMethodAttribute = (INamedType) TypeFactory.GetType( typeof(OnUnmonitoredObservablePropertyChangedMethodAttribute) );
        }

        // ReSharper disable once InconsistentNaming
        public INamedType INotifyPropertyChanged { get; }

        public INamedType NullableINotifyPropertyChanged { get; }

        public IEvent PropertyChangedEventOfINotifyPropertyChanged { get; }

        private INamedType PropertyChangedEventHandler { get; }

        public INamedType NullablePropertyChangedEventHandler { get; }

        public INamedType IgnoreAutoChangeNotificationAttribute { get; }

        public INamedType EqualityComparerOfT { get; }

        public INamedType OnChildPropertyChangedMethodAttribute { get; }

        public INamedType OnUnmonitoredObservablePropertyChangedMethodAttribute { get; }
    }
}