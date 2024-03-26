// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using JetBrains.Annotations;
using Metalama.Framework.Aspects;
using Metalama.Framework.Options;
using Metalama.Patterns.Xaml.Implementation.CommandNamingConvention;
using Metalama.Patterns.Xaml.Implementation.NamingConvention;
using System.ComponentModel;
using System.Windows.Input;

namespace Metalama.Patterns.Xaml.Options;

[PublicAPI]
[CompileTime]
public sealed class CommandOptionsBuilder
{
    private CommandOptions _options = new();

    /// <summary>
    /// Gets the key of the default naming convention.
    /// </summary>
    public static string DefaultNamingConventionKey => DefaultCommandNamingConvention.RegistrationKey;

    public static class Names
    {
        public const string CommandNameGroup = RegexCommandNamingConvention.CommandNameGroup;
        public const string CommandNameToken = RegexCommandNamingConvention.CommandNameToken;
    }

    /// <summary>
    /// Adds or updates a naming convention identified by the given <paramref name="key"/>, using a regular expression to select the 
    /// command name from the target method of the <see cref="CommandAttribute"/>, and substitution patterns to determine 
    /// the command property name and the name of the can-execute method or property.
    /// </summary>
    /// <param name="key">A unique identifier used as a key in the keyed collection of naming conventions.</param>
    /// <param name="diagnosticName">A short name describing the convention, used when reporting diagnostics.</param>
    /// <param name="matchCommandName">
    /// A regex match expression that will be evaluated against the name of the target method of the <see cref="CommandAttribute"/> aspect. 
    /// The expression should yield a match group named <see cref="Names.CommandNameGroup"/>. If <paramref name="matchCommandName"/> is <see langword="null"/>,
    /// the name produced by <see cref="DefaultCommandNamingConvention.GetCommandNameFromExecuteMethodName(string)"/> is used.
    /// </param>
    /// <param name="commandPropertyNamePattern">
    /// The name of the command property to be introduced. A string in which the substring <see cref="Names.CommandNameToken"/> will be replaced with the
    /// command name determined according to <paramref name="matchCommandName"/>. If <paramref name="commandPropertyNamePattern"/> is <see langword="null"/>,
    /// the name produced by <see cref="DefaultCommandNamingConvention.GetCommandPropertyNameFromCommandName(string)"/> is used.
    /// </param>
    /// <param name="matchCanExecute">
    /// <para>
    /// A regex match expression that will be evaluated against method and/or property names to identify candidate can-execute members. 
    /// All occurrences of the substring <see cref="Names.CommandNameToken"/> in <paramref name="matchCanExecute"/> will be replaced with the command name
    /// determined according to <paramref name="matchCommandName"/> before the expression is evaluated.
    /// </para>
    /// <para>
    /// If <paramref name="matchCanExecute"/> is <see langword="null"/>, the name produced by
    /// <see cref="DefaultCommandNamingConvention.GetCanExecuteNameFromCommandName(string)"/> is matched.
    /// </para>
    /// </param>
    /// <param name="priority">
    /// The priority of the naming convention. The default priority is 0. The system-registered default naming convention has priority 1000. Naming conventions are
    /// matched in ascending priority. The first successful match is used.
    /// </param>
    /// <param name="requireCanExecuteMatch">
    /// If <see langword="true"/>, a matching valid unambiguous can-execute method or property must be found for a match to be considered successful.
    /// The default value is <see langword="true"/> when <paramref name="matchCanExecute"/> is specified, otherwise <see langword="false"/>.
    /// </param>
    /// <param name="considerCanExecuteMethod">
    /// If <see langword="true"/> (the default), can-execute methods named according to <paramref name="matchCanExecute"/> will be considered.
    /// At least one of <paramref name="considerCanExecuteMethod"/> and <paramref name="considerCanExecuteProperty"/> must be <see langword="true"/>.
    /// </param>
    /// <param name="considerCanExecuteProperty">
    /// If <see langword="true"/> (the default), a can-execute property named according to <paramref name="matchCanExecute"/> will be considered.
    /// At least one of <paramref name="considerCanExecuteMethod"/> and <paramref name="considerCanExecuteProperty"/> must be <see langword="true"/>.
    /// </param>
    public void ApplyRegexNamingConvention(
        string key,
        string diagnosticName,
        string? matchCommandName,
        string? commandPropertyNamePattern,
        string? matchCanExecute,
        int priority = 0,
        bool? requireCanExecuteMatch = null,
        bool considerCanExecuteMethod = true,
        bool considerCanExecuteProperty = true )
    {
        if ( key == DefaultCommandNamingConvention.RegistrationKey )
        {
            throw new InvalidOperationException( "The default naming convention cannot be modified." );
        }

        this._options = this._options with
        {
            NamingConventionRegistrations =
            this._options.NamingConventionRegistrations.AddOrApplyChanges(
                new NamingConventionRegistration<ICommandNamingConvention>(
                    key,
                    new RegexCommandNamingConvention(
                        diagnosticName,
                        matchCommandName,
                        commandPropertyNamePattern,
                        matchCanExecute,
                        requireCanExecuteMatch ?? matchCanExecute != null,
                        considerCanExecuteMethod,
                        considerCanExecuteProperty ),
                    priority ) )
        };
    }

    public void SetNamingConventionPriority( string key, int priority )
    {
        this._options = this._options with
        {
            NamingConventionRegistrations =
            this._options.NamingConventionRegistrations.AddOrApplyChanges( new NamingConventionRegistration<ICommandNamingConvention>( key, null, priority ) )
        };
    }

    public void RemoveNamingConvention( string key )
    {
        this._options = this._options with
        {
            NamingConventionRegistrations =
            this._options.NamingConventionRegistrations.Remove( key )
        };
    }

    /// <summary>
    /// Resets naming convention registrations to the default state, removing any user-registered naming conventions.
    /// </summary>
    public void ResetNamingConventions()
    {
        this._options = this._options with
        {
            NamingConventionRegistrations =
            this._options.NamingConventionRegistrations
                .ApplyChanges( IncrementalKeyedCollection.Clear<string, NamingConventionRegistration<ICommandNamingConvention>>(), default )
                .AddOrApplyChanges( CommandOptions.DefaultNamingConventionRegistrations() )
        };
    }

    /// <summary>
    /// Gets or sets a value indicating whether integration with <see cref="INotifyPropertyChanged"/> is enabled. The default is <see langword="true"/>.
    /// </summary>
    /// <remarks>
    /// <para>
    /// When <see cref="EnableINotifyPropertyChangedIntegration"/> is <see langword="true"/> (the default), and when a can-execute property (not a method) is used,
    /// and when the containing type of the target property implements <see cref="INotifyPropertyChanged"/>,then the <see cref="ICommand.CanExecuteChanged"/> event of 
    /// the command will be raised when the can-execute property changes. A warning is reported if the can-execute property is not public because <see cref="INotifyPropertyChanged"/>
    /// implementations typically only notify changes to public properties.
    /// </para>
    /// </remarks>
    public bool? EnableINotifyPropertyChangedIntegration
    {
        get => this._options.EnableINotifyPropertyChangedIntegration;
        set => this._options = this._options with { EnableINotifyPropertyChangedIntegration = value };
    }

    internal CommandOptions Build() => this._options;
}