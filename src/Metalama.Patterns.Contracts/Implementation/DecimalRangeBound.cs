// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Aspects;
using Metalama.Framework.Code.SyntaxBuilders;

namespace Metalama.Patterns.Contracts.Implementation;

[RunTimeOrCompileTime]
internal class DecimalRangeBound : RangeBound
{
    private readonly decimal _value;

    public DecimalRangeBound( decimal value, bool isAllowed ) : base( isAllowed )
    {
        this._value = value;
    }

    public override object ObjectValue => this._value;

    #region Conversions

    public override bool TryConvertToByte( out byte value, out ConversionResult conversionResult )
    {
        switch ( this._value )
        {
            case < byte.MinValue:
                value = 0;
                conversionResult = ConversionResult.TooSmall;

                return false;

            case > byte.MaxValue:
                value = 0;
                conversionResult = ConversionResult.TooLarge;

                return false;

            default:
                value = (byte) this._value;
                conversionResult = ConversionResult.Success;

                return true;
        }
    }

    public override bool TryConvertToSByte( out sbyte value, out ConversionResult conversionResult )
    {
        switch ( this._value )
        {
            case < sbyte.MinValue:
                value = 0;
                conversionResult = ConversionResult.TooSmall;

                return false;

            case > sbyte.MaxValue:
                value = 0;
                conversionResult = ConversionResult.TooLarge;

                return false;

            default:
                value = (sbyte) this._value;
                conversionResult = ConversionResult.Success;

                return true;
        }
    }

    public override bool TryConvertToInt16( out short value, out ConversionResult conversionResult )
    {
        switch ( this._value )
        {
            case < short.MinValue:
                value = 0;
                conversionResult = ConversionResult.TooSmall;

                return false;

            case > short.MaxValue:
                value = 0;
                conversionResult = ConversionResult.TooLarge;

                return false;

            default:
                value = (short) this._value;
                conversionResult = ConversionResult.Success;

                return true;
        }
    }

    public override bool TryConvertToUInt16( out ushort value, out ConversionResult conversionResult )
    {
        switch ( this._value )
        {
            case < ushort.MinValue:
                value = 0;
                conversionResult = ConversionResult.TooSmall;

                return false;

            case > ushort.MaxValue:
                value = 0;
                conversionResult = ConversionResult.TooLarge;

                return false;

            default:
                value = (ushort) this._value;
                conversionResult = ConversionResult.Success;

                return true;
        }
    }

    public override bool TryConvertToInt32( out int value, out ConversionResult conversionResult )
    {
        switch ( this._value )
        {
            case < int.MinValue:
                value = 0;
                conversionResult = ConversionResult.TooSmall;

                return false;

            case > int.MaxValue:
                value = 0;
                conversionResult = ConversionResult.TooLarge;

                return false;

            default:
                value = (int) this._value;
                conversionResult = ConversionResult.Success;

                return true;
        }
    }

    public override bool TryConvertToUInt32( out uint value, out ConversionResult conversionResult )
    {
        switch ( this._value )
        {
            case < uint.MinValue:
                value = 0;
                conversionResult = ConversionResult.TooSmall;

                return false;

            case > uint.MaxValue:
                value = 0;
                conversionResult = ConversionResult.TooLarge;

                return false;

            default:
                value = (uint) this._value;
                conversionResult = ConversionResult.Success;

                return true;
        }
    }

    public override bool TryConvertToInt64( out long value, out ConversionResult conversionResult )
    {
        switch ( this._value )
        {
            case < long.MinValue:
                value = 0;
                conversionResult = ConversionResult.TooSmall;

                return false;

            case > long.MaxValue:
                value = 0;
                conversionResult = ConversionResult.TooLarge;

                return false;

            default:
                value = (long) this._value;
                conversionResult = ConversionResult.Success;

                return true;
        }
    }

    public override bool TryConvertToUInt64( out ulong value, out ConversionResult conversionResult )
    {
        switch ( this._value )
        {
            case < 0:
                value = 0;
                conversionResult = ConversionResult.TooSmall;

                return false;

            case > ulong.MaxValue:
                value = 0;
                conversionResult = ConversionResult.TooLarge;

                return false;

            default:
                value = (ulong) this._value;
                conversionResult = ConversionResult.Success;

                return true;
        }
    }

    public override bool TryConvertToDecimal( out decimal value, out ConversionResult conversionResult )
    {
        value = this._value;
        conversionResult = ConversionResult.Success;

        return true;
    }

    public override bool TryConvertToDouble( out double value, out ConversionResult conversionResult )
    {
        value = (double) this._value;
        conversionResult = ConversionResult.Success;

        return true;
    }

    public override bool TryConvertToSingle( out float value, out ConversionResult conversionResult )
    {
        value = (float) this._value;
        conversionResult = ConversionResult.Success;

        return true;
    }

    #endregion

    public override void AppendValueToExpression( ExpressionBuilder expressionBuilder ) => expressionBuilder.AppendLiteral( this._value );
}