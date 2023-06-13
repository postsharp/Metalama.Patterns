﻿// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using System.Globalization;

namespace Metalama.Patterns.Contracts;

public static class EnumDataTypeAttributeHelper
{
    public static bool IsValidEnumValue( string value, Type enumType )
    {
        if ( string.IsNullOrEmpty( value ) )
        {
            return true;
        }

        object enumValue;
        try
        {
            enumValue = Enum.Parse( enumType, value, false );
        }
        catch ( ArgumentException )
        {
            return false;
        }

        return IsValidEnumValueCore( enumValue, enumType );
    }

    public static bool IsValidEnumValue( object value, Type enumType )
    {
        if ( value == null )
        {
            return true;
        }

        if ( value is string str )
        {
            return IsValidEnumValue( str, enumType );
        }

        var type = value.GetType();
        if ( !type.IsEnum || !enumType.Equals( type ) )
        {
            return false;
        }

        return IsValidEnumValueCore( value, enumType );
    }

    private static bool IsValidEnumValueCore( object value, Type enumType )
    {
        if ( IsEnumTypeInFlagsMode( enumType ) )
        {
            if ( !GetUnderlyingTypeValueString( enumType, value ).Equals( value.ToString(), StringComparison.Ordinal ) )
            {
                return true;
            }

            return false;
        }
        else
        {
            return Enum.IsDefined( enumType, value );
        }
    }

    private static bool IsEnumTypeInFlagsMode( Type enumType )
    {
        return enumType.GetCustomAttributes( typeof( FlagsAttribute ), false ).Length != 0;
    }

    private static string GetUnderlyingTypeValueString( Type enumType, object enumValue )
    {
        return Convert.ChangeType( enumValue, Enum.GetUnderlyingType( enumType ), CultureInfo.InvariantCulture ).ToString();
    }
}