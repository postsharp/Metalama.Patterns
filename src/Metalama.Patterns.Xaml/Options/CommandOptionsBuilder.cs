// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using JetBrains.Annotations;
using Metalama.Framework.Aspects;
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

    public static class Names
    {
        public const string CommandNameGroup = RegexCommandNamingConvention.CommandNameGroup;
        public const string CommandNameToken = RegexCommandNamingConvention.CommandNameToken;
    }

    /// <summary>
    /// Registers a naming convention using a regular expression to select the command name from the target method of the <see cref="CommandAttribute"/>,
    /// and a substituion pattern to determine the command property name and the name of the can-execute method or property.
    /// </summary>
    /// <param name="conventionName">A short name describing the convention. This is used when reporting diagnostics.</param>
    /// <param name="matchCommandName">
    /// A regex match expression that will be evaluated against the name of the method upon which <see cref="CommandAttribute"/> is applied. 
    /// The expression should yield a match group named <see cref="Names.CommandNameGroup"/>. If <paramref name="matchCommandName"/> is <see langword="null"/>,
    /// the name produced by <see cref="DefaultCommandNamingConvention.GetCommandNameFromExecuteMethodName(string)"/> is used.
    /// </param>
    /// <param name="commandPropertyNamePattern">
    /// The name of the command property. A string in which the substring <see cref="Names.CommandNameToken"/> will be replaced with the command name.
    /// </param>
    /// <param name="canExecuteNamePattern">
    /// The name of the can-execute member. A string in which the substring <see cref="Names.CommandNameToken"/> will be replaced with the command name.
    /// </param>
    /// <param name="priority">
    /// The priority of the naming convention. The default priority is 0. The system-registered default naming convention has priority 100. Naming conventions are
    /// matched in ascending priority. The first successful match is used.
    /// </param>
    /// <param name="requireCanExecuteMatch">
    /// If <see langword="true"/> (the default), a matching valid unambiguous can-execute method or property must be found for a match to be considered successful.
    /// </param>
    /// <param name="considerCanExecuteMethod">
    /// If <see langword="true"/> (the default), can-execute methods named according to <paramref name="canExecuteNamePattern"/> will be considered.
    /// At least one of <paramref name="considerCanExecuteMethod"/> and <paramref name="considerCanExecuteProperty"/> must be <see langword="true"/>.
    /// </param>
    /// <param name="considerCanExecuteProperty">
    /// If <see langword="true"/> (the default), a can-execute property named according to <paramref name="canExecuteNamePattern"/> will be considered.
    /// At least one of <paramref name="considerCanExecuteMethod"/> and <paramref name="considerCanExecuteProperty"/> must be <see langword="true"/>.
    /// </param>
    public void RegisterRegexNamingConvention(
        string conventionName,
        string? matchCommandName,
        string commandPropertyNamePattern,
        string canExecuteNamePattern,
        int priority = 0,
        bool requireCanExecuteMatch = true,
        bool considerCanExecuteMethod = true,
        bool considerCanExecuteProperty = true )
    {
        this.RegisterOrUpdateNamingConvention( 
            new RegexCommandNamingConvention(
                conventionName,
                matchCommandName,
                commandPropertyNamePattern,
                canExecuteNamePattern,
                requireCanExecuteMatch: requireCanExecuteMatch,
                considerCanExecuteMethod: considerCanExecuteMethod,
                considerCanExecuteProperty: considerCanExecuteProperty ), 
            priority );
    }

    private void RegisterOrUpdateNamingConvention( ICommandNamingConvention namingConvention, int priority = 0 )
    {
        this._options = this._options with
        {
            NamingConventionRegistrations =
                this._options.NamingConventionRegistrations.AddOrApplyChanges( new NamingConventionRegistration<ICommandNamingConvention>( namingConvention, priority ) )
        };
    }

    private void UnregisterNamingConvention( ICommandNamingConvention namingConvention )
    {
        this._options = this._options with
        {
            NamingConventionRegistrations = this._options.NamingConventionRegistrations.Remove( namingConvention )
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
    public bool? EnableINotifyPropertyChangedIntegration { get; set; }

    internal CommandOptions Build() => this._options;
}