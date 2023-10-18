// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using JetBrains.Annotations;
using System.ComponentModel;
using System.Windows.Input;

namespace Metalama.Patterns.Xaml.Implementation;

/// <summary>
/// An implementation of <see cref="ICommand"/> which uses delegates to access callbacks. This class supports
/// the <see cref="CommandAttribute"/> aspect infrastructure and should not be used directly.
/// </summary>
/// <remarks>
/// <para>
/// This class is inspired by PostSharp.Patterns.Xaml.CommandAttribute.CommandImpl, notably as regards
/// the use of <see cref="SynchronizationContext"/>.
/// </para>
/// <para>
/// As and when Metalama supports type introduction, this class could be replaced by generated nested types which
/// could call the callbacks directly rather than via delegates, the delegates themselves must presently originate
/// from generated local functions.
/// </para>
/// </remarks>
[PublicAPI]
[EditorBrowsable( EditorBrowsableState.Never )]
public sealed class DelegateCommand : ICommand
{
    /* Original comment from PostSharp:
    * Two options how to support this:
    *   - have INPC detect dependencies in the CanExecuteChanged method
    *   - use a property instead of method and route the notification (NPC -> CanExecuteChanged)
    *     limitation: parameters would not be easily supported
    * Currently: only property in [NPC] classes is supported.
    * 
    * In this (Metalama) implementation: 
    * 
    * Currently INPC integration is strictly via INotifyPropertyChanged, where CanExecute must be a public property. 
    * The only requirement for integration with [NPC] is aspect ordering so that [NPC] aspect is applied before [Command], so [Command]
    * sees that INotifyPropertyChanged is implemented.
    */

    private static readonly SendOrPostCallback _onCanExecuteChangedDelegate = OnCanExecuteChanged;

    private readonly SynchronizationContext? _synchronizationContext;
    private readonly Func<object?, bool>? _canExecute;
    private readonly Action<object?> _execute;
    private readonly string? _canExecutePropertyName;

    /// <summary>
    /// Initializes a new instance of the <see cref="DelegateCommand"/> class, without <see cref="INotifyPropertyChanged"/> integration.
    /// </summary>
    public DelegateCommand( Action<object?> execute, Func<object?, bool>? canExecute )
    {
        this._synchronizationContext = SynchronizationContext.Current;
        this._execute = execute;
        this._canExecute = canExecute;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DelegateCommand"/> class, with <see cref="INotifyPropertyChanged"/> integration.
    /// </summary>
    public DelegateCommand(
        Action<object?> execute,
        Func<object?, bool> canExecute,
        INotifyPropertyChanged canExecutePropertyChangeNotifier,
        string canExecutePropertyName )
    {
        this._synchronizationContext = SynchronizationContext.Current;
        this._execute = execute;
        this._canExecute = canExecute;
        this._canExecutePropertyName = canExecutePropertyName;
        canExecutePropertyChangeNotifier.PropertyChanged += this.OnPropertyChanged;
    }

    public event EventHandler? CanExecuteChanged;

    public void Execute( object? parameter )
    {
        if ( !this.CanExecute( parameter ) )
        {
            throw new InvalidOperationException( "Command cannot be executed." );
        }

        this._execute( parameter );
    }

    public bool CanExecute( object? parameter )
        => this._canExecute == null || this._canExecute( parameter );

    private void OnPropertyChanged( object? sender, PropertyChangedEventArgs args )
    {
        if ( this.CanExecuteChanged != null )
        {
            if ( args.PropertyName == this._canExecutePropertyName )
            {
                if ( this._synchronizationContext != null )
                {
                    this._synchronizationContext.Send( _onCanExecuteChangedDelegate, this );
                }
                else
                {
                    OnCanExecuteChanged( this );
                }
            }
        }
    }

    private static void OnCanExecuteChanged( object? obj )
    {
        (obj as DelegateCommand)?.CanExecuteChanged?.Invoke( obj, EventArgs.Empty );
    }
}