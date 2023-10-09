﻿// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Aspects;
using Metalama.Framework.Options;
using Metalama.Patterns.NotifyPropertyChanged.Implementation;

namespace Metalama.Patterns.NotifyPropertyChanged.Options;

[RunTimeOrCompileTime]
[AttributeUsage( AttributeTargets.Class | AttributeTargets.Assembly )]
public class NotifyPropertyChangedOptionsAttribute : Attribute, IHierarchicalOptionsProvider
{
    private int? _diagnosticCommentVerbosity;

    /// <summary>
    /// Gets or sets a value indicating the verbosity of diagnostic comments inserted into generated code. Must be a value
    /// between 0 and 3 (inclusive). 0 (default) inserts no comments, 3 is the most verbose.
    /// </summary>
    public int DiagnosticCommentVerbosity
    {
        get => this._diagnosticCommentVerbosity ?? 0;

        set
        {
            if ( value is < 0 or > 3 )
            {
                throw new ArgumentOutOfRangeException( nameof(value) );
            }

            this._diagnosticCommentVerbosity = value;
        }
    }

    public Type? ImplementationStrategyFactoryType { get; set; }

    IEnumerable<IHierarchicalOptions> IHierarchicalOptionsProvider.GetOptions( in OptionsProviderContext context )
    {
        IImplementationStrategyFactory? factory = null;

        if ( this.ImplementationStrategyFactoryType != null )
        {
            if ( typeof(IImplementationStrategyFactory).IsAssignableFrom( this.ImplementationStrategyFactoryType ) )
            {
                var ctor = this.ImplementationStrategyFactoryType.GetConstructor( Type.EmptyTypes );

                if ( ctor != null )
                {
                    factory = (IImplementationStrategyFactory) ctor.Invoke( null );
                }
                else
                {
                    context.Diagnostics.Report(
                        DiagnosticDescriptors.ErrorTypeMustHaveAPublicParameterlessConstructor.WithArguments(
                            nameof(this.ImplementationStrategyFactoryType) ) );
                }
            }
            else
            {
                context.Diagnostics.Report(
                    DiagnosticDescriptors.ErrorTypeMustImplementInterface.WithArguments(
                        (nameof(this.ImplementationStrategyFactoryType), nameof(IImplementationStrategyFactory)) ) );
            }
        }

        return new[]
        {
            new NotifyPropertyChangedOptions() { ImplementationStrategyFactory = factory, DiagnosticCommentVerbosity = this._diagnosticCommentVerbosity }
        };
    }
}