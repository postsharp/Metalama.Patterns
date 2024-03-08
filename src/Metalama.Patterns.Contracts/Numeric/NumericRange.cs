// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using JetBrains.Annotations;
using Metalama.Framework.Aspects;
using Metalama.Framework.Code;
using Metalama.Framework.Code.SyntaxBuilders;
using System.Diagnostics;
using System.Text;

namespace Metalama.Patterns.Contracts.Numeric;

#pragma warning disable IDE0032 // Intentionally not using auto-properties.

/// <summary>
/// Describes a numeric range. Used by <see cref="RangeAttribute"/>.
/// </summary>
/// <seealso cref="RangeAttribute"/>
[PublicAPI]
[RunTimeOrCompileTime]
public readonly struct NumericRange : IExpressionBuilder
{
    private readonly NumericBound? _minValue;
    private readonly NumericBound? _maxValue;

    /// <summary>
    /// Initializes a new instance of the <see cref="NumericRange"/> struct.
    /// </summary>
    public NumericRange( NumericBound? min, NumericBound? max )
    {
        this._minValue = min;
        this._maxValue = max;
    }

    /// <summary>
    /// Gets the minimal value, or <c>null</c> if the range has no minimal value.
    /// </summary>
    public NumericBound? MinValue => this._minValue;

    /// <summary>
    /// Gets the maximal value, or <c>null</c> if the range has no maximal value.
    /// </summary>
    public NumericBound? MaxValue => this._maxValue;

    /// <summary>
    /// Determines if a value of type <see cref="double"/> is in the current range.
    /// </summary>
    public bool IsInRange( double value )
    {
        if ( this._minValue != null )
        {
            if ( this._minValue.TryConvertToDouble( out var minValue, out _ ) )
            {
                if ( this._minValue.IsAllowed )
                {
                    if ( value < minValue )
                    {
                        return false;
                    }
                }
                else
                {
                    if ( value <= minValue )
                    {
                        return false;
                    }
                }
            }
        }

        if ( this._maxValue != null )
        {
            if ( this._maxValue.TryConvertToDouble( out var maxValue, out _ ) )
            {
                if ( this._maxValue.IsAllowed )
                {
                    if ( value > maxValue )
                    {
                        return false;
                    }
                }
                else
                {
                    if ( value >= maxValue )
                    {
                        return false;
                    }
                }
            }
        }

        return true;
    }

    /// <summary>
    /// Determines if a value of type <see cref="decimal"/> is in the current range.
    /// </summary>
    public bool IsInRange( decimal value )
    {
        if ( this._minValue != null )
        {
            if ( this._minValue.TryConvertToDecimal( out var minValue, out _ ) )
            {
                if ( this._minValue.IsAllowed )
                {
                    if ( value < minValue )
                    {
                        return false;
                    }
                }
                else
                {
                    if ( value <= minValue )
                    {
                        return false;
                    }
                }
            }
        }

        if ( this._maxValue != null )
        {
            if ( this._maxValue.TryConvertToDecimal( out var maxValue, out _ ) )
            {
                if ( this._maxValue.IsAllowed )
                {
                    if ( value > maxValue )
                    {
                        return false;
                    }
                }
                else
                {
                    if ( value >= maxValue )
                    {
                        return false;
                    }
                }
            }
        }

        return true;
    }

    /// <summary>
    /// Determines if a value of type <see cref="long"/> is in the current range.
    /// </summary>
    public bool IsInRange( long value )
    {
        if ( this._minValue != null )
        {
            if ( this._minValue.TryConvertToInt64( out var minValue, out _ ) )
            {
                if ( this._minValue.IsAllowed )
                {
                    if ( value < minValue )
                    {
                        return false;
                    }
                }
                else
                {
                    if ( value <= minValue )
                    {
                        return false;
                    }
                }
            }
        }

        if ( this._maxValue != null )
        {
            if ( this._maxValue.TryConvertToInt64( out var maxValue, out _ ) )
            {
                if ( this._maxValue.IsAllowed )
                {
                    if ( value > maxValue )
                    {
                        return false;
                    }
                }
                else
                {
                    if ( value >= maxValue )
                    {
                        return false;
                    }
                }
            }
        }

        return true;
    }

    /// <summary>
    /// Determines if a value of type <see cref="ulong"/> is in the current range.
    /// </summary>
    public bool IsInRange( ulong value )
    {
        if ( this._minValue != null )
        {
            if ( this._minValue.TryConvertToUInt64( out var minValue, out _ ) )
            {
                if ( this._minValue.IsAllowed )
                {
                    if ( value < minValue )
                    {
                        return false;
                    }
                }
                else
                {
                    if ( value <= minValue )
                    {
                        return false;
                    }
                }
            }
        }

        if ( this._maxValue != null )
        {
            if ( this._maxValue.TryConvertToUInt64( out var maxValue, out _ ) )
            {
                if ( this._maxValue.IsAllowed )
                {
                    if ( value > maxValue )
                    {
                        return false;
                    }
                }
                else
                {
                    if ( value >= maxValue )
                    {
                        return false;
                    }
                }
            }
        }

        return true;
    }

    /// <summary>
    /// Determines if a value of unknown type is in the current range. Returns <c>false</c> if the type
    /// is unsupported.
    /// </summary>
    public bool IsInRange( object? value )
    {
        switch ( value )
        {
            case null:
                return true;

            case float floatValue:
                return this.IsInRange( floatValue );

            case double doubleValue:
                return this.IsInRange( doubleValue );

            case decimal decimalValue:
                return this.IsInRange( decimalValue );

            case long longValue:
                return this.IsInRange( longValue );

            case ulong ulongValue:
                return this.IsInRange( ulongValue );

            case int intValue:
                return this.IsInRange( intValue );

            case uint uintValue:
                return this.IsInRange( uintValue );

            case short shortValue:
                return this.IsInRange( shortValue );

            case ushort ushortValue:
                return this.IsInRange( ushortValue );

            case byte byteValue:
                return this.IsInRange( byteValue );

            case sbyte sbyteValue:
                return this.IsInRange( sbyteValue );

            default:
                return false;
        }
    }

    // Generates run-time code that represents the current compile-time object.
    IExpression IExpressionBuilder.ToExpression()
    {
        var expressionBuilder = new ExpressionBuilder();

        expressionBuilder.AppendVerbatim( "new " );
        expressionBuilder.AppendTypeName( typeof(NumericRange) );
        expressionBuilder.AppendVerbatim( "(" );

        if ( this._minValue != null )
        {
            this._minValue.AppendToExpression( expressionBuilder );
        }
        else
        {
            expressionBuilder.AppendVerbatim( "null" );
        }

        expressionBuilder.AppendVerbatim( ", " );

        if ( this._maxValue != null )
        {
            this._maxValue.AppendToExpression( expressionBuilder );
        }
        else
        {
            expressionBuilder.AppendVerbatim( "null" );
        }

        expressionBuilder.AppendVerbatim( ")" );

        return expressionBuilder.ToExpression();
    }

    internal bool GeneratePattern( IType valueType, ExpressionBuilder builder, IExpression value )
    {
        var range = this;

        var nonNullableType = valueType.ToNonNullableType();

        // Check if we have checks of min or max value.
        ConversionResult minConversionResult = default;
        ConversionResult maxConversionResult = default;
        var hasMinCheck = this._minValue != null && this._minValue.TryConvert( nonNullableType, out _, out minConversionResult );
        var hasMaxCheck = this._maxValue != null && this._maxValue.TryConvert( nonNullableType, out _, out maxConversionResult );

        Debugger.Break();

        // Eliminate redundant checks.
        if ( hasMinCheck && minConversionResult == ConversionResult.ExactlyMinValue && this._minValue!.IsAllowed )
        {
            hasMinCheck = false;
        }

        if ( hasMaxCheck && maxConversionResult == ConversionResult.ExactlyMaxValue && this._maxValue!.IsAllowed )
        {
            hasMaxCheck = false;
        }

        if ( !hasMaxCheck && !hasMinCheck )
        {
            // No condition necessary.
            return false;
        }
        else if ( hasMinCheck && hasMaxCheck )
        {
            builder.AppendExpression( value );
            builder.AppendVerbatim( " is " );

            AppendMin();

            builder.AppendVerbatim( " or " );

            AppendMax();
        }
        else if ( hasMinCheck )
        {
            builder.AppendExpression( value );
            builder.AppendVerbatim( " is " );

            AppendMin();
        }
        else
        {
            builder.AppendExpression( value );
            builder.AppendVerbatim( " is " );

            AppendMax();
        }

        return true;

        void AppendMin()
        {
            if ( range._minValue!.IsAllowed )
            {
                builder.AppendVerbatim( " < " );
            }
            else
            {
                builder.AppendVerbatim( " <= " );
            }

            range._minValue.AppendConvertedValueToExpression( nonNullableType, builder );
        }

        void AppendMax()
        {
            if ( range._maxValue!.IsAllowed )
            {
                builder.AppendVerbatim( " > " );
            }
            else
            {
                builder.AppendVerbatim( " >= " );
            }

            range._maxValue.AppendConvertedValueToExpression( nonNullableType, builder );
        }
    }

    internal NumericRangeTypeSupport IsTypeSupported( IType type )
    {
        switch ( type.ToNonNullableType().SpecialType )
        {
            case SpecialType.Object:
                return NumericRangeTypeSupport.Supported;

            case SpecialType.Byte:
            case SpecialType.SByte:
            case SpecialType.Int16:
            case SpecialType.UInt16:
            case SpecialType.Int32:
            case SpecialType.UInt32:
            case SpecialType.Int64:
            case SpecialType.UInt64:
            case SpecialType.Decimal:
            case SpecialType.Single:
            case SpecialType.Double:
                var isRedundant = true;

                if ( this._minValue != null )
                {
                    this._minValue.TryConvert( type, out _, out var minConversionResult );

                    if ( minConversionResult is ConversionResult.TooLarge or ConversionResult.UnsupportedType )
                    {
                        return NumericRangeTypeSupport.NotSupported;
                    }

                    if ( minConversionResult == ConversionResult.ExactlyMaxValue && !this._minValue.IsAllowed )
                    {
                        return NumericRangeTypeSupport.NotSupported;
                    }

                    if ( minConversionResult == ConversionResult.TooSmall ||
                         (minConversionResult == ConversionResult.ExactlyMinValue && !this._minValue.IsAllowed) )
                    {
                        // This check is redundant.
                    }
                    else
                    {
                        isRedundant = false;
                    }
                }

                if ( this._maxValue != null )
                {
                    this._maxValue.TryConvert( type, out _, out var maxConversionResult );

                    if ( maxConversionResult is ConversionResult.TooSmall or ConversionResult.UnsupportedType )
                    {
                        return NumericRangeTypeSupport.NotSupported;
                    }

                    if ( maxConversionResult == ConversionResult.ExactlyMinValue && !this._maxValue.IsAllowed )
                    {
                        return NumericRangeTypeSupport.NotSupported;
                    }

                    if ( maxConversionResult == ConversionResult.TooLarge ||
                         (maxConversionResult == ConversionResult.ExactlyMaxValue && !this._maxValue.IsAllowed) )
                    {
                        // This check is redundant.
                    }
                    else
                    {
                        isRedundant = false;
                    }
                }

                if ( isRedundant )
                {
                    return NumericRangeTypeSupport.Redundant;
                }

                return NumericRangeTypeSupport.Supported;

            default:
                return NumericRangeTypeSupport.NotSupported;
        }
    }

    public override string ToString()
    {
        var stringBuilder = new StringBuilder();

        if ( this._minValue != null )
        {
            stringBuilder.Append( this._minValue.IsAllowed ? '[' : ']' );
            stringBuilder.Append( this._minValue.ObjectValue );
        }
        else
        {
            stringBuilder.Append( "[\u221e" );
        }

        stringBuilder.Append( ", " );

        if ( this._maxValue != null )
        {
            stringBuilder.Append( this._maxValue.ObjectValue );
            stringBuilder.Append( this._maxValue.IsAllowed ? ']' : '[' );
        }
        else
        {
            stringBuilder.Append( "\u221e]" );
        }

        return stringBuilder.ToString();
    }
}