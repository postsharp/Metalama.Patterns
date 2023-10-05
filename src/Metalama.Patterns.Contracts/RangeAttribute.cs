// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

// TODO: #33303 The original PS implementation tried to avoid calling GetInvalidTypes at runtime, consider reinstating this behaviour.
// Note that it's not clear exactly why this was done in PS.

using JetBrains.Annotations;
using Metalama.Framework.Aspects;
using Metalama.Framework.Code;
using Metalama.Framework.Code.SyntaxBuilders;
using Metalama.Framework.Eligibility;

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
[Inheritable]
public class RangeAttribute : ContractAspect
{
    [Flags]
    internal enum TypeFlag
    {
        None = 0,
        Single = 1,
        Byte = 2,
        Int16 = 4,
        UInt16 = 8,
        Int32 = 16,
        UInt32 = 32,
        Int64 = 64,
        UInt64 = 128,
        Decimal = 256,
        SByte = 512,
        Double = 1024
    }

    /// <summary>
    /// Gets the minimal value to be used when generating the error message.
    /// </summary>
    protected object DisplayMinValue { get; }

    /// <summary>
    /// Gets the maximal value to be used when generating the error message.
    /// </summary>
    protected object DisplayMaxValue { get; }

    private readonly long _minInt64;
    private readonly long _maxInt64;

    private readonly ulong _minUInt64;
    private readonly ulong _maxUInt64;

    private readonly double _minDouble;
    private readonly double _maxDouble;

    private readonly decimal _minDecimal;
    private readonly decimal _maxDecimal;

    private readonly TypeFlag _invalidTypes;

    private readonly bool _shouldTestMinBound;
    private readonly bool _shouldTestMaxBound;

    internal RangeAttribute(
        object displayMin,
        object displayMax,
        long minInt64,
        long maxInt64,
        ulong minUInt64,
        ulong maxUInt64,
        double minDouble,
        double maxDouble,
        decimal minDecimal,
        decimal maxDecimal,
        TypeFlag invalidTypes,
        bool shouldTestMinBound = true,
        bool shouldTestMaxBound = true )
    {
        this.DisplayMinValue = displayMin;
        this.DisplayMaxValue = displayMax;
        this._minInt64 = minInt64;
        this._maxInt64 = maxInt64;
        this._minUInt64 = minUInt64;
        this._maxUInt64 = maxUInt64;
        this._minDouble = minDouble;
        this._maxDouble = maxDouble;
        this._minDecimal = minDecimal;
        this._maxDecimal = maxDecimal;
        this._invalidTypes = invalidTypes;
        this._shouldTestMinBound = shouldTestMinBound;
        this._shouldTestMaxBound = shouldTestMaxBound;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RangeAttribute"/> class specifying integer bounds.
    /// </summary>
    /// <param name="min">The lower bound.</param>
    /// <param name="max">The upper bound.</param>
    public RangeAttribute( long min, long max )
    {
        this.DisplayMinValue = min;
        this.DisplayMaxValue = max;

        this._minInt64 = min;
        this._maxInt64 = max;

        this._minUInt64 = min >= 0 ? (ulong) min : 0;
        this._maxUInt64 = max >= 0 ? (ulong) max : 0;

        this._minDouble = min;
        this._maxDouble = max;

        this._minDecimal = min;
        this._maxDecimal = max;

        this._invalidTypes = GetInvalidTypes( min, max );

        this._shouldTestMinBound = true;
        this._shouldTestMaxBound = true;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RangeAttribute"/> class specifying unsigned integer bounds.
    /// </summary>
    /// <param name="min">The lower bound.</param>
    /// <param name="max">The upper bound.</param>
    public RangeAttribute( ulong min, ulong max )
    {
        this.DisplayMinValue = min;
        this.DisplayMaxValue = max;

        this._minUInt64 = min;
        this._maxUInt64 = max;

        this._minInt64 = ConvertUInt64ToInt64( min );
        this._maxInt64 = ConvertUInt64ToInt64( max );

        this._minDouble = min;
        this._maxDouble = max;

        this._minDecimal = min;
        this._maxDecimal = max;

        this._invalidTypes = GetInvalidTypes( min );

        this._shouldTestMinBound = true;
        this._shouldTestMaxBound = true;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RangeAttribute"/> class specifying floating-point bounds.
    /// </summary>
    /// <param name="min">The lower bound.</param>
    /// <param name="max">The upper bound.</param>
    public RangeAttribute( double min, double max )
    {
        this.DisplayMinValue = min;
        this.DisplayMaxValue = max;

        this._minDouble = min;
        this._maxDouble = max;

        this._minDecimal = ConvertDoubleToDecimal( min );
        this._maxDecimal = ConvertDoubleToDecimal( max );

        this._minInt64 = ConvertDoubleToInt64( min );
        this._maxInt64 = ConvertDoubleToInt64( max );

        this._minUInt64 = ConvertDoubleToUInt64( min );
        this._maxUInt64 = ConvertDoubleToUInt64( max );

        this._invalidTypes = GetInvalidTypes( min, max );

        this._shouldTestMinBound = true;
        this._shouldTestMaxBound = true;
    }

    internal static TypeFlag GetInvalidTypes( long min, long max )
    {
        var invalidTypes = TypeFlag.None;

        if ( min > byte.MaxValue || max < byte.MinValue )
        {
            invalidTypes |= TypeFlag.Byte;
        }

        if ( min > sbyte.MaxValue || max < sbyte.MinValue )
        {
            invalidTypes |= TypeFlag.SByte;
        }

        if ( min > short.MaxValue || max < short.MinValue )
        {
            invalidTypes |= TypeFlag.Int16;
        }

        if ( min > ushort.MaxValue || max < ushort.MinValue )
        {
            invalidTypes |= TypeFlag.UInt16;
        }

        if ( min > int.MaxValue || max < int.MinValue )
        {
            invalidTypes |= TypeFlag.Int32;
        }

        if ( min > uint.MaxValue || max < uint.MinValue )
        {
            invalidTypes |= TypeFlag.UInt32;
        }

        return invalidTypes;
    }

    internal static TypeFlag GetInvalidTypes( ulong min )
    {
        var invalidTypes = TypeFlag.None;

        if ( min > byte.MaxValue )
        {
            invalidTypes |= TypeFlag.Byte;
        }

        if ( min > (ulong) sbyte.MaxValue )
        {
            invalidTypes |= TypeFlag.SByte;
        }

        if ( min > (ulong) short.MaxValue )
        {
            invalidTypes |= TypeFlag.Int16;
        }

        if ( min > ushort.MaxValue )
        {
            invalidTypes |= TypeFlag.UInt16;
        }

        if ( min > int.MaxValue )
        {
            invalidTypes |= TypeFlag.Int32;
        }

        if ( min > uint.MaxValue )
        {
            invalidTypes |= TypeFlag.UInt32;
        }

        if ( min > long.MaxValue )
        {
            invalidTypes |= TypeFlag.Int64;
        }

        return invalidTypes;
    }

    internal static TypeFlag GetInvalidTypes( double min, double max )
    {
        var invalidTypes = TypeFlag.None;

        if ( min > byte.MaxValue || max < byte.MinValue )
        {
            invalidTypes |= TypeFlag.Byte;
        }

        if ( min > sbyte.MaxValue || max < sbyte.MinValue )
        {
            invalidTypes |= TypeFlag.SByte;
        }

        if ( min > short.MaxValue || max < short.MinValue )
        {
            invalidTypes |= TypeFlag.Int16;
        }

        if ( min > ushort.MaxValue || max < ushort.MinValue )
        {
            invalidTypes |= TypeFlag.UInt16;
        }

        if ( min > int.MaxValue || max < int.MinValue )
        {
            invalidTypes |= TypeFlag.Int32;
        }

        if ( min > uint.MaxValue || max < uint.MinValue )
        {
            invalidTypes |= TypeFlag.UInt32;
        }

        if ( min > long.MaxValue || max < long.MinValue )
        {
            invalidTypes |= TypeFlag.Int64;
        }

        if ( min > ulong.MaxValue || max < ulong.MinValue )
        {
            invalidTypes |= TypeFlag.UInt64;
        }

        if ( min > (double) decimal.MaxValue || max < (double) decimal.MinValue )
        {
            invalidTypes |= TypeFlag.Decimal;
        }

        return invalidTypes;
    }

    internal static TypeFlag GetInvalidTypes( decimal min, decimal max )
    {
        var invalidTypes = TypeFlag.None;

        if ( min > byte.MaxValue || max < byte.MinValue )
        {
            invalidTypes |= TypeFlag.Byte;
        }

        if ( min > sbyte.MaxValue || max < sbyte.MinValue )
        {
            invalidTypes |= TypeFlag.SByte;
        }

        if ( min > short.MaxValue || max < short.MinValue )
        {
            invalidTypes |= TypeFlag.Int16;
        }

        if ( min > ushort.MaxValue || max < ushort.MinValue )
        {
            invalidTypes |= TypeFlag.UInt16;
        }

        if ( min > int.MaxValue || max < int.MinValue )
        {
            invalidTypes |= TypeFlag.Int32;
        }

        if ( min > uint.MaxValue || max < uint.MinValue )
        {
            invalidTypes |= TypeFlag.UInt32;
        }

        if ( min > long.MaxValue || max < long.MinValue )
        {
            invalidTypes |= TypeFlag.Int64;
        }

        if ( min > ulong.MaxValue || max < ulong.MinValue )
        {
            invalidTypes |= TypeFlag.UInt64;
        }

        return invalidTypes;
    }

    internal static long ConvertUInt64ToInt64( ulong value ) => value <= long.MaxValue ? (long) value : long.MaxValue;

    internal static decimal ConvertDoubleToDecimal( double value )
    {
        if ( value < (double) decimal.MinValue )
        {
            return decimal.MinValue;
        }
        else if ( value > (double) decimal.MaxValue )
        {
            return decimal.MaxValue;
        }

        return (decimal) value;
    }

    internal static long ConvertDoubleToInt64( double value )
    {
        if ( value < long.MinValue )
        {
            return long.MinValue;
        }
        else if ( value > long.MaxValue )
        {
            return long.MaxValue;
        }

        return (long) value;
    }

    private static ulong ConvertDoubleToUInt64( double value )
    {
        if ( value < ulong.MinValue )
        {
            return ulong.MinValue;
        }
        else if ( value > long.MaxValue )
        {
            return long.MaxValue;
        }

        return (ulong) value;
    }

    [CompileTime]
    private static TypeFlag GetTypeFlag( IType locationType )
        => locationType.SpecialType switch
        {
            SpecialType.Byte => TypeFlag.Byte,
            SpecialType.SByte => TypeFlag.SByte,
            SpecialType.Single => TypeFlag.Single,
            SpecialType.Decimal => TypeFlag.Decimal,
            SpecialType.Double => TypeFlag.Double,
            SpecialType.Int16 => TypeFlag.Int16,
            SpecialType.Int32 => TypeFlag.Int32,
            SpecialType.Int64 => TypeFlag.Int64,
            SpecialType.UInt16 => TypeFlag.UInt16,
            SpecialType.UInt32 => TypeFlag.UInt32,
            SpecialType.UInt64 => TypeFlag.UInt64,
            _ => TypeFlag.None
        };

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
        var typeFlag = GetTypeFlag( basicType );

        if ( (typeFlag & this._invalidTypes) != 0 )
        {
            builder.Diagnostics.Report(
                ContractDiagnostics.RangeCannotBeApplied
                    .WithArguments(
                        (builder.Target,
                         basicType.Name,
                         builder.AspectInstance.AspectClass.ShortName) ) );
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

    [CompileTime]
    private (IExpression Min, IExpression Max) GetMinAndMaxExpressions( SpecialType specialType )
    {
        var minBuilder = new ExpressionBuilder();
        var maxBuilder = new ExpressionBuilder();

        switch ( specialType )
        {
            case SpecialType.Byte:
            case SpecialType.UInt16:
            case SpecialType.UInt32:
            case SpecialType.UInt64:
                minBuilder.AppendLiteral( this._minUInt64 );
                maxBuilder.AppendLiteral( this._maxUInt64 );

                break;

            case SpecialType.SByte:
            case SpecialType.Int16:
            case SpecialType.Int32:
            case SpecialType.Int64:
                minBuilder.AppendLiteral( this._minInt64 );
                maxBuilder.AppendLiteral( this._maxInt64 );

                break;

            case SpecialType.Single:
            case SpecialType.Double:
                minBuilder.AppendLiteral( this._minDouble );
                maxBuilder.AppendLiteral( this._maxDouble );

                break;

            case SpecialType.Decimal:
                minBuilder.AppendLiteral( this._minDecimal );
                maxBuilder.AppendLiteral( this._maxDecimal );

                break;

            default:
                throw new InvalidOperationException( $"SpecialType.{specialType} was not expected here." );
        }

        return (minBuilder.ToExpression(), maxBuilder.ToExpression());
    }

    /// <inheritdoc/>
    public override void Validate( dynamic? value )
    {
        var type = meta.Target.GetTargetType();
        var basicType = (INamedType) type.ToNonNullableType();
        var isNullable = type.IsNullable == true;

        if ( type.SpecialType == SpecialType.Object )
        {
            if ( value != null )
            {
                if ( !ContractHelpers.IsInRange(
                        value,
                        this._minInt64,
                        this._maxInt64,
                        this._minUInt64,
                        this._maxUInt64,
                        this._minDouble,
                        this._maxDouble,
                        this._minDecimal,
                        this._maxDecimal ) )
                {
                    this.OnContractViolated( value );
                }
            }
        }
        else
        {
            var (min, max) = this.GetMinAndMaxExpressions( basicType.SpecialType );
            var testMin = this._shouldTestMinBound;
            var testMax = this._shouldTestMaxBound;

            if ( isNullable )
            {
                if ( testMin && testMax )
                {
                    if ( value!.HasValue && (value < min.Value || value > max.Value) )
                    {
                        this.OnContractViolated( value );
                    }
                }
                else if ( testMin )
                {
                    if ( value!.HasValue && value < min.Value )
                    {
                        this.OnContractViolated( value );
                    }
                }
                else if ( testMax )
                {
                    if ( value!.HasValue && value > max.Value )
                    {
                        this.OnContractViolated( value );
                    }
                }
            }

            // TODO: Pending fix for #33920
#if false
            // This way, the entire following block is absent/ignored/dropped by the template compiler.
            else
#else

            // Workaround - repeat the condition (negated)
            if ( !isNullable )
#endif
            {
                if ( testMin && testMax )
                {
                    if ( value < min.Value || value > max.Value )
                    {
                        this.OnContractViolated( value );
                    }
                }
                else if ( testMin )
                {
                    if ( value < min.Value )
                    {
                        this.OnContractViolated( value );
                    }
                }
                else if ( testMax )
                {
                    if ( value > max.Value )
                    {
                        this.OnContractViolated( value );
                    }
                }
            }
        }
    }

    [Template]
    protected virtual void OnContractViolated( dynamic? value )
    {
        meta.Target.GetContractOptions().Templates!.OnRangeContractViolated( value, this.DisplayMinValue, this.DisplayMaxValue );
    }
}