// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Aspects;
using Metalama.Framework.Code;
using Metalama.Framework.Serialization;
using Metalama.Patterns.Xaml.Implementation.NamingConvention;
using System.Text.RegularExpressions;

namespace Metalama.Patterns.Xaml.Implementation.CommandNamingConvention;

[CompileTime]
internal sealed class RegexCommandNamingConvention : ICommandNamingConvention
{
    private readonly string _conventionName;
    private readonly string? _matchCommandName;
    private readonly string _commandPropertyNamePattern;
    private readonly string _matchCanExecuteName;
    private readonly bool _requireCanExecuteMatch;
    private readonly bool _considerCanExecuteMethod;
    private readonly bool _considerCanExecuteProperty;

    public const string CommandNameGroup = "CommandName";
    public const string CommandNameToken = "$" + CommandNameGroup + "$";

    [NonCompileTimeSerialized]
    private Regex? _matchCommandNameRegex;

    /// <summary>
    /// Initializes a new instance of the <see cref="RegexCommandNamingConvention"/> class.
    /// </summary>
    /// <param name="conventionName">A short name describing the convention. This is used when reporting diagnostics.</param>
    /// <param name="matchCommandName">
    /// A regex match expression that will be evaluated against the name of the method upon which <see cref="CommandAttribute"/> is applied. 
    /// The expression should yield a match group named <see cref="CommandNameGroup"/>. If <paramref name="matchCommandName"/> is <see langword="null"/>,
    /// the name produced by <see cref="DefaultCommandNamingConvention.GetCommandNameFromExecuteMethodName(string)"/> is used.
    /// </param>
    /// <param name="commandPropertyNamePattern">
    /// The name of the command property. A string in which the substring <see cref="CommandNameToken"/> will be replaced with the command name.
    /// </param>
    /// <param name="matchCanExecuteName">
    /// A regex match expression that will be evauluated against method and/or property names to locate candidate can-execute members. 
    /// All occurences of the substring <see cref="CommandNameToken"/> in <paramref name="matchCanExecuteName"/> will be replaced with the command name
    /// before the expression is evaluated.
    /// </param>
    /// <param name="considerCanExecuteMethod">
    /// If <see langword="true"/> (the default), can-execute methods named according to <paramref name="matchCanExecuteName"/> will be considered.
    /// At least one of <paramref name="considerCanExecuteMethod"/> and <paramref name="considerCanExecuteProperty"/> must be <see langword="true"/>.
    /// </param>
    /// <param name="considerCanExecuteProperty">
    /// If <see langword="true"/> (the default), a can-execute property named according to <paramref name="matchCanExecuteName"/> will be considered.
    /// At least one of <paramref name="considerCanExecuteMethod"/> and <paramref name="considerCanExecuteProperty"/> must be <see langword="true"/>.
    /// </param>
    public RegexCommandNamingConvention(
        string conventionName,
        string? matchCommandName,
        string commandPropertyNamePattern,
        string matchCanExecuteName,
        bool requireCanExecuteMatch = true,
        bool considerCanExecuteMethod = true,
        bool considerCanExecuteProperty = true )
    {
        if ( !(considerCanExecuteMethod || considerCanExecuteProperty) )
        {
            throw new ArgumentException( "At least one of " + nameof( considerCanExecuteMethod ) + " and " + nameof( considerCanExecuteProperty ) + " must be true." );
        }

        this._conventionName = conventionName ?? throw new ArgumentNullException( nameof( conventionName ) );
        this._matchCommandName = matchCommandName;
        this._commandPropertyNamePattern = commandPropertyNamePattern ?? throw new ArgumentNullException( nameof( commandPropertyNamePattern ) );
        this._matchCanExecuteName = matchCanExecuteName ?? throw new ArgumentNullException ( nameof( matchCanExecuteName ) );
        this._requireCanExecuteMatch = requireCanExecuteMatch;
        this._considerCanExecuteMethod = considerCanExecuteMethod;
        this._considerCanExecuteProperty = considerCanExecuteProperty;
    }

    public string DiagnosticName => $"'{this._conventionName}' regex";
    
    public CommandNamingConventionMatch Match( IMethod executeMethod, InspectedDeclarationsAdder inspectedDeclarations )
    {
        string? commandName = null;

        if ( this._matchCommandName != null )
        {
            this._matchCommandNameRegex ??= new Regex( this._matchCommandName );

            var m = this._matchCommandNameRegex.Match( executeMethod.Name );
            if ( m.Success )
            {
                var g = m.Groups[CommandNameGroup];
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
            return new CommandNamingConventionMatch( this, null, DeclarationMatch<IMember>.NotFound(), this._requireCanExecuteMatch );
        }

#if NETCOREAPP
#pragma warning disable CA1307 // Specify StringComparison for clarity
#endif
        var commandPropertyName = this._commandPropertyNamePattern.Replace( CommandNameToken, commandName );
#if NETCOREAPP
#pragma warning restore CA1307 // Specify StringComparison for clarity
#endif

#if NETCOREAPP
#pragma warning disable CA1307 // Specify StringComparison for clarity
#endif
        var matchCanExecuteName = this._matchCanExecuteName.Replace( CommandNameToken, commandName );
#if NETCOREAPP
#pragma warning restore CA1307 // Specify StringComparison for clarity
#endif

        var matchCanExecuteNameRegex = new Regex( matchCanExecuteName );

        return CommandNamingConventionHelper.Match(
            this,
            executeMethod,
            inspectedDeclarations,
            commandPropertyName,
            new RegexNameMatchPredicate( matchCanExecuteNameRegex ),
            considerMethod: this._considerCanExecuteMethod,
            considerProperty: this._considerCanExecuteProperty,
            requireCanExecuteMatch: this._requireCanExecuteMatch );
    }
}