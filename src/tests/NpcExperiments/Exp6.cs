// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

#pragma warning disable

using Metalama.Patterns.NotifyPropertyChanged;
using Metalama.Patterns.NotifyPropertyChanged.Metadata;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace NpcExperiments.Exp6;

[NotifyPropertyChanged]
class A
{
    public int A1 { get; set; }

    public C A2 { get; set; }

    public int A3 => A2.C1;

    public int A4 => A2.C2.D1;
}

class B : A
{
    public int B1 { get; set; }

    public int B2 => A1;

    public int B3 => A2.C1;
}

[NotifyPropertyChanged]
class C
{
    public int C1 { get; set; }

    public D C2 { get; set; }
}

[NotifyPropertyChanged]
class D
{
    public int D1 { get; set; }
}

/* Regarding metadata atrributes (OnChanged, OnChildChanged etc):
 * 
 * When introducing methods, we append a numeric suffix if the user has already defined the desired name.
 * It can also be that combining the name of two different pairs of properties yields the same name. For
 * example:
 * 
 *  A.BA => UpdateABA()
 *  AB.A => UpdateABA_2()
 *  
 * Using a separator would not solve the problem, eg, use underscore, then apply to
 * 
 *  A_.BA => UpdateA__BA
 *  A._BA => UpdateA__BA
 * 
 * When a derived class is looking for methods to overload in the base class, we need to determine
 * unambiguously which OnXXChanged etc method is which. Because of the above scenarios, this is made clear
 * and unambiguous by applying and looking for the various metadata attributes.
 */

class A_Desired : INotifyPropertyChanged
{
    private int _a1;

    //[Origin( "A1" )] - not strictly required as the property won't be renamed, but would increase robustness wrt obfuscation - but that's ouside current requirements.
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
                this.OnA1Changed();
            }
        }
    }

    [OnChanged( "A1" )]
    protected virtual void OnA1Changed()
    {
        this.OnPropertyChanged( "A1" );
    }

    private C_Desired _a2 = default!;
    private PropertyChangedEventHandler? _onA2PropertyChangedHandler;

    //[Origin( "A2" )]
    public C_Desired A2
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
                this.OnA2Changed();
                if ( value != null )
                {
                    this._onA2PropertyChangedHandler ??= OnSpecificPropertyChanged;
                    value.PropertyChanged += this._onA2PropertyChangedHandler;
                }
            }

            void OnSpecificPropertyChanged( object sender, PropertyChangedEventArgs e )
                => this.OnA2ChildChanged( e.PropertyName );
        }
    }

    [OnChanged( "A2" )]
    protected virtual void OnA2Changed()
    {
        this.UpdateA2C2();
        this.OnPropertyChanged( "A3" );
        this.OnPropertyChanged( "A2" );
    }

    [OnChildChanged( "A2" )]
    protected virtual void OnA2ChildChanged( string propertyName )
    {
        if ( propertyName == "C1" )
        {
            this.OnPropertyChanged( "A3" );
            return;
        }
        if ( propertyName == "C2" )
        {
            this.UpdateA2C2();
            return;
        }
    }

    private D_Desired? _lastA2C2;

    private PropertyChangedEventHandler? _onA2C2ChangedHandler;

    // [Update] is not strictly necessary as deriving types don't need to identify this specific
    // method. However, it provides a consistency check, because all three methods - [Update],
    // [OnChanged] and [OnChildChanged] - for a given property path must be all defined
    // or none defined. On second thought, we might as well just check that both [OnChanged]
    // and [OnChildChanged] are defined. Also, maybe [OnChildChanged] won't be defined for
    // non-INPC types?
    //[Update( "A2.C2" )]
    /*protected virtual*/ private void UpdateA2C2()
    {
        var newValue = A2?.C2;
        if ( !object.ReferenceEquals( newValue, this._lastA2C2 ) )
        {
            if ( !object.ReferenceEquals( this._lastA2C2, null ) )
            {
                this._lastA2C2.PropertyChanged -= this._onA2C2ChangedHandler;
            }

            if ( newValue != null )
            {
                this._onA2C2ChangedHandler ??= OnSpecificPropertyChanged;
                newValue.PropertyChanged += this._onA2C2ChangedHandler;
            }

            this._lastA2C2 = newValue;
            this.OnA2C2Changed();            
        }

        void OnSpecificPropertyChanged( object? sender, PropertyChangedEventArgs e )
            => OnA2C2ChildChanged( e.PropertyName );
    }

    // NB: If UpdateA2C2() is implemented, then OnA2C2Changed and OnA2C2ChildChanged MUST
    // both be implemented even if empty so that they can be extended by derived types.

    [OnChanged( "A2.C2" )]
    protected virtual void OnA2C2Changed()
    {
        this.OnPropertyChanged( "A4" );
    }

    [OnChildChanged( "A2.C2" )]
    protected virtual void OnA2C2ChildChanged( string propertyName )
    {
        if ( propertyName == "D1" )
        {
            this.OnPropertyChanged( "A4" );
            return;
        }
    }

    public int A3 => A2.C1;

    public int A4 => A2.C2.D1;

    protected virtual void OnPropertyChanged( [CallerMemberName] string? propertyName = null )
    {
        this.PropertyChanged?.Invoke( this, new PropertyChangedEventArgs( propertyName ) );
    }

    public event PropertyChangedEventHandler? PropertyChanged;
}

class B_Desired : A_Desired
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

    public int B2 => A1;

    public int B3 => A2.C1;

    public int B4 => A2.C2.D1;

    protected override void OnA1Changed()
    {
        this.OnPropertyChanged( "B2" );
        base.OnA1Changed();
    }

    protected override void OnA2Changed()
    {
        this.OnPropertyChanged( "B3" );
        base.OnA2Changed();
    }

    protected override void OnA2ChildChanged( string propertyName )
    {
        if ( propertyName == "C1" )
        {
            this.OnPropertyChanged( "B3" );
        }
        base.OnA2ChildChanged( propertyName );
    }

    // If class A did not have any of its own refs to A2.C2, then
    // class B would need to define UpdateA2C2, OnA2C2Changed and
    // OnA2C2ChildChanged. But it should always extend the base impl
    // if defined there.
    protected override void OnA2C2Changed()
    {
        this.OnPropertyChanged( "B4" );
        base.OnA2C2Changed();
    }

    protected override void OnA2C2ChildChanged( string propertyName )
    {
        if ( propertyName == "D1" )
        {
            this.OnPropertyChanged( "B4" );
            return;
        }
        base.OnA2C2ChildChanged( propertyName );
    }
}

class C_Desired : INotifyPropertyChanged
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

    private D_Desired _c2 = default!;

    public D_Desired C2
    {
        get
        {
            return this._c2;


        }
        set
        {
            if ( !object.ReferenceEquals( value, this._c2 ) )
            {
                this._c2 = value;
                this.OnPropertyChanged( "D2" );
            }

            void OnSpecificPropertyChanged( object sender, PropertyChangedEventArgs e )
            {
            }

        }
    }

    protected virtual void OnPropertyChanged( [CallerMemberName] string? propertyName = null )
    {
        this.PropertyChanged?.Invoke( this, new PropertyChangedEventArgs( propertyName ) );
    }

    public event PropertyChangedEventHandler? PropertyChanged;
}

class D_Desired : INotifyPropertyChanged
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
    protected virtual void OnPropertyChanged( [CallerMemberName] string? propertyName = null )
    {
        this.PropertyChanged?.Invoke( this, new PropertyChangedEventArgs( propertyName ) );
    }

    public event PropertyChangedEventHandler? PropertyChanged;

}
