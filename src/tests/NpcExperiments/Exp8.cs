// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

#pragma warning disable

using Metalama.Patterns.NotifyPropertyChanged;
using Metalama.Patterns.NotifyPropertyChanged.Metadata;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace NpcExperiments.Exp8
{
    // NOTE: OnUnmonitoredInpcPropertyChanged solves a specific problem, but expands the contract and would likey be 
    // opt-in via configuration.

    /// <summary>
    /// Applied to a method of a class implementing <see cref="INotifyPropertyChanged"/>. The method will be called when a property 
    /// changes, and where that property is of a type implementing <see cref="INotifyPropertyChanged"/>, and the declaring class
    /// does not monitor (ie, maintiain a subscription) to the property.
    /// </summary>
    /// <remarks>
    /// To avoid duplicate subscribtions, implementors should only call the base implementation for unhandled properties.
    /// Implementations must call <c>OnChildPropertyChanged(...)</c> in the subscribed event handler.
    /// </remarks>
    public sealed class OnUnmonitoredInpcPropertyChangedAttribute : Attribute { }


    public sealed class RefsAttribute : Attribute
    {
        public RefsAttribute( params string[] refs ) { }
    }

    [NotifyPropertyChanged]
    class A
    {
        public B A1 { get; set; }

        public int A2 => A1.B1;

        public B A3 { get; set; }

        public int A4 => A1.B3.D1;
    }

    [NotifyPropertyChanged]
    class B
    {
        public int B1 { get; set; }

        public int B2 { get; set; }

        public D B3 { get; set; }
    }

    [NotifyPropertyChanged]
    class D
    {
        public int D1 { get; set; }
    }


    class C : A
    {
        public int C1 {  get; set;}
        
        // A1 is already monitored by class A
        public int C2 => A1.B1;

        // A3 is not monitored by any base class.
        public int C3 => A3.B1;

        // No child of C4 is referenced in this class, so C4 is not monitored here.
        public D C4 { get; set; }

        // A child of C5 is referenced in this class, so C5 is monitored here.
        public D C5 { get; set; }

        // References a child of C5.
        public int C6 => C5.D1;
    }

    class E : C
    {
        // Class E should introduce monitoring only of C4.

        // A1 is monitored in class A.
        public int E1 => A1.B1;

        // C5 is monitored in class C.
        public int E2 => C5.D1;

        // C4 is not monitored by any base class.
        public int E3 => C4.D1;

        // A3 is defined in class A, but is first monitored in class C.
        public int E4 => A3.B1;
    }

#if false
    namespace Desired_MinimalContract
    {
        class A : INotifyPropertyChanged
        {
            private B _a1 = default!;
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
                        this.OnPropertyChanged( "A1" );

                        if ( value != null )
                        {
                            this._onA1PropertyChangedHandler ??= OnSpecificPropertyChanged;
                            value.PropertyChanged += this._onA1PropertyChangedHandler;
                        }
                    }

                    void OnSpecificPropertyChanged( object sender, PropertyChangedEventArgs e )
                    {
                        OnChildPropertyChanged( "A1", e.PropertyName );
                    }
                }
            }

            public int A2 => A1.B1;

            private PropertyChangedEventHandler? _onA1PropertyChangedHandler;

            private B _a3 = default!;

            // Class A has no refs to child props of A3, so does not maintain a sub to A3.
            public B A3
            {
                get
                {
                    return this._a3;
                }
                set
                {
                    if ( !object.ReferenceEquals( value, this._a3 ) )
                    {
                        var oldValue = this._a3;
                        this._a3 = value;

                        // Call OnUnmonitoredInpcPropertyChanged so that a derived class can maintain a subscription.
                        this.OnUnmonitoredInpcPropertyChanged( "A3", oldValue, value );
                        // Also call vanilla OnPropertyChanged to indicate that the ref has changed.
                        this.OnPropertyChanged( "A3" );
                    }
                }
            }

            /* [Refs] defines which props are reported via OnUnmonitoredInpcPropertyChanged. It's possible to have a non-instrumented
             * base from which a [NPC] class is derived. Then the base would not have OnUnmonitoredInpcPropertyChanged for any of its
             * props. Removing OnUnmonitoredInpcPropertyChanged from an existing base class via assy update would be a breaking change.
             */
            [Refs("A3")]
            [OnUnmonitoredInpcPropertyChanged]
            protected virtual void OnUnmonitoredInpcPropertyChanged( string propertyName, INotifyPropertyChanged? oldValue, INotifyPropertyChanged? newValue )
            {
            }

            protected virtual void OnPropertyChanged( string propertyName )
            {
                // TODO: Should this be at start or end? Does it matter?
                this.PropertyChanged?.Invoke( this, new PropertyChangedEventArgs( propertyName ) );
                
                if ( propertyName == "A1" )
                {
                    OnPropertyChanged( "A2" );
                }                
            }

            /* A later build of class A might add or remove refs.
             * Adding refs would cause duplicate events (not so bad).
             * Removing refs would cause missed events (bad).
             * 
             * A mitigation would be to support dynamic subs at runtime, in addition to hard-coded subs at CT.
             * Each inheritance layer would be responsible for its own properties.
             * 
             * A derived class could disable its handling of a particular property at runtime if the base
             * now handles it.
             * 
             * Another mitigation is to store a hash of the refs list and throw at runtime if different.
             * 
             * ---
             * 
             * If a class has [Refs("A1.B1.C1")] it must maintain subs all the way along the chain
             * and raise OnChildPropertyChanged along the chain.
            */
            [Refs("A1")] // Class A maintains a subscription to property A1
            protected virtual void OnChildPropertyChanged( string parentPropertyPath, string propertyName )
            {
                if ( parentPropertyPath == "A1" )
                {
                    if ( propertyName == "B1" )
                    {
                        this.OnPropertyChanged( "A2" );
                        return;
                    }
                }
            }

            public event PropertyChangedEventHandler? PropertyChanged;
        }

        class C : A
        {
            private int _c1;

            public int C1
            {
                get
                {
                    return this._c1;
                }
                set
                {
                    if ( this._c1 != value )
                    {
                        this._c1 = value;
                        this.OnPropertyChanged( "C1" );
                    }
                }
            }

            public int C2 => A1.B1;

            public int C3 => A3.B1;

            private PropertyChangedEventHandler? _onA3PropertyChangedHandler;

#if true // Variation 1: Base implements OnUnmonitoredInpcPropertyChanged, we don't need to track old value, we don't need to use getter to get new value.
            protected override void OnUnmonitoredInpcPropertyChanged( string propertyName, INotifyPropertyChanged oldValue, INotifyPropertyChanged newValue )
            {
                if ( propertyName == "A3" )
                {
                    // NOTE: We intentionally DO NOT fire OnPropertyChanged("A3"), because the class A will do that after calling OnUnmonitoredInpcPropertyChanged.
                    if ( oldValue != null )
                    {
                        oldValue.PropertyChanged -= this._onA3PropertyChangedHandler;
                    }

                    if ( newValue != null )
                    {
                        this._onA3PropertyChangedHandler ??= OnChildPropertyChanged;
                        newValue.PropertyChanged += this._onA3PropertyChangedHandler;
                    }

                    // We've handled it - don't call base impl.

                    void OnChildPropertyChanged( object sender, PropertyChangedEventArgs e )
                    {
                        this.OnChildPropertyChanged( "A3", e.PropertyName );
                    }
                }
                else
                {
                    base.OnUnmonitoredInpcPropertyChanged( propertyName, oldValue, newValue );
                }
            }

            protected override void OnPropertyChanged( string propertyName )
            {
                // NB: Changes to A3 will be notified here, but we will ignore because we see them in OnUnmonitoredInpcPropertyChanged, and [Refs] on ancestor OnUnmonitoredInpcPropertyChanged
                // confirms that A3 is notified via OnUnmonitoredInpcPropertyChanged.
                if ( propertyName == "A1" )
                {
                    // A1 ref has changed. No false +ve detection, no old value stored, assume changed.
                    OnPropertyChanged( "C2" );
                }

                base.OnPropertyChanged( propertyName );
            }

#else // Variation 2: Base does not impl OnPropertyChanged( string propertyName, INPC oldValue, INPC newValue ), we must keep track, and use the property getter to get the new value, and we may end up with duplicate subs and notifications.
            private B _lastA3 = default!;

            // Class A does not maintain a sub to A3, so we need to.
            protected override void OnPropertyChanged( string propertyName )
            {
                if ( propertyName == "A1" )
                {
                    // A1 ref has changed. No false +ve detection, no old value stored, assume changed.
                    OnPropertyChanged( "C2" );
                }

                if ( propertyName == "A3" )
                {
                    var oldValue = this._lastA3;
                    var newValue = this.A3;

                    if ( oldValue != null )
                    {
                        oldValue.PropertyChanged -= this._onA3PropertyChangedHandler;
                    }

                    this._lastA3 = newValue;

                    if ( newValue != null )
                    {
                        this._onA3PropertyChangedHandler ??= OnSpecificPropertyChanged;
                        newValue.PropertyChanged += this._onA3PropertyChangedHandler;
                    }

                    void OnSpecificPropertyChanged( object sender, PropertyChangedEventArgs e )
                    {
                        this.OnChildPropertyChanged( "A3", e.PropertyName );
                    }
                }
                base.OnPropertyChanged( propertyName );
            }
#endif
            // Class A does not maintain a sub to A3, but we do.
            [Refs( "A3" )]
            protected override void OnChildPropertyChanged( string parentPropertyPath, string propertyName )
            {
                if ( parentPropertyPath == "A1" )
                {
                    if ( propertyName == "B1" )
                    {
                        OnPropertyChanged( "C2" );
                    }
                }
                else if ( parentPropertyPath == "A3" )
                {
                    if ( propertyName == "B1" )
                    {
                        this.OnPropertyChanged( "C3" );
                    }
                }

                base.OnChildPropertyChanged( parentPropertyPath, propertyName );
            }
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
                        this.OnPropertyChanged( "B1" );
                    }
                }
            }

            protected virtual void OnPropertyChanged( string propertyName )
            {
                this.PropertyChanged?.Invoke( this, new PropertyChangedEventArgs( propertyName ) );
            }

            public event PropertyChangedEventHandler? PropertyChanged;
        }
    }

#if false
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

                    
                    void OnSpecificPropertyChanged( object sender, PropertyChangedEventArgs e )
                    {
                        this.OnA1ChildChanged( e.PropertyName );
                    }
                    
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
#endif
#endif
}
