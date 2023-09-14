// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Patterns.NotifyPropertyChanged.Metadata;
using System.ComponentModel;

namespace NpcExperiments.Exp10;

#pragma warning disable

// Sketching based on LamaDebug from unittests/Assets/ChildPropertyAssets.cs

public partial class A : INotifyPropertyChanged
{
    public A()
    {
        this.A2 = new();
    }


    private int _a1;

    public int A1
    {
        get
        {
            return this._a1;
        }

        set
        {
            if ( this._a1 != value )
            {
                this._a1 = value;
                this.OnPropertyChanged( "A1" );
            }
        }
    }

    private B _a2 = default!;

    // TODO: Can't use public B A2 { get; set; } = new(); because transformed property has `new()` assigned to the backing field bypassing the setter.
    public B A2
    {
        get
        {
            return this._a2;
        }

        set
        {
            if ( !object.ReferenceEquals( value, this._a2 ) )
            {
                var oldValue = this._a2;
                if ( oldValue != null )
                {
                    oldValue.PropertyChanged -= this._onA2PropertyChangedHandler;
                }

                this._a2 = value;

                // + cascade updates for children of A2, and notify all refs to self (not children)
                this.UpdateA2B2();

                this.OnPropertyChanged( "A2" );

                if ( value != null )
                {
                    this._onA2PropertyChangedHandler ??= OnChildPropertyChanged_1;
                    value.PropertyChanged += this._onA2PropertyChangedHandler;
                    void OnChildPropertyChanged_1( object sender, PropertyChangedEventArgs e )
                    {
                        this.OnChildPropertyChanged( "A2", e.PropertyName );
                    }
                }
            }
        }
    }

    /// <summary>
    /// Leaf ref (A2.B2.C2.D1).
    /// </summary>
    public int A3 => this.A2.B2.C2.D1;

    private C? _lastA2B2;
    private D? _lastA2B2C2;
    private PropertyChangedEventHandler? _onA2B2C2PropertyChangedHandler;
    private PropertyChangedEventHandler? _onA2B2PropertyChangedHandler;
    private PropertyChangedEventHandler? _onA2PropertyChangedHandler;

    [OnChildPropertyChangedMethod( new string[]
    {
    "A2"
    } )]
    protected virtual void OnChildPropertyChanged( string parentPropertyPath, string propertyName )
    {
        /* Moves to [2]
        if ( parentPropertyPath == "A2" && propertyName == "B2" )
        {
            this.UpdateA2B2C2();
        }
        */

        /* Moves to [1]
        if ( parentPropertyPath == "A2.B2" && propertyName == "C2" )
        {
            this.OnPropertyChanged( "A3" );
        }
        */
        
        /* Moves to [3]
        if ( parentPropertyPath == "A2.B2.C2" && propertyName == "D1" )
        {
            this.OnPropertyChanged( "A3" );
        }
        */
    }

    protected virtual void OnPropertyChanged( string propertyName )
    {
        // There's no good reason to respond to a derived class calling this with names local to this class.
        /*
        if ( propertyName == "A2" )
        {
            this.UpdateA2B2();
        }
        */

        this.PropertyChanged?.Invoke( this, new PropertyChangedEventArgs( propertyName ) );
    }

    [OnUnmonitoredInpcPropertyChangedMethod( new string[]
    {
    } )]
    protected virtual void OnUnmonitoredInpcPropertyChanged( string propertyPath, INotifyPropertyChanged? oldValue, INotifyPropertyChanged? newValue )
    {
    }
    
    private void UpdateA2B2()
    {
        var newValue = A2?.B2;
        if ( !object.ReferenceEquals( newValue, this._lastA2B2 ) )
        {
            if ( !object.ReferenceEquals( this._lastA2B2, null ) )
            {
                _lastA2B2.PropertyChanged -= this._onA2B2PropertyChangedHandler;
            }

            if ( newValue != null )
            {
                this._onA2B2PropertyChangedHandler ??= OnChildPropertyChanged_1;
                newValue.PropertyChanged += this._onA2B2PropertyChangedHandler;
                void OnChildPropertyChanged_1( object? sender, PropertyChangedEventArgs e )
                {
                    // + conditional cascade updates for specific child of A2.B2 (from [1])
                    var propertyName = e.PropertyName;

                    if ( propertyName == "C2" )
                    {
                        this.UpdateA2B2C2();
                        //this.OnPropertyChanged( "A3" );
                    }

                    this.OnChildPropertyChanged( "A2.B2", propertyName );
                }
            }

            this._lastA2B2 = newValue;

            // + cascade updates for children of A2.B2 (from [2])
            this.UpdateA2B2C2();

            this.OnChildPropertyChanged( "A2", "B2" );
        }
    }

    private void UpdateA2B2C2()
    {
        var newValue = A2?.B2?.C2;
        if ( !object.ReferenceEquals( newValue, this._lastA2B2C2 ) )
        {
            if ( !object.ReferenceEquals( this._lastA2B2C2, null ) )
            {
                _lastA2B2C2.PropertyChanged -= this._onA2B2C2PropertyChangedHandler;
            }

            if ( newValue != null )
            {
                this._onA2B2C2PropertyChangedHandler ??= OnChildPropertyChanged_1;
                newValue.PropertyChanged += this._onA2B2C2PropertyChangedHandler;
                void OnChildPropertyChanged_1( object? sender, PropertyChangedEventArgs e )
                {
                    // + conditional cascade updates for child of A2.B2.C2 (from [3])
                    var propertyName = e.PropertyName;
                    if ( propertyName == "D1" )
                    {
                        this.OnPropertyChanged( "A3" );
                    }

                    this.OnChildPropertyChanged( "A2.B2.C2", propertyName );;
                }
            }

            this._lastA2B2C2 = newValue;

            // C2 has changed. We dont' store D1 because no false +ve detect, so assume changed.
            // + cascade notify refs
            this.OnPropertyChanged( "A3" );

            this.OnChildPropertyChanged( "A2.B2", "C2" );
        }
    }

    private void UpdatePattern()
    {
        /* if ( valueHasChanged )
         * {
         *  de-reg
         *  re-reg
         *  store new val
         *  per-property-name { NPC for all refs for named prop to self, cascade child updates }
         *  call OCPC
         *  
         * }
         * 
         * ----
         * reg handler:
         *      per-property-name { NPC for all refs for named prop to self, cascade child updates }
         *      call OCPC
         */
    }

    private void NPCPattern()
    {
        /* No handling for properties declared in the current type.
         * 
         */
    }

    private void NCPCPattern()
    {
        /* No handling for properties declared in the current type.
         * 
         */
    }

    public event PropertyChangedEventHandler PropertyChanged;
}

public partial class B : INotifyPropertyChanged
{
    public B()
    {
        this.B2 = new();
    }


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

    private C _b2 = default!;

    public C B2
    {
        get
        {
            return this._b2;
        }

        set
        {
            if ( !object.ReferenceEquals( value, this._b2 ) )
            {
                var oldValue = this._b2;
                this._b2 = value;
                this.OnUnmonitoredInpcPropertyChanged( "B2", (INotifyPropertyChanged?) oldValue, (INotifyPropertyChanged?) value );
                this.OnPropertyChanged( "B2" );
            }
        }
    }

    [OnChildPropertyChangedMethod( new string[]
    {
    } )]
    protected virtual void OnChildPropertyChanged( string parentPropertyPath, string propertyName )
    {
    }

    protected virtual void OnPropertyChanged( string propertyName )
    {
        this.PropertyChanged?.Invoke( this, new PropertyChangedEventArgs( propertyName ) );
    }

    [OnUnmonitoredInpcPropertyChangedMethod( new string[]
    {
    "B2"
    } )]
    protected virtual void OnUnmonitoredInpcPropertyChanged( string propertyPath, INotifyPropertyChanged? oldValue, INotifyPropertyChanged? newValue )
    {
    }

    public event PropertyChangedEventHandler PropertyChanged;
}

public partial class C : INotifyPropertyChanged
{
    public C()
    {
        this.C2 = new();
    }


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

    private D _c2 = default!;

    public D C2
    {
        get
        {
            return this._c2;
        }

        set
        {
            if ( !object.ReferenceEquals( value, this._c2 ) )
            {
                var oldValue = this._c2;
                this._c2 = value;
                this.OnUnmonitoredInpcPropertyChanged( "C2", (INotifyPropertyChanged?) oldValue, (INotifyPropertyChanged?) value );
                this.OnPropertyChanged( "C2" );
            }
        }
    }

    [OnChildPropertyChangedMethod( new string[]
    {
    } )]
    protected virtual void OnChildPropertyChanged( string parentPropertyPath, string propertyName )
    {
    }

    protected virtual void OnPropertyChanged( string propertyName )
    {
        this.PropertyChanged?.Invoke( this, new PropertyChangedEventArgs( propertyName ) );
    }

    [OnUnmonitoredInpcPropertyChangedMethod( new string[]
    {
    "C2"
    } )]
    protected virtual void OnUnmonitoredInpcPropertyChanged( string propertyPath, INotifyPropertyChanged? oldValue, INotifyPropertyChanged? newValue )
    {
    }

    public event PropertyChangedEventHandler PropertyChanged;
}

public partial class D : INotifyPropertyChanged
{


    private int _d1;
    public int D1
    {
        get
        {
            return this._d1;
        }

        set
        {
            if ( this._d1 != value )
            {
                this._d1 = value;
                this.OnPropertyChanged( "D1" );
            }
        }
    }

    private int _d2;

    public int D2
    {
        get
        {
            return this._d2;
        }

        set
        {
            if ( this._d2 != value )
            {
                this._d2 = value;
                this.OnPropertyChanged( "D2" );
            }
        }
    }

    [OnChildPropertyChangedMethod( new string[]
    {
    } )]
    protected virtual void OnChildPropertyChanged( string parentPropertyPath, string propertyName )
    {
    }

    protected virtual void OnPropertyChanged( string propertyName )
    {
        this.PropertyChanged?.Invoke( this, new PropertyChangedEventArgs( propertyName ) );
    }

    [OnUnmonitoredInpcPropertyChangedMethod( new string[]
    {
    } )]
    protected virtual void OnUnmonitoredInpcPropertyChanged( string propertyPath, INotifyPropertyChanged? oldValue, INotifyPropertyChanged? newValue )
    {
    }

    public event PropertyChangedEventHandler PropertyChanged;
}

public partial class E : INotifyPropertyChanged
{
    public E()
    {
        this.E2 = new();
    }


    private int _e1;

    public int E1
    {
        get
        {
            return this._e1;
        }

        set
        {
            if ( this._e1 != value )
            {
                this._e1 = value;
                this.OnPropertyChanged( "E1" );
            }
        }
    }

    private B _e2 = default!;

    public B E2
    {
        get
        {
            return this._e2;
        }

        set
        {
            if ( !object.ReferenceEquals( value, this._e2 ) )
            {
                var oldValue = this._e2;
                if ( oldValue != null )
                {
                    oldValue.PropertyChanged -= this._onE2PropertyChangedHandler;
                }

                this._e2 = value;
                this.OnPropertyChanged( "E2" );
                if ( value != null )
                {
                    this._onE2PropertyChangedHandler ??= OnChildPropertyChanged_1;
                    value.PropertyChanged += this._onE2PropertyChangedHandler;
                    void OnChildPropertyChanged_1( object sender, PropertyChangedEventArgs e )
                    {
                        this.OnChildPropertyChanged( "E2", e.PropertyName );
                    }
                }
            }
        }
    }

    /// <summary>
    /// Leaf ref (E2.B2.C2.D1).
    /// </summary>
    public int LR => this.E2.B2.C2.D1;

    /// <summary>
    /// Leaf-parent ref (E2.B2.C1).
    /// </summary>
    public int LP1R => this.E2.B2.C1;

    /// <summary>
    /// Leaf-parent-parent ref (E2.B1).
    /// </summary>
    public int LP2R => this.E2.B1;

    private C? _lastE2B2;
    private D? _lastE2B2C2;
    private PropertyChangedEventHandler? _onE2B2C2PropertyChangedHandler;
    private PropertyChangedEventHandler? _onE2B2PropertyChangedHandler;
    private PropertyChangedEventHandler? _onE2PropertyChangedHandler;

    [OnChildPropertyChangedMethod( new string[]
    {
    "E2"
    } )]
    protected virtual void OnChildPropertyChanged( string parentPropertyPath, string propertyName )
    {
        if ( parentPropertyPath == "E2" && propertyName == "B1" )
        {
            this.OnPropertyChanged( "LP2R" );
        }

        if ( parentPropertyPath == "E2" && propertyName == "B2" )
        {
            this.UpdateE2B2C2();
            this.OnPropertyChanged( "LP1R" );
        }

        if ( parentPropertyPath == "E2.B2" && propertyName == "C1" )
        {
            this.OnPropertyChanged( "LP1R" );
        }

        if ( parentPropertyPath == "E2.B2" && propertyName == "C2" )
        {
            this.OnPropertyChanged( "LR" );
        }

        if ( parentPropertyPath == "E2.B2.C2" && propertyName == "D1" )
        {
            this.OnPropertyChanged( "LR" );
        }
    }

    protected virtual void OnPropertyChanged( string propertyName )
    {
        if ( propertyName == "E2" )
        {
            this.UpdateE2B2();
            this.OnPropertyChanged( "LP2R" );
        }

        this.PropertyChanged?.Invoke( this, new PropertyChangedEventArgs( propertyName ) );
    }

    [OnUnmonitoredInpcPropertyChangedMethod( new string[]
    {
    } )]
    protected virtual void OnUnmonitoredInpcPropertyChanged( string propertyPath, INotifyPropertyChanged? oldValue, INotifyPropertyChanged? newValue )
    {
    }

    private void UpdateE2B2()
    {
        var newValue = E2?.B2;
        if ( !object.ReferenceEquals( newValue, this._lastE2B2 ) )
        {
            if ( !object.ReferenceEquals( this._lastE2B2, null ) )
            {
                _lastE2B2.PropertyChanged -= this._onE2B2PropertyChangedHandler;
            }

            if ( newValue != null )
            {
                this._onE2B2PropertyChangedHandler ??= OnChildPropertyChanged_1;
                newValue.PropertyChanged += this._onE2B2PropertyChangedHandler;
                void OnChildPropertyChanged_1( object? sender, PropertyChangedEventArgs e )
                {
                    this.OnChildPropertyChanged( "E2.B2", e.PropertyName );
                }
            }

            this._lastE2B2 = newValue;
            this.OnChildPropertyChanged( "E2", "B2" );
        }
    }

    private void UpdateE2B2C2()
    {
        var newValue = E2?.B2?.C2;
        if ( !object.ReferenceEquals( newValue, this._lastE2B2C2 ) )
        {
            if ( !object.ReferenceEquals( this._lastE2B2C2, null ) )
            {
                _lastE2B2C2.PropertyChanged -= this._onE2B2C2PropertyChangedHandler;
            }

            if ( newValue != null )
            {
                this._onE2B2C2PropertyChangedHandler ??= OnChildPropertyChanged_1;
                newValue.PropertyChanged += this._onE2B2C2PropertyChangedHandler;
                void OnChildPropertyChanged_1( object? sender, PropertyChangedEventArgs e )
                {
                    this.OnChildPropertyChanged( "E2.B2.C2", e.PropertyName );
                }
            }

            this._lastE2B2C2 = newValue;
            this.OnChildPropertyChanged( "E2.B2", "C2" );
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;
}