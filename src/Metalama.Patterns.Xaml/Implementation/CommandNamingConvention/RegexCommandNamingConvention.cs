// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Aspects;
using Metalama.Framework.Code;
using Metalama.Framework.Serialization;
using Metalama.Patterns.Xaml.Implementation.NamingConvention;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace Metalama.Patterns.Xaml.Implementation.CommandNamingConvention;

// Prevent netframework-only false positives

#if NETFRAMEWORK
#pragma warning disable CS8604 // Possible null reference argument.
#endif

[CompileTime]
internal sealed class RegexCommandNamingConvention : ICommandNamingConvention
{
    private const string _commandNameGroup = "CommandName";
    private const string _commandNameToken = "{CommandName}";

    private readonly string? _commandNamePattern;
    private readonly string? _commandPropertyPattern;
    private readonly string? _canExecutePattern;
    private readonly bool _requireCanExecuteMatch;
    private readonly bool _considerCanExecuteMethod;
    private readonly bool _considerCanExecuteProperty;

    [NonCompileTimeSerialized]
    private Regex? _commandNameRegex;

    /// <summary>
    /// Initializes a new instance of the <see cref="RegexCommandNamingConvention"/> class.
    /// </summary>
    /// <param name="name">A short name describing the convention, used when reporting diagnostics.</param>
    /// <param name="commandNamePattern">
    /// A regex match expression that will be evaluated against the name of the target method of the <see cref="CommandAttribute"/> aspect. 
    /// The expression should yield a match group named <see cref="_commandNameGroup"/>. If <paramref name="commandNamePattern"/> is <see langword="null"/>,
    /// the name produced by <see cref="DefaultCommandNamingConvention.GetCommandNameFromExecuteMethodName(string)"/> is used.
    /// </param>
    /// <param name="commandPropertyPattern">
    /// The name of the command property to be introduced. A string in which the substring <see cref="_commandNameToken"/> will be replaced with the
    /// command name determined according to <paramref name="commandNamePattern"/>. If <paramref name="commandPropertyPattern"/> is <see langword="null"/>,
    /// the name produced by <see cref="DefaultCommandNamingConvention.GetCommandPropertyNameFromCommandName(string)"/> is used.
    /// </param>
    /// <param name="canExecutePattern">
    /// <para>
    /// A regex match expression that will be evaluated against method and/or property names to identify candidate can-execute members. 
    /// All occurrences of the substring <see cref="_commandNameToken"/> in <paramref name="canExecutePattern"/> will be replaced with the command name
    /// determined according to <paramref name="commandNamePattern"/> before the expression is evaluated.
    /// </para>
    /// <para>
    /// If <paramref name="canExecutePattern"/> is <see langword="null"/>, the name produced by
    /// <see cref="DefaultCommandNamingConvention.GetCanExecuteNameFromCommandName(string)"/> is matched.
    /// </para>
    /// </param>
    /// <param name="requireCanExecuteMatch">If <see langword="true"/> (the default), a matching can-execute method or property is required
    /// for a match to be considered successful.</param>
    /// <param name="considerCanExecuteMethod">
    /// If <see langword="true"/> (the default), can-execute methods named according to <paramref name="canExecutePattern"/> will be considered.
    /// At least one of <paramref name="considerCanExecuteMethod"/> and <paramref name="considerCanExecuteProperty"/> must be <see langword="true"/>.
    /// </param>
    /// <param name="considerCanExecuteProperty">
    /// If <see langword="true"/> (the default), a can-execute property named according to <paramref name="canExecutePattern"/> will be considered.
    /// At least one of <paramref name="considerCanExecuteMethod"/> and <paramref name="considerCanExecuteProperty"/> must be <see langword="true"/>.
    /// </param>
    public RegexCommandNamingConvention(
        string name,
        string? commandNamePattern,
        string? commandPropertyPattern,
        string? canExecutePattern,
        bool requireCanExecuteMatch = true,
        bool considerCanExecuteMethod = true,
        bool considerCanExecuteProperty = true )
    {
        if ( !(considerCanExecuteMethod || considerCanExecuteProperty) )
        {
            throw new ArgumentException(
                "At least one of " + nameof(considerCanExecuteMethod) + " and " + nameof(considerCanExecuteProperty) + " must be true." );
        }

        if ( string.IsNullOrWhiteSpace( name ) )
        {
            throw new ArgumentException( "Must not be null, empty or only white space.", nameof(name) );
        }

        this.Name = name;
        this._commandNamePattern = commandNamePattern;
        this._commandPropertyPattern = commandPropertyPattern;
        this._canExecutePattern = canExecutePattern;
        this._requireCanExecuteMatch = requireCanExecuteMatch;
        this._considerCanExecuteMethod = considerCanExecuteMethod;
        this._considerCanExecuteProperty = considerCanExecuteProperty;
    }

    public string Name { get; }

    public CommandNamingConventionMatch Match( IMethod executeMethod, InspectedMemberAdder inspectedMember )
    {
        Debugger.Break();

        string? commandName = null;

        if ( this._commandNamePattern != null )
        {
            this._commandNameRegex ??= new Regex( this._commandNamePattern );

            var m = this._commandNameRegex.Match( executeMethod.Name );

            if ( m.Success )
            {
                var g = m.Groups[_commandNameGroup];

                if ( g.Success )
                {
                    commandName = g.Value;
                }
            }
        }
        else
        {
            commandName = DefaultCommandNamingConvention.GetCommandNameFromExecuteMethodName( executeMethod.Name );
        }

        if ( string.IsNullOrWhiteSpace( commandName ) )
        {
            return new CommandNamingConventionMatch(
                this,
                null,
                MemberMatch<IMemberOrNamedType, DefaultMatchKind>.Invalid(),
                MemberMatch<IMember, DefaultMatchKind>.NotFound(),
                this._requireCanExecuteMatch );
        }

#if NETCOREAPP
#pragma warning disable CA1307 // Specify StringComparison for clarity
#endif
        var commandPropertyName = this._commandPropertyPattern != null
            ? this._commandPropertyPattern.Replace( _commandNameToken, commandName )
            : DefaultCommandNamingConvention.GetCommandPropertyNameFromCommandName( commandName );

        var canExecutePredicate = this._canExecutePattern != null
            ? (INameMatchPredicate) new RegexNameMatchPredicate( new Regex( this._canExecutePattern.Replace( _commandNameToken, commandName ) ) )
            : new StringNameMatchPredicate( DefaultCommandNamingConvention.GetCanExecuteNameFromCommandName( commandName ) );

#if NETCOREAPP
#pragma warning restore CA1307 // Specify StringComparison for clarity
#endif

        return CommandNamingConventionMatcher.Match(
            this,
            executeMethod,
            inspectedMember,
            commandPropertyName,
            canExecutePredicate,
            considerMethod: this._considerCanExecuteMethod,
            considerProperty: this._considerCanExecuteProperty,
            requireCanExecuteMatch: this._requireCanExecuteMatch );
    }
}