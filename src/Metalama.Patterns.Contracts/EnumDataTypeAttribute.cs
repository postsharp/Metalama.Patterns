// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Aspects;
using Metalama.Framework.Code;
using Metalama.Framework.Eligibility;

namespace Metalama.Patterns.Contracts;

/// <summary>
/// Custom attribute that, when added to a field, property or parameter, throws
/// an <see cref="ArgumentException"/> if the target is assigned a value that
/// is not a valid member of an enumeration. 
/// </summary>
/// <remarks>
///     <para>Strings are matched against enumeration member names. Integers
/// are matched against enumeration member values.     Null values are accepted and do not
/// throw exception.
/// </para>
/// <para>Error message is identified by <see cref="ContractLocalizedTextProvider.EnumDataTypeErrorMessage"/>.</para>
/// <para>Error message can use additional argument <value>{4}</value> to refer to <see cref="EnumType"/> name.</para>
/// </remarks>
[Inheritable]
public sealed class EnumDataTypeAttribute : ContractAspect
{
    /// <summary>
    /// Initializes a new instance of the <see cref="EnumDataTypeAttribute"/> class.
    /// </summary>
    /// <param name="enumType">Enumeration type.</param>
    public EnumDataTypeAttribute( Type enumType )
    {
        this.EnumType = enumType;
    }

    /// <summary>
    /// Gets the enumeration type.
    /// </summary>
    public Type EnumType { get; }

    /// <inheritdoc/>
    public override void BuildEligibility( IEligibilityBuilder<IFieldOrPropertyOrIndexer> builder )
    {
        base.BuildEligibility( builder );

        builder.MustSatisfy(
            f => IsElibigleType( f.Type ),
            f => $"the type of {f} must be string, an integer type or a nullable integer type" );
    }

    /// <inheritdoc/>
    public override void BuildEligibility( IEligibilityBuilder<IParameter> builder )
    {
        base.BuildEligibility( builder );

        builder.MustSatisfy(
            p => IsElibigleType( p.Type ),
            p => $"the type of {p} must be string, an integer type or a nullable integer type" );
    }

    [CompileTime]
    private static bool IsElibigleType( IType type )
        => type.ToNonNullableType().SpecialType switch
        {
            SpecialType.String or
                SpecialType.UInt16 or
                SpecialType.UInt32 or
                SpecialType.UInt64 or
                SpecialType.Int16 or
                SpecialType.Int32 or
                SpecialType.Int64 or
                SpecialType.Byte or
                SpecialType.SByte or
                SpecialType.Object => true,
            _ => false
        };

    /// <inheritdoc/>
    public override void Validate( dynamic? value )
    {
        var targetKind = meta.Target.GetTargetKind();
        var targetName = meta.Target.GetTargetName();
        var targetType = meta.Target.GetTargetType();

        if ( targetType.SpecialType == SpecialType.String || targetType.SpecialType == SpecialType.Object )
        {
            if ( value != null! && !EnumDataTypeAttributeHelper.IsValidEnumValue( value, this.EnumType ) )
            {
                throw ContractsServices.Default.ExceptionFactory.CreateException(
                    ContractExceptionInfo.Create(
                        typeof(ArgumentException),
                        typeof(EnumDataTypeAttribute),
                        value,
                        targetName,
                        targetKind,
                        meta.Target.ContractDirection,
                        ContractLocalizedTextProvider.EnumDataTypeErrorMessage,
                        this.EnumType.Name ) );
            }
        }
        else if ( targetType.IsNullable == true )
        {
            // target is a nullable integer type.
            if ( value!.HasValue )
            {
                var enumValue = Enum.ToObject( this.EnumType, value );

                if ( !EnumDataTypeAttributeHelper.IsValidEnumValue( enumValue, this.EnumType ) )
                {
                    throw ContractsServices.Default.ExceptionFactory.CreateException(
                        ContractExceptionInfo.Create(
                            typeof(ArgumentException),
                            typeof(EnumDataTypeAttribute),
                            enumValue,
                            targetName,
                            targetKind,
                            meta.Target.ContractDirection,
                            ContractLocalizedTextProvider.EnumDataTypeErrorMessage,
                            this.EnumType.Name ) );
                }
            }
        }
        else
        {
            // target is a non-nullable integer type.
            var enumValue = Enum.ToObject( this.EnumType, value );

            if ( !EnumDataTypeAttributeHelper.IsValidEnumValue( enumValue, this.EnumType ) )
            {
                throw ContractsServices.Default.ExceptionFactory.CreateException(
                    ContractExceptionInfo.Create(
                        typeof(ArgumentException),
                        typeof(EnumDataTypeAttribute),
                        enumValue,
                        targetName,
                        targetKind,
                        meta.Target.ContractDirection,
                        ContractLocalizedTextProvider.EnumDataTypeErrorMessage,
                        this.EnumType.Name ) );
            }
        }
    }
}