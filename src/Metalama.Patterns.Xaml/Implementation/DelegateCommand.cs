// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using JetBrains.Annotations;
using System.ComponentModel;
using System.Windows.Input;

namespace Metalama.Patterns.Xaml.Implementation;

/// <summary>
/// An implementation of <see cref="ICommand"/> which uses delegates to access callbacks.
/// </summary>
/// <remarks>
/// <para>
/// This class is paraphrased from PostSharp.Patterns.Xaml.CommandAttribute.CommandImpl, notably as regards
/// the use of <see cref="SynchronizationContext"/>. The plubming for [NPC] will likely need to be adjusted.
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
    private static readonly SendOrPostCallback _onCanExecuteChangedDelegate = OnCanExecuteChanged;

    private readonly SynchronizationContext _synchronizationContext;
    private readonly Func<object, bool>? _canExecute;
    private readonly Action<object> _execute;

    /* Original comment from PostSharp:
    * Two options how to support this:
    *   - have INPC detect dependencies in the CanExecuteChanged method
    *   - use a property instead of method and route the notification (NPC -> CanExecuteChanged)
    *     limitation: parameters would not be easily supported
    * Currently: only property in [NPC] classes is supported.
    */

    public DelegateCommand( Action<object> execute, Func<object, bool>? canExecute )
    {
        this._synchronizationContext = SynchronizationContext.Current;
        this._execute = execute;
        this._canExecute = canExecute;
    }

    public event EventHandler? CanExecuteChanged;

    public void Execute( object parameter )
    {
        if ( !this.CanExecute( parameter ) )
        {
            throw new InvalidOperationException( "Command cannot be executed." );
        }

        this._execute( parameter );
    }

    public bool CanExecute( object parameter )
        => this._canExecute == null || this._canExecute( parameter );

    public void OnPropertyChanged( object sender, PropertyChangedEventArgs args )
    {
        // TODO: NPC integration
        // This method used to be called from NPC code in PostSharp. It used to be internal.
        // Could do this like PostSharp: have a _propertyName field initialized via ctor, and call this method for all calls to NPC's OnPropertyChanged.
        // Or better, where possible, only call this method from genereated NPC code where applicable.

        throw new NotImplementedException();

#pragma warning disable CS0162 // Unreachable code detected
        const string PropertyName = "TODO";
#pragma warning restore CS0162 // Unreachable code detected

        if ( args.PropertyName == PropertyName )
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

    private static void OnCanExecuteChanged( object obj )
    {
        (obj as DelegateCommand)?.CanExecuteChanged?.Invoke( obj, EventArgs.Empty );
    }
}