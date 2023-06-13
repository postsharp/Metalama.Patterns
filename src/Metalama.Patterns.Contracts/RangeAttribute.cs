// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

// TODO #33303: The original PS impementation tried to avoid calling GetInvalidTypes at runtime, consider reinstating this behaviour.
/* Note that it's not clear exactly why this was done in PS, my guess would be a performance issue under certain circumstances.
 */

using Metalama.Framework.Aspects;
using Metalama.Framework.Code;
using Metalama.Framework.Code.SyntaxBuilders;
using Metalama.Framework.Diagnostics;
using Metalama.Framework.Eligibility;
using System.Runtime.CompilerServices;

namespace Metalama.Patterns.Contracts;
// TODO: Remove comment below once current non-string approach is proven.
/* Important information regarding numbers (especially floating point) represented as strings:
 * 
 * https://learn.microsoft.com/en-us/dotnet/standard/base-types/standard-numeric-format-strings#RFormatString
 * https://learn.microsoft.com/en-us/dotnet/standard/base-types/standard-numeric-format-strings#GFormatString
 * 
 * tl;dr:
 * 
 * Use the following format specifiers to ensure roundtripable string representation:
 * 
 * double       "G17"
 * single       "G9"
 * half         "R"
 * BigInteger   "R"
 * 
 */

/// <summary>
/// Custom attribute that, when added to a field, property or parameter, throws
/// an <see cref="ArgumentOutOfRangeException"/> if the target is assigned a value that
/// is outside a given range.
/// </summary>
/// <remarks>
///     <para>Null values are accepted and do not throw an exception.
/// </para>
/// <para>Error message is identified by <see cref="ContractLocalizedTextProvider.RangeErrorMessage"/>.</para>
/// <para>Error message can use additional argument <value>{4}</value> to refer to the minimum value used and <value>{5}</value> to refer to the maximum value used.</para>
/// </remarks>
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
    /// Gets the minimal value to be used when generating the error message, typically in the implementation of <see cref="GetErrorMessageArguments"/>.
    /// </summary>
    protected object DisplayMinValue { get; private set; }

    /// <summary>
    /// Gets the maximal value to be used when generating the error message, typically in the implementation of <see cref="GetErrorMessageArguments"/>.
    /// </summary>
    protected object DisplayMaxValue { get; private set; }

    private readonly long _minInt64;
    private readonly long _maxInt64;

    private readonly ulong _minUInt64;
    private readonly ulong _maxUInt64;

    private readonly double _minDouble;
    private readonly double _maxDouble;

    private readonly decimal _minDecimal;
    private readonly decimal _maxDecimal;

    private readonly TypeFlag _invalidTypes;

    internal RangeAttribute( object displayMin, object displayMax, long minInt64, long maxInt64, ulong minUInt64, ulong maxUInt64, double minDouble,
                            double maxDouble, decimal minDecimal, decimal maxDecimal, TypeFlag invalidTypes )
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

    internal static long ConvertUInt64ToInt64( ulong value )
    {
        return value <= long.MaxValue ? (long) value : long.MaxValue;
    }

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

    internal static long ConvertDecimalToInt64( decimal value )
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

    private static ulong ConvertDecimalToUInt64( decimal value )
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

    // TODO: Remove
#if false
    private static TypeFlag GetTypeFlag( Type locationType )
    {
        TypeFlag typeFlag;

        switch ( Type.GetTypeCode( locationType ) )
        {
            case TypeCode.SByte:
                typeFlag = TypeFlag.SByte;
                break;
            case TypeCode.Byte:
                typeFlag = TypeFlag.Byte;
                break;
            case TypeCode.Int16:
                typeFlag = TypeFlag.Int16;
                break;
            case TypeCode.UInt16:
                typeFlag = TypeFlag.UInt16;
                break;
            case TypeCode.Int32:
                typeFlag = TypeFlag.Int32;
                break;
            case TypeCode.UInt32:
                typeFlag = TypeFlag.UInt32;
                break;
            case TypeCode.Int64:
                typeFlag = TypeFlag.Int64;
                break;
            case TypeCode.UInt64:
                typeFlag = TypeFlag.UInt64;
                break;
            case TypeCode.Single:
                typeFlag = TypeFlag.Single;
                break;
            case TypeCode.Double:
                typeFlag = TypeFlag.Double;
                break;
            case TypeCode.Decimal:
                typeFlag = TypeFlag.Decimal;
                break;
            default:
                typeFlag = TypeFlag.None;
                break;
        }

        return typeFlag;
    }
#endif

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

    public override void BuildEligibility( IEligibilityBuilder<IFieldOrPropertyOrIndexer> builder )
    {
        base.BuildEligibility( builder );
        builder.MustSatisfy( f => IsElibigleType( f.Type ), f => $"the type of {f} must be a numeric type, a nullable numeric type, or object" );
    }

    public override void BuildEligibility( IEligibilityBuilder<IParameter> builder )
    {
        base.BuildEligibility( builder );
        builder.MustSatisfy( p => IsElibigleType( p.Type ), p => $"the type of {p} must be a numeric type, a nullable numeric type, or object" );
    }

    [CompileTime]
    private static bool IsElibigleType( IType type )
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

    // COM010 in PostSharp
    private static readonly DiagnosticDefinition<(string, string, string)> _rangeCannotBeApplied = new(
        "ML5000",
        Severity.Error,
        "{0} cannot be applied to {1} because the value range cannot be satisfied by the type {2}." );

    public override void BuildAspect( IAspectBuilder<IFieldOrPropertyOrIndexer> builder )
    {
        base.BuildAspect( builder );

        var basicType = (INamedType) builder.Target.Type.ToNonNullableType();
        var typeFlag = GetTypeFlag( basicType );
        
        if ( (typeFlag & this._invalidTypes) != 0 )
        {
            builder.Diagnostics.Report(
                _rangeCannotBeApplied.WithArguments( (this.GetType().Name, builder.Target.ToDisplayString(), basicType.Name) ) );
        }
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

    public override void Validate( dynamic? value )
    {
        CompileTimeHelpers.GetTargetKindAndName( meta.Target, out var targetKind, out var targetName );
        var type = CompileTimeHelpers.GetTargetType( meta.Target );
        var basicType = (INamedType) type.ToNonNullableType();
        var isNullable = type.IsNullable == true;
        var exceptionInfo = this.GetExceptioninfo();
        var exceptionType = meta.CompileTime( this.GetType() );//.ToTypeOf().Value;

        if ( type.SpecialType == SpecialType.Object )
        {
            if ( value != null )
            {
                var rangeValues = new RangeValues( this._minInt64, this._maxInt64, this._minUInt64, this._maxUInt64,
                    this._minDouble, this._maxDouble, this._minDecimal, this._maxDecimal );

                var validateResult = RangeAttributeHelpers.Validate( value, rangeValues );

                // TODO: Maybe include validateResult.UnderlyingType in the message?

                if ( !validateResult.IsInRange )
                {
                    if ( exceptionInfo.IncludeMinValue && exceptionInfo.IncludeMaxValue )
                    {
                        throw ContractServices.ExceptionFactory.CreateException( ContractExceptionInfo.Create(
                            typeof( ArgumentOutOfRangeException ),
                            exceptionType,
                            value,
                            targetName,
                            targetKind,
                            meta.Target.ContractDirection,
                            exceptionInfo.MessageIdExpression.Value,
                            this.DisplayMinValue,
                            this.DisplayMaxValue ) );
                    }
                    else if ( exceptionInfo.IncludeMinValue )
                    {
                        throw ContractServices.ExceptionFactory.CreateException( ContractExceptionInfo.Create(
                            typeof( ArgumentOutOfRangeException ),
                            exceptionType,
                            value,
                            targetName,
                            targetKind,
                            meta.Target.ContractDirection,
                            exceptionInfo.MessageIdExpression.Value,
                            this.DisplayMinValue ) );
                    }
                    else if ( exceptionInfo.IncludeMaxValue )
                    {
                        throw ContractServices.ExceptionFactory.CreateException( ContractExceptionInfo.Create(
                            typeof( ArgumentOutOfRangeException ),
                            exceptionType,
                            value,
                            targetName,
                            targetKind,
                            meta.Target.ContractDirection,
                            exceptionInfo.MessageIdExpression.Value,
                            this.DisplayMaxValue ) );
                    }
                    else
                    {
                        throw ContractServices.ExceptionFactory.CreateException( ContractExceptionInfo.Create(
                            typeof( ArgumentOutOfRangeException ),
                            exceptionType,
                            value,
                            targetName,
                            targetKind,
                            meta.Target.ContractDirection,
                            exceptionInfo.MessageIdExpression.Value ) );
                    }
                }
            }
        }
        else
        {
#if true
            var (min, max) = this.GetMinAndMaxExpressions( basicType.SpecialType );

            if ( isNullable )
            {
                if ( value!.HasValue && (value < min.Value || value > max.Value) )
                {
                    if ( exceptionInfo.IncludeMinValue && exceptionInfo.IncludeMaxValue )
                    {
                        throw ContractServices.ExceptionFactory.CreateException( ContractExceptionInfo.Create(
                            typeof( ArgumentOutOfRangeException ),
                            exceptionType,
                            value,
                            targetName,
                            targetKind,
                            meta.Target.ContractDirection,
                            exceptionInfo.MessageIdExpression.Value,
                            this.DisplayMinValue,
                            this.DisplayMaxValue ) );
                    }
                    else if ( exceptionInfo.IncludeMinValue )
                    {
                        throw ContractServices.ExceptionFactory.CreateException( ContractExceptionInfo.Create(
                            typeof( ArgumentOutOfRangeException ),
                            exceptionType,
                            value,
                            targetName,
                            targetKind,
                            meta.Target.ContractDirection,
                            exceptionInfo.MessageIdExpression.Value,
                            this.DisplayMinValue ) );
                    }
                    else if ( exceptionInfo.IncludeMaxValue )
                    {
                        throw ContractServices.ExceptionFactory.CreateException( ContractExceptionInfo.Create(
                            typeof( ArgumentOutOfRangeException ),
                            exceptionType,
                            value,
                            targetName,
                            targetKind,
                            meta.Target.ContractDirection,
                            exceptionInfo.MessageIdExpression.Value,
                            this.DisplayMaxValue ) );
                    }
                    else
                    {
                        throw ContractServices.ExceptionFactory.CreateException( ContractExceptionInfo.Create(
                            typeof( ArgumentOutOfRangeException ),
                            exceptionType,
                            value,
                            targetName,
                            targetKind,
                            meta.Target.ContractDirection,
                            exceptionInfo.MessageIdExpression.Value ) );
                    }
                }
            }
            else
            {
                if ( value < min.Value || value > max.Value )
                {
                    if ( exceptionInfo.IncludeMinValue && exceptionInfo.IncludeMaxValue )
                    {
                        throw ContractServices.ExceptionFactory.CreateException( ContractExceptionInfo.Create(
                            typeof( ArgumentOutOfRangeException ),
                            exceptionType,
                            value,
                            targetName,
                            targetKind,
                            meta.Target.ContractDirection,
                            exceptionInfo.MessageIdExpression.Value,
                            this.DisplayMinValue,
                            this.DisplayMaxValue ) );
                    }
                    else if ( exceptionInfo.IncludeMinValue )
                    {
                        throw ContractServices.ExceptionFactory.CreateException( ContractExceptionInfo.Create(
                            typeof( ArgumentOutOfRangeException ),
                            exceptionType,
                            value,
                            targetName,
                            targetKind,
                            meta.Target.ContractDirection,
                            exceptionInfo.MessageIdExpression.Value,
                            this.DisplayMinValue ) );
                    }
                    else if ( exceptionInfo.IncludeMaxValue )
                    {
                        throw ContractServices.ExceptionFactory.CreateException( ContractExceptionInfo.Create(
                            typeof( ArgumentOutOfRangeException ),
                            exceptionType,
                            value,
                            targetName,
                            targetKind,
                            meta.Target.ContractDirection,
                            exceptionInfo.MessageIdExpression.Value,
                            this.DisplayMaxValue ) );
                    }
                    else
                    {
                        throw ContractServices.ExceptionFactory.CreateException( ContractExceptionInfo.Create(
                            typeof( ArgumentOutOfRangeException ),
                            exceptionType,
                            value,
                            targetName,
                            targetKind,
                            meta.Target.ContractDirection,
                            exceptionInfo.MessageIdExpression.Value ) );
                    }
                }
            }
#else // TODO: Why doesn't this work? Discuss with gael.
            var (min, max) = this.GetMinAndMaxExpressions( basicType.SpecialType );

            var b = new ExpressionBuilder();
            if ( isNullable )
            {
                b.AppendExpression( value!.HasValue && (value < min.Value || value > max.Value) );
            }
            else
            {
                b.AppendExpression( value < min.Value || value > max.Value );
            }

            var expr = b.ToExpression();
            if ( expr.Value )
            {
                throw ContractServices.ExceptionFactory.CreateException( ContractExceptionInfo.Create(
                    typeof( ArgumentOutOfRangeException ),
                    typeof( RangeAttribute ),
                    value,
                    targetName,
                    targetKind,
                    meta.Target.ContractDirection,
                    ContractLocalizedTextProvider.RangeErrorMessage,
                    this.DisplayMinValue,
                    this.DisplayMaxValue ) );
            }
#endif
        }
    }

    [CompileTime]
    protected virtual (IExpression MessageIdExpression, bool IncludeMinValue, bool IncludeMaxValue) GetExceptioninfo()
        => (CompileTimeHelpers.GetContractLocalizedTextProviderField( nameof( ContractLocalizedTextProvider.RangeErrorMessage ) ),
            true, true);
}