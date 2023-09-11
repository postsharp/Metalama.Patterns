// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

#pragma warning disable

using Metalama.Patterns.NotifyPropertyChanged;
using Metalama.Patterns.NotifyPropertyChanged.Metadata;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace NpcExperiments.Exp8
{
    [NotifyPropertyChanged]
    class A
    {
        public B A1 { get; set; }// = new();

        public int A2 => A1.B1;
    }

    [NotifyPropertyChanged]
    class B
    {
        public int B1 { get; set; }
    }

    namespace DesiredV1
    {
        class A : INotifyPropertyChanged
        {
            public A() 
            {
                // Introduce instance initializer only in the root class (the base-most class which has [NPC]).
                // Derived classes override UpdateFieldsAndPropertiesWithInitializers() and call base.
                // This ensures that the update is processed before any user ctor code is executed, which
                // maintains the behaviour of initializers.
                // NB: Initializers set the value before calling base ctor.
                UpdateFieldsAndPropertiesWithInitializers();
            }
            
            protected virtual void UpdateFieldsAndPropertiesWithInitializers()
            {
                if ( this._a1 != null )
                {
                    this._onA1PropertyChangedHandler ??= this.GetOnA1ChildChangedHandler();
                    this._a1.PropertyChanged += this._onA1PropertyChangedHandler;
                }

                // ... repeat for all INPC-type props/fields ...
            }

            PropertyChangedEventHandler GetOnA1ChildChangedHandler()
            {
                return OnSpecificPropertyChanged;

                void OnSpecificPropertyChanged( object sender, PropertyChangedEventArgs e )
                {
                    this.OnA1ChildChanged( e.PropertyName );
                }
            }

            private B _a1 = new();
            
            public B A1
            {
                get
                {
                    return this._a1;
                }
                set
                {
                    if ( !object.ReferenceEquals( value, this._a1 ) )
                    {
                        var oldValue = this._a1;
                        if ( oldValue != null )
                        {
                            oldValue.PropertyChanged -= this._onA1PropertyChangedHandler;
                        }

                        this._a1 = value;
                        this.OnA1Changed();
                        if ( value != null )
                        {
                            this._onA1PropertyChangedHandler ??= this.GetOnA1ChildChangedHandler();
                            value.PropertyChanged += this._onA1PropertyChangedHandler;
                        }
                    }

                    /*
                    void OnSpecificPropertyChanged( object sender, PropertyChangedEventArgs e )
                    {
                        this.OnA1ChildChanged( e.PropertyName );
                    }
                    */
                }
            }

            private PropertyChangedEventHandler? _onA1PropertyChangedHandler;

            [OnChanged( "A1" )]
            protected virtual void OnA1Changed()
            {
                this.OnPropertyChanged( "A1" );
            }

            [OnChildChanged( "A1" )]
            protected virtual void OnA1ChildChanged( string propertyName )
            {
            }

            protected virtual void OnPropertyChanged( [CallerMemberName] string? propertyName = null )
            {
                this.PropertyChanged?.Invoke( this, new PropertyChangedEventArgs( propertyName ) );
            }

            public event PropertyChangedEventHandler? PropertyChanged;
        }
        
        class B : INotifyPropertyChanged
        {
            private int _b1;

            public int B1
            {
                get
                {
                    return this._b1;
                }
                set
                {
                    if ( this._b1 != value )
                    {
                        this._b1 = value;
                        this.OnB1Changed();
                    }
                }
            }

            [OnChanged( "B1" )]
            protected virtual void OnB1Changed()
            {
                this.OnPropertyChanged( "B1" );
            }

            protected virtual void OnPropertyChanged( [CallerMemberName] string? propertyName = null )
            {
                this.PropertyChanged?.Invoke( this, new PropertyChangedEventArgs( propertyName ) );
            }

            public event PropertyChangedEventHandler? PropertyChanged;
        }

    }
}
