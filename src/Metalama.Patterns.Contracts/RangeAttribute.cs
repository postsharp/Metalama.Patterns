// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

// TODO: #33303 The original PS implementation tried to avoid calling GetInvalidTypes at runtime, consider reinstating this behaviour.
// Note that it's not clear exactly why this was done in PS.

using JetBrains.Annotations;
using Metalama.Framework.Aspects;
using Metalama.Framework.Code;
using Metalama.Framework.Code.SyntaxBuilders;
using Metalama.Framework.Eligibility;
using Metalama.Patterns.Contracts.Implementation;
using System.Diagnostics;
using Range = Metalama.Patterns.Contracts.Implementation.Range;

namespace Metalama.Patterns.Contracts;

/// <summary>
/// Custom attribute that, when added to a field, property or parameter, throws
/// an <see cref="ArgumentOutOfRangeException"/> if the target is assigned a value that
/// is outside a given range.
/// </summary>
/// <remarks>
///     <para>Null values are accepted and do not throw an exception.
/// </para>
/// <para>Error message can use additional argument <value>{4}</value> to refer to the minimum value used and <value>{5}</value> to refer to the maximum value used.</para>
/// </remarks>
[PublicAPI]
public class RangeAttribute : ContractBaseAttribute
{
    protected Range Range { get; private set; }

    internal RangeAttribute(
        RangeBound? minValue,
        RangeBound? maxValue )
    {
        this.Range = new Range( minValue, maxValue );
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RangeAttribute"/> class specifying integer bounds.
    /// </summary>
    /// <param name="min">The lower bound.</param>
    /// <param name="max">The upper bound.</param>
    public RangeAttribute( long min, long max, bool minAllowed = true, bool maxAllowed = true ) : this(
        RangeBound.Create( min, minAllowed ),
        RangeBound.Create( max, maxAllowed ) ) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="RangeAttribute"/> class specifying unsigned integer bounds.
    /// </summary>
    /// <param name="min">The lower bound.</param>
    /// <param name="max">The upper bound.</param>
    public RangeAttribute( ulong min, ulong max, bool minAllowed = true, bool maxAllowed = true ) : this(
        RangeBound.Create( min, minAllowed ),
        RangeBound.Create( max, maxAllowed ) ) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="RangeAttribute"/> class specifying floating-point bounds.
    /// </summary>
    /// <param name="min">The lower bound.</param>
    /// <param name="max">The upper bound.</param>
    public RangeAttribute( double min, double max, bool minAllowed = true, bool maxAllowed = true ) : this(
        RangeBound.Create( min, minAllowed ),
        RangeBound.Create( max, maxAllowed ) ) { }

    public RangeAttribute( decimal min, decimal max, bool minAllowed = true, bool maxAllowed = true ) : this(
        RangeBound.Create( min, minAllowed ),
        RangeBound.Create( max, maxAllowed ) ) { }

    public RangeAttribute( float min, float max, bool minAllowed = true, bool maxAllowed = true ) : this(
        RangeBound.Create( min, minAllowed ),
        RangeBound.Create( max, maxAllowed ) ) { }

    public RangeAttribute( int min, int max, bool minAllowed = true, bool maxAllowed = true ) : this(
        RangeBound.Create( min, minAllowed ),
        RangeBound.Create( max, maxAllowed ) ) { }

    public RangeAttribute( uint min, uint max, bool minAllowed = true, bool maxAllowed = true ) : this(
        RangeBound.Create( min, minAllowed ),
        RangeBound.Create( max, maxAllowed ) ) { }

    public RangeAttribute( byte min, sbyte max, bool minAllowed = true, bool maxAllowed = true ) : this(
        RangeBound.Create( min, minAllowed ),
        RangeBound.Create( max ) ) { }

    /// <inheritdoc/>
    public override void BuildEligibility( IEligibilityBuilder<IFieldOrPropertyOrIndexer> builder )
    {
        base.BuildEligibility( builder );

        builder.MustSatisfy(
            f => IsEligibleType( f.Type ),
            f => $"the type of {f} must be a numeric type, a nullable numeric type, or object" );
    }

    /// <inheritdoc/>
    public override void BuildEligibility( IEligibilityBuilder<IParameter> builder )
    {
        base.BuildEligibility( builder );

        builder.MustSatisfy(
            p => IsEligibleType( p.Type ),
            p => $"the type of {p} must be a numeric type, a nullable numeric type, or object" );
    }

    [CompileTime]
    private static bool IsEligibleType( IType type )
        => type.ToNonNullableType().SpecialType switch
        {
            SpecialType.UInt16 or
                SpecialType.UInt32 or
                SpecialType.UInt64 or
                SpecialType.Int16 or
                SpecialType.Int32 or
                SpecialType.Int64 or
                SpecialType.Byte or
                SpecialType.SByte or
                SpecialType.Single or
                SpecialType.Double or
                SpecialType.Decimal or
                SpecialType.Object => true,
            _ => false
        };

    private void BuildAspect( IAspectBuilder builder, IType targetType )
    {
        var basicType = (INamedType) targetType.ToNonNullableType();

        if ( !this.Range.IsTypeSupported( basicType ) )
        {
            builder.Diagnostics.Report(
                ContractDiagnostics.RangeCannotBeApplied
                    .WithArguments(
                        (builder.Target,
                         basicType.Name,
                         builder.AspectInstance.AspectClass.ShortName,
                         this.Range) ) );

            builder.SkipAspect();
        }
    }

    /// <inheritdoc/>
    public override void BuildAspect( IAspectBuilder<IFieldOrPropertyOrIndexer> builder )
    {
        base.BuildAspect( builder );
        this.BuildAspect( builder, builder.Target.Type );
    }

    /// <inheritdoc/>
    public override void BuildAspect( IAspectBuilder<IParameter> builder )
    {
        base.BuildAspect( builder );
        this.BuildAspect( builder, builder.Target.Type );
    }

    /// <inheritdoc/>
    public override void Validate( dynamic? value )
    {
        var type = meta.Target.GetTargetType();

        if ( type.SpecialType == SpecialType.Object )
        {
            if ( value != null )
            {
                var range = meta.RunTime( this.Range );

                if ( !range.IsInRange( value ) )
                {
                    this.OnContractViolated( value );
                }
            }
        }
        else
        {
            var expressionBuilder = new ExpressionBuilder();
            var expression = (IExpression) value!;

            if ( this.Range.GeneratePattern( type, expressionBuilder, expression ) )
            {
                if ( expressionBuilder.ToValue() )
                {
                    this.OnContractViolated( value );
                }
            }
            else
            {
                meta.InsertComment( $"The {this.Range} validation on {expression} is fully redundant and has been skipped." );
            }
        }
    }

    [Template]
    protected virtual void OnContractViolated( dynamic? value )
    {
        meta.Target.GetContractOptions().Templates!.OnRangeContractViolated( value, this.Range );
    }
}