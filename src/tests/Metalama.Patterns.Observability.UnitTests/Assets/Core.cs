﻿// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using System.ComponentModel;

namespace Metalama.Patterns.Observability.UnitTests.Assets.Core;

/// <summary>
/// An [NPC]-enhanced base class with three int auto-properties.
/// </summary>
[Observable]
public partial class Simple
{
    /// <summary>
    /// Auto
    /// </summary>
    public int S1 { get; set; }

    /// <summary>
    /// Auto
    /// </summary>
    public int S2 { get; set; }

    /// <summary>
    /// Auto
    /// </summary>
    public int S3 { get; set; }
}

[Observable]
public partial class SimpleWithInpcProperties
{
    /// <summary>
    /// Auto
    /// </summary>
    public int A1 { get; set; }

    /// <summary>
    /// Ref to R1.S1. Will throw if R1 is null.
    /// </summary>
    public int A2 => this.R1!.S1;

    /// <summary>
    /// Property type implements <see cref="INotifyPropertyChanged"/>. <c>R1.S1</c> is referenced by <see cref="A2"/>.
    /// </summary>
    public Simple? R1 { get; set; }

    /// <summary>
    /// Property type implements <see cref="INotifyPropertyChanged"/>, is not referenced in the current class.
    /// </summary>
    public Simple? R2 { get; set; }
}

public class ExistingInpcImplWithValidOpcMethod : INotifyPropertyChanged
{
    private int _ex1;

    public int EX1
    {
        get => this._ex1;
        set
        {
            if ( value != this._ex1 )
            {
                this._ex1 = value;
                this.OnPropertyChanged( nameof(this.EX1) );
            }
        }
    }

    private Simple? _ex2 = new();

    public Simple? EX2
    {
        get => this._ex2;

        set
        {
            if ( this._ex2 != value )
            {
                this._ex2 = value;
                this.OnPropertyChanged( nameof(this.EX2) );
            }
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged( string propertyName )
    {
        this.PropertyChanged?.Invoke( this, new PropertyChangedEventArgs( propertyName ) );
    }
}

public abstract class ExistingAbstractInpcImplWithValidOPCMethod : INotifyPropertyChanged
{
    private int _ex1;

    public int EX1
    {
        get => this._ex1;
        set
        {
            if ( value != this._ex1 )
            {
                this._ex1 = value;
                this.OnPropertyChanged( nameof(this.EX1) );
            }
        }
    }

    private Simple? _ex2 = new();

    public Simple? EX2
    {
        get => this._ex2;

        set
        {
            if ( this._ex2 != value )
            {
                this._ex2 = value;
                this.OnPropertyChanged( nameof(this.EX2) );
            }
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged( string propertyName )
    {
        this.PropertyChanged?.Invoke( this, new PropertyChangedEventArgs( propertyName ) );
    }
}