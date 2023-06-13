// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Project;

namespace Metalama.Patterns.Contracts;

public class ContractsOptions : ProjectExtension
{
    public override void Initialize( IProject project, bool isReadOnly )
    {
        base.Initialize( project, isReadOnly );

        // TODO: Review - naming convention.

        if ( project.TryGetProperty( "Metalama_Patterns_Contracts_EmulatePostSharp", out var value ) )
        {
            if ( bool.TryParse(value, out var boolValue ) )
            {
                this._emulatePostSharp = boolValue;
            }
        }
    }

    private bool _emulatePostSharp;

    public bool EmulatePostSharp
    {
        get => this._emulatePostSharp;

        set
        {
            if ( this.IsReadOnly )
            {
                throw new InvalidOperationException();
            }

            this._emulatePostSharp = value;
        }
    }

    /// <summary>
    /// TEMPORARY SOLUTION! Not sure if we'll do it this way.
    /// </summary>
    public ContractLocalizedTextProvider LocalizedTextProvider { get; } = new ContractLocalizedTextProvider( null );
}