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

// TODO: Skip [Observable] on [Command]-targeted auto properties. No functional impact, would just avoid unnecessary generated code.

[assembly: AspectOrder( "Metalama.Patterns.Xaml.CommandAttribute:*", "Metalama.Patterns.Observability.ObservableAttribute:*" )]

namespace Metalama.Patterns.Xaml;

[AttributeUsage( AttributeTargets.Method )]
public sealed partial class CommandAttribute : Attribute, IAspect<IMethod>
{
    private const string _canExecuteMethodCategory = "can-execute method";
    private const string _canExecutePropertyCategory = "can-execute property";

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
        builder.ReturnType().MustBe( typeof( void ) );
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

        var ncResult =
            this.CanExecuteMethod != null || this.CanExecuteProperty != null
                ? NamingConventionEvaluator<ICommandNamingMatchContext, NameMatchingContext>.Evaluate( new ExplicitCommandNamingConvention( this.CommandPropertyName, this.CanExecuteMethod, this.CanExecuteProperty ), target, default( NameMatchingContextFactory ) )
                : NamingConventionEvaluator<ICommandNamingMatchContext, NameMatchingContext>.Evaluate( options.GetSortedNamingConventions(), target, default( NameMatchingContextFactory ) );

        var successfulMatch = ncResult.SuccessfulMatch;       

        var canTransform = true;
        IProperty? commandProperty = null;
        IMethod? canExecuteMethod = null;
        IProperty? canExecuteProperty = null;

        if ( successfulMatch != null )
        {            
            if ( successfulMatch.CanExecuteMatch.Outcome == DeclarationMatchOutcome.NotFound && successfulMatch.CanExecuteMatch.CandidateNames != null )
            {
                // Report candidate names with a specific warning for easy suppression, as this is a
                // weaker warning and more likley to be an intended scenario.

                var names = string.Join( ", ", successfulMatch.CanExecuteMatch.CandidateNames );

                if ( names.Length > 0 )
                {
                    builder.Diagnostics.Report(
                        Diagnostics.WarningCandidateNamesNotFound.WithArguments(
                            (
                                $"{_canExecuteMethodCategory} or {_canExecutePropertyCategory}",                                
                                successfulMatch.NamingConvention.DiagnosticName,
                                names
                            ) ) );
                }
            }
            
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

            // Check for a conflicting member name explicitly because introduction won't fail unless the conflict comes from another field.

            var conflictingMember = (IMemberOrNamedType?) declaringType.AllMembers().FirstOrDefault( m => m.Name == successfulMatch.CommandPropertyName )
                                    ?? declaringType.NestedTypes.FirstOrDefault( t => t.Name == successfulMatch.CommandPropertyName );

            if ( conflictingMember != null )
            {
                builder.Diagnostics.Report(
                    Diagnostics.ErrorRequiredCommandPropertyNameIsAlreadyUsed.WithArguments(
                        (conflictingMember, declaringType, successfulMatch.CommandPropertyName!) ) );

                canTransform = false;
            }
            else
            {   
                var introducePropertyResult = builder.Advice.IntroduceProperty(
                    declaringType,                    
                    nameof(CommandProperty),
                    IntroductionScope.Instance,
                    OverrideStrategy.Fail,
                    b =>
                    {
                        b.Name = successfulMatch.CommandPropertyName!;

                        // ReSharper disable once RedundantNameQualifier
                        b.Accessibility = Framework.Code.Accessibility.Public;

                        // ReSharper disable once RedundantNameQualifier
                        b.GetMethod!.Accessibility = Framework.Code.Accessibility.Public;
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
        }
        else
        {
            canTransform = false;

            if ( ncResult.UnsuccessfulMatches != null )
            {
                foreach ( var um in ncResult.UnsuccessfulMatches )
                {
                    if ( um.Match.CanExecuteMatch.Outcome == DeclarationMatchOutcome.Ambiguous )
                    {                        
                        // Report the ambiguous (valid) matches.

                        foreach ( var c in um.InspectedDeclarations.Where( i => i.IsValid ) )
                        {
                            builder.Diagnostics.Report(
                                Diagnostics.WarningValidCandidateMemberIsAmbiguous.WithArguments(
                                    (
                                    c.Declaration.DeclarationKind,
                                    c.Category,
                                    "[Command] method ",
                                    target,
                                    um.Match.NamingConvention.DiagnosticName
                                    ) ),
                                c.Declaration );
                        }
                    }
                    else if ( um.Match.CanExecuteMatch.Outcome == DeclarationMatchOutcome.Invalid )
                    {
                        // Report invalid inspections, as these are strong candidates for being intended matches.

                        foreach ( var c in um.InspectedDeclarations.Where( i => !i.IsValid ))
                        {
                            builder.Diagnostics.Report( 
                                Diagnostics.WarningInvalidCandidateMemberSignature.WithArguments(
                                    (
                                    c.Declaration.DeclarationKind, 
                                    c.Category, 
                                    "[Command] method ", 
                                    target, 
                                    um.Match.NamingConvention.DiagnosticName,
                                    c.Declaration.DeclarationKind == DeclarationKind.Property
                                        ? " The property must be of type bool and have a getter."
                                        : " The method must not be generic, must return bool and may optionally have a single parameter of any type, but which must not be a ref or out parameter."
                                    ) ),
                                c.Declaration );
                        }
                    }
                }
            }
        }

        if ( hasExplicitCanExecuteNaming && successfulMatch?.CanExecuteMatch.Outcome != DeclarationMatchOutcome.Success )
        {
            builder.Diagnostics.Report( Diagnostics.ErrorMemberNotFound.WithArguments(
                (
                $"unambiguous valid explicitly-configured can-execute {(this.CanExecuteMethod != null ? "method" : "property")} named '{this.CanExecuteMethod ?? this.CanExecuteProperty}'",
                declaringType
                ) ) );
        }

        var useInpcIntegration = false;

        if ( canTransform && canExecuteProperty != null && options.EnableINotifyPropertyChangedIntegration == true )
        {
            if ( declaringType.AllImplementedInterfaces.Contains( typeof(INotifyPropertyChanged) ) )
            {
                // ReSharper disable once RedundantNameQualifier
                if ( canExecuteProperty.Accessibility != Framework.Code.Accessibility.Public )
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

    private static bool IsValidCanExecuteMethod( IMethod method )
        => method.ReturnType.SpecialType == SpecialType.Boolean
           && method.Parameters is [] or [{ RefKind: RefKind.None or RefKind.In }]
           && method.TypeParameters.Count == 0;

    private static bool IsValidCanExecuteProperty( IProperty property )
        => property.Type.SpecialType == SpecialType.Boolean
           && property.GetMethod != null;

    [Template]
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
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