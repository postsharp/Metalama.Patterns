// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Aspects;
using Metalama.Framework.Code;
using Metalama.Patterns.Xaml.Implementation.NamingConvention;

namespace Metalama.Patterns.Xaml.Implementation.CommandNamingConvention;

[CompileTime]
internal sealed class DefaultCommandNamingConvention : ICommandNamingConvention
{
    public static string RegistrationKey { get; } = "{43954F4F-1606-4A44-9DEB-41E7C686C149}";

    public string DiagnosticName => "default";

    public CommandNamingConventionMatch Match<TContextImpl>( in IMethod executeMethod, in TContextImpl context )
        where TContextImpl : ICommandNamingMatchContext
    {
        var commandName = GetCommandNameFromExecuteMethodName( executeMethod.Name );

        var commandPropertyName = GetCommandPropertyNameFromCommandName( commandName );

        var canExecuteName = GetCanExecuteNameFromCommandName( commandName );

        return CommandNamingConventionHelper.Match( this, executeMethod, context, commandPropertyName, new StringNameMatchPredicate( canExecuteName ) );
    }

    public static string GetCommandNameFromExecuteMethodName( string name )
    {
        var useName = name;

        TrimStart( ref useName, "_", StringComparison.OrdinalIgnoreCase );
        TrimStart( ref useName, "m_", StringComparison.OrdinalIgnoreCase );
        TrimStart( ref useName, "execute", StringComparison.OrdinalIgnoreCase );
        TrimStart( ref useName, "_", StringComparison.OrdinalIgnoreCase );

        _ = TrimEnd( ref useName, "_command", StringComparison.OrdinalIgnoreCase )
            || TrimEnd( ref useName, "Command", StringComparison.Ordinal );

        if ( string.IsNullOrEmpty( useName ) )
        {
            // It's an unusual name comprised of expected prefixes and/or suffixes.
            // Just use it as-is.

            return name;
        }

        if ( char.IsLower( useName[0] ) )
        {
            useName = char.ToUpperInvariant( useName[0] ) + useName.Substring( 1 );
        }

        return useName;
    }

    public static bool TrimStart( ref string s, string trim, StringComparison stringComparison )
    {
        if ( s.StartsWith( trim, stringComparison ) )
        {
            s = s.Substring( trim.Length );
            return true;
        }

        return false;
    }

    public static bool TrimEnd( ref string s, string trim, StringComparison stringComparison )
    {
        if ( s.EndsWith( trim, stringComparison ) )
        {
            s = s.Substring( 0, s.Length - trim.Length );
            return true;
        }

        return false;
    }

    public static string GetCommandPropertyNameFromCommandName( string commandName )
        => $"{commandName}Command";

    public static string GetCanExecuteNameFromCommandName( string commandName )
        => $"CanExecute{commandName}";
}