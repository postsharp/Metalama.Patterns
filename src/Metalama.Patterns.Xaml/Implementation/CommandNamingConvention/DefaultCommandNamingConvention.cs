// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Aspects;
using Metalama.Framework.Code;

namespace Metalama.Patterns.Xaml.Implementation.CommandNamingConvention;

[CompileTime]
internal sealed class DefaultCommandNamingConvention : ICommandNamingConvention
{
    public string DiagnosticName => "default";

    bool IEquatable<ICommandNamingConvention>.Equals( ICommandNamingConvention? other )
    {
        return other is DefaultCommandNamingConvention;
    }

    public CommandNamingConventionMatch Match<TContextImpl>( in IMethod executeMethod, in TContextImpl context )
        where TContextImpl : ICommandNamingMatchContext
    {
        var commandName = GetCommandNameFromExecuteMethodName( executeMethod.Name );

        var commandPropertyName = GetCommandPropertyNameFromCommandName( commandName );

        var canExecuteName = GetCanExecuteNameFromCommandName( commandName );

        return CommandNamingConventionHelper.Match( this, executeMethod, context, commandPropertyName, canExecuteName );
    }

    public static string GetCommandNameFromExecuteMethodName( string name )
    {
        var useName = name;

        _ = TrimStartIgnoringCase( ref useName, "execute" )
            || TrimStartIgnoringCase( ref useName, "m_execute" )
            || TrimStartIgnoringCase( ref useName, "m_" );

        _ = TrimEnd( ref useName, "Command" )
            || TrimEnd( ref useName, "_command" );

        if ( string.IsNullOrEmpty( useName ) )
        {
            useName = name;
        }

        return useName;
    }

    public static bool TrimStartIgnoringCase( ref string s, string trim )
    {
        if ( s.StartsWith( trim, StringComparison.OrdinalIgnoreCase ) )
        {
            s = s.Substring( trim.Length );
            return true;
        }

        return false;
    }

    public static bool TrimEnd( ref string s, string trim )
    {
        if ( s.EndsWith( trim, StringComparison.Ordinal ) )
        {
            s = s.Substring( s.Length - trim.Length );
            return true;
        }

        return false;
    }

    public static string GetCommandPropertyNameFromCommandName( string commandName )
        => $"{commandName}Command";

    public static string GetCanExecuteNameFromCommandName( string commandName )
        => $"CanExecute{commandName}";
}