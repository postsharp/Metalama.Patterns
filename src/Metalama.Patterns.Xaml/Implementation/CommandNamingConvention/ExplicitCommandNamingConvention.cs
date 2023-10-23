// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Aspects;
using Metalama.Framework.Code;

namespace Metalama.Patterns.Xaml.Implementation.CommandNamingConvention;

[CompileTime]
internal sealed class ExplicitCommandNamingConvention : ICommandNamingConvention
{
    private readonly string? _commandPropertyName;
    private readonly string? _canExecuteMethodName;
    private readonly string? _canExecutePropertyName;

    public ExplicitCommandNamingConvention( string? commandPropertyName, string? canExecuteMethodName, string? canExecutePropertyName )
    {
        this._commandPropertyName = commandPropertyName;
        this._canExecuteMethodName = canExecuteMethodName;
        this._canExecutePropertyName = canExecutePropertyName;
    }

    public string DiagnosticName => "explicitly-configured";

    public bool Equals( ICommandNamingConvention? other )
        => ReferenceEquals( this, other ) || (
        other is ExplicitCommandNamingConvention c
        && c._commandPropertyName == this._commandPropertyName
        && c._canExecuteMethodName == this._canExecuteMethodName
        && c._canExecutePropertyName == this._canExecutePropertyName);

    public CommandNamingConventionMatch Match<TContextImpl>( in IMethod executeMethod, in TContextImpl context )
        where TContextImpl : ICommandNamingMatchContext
    {
        var commandName = DefaultCommandNamingConvention.GetCommandNameFromExecuteMethodName( executeMethod.Name );
        var commandPropertyName = this._commandPropertyName ?? DefaultCommandNamingConvention.GetCommandPropertyNameFromCommandName( commandName );

        return CommandNamingConventionHelper.Match(
            this,
            executeMethod,
            context,
            commandPropertyName,
            this._canExecuteMethodName ?? this._canExecutePropertyName ?? DefaultCommandNamingConvention.GetCanExecuteNameFromCommandName( commandName ),
            considerMethod: this._canExecuteMethodName != null || this._canExecutePropertyName == null,
            considerProperty: this._canExecutePropertyName != null || this._canExecuteMethodName == null,
            requireCanExecuteMatch: this._canExecuteMethodName != null || this._canExecutePropertyName != null );
    }
}