﻿// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Advising;
using Metalama.Framework.Aspects;
using Metalama.Framework.Code;
using Metalama.Framework.Code.SyntaxBuilders;
using Metalama.Framework.Eligibility;
using Metalama.Framework.Project;
using Metalama.Patterns.Xaml.Implementation;
using Metalama.Patterns.Xaml.Implementation.CommandNamingConvention;
using Metalama.Patterns.Xaml.Implementation.NamingConvention;
using Metalama.Patterns.Xaml.Options;
using System.ComponentModel;
using System.Windows.Input;
using MetalamaAccessibility = Metalama.Framework.Code.Accessibility;

// TODO: Skip [Observable] on [Command]-targeted auto properties. No functional impact, would just avoid unnecessary generated code.

[assembly: AspectOrder( "Metalama.Patterns.Xaml.CommandAttribute:*", "Metalama.Patterns.Observability.ObservableAttribute:*" )]

namespace Metalama.Patterns.Xaml;

[AttributeUsage( AttributeTargets.Method )]
public sealed partial class CommandAttribute : Attribute, IAspect<IMethod>
{
    internal const string CommandPropertyCategory = "command property";
    internal const string CanExecuteMethodCategory = "can-execute method";
    internal const string CanExecutePropertyCategory = "can-execute property";

    /// <summary>
    /// Gets or sets the name of the <see cref="ICommand"/> property that is introduced.
    /// </summary>
    public string? CommandPropertyName { get; set; }

    /// <summary>
    /// Gets or sets the name of the method that is called to determine whether the command can be executed.
    /// This method corresponds to the <see cref="ICommand.CanExecute"/> method.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The <c>CanExecute</c> method must be declared in the same class as the command property, return a <c>bool</c> value and can have zero or one parameter.
    /// </para>
    /// <para>
    /// If this property is not set, then the aspect will look for a method named <c>CanFoo</c> or <c>CanExecuteFoo</c>, where <c>Foo</c> is the name of the command without the <c>Command</c> suffix.
    /// </para>
    /// <para>
    /// At most one of the <see cref="CanExecuteMethod"/> and <see cref="CanExecuteProperty"/> properties may be set.
    /// </para>
    /// </remarks>
    public string? CanExecuteMethod { get; set; }

    /// <summary>
    /// Gets or sets the name of the property that is evaluated to determine whether the command can be executed.
    /// This property corresponds to the <see cref="ICommand.CanExecute"/> method.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The <c>CanExecute</c> property must be declared in the same class as the command property and return a <c>bool</c> value.
    /// </para>
    /// <para>
    /// If this property is not set, then the aspect will look for a property named <c>CanFoo</c> or <c>CanExecuteFoo</c>, where <c>Foo</c> is the name of the command without the <c>Command</c> suffix.
    /// </para>
    /// <para>
    /// At most one of the <see cref="CanExecuteMethod"/> and <see cref="CanExecuteProperty"/> properties may be set.
    /// </para>
    /// </remarks>
    public string? CanExecuteProperty { get; set; }

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

    void IEligible<IMethod>.BuildEligibility( IEligibilityBuilder<IMethod> builder )
    {
        builder.ReturnType().MustBe( typeof(void) );
        builder.MustSatisfy( m => m.Parameters.Count is 0 or 1, m => $"{m} must have zero or one parameter" );
        builder.MustNotHaveRefOrOutParameter();
        builder.MustSatisfy( m => m.TypeParameters.Count == 0, m => $"{m} must not be generic" );
    }

    void IAspect<IMethod>.BuildAspect( IAspectBuilder<IMethod> builder )
    {
        var target = builder.Target;
        var declaringType = target.DeclaringType;
        var options = target.Enhancements().GetOptions<CommandOptions>();

        if ( this is { CanExecuteMethod: not null, CanExecuteProperty: not null } )
        {
            builder.Diagnostics.Report( Diagnostics.ErrorCannotSpecifyBothCanExecuteMethodAndCanExecuteProperty );

            // Further diagnostics would be confusing and transformation is not possible.

            return;
        }

        var hasExplicitCanExecuteNaming = this.CanExecuteMethod != null || this.CanExecuteProperty != null;

        var ncResult = hasExplicitCanExecuteNaming
            ? NamingConventionEvaluator.Evaluate(
                new ExplicitCommandNamingConvention( this.CommandPropertyName, this.CanExecuteMethod, this.CanExecuteProperty ),
                target )
            : NamingConventionEvaluator.Evaluate( options.GetSortedNamingConventions(), target );

        ncResult.ReportDiagnostics( new DiagnosticReporter( builder ) );

        var successfulMatch = ncResult.SuccessfulMatch?.Match;

        var canTransform = true;
        IProperty? commandProperty = null;
        IMethod? canExecuteMethod = null;
        IProperty? canExecuteProperty = null;

        if ( successfulMatch != null )
        {
            switch ( successfulMatch.CanExecuteMatch.Declaration )
            {
                case null:
                    break;

                case IProperty property:
                    canExecuteProperty = property;

                    break;

                case IMethod method:
                    canExecuteMethod = method;

                    break;

                default:
                    throw new NotSupportedException( "Expected a method or property." );
            }

            var introducePropertyResult = builder.Advice.IntroduceProperty(
                declaringType,
                nameof(CommandProperty),
                IntroductionScope.Instance,
                OverrideStrategy.Fail,
                b =>
                {
                    b.Name = successfulMatch.CommandPropertyName!;

                    // ReSharper disable once RedundantNameQualifier
                    b.Accessibility = MetalamaAccessibility.Public;

                    // ReSharper disable once RedundantNameQualifier
                    b.GetMethod!.Accessibility = MetalamaAccessibility.Public;
                } );

            if ( introducePropertyResult.Outcome == AdviceOutcome.Default )
            {
                commandProperty = introducePropertyResult.Declaration;
            }
            else
            {
                canTransform = false;
            }
        }
        else
        {
            canTransform = false;
        }

        var useInpcIntegration = false;

        if ( canTransform && canExecuteProperty != null && options.EnableINotifyPropertyChangedIntegration == true )
        {
            if ( declaringType.AllImplementedInterfaces.Contains( typeof(INotifyPropertyChanged) ) )
            {
                // ReSharper disable once RedundantNameQualifier
                if ( canExecuteProperty.Accessibility != MetalamaAccessibility.Public )
                {
                    builder.Diagnostics.Report(
                        Diagnostics.WarningCommandNotifiableCanExecutePropertyIsNotPublic.WithArguments( target ),
                        canExecuteProperty );
                }

                useInpcIntegration = true;
            }
        }

        if ( !canTransform || !MetalamaExecutionContext.Current.ExecutionScenario.CapturesNonObservableTransformations )
        {
            if ( canTransform )
            {
                builder.Diagnostics.Suppress( Suppressions.SuppressRemoveUnusedPrivateMembersIDE0051, target );

                if ( canExecuteMethod != null )
                {
                    builder.Diagnostics.Suppress( Suppressions.SuppressRemoveUnusedPrivateMembersIDE0051, canExecuteMethod );
                }

                if ( canExecuteProperty != null )
                {
                    builder.Diagnostics.Suppress( Suppressions.SuppressRemoveUnusedPrivateMembersIDE0051, canExecuteProperty );
                }
            }

            return;
        }

        builder.Advice.AddInitializer(
            declaringType,
            nameof(InitializeCommand),
            InitializerKind.BeforeInstanceConstructor,
            args: new
            {
                commandProperty,
                executeMethod = target,
                canExecuteMethod,
                canExecuteProperty,
                useInpcIntegration
            } );
    }

    // ReSharper disable once UnassignedGetOnlyAutoProperty
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    [Template]
    private static ICommand CommandProperty { get; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    [Template]
    private static void InitializeCommand(
        [CompileTime] IProperty commandProperty,
        [CompileTime] IMethod executeMethod,
        [CompileTime] IMethod? canExecuteMethod,
        [CompileTime] IProperty? canExecuteProperty,
        [CompileTime] bool useInpcIntegration )
    {
        IExpression? canExecuteExpression = null;

        if ( canExecuteMethod != null || canExecuteProperty != null )
        {
            bool CanExecute( object? parameter )
            {
                if ( canExecuteMethod != null )
                {
                    if ( canExecuteMethod.Parameters.Count == 0 )
                    {
                        return canExecuteMethod.Invoke();
                    }
                    else
                    {
                        return canExecuteMethod.Invoke( meta.Cast( canExecuteMethod.Parameters[0].Type, parameter ) );
                    }
                }
                else
                {
                    return canExecuteProperty!.Value;
                }
            }

            canExecuteExpression = ExpressionFactory.Capture( (Func<object, bool>) CanExecute );
        }

        void Execute( object? parameter )
        {
            if ( executeMethod.Parameters.Count == 0 )
            {
                executeMethod.Invoke();
            }
            else
            {
                executeMethod.Invoke( meta.Cast( executeMethod.Parameters[0].Type, parameter ) );
            }
        }

        if ( useInpcIntegration )
        {
            commandProperty.Value = new DelegateCommand( (Action<object>) Execute, canExecuteExpression!.Value, meta.This, canExecuteProperty!.Name );
        }
        else
        {
            // ReSharper disable once MergeConditionalExpression
#pragma warning disable IDE0031 // Use null propagation
            commandProperty.Value = new DelegateCommand( (Action<object>) Execute, canExecuteExpression == null ? null : canExecuteExpression.Value );
#pragma warning restore IDE0031 // Use null propagation
        }
    }
}