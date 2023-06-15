// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

namespace Metalama.Patterns.Contracts;

public static class CreditCardAttributeHelper
{
    public static bool IsValidCreditCardNumber( string? value )
    {
        if ( value == null )
        {
            return true;
        }

#if NET5_0_OR_GREATER
        var str2 =
            value.Replace( "-", "", StringComparison.OrdinalIgnoreCase )
                 .Replace( " ", "", StringComparison.OrdinalIgnoreCase );
#else
        var str2 = value.Replace( "-", "" ).Replace( " ", "" );
#endif
        var checksum = 0;
        var toggle = false;

        foreach ( var digit in str2.Reverse() )
        {
            if ( digit < 48 || digit > 57 )
            {
                return false;
            }

            var digitChecksum = (digit - 48) * (toggle ? 2 : 1);
            toggle = !toggle;
            while ( digitChecksum > 0 )
            {
                checksum += digitChecksum % 10;
                digitChecksum /= 10;
            }
        }

        return checksum % 10 == 0;
    }
}