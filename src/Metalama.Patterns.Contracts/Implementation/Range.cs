// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using JetBrains.Annotations;
using Metalama.Framework.Aspects;
using Metalama.Framework.Code;
using Metalama.Framework.Code.SyntaxBuilders;

namespace Metalama.Patterns.Contracts.Implementation;

#pragma warning disable IDE0032 // Intentionally not using auto-properties.

/// <summary>
/// Describes a numeric range in the representation of various different numeric types.
/// </summary>
/// <seealso cref="RangeAttribute"/>
[PublicAPI]
[RunTimeOrCompileTime]
public readonly struct Range : IExpressionBuilder
{
    private readonly RangeBound? _minValue;
    private readonly RangeBound? _maxValue;

    /// <summary>
    /// Initializes a new instance of the <see cref="Range"/> struct.
    /// </summary>
    public Range( RangeBound? min, RangeBound? max )
    {
        this._minValue = min;
        this._maxValue = max;
    }

    public RangeBound? MinValue => this._minValue;

    public RangeBound? MaxValue => this._maxValue;

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
        expressionBuilder.AppendTypeName( typeof(Range) );
        expressionBuilder.AppendVerbatim( "(" );

        if ( this._minValue != null )
        {
            this._minValue.AppendToExpression( expressionBuilder );
        }

        expressionBuilder.AppendVerbatim( ")" );

        return expressionBuilder.ToExpression();
    }

    public bool GeneratePattern( IType valueType, ExpressionBuilder builder, IExpression value )
    {
        var hasMinCheck = this._minValue != null && this._minValue.TryConvert( valueType, out _, out _ );
        var hasMaxCheck = this._maxValue != null && this._maxValue.TryConvert( valueType, out _, out _ );
        var range = this;

        if ( !hasMaxCheck && !hasMinCheck )
        {
            // No condition necessary.
            return false;
        }
        else if ( hasMinCheck && hasMaxCheck )
        {
            if ( this._minValue!.SupportsPattern && this._maxValue!.SupportsPattern )
            {
                builder.AppendExpression( value );
                builder.AppendVerbatim( " is " );

                AppendMin();

                builder.AppendVerbatim( " or " );

                AppendMax();

                this._minValue.AppendValueToExpression( builder );
            }
            else
            {
                builder.AppendExpression( value );

                AppendMin();

                builder.AppendVerbatim( " || " );

                builder.AppendExpression( value );

                AppendMax();
            }
        }
        else if ( hasMinCheck )
        {
            builder.AppendExpression( value );

            AppendMin();
        }
        else
        {
            builder.AppendExpression( value );

            AppendMax();
        }

        return true;

        void AppendMin()
        {
            if ( range._minValue!.IsAllowed )
            {
                builder.AppendVerbatim( "< " );
            }
            else
            {
                builder.AppendVerbatim( "<= " );
            }

            range._minValue.AppendValueToExpression( builder );
        }

        void AppendMax()
        {
            if ( range._maxValue!.IsAllowed )
            {
                builder.AppendVerbatim( "> " );
            }
            else
            {
                builder.AppendVerbatim( ">= " );
            }

            range._maxValue.AppendValueToExpression( builder );
        }
    }

    public bool IsTypeSupported( IType type )
    {
        switch ( type.ToNonNullableType().SpecialType )
        {
            case SpecialType.Object:
                return true;

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
                if ( this._minValue != null )
                {
                    if ( !this._minValue.TryConvert( type, out _, out var minConversionResult ) )
                    {
                        if ( minConversionResult is ConversionResult.TooLarge or ConversionResult.UnsupportedType )
                        {
                            return false;
                        }
                    }
                }

                if ( this._maxValue != null )
                {
                    if ( !this._maxValue.TryConvert( type, out _, out var maxConversionResult ) )
                    {
                        if ( maxConversionResult is ConversionResult.TooSmall or ConversionResult.UnsupportedType )
                        {
                            return false;
                        }
                    }
                }

                return true;

            default:
                return false;
        }
    }
}