using System;

namespace SharedCode.Utils
{
    public static class Enum<TEnum>
        where TEnum : struct
    {
        public static bool TryParse( string value, out TEnum result, bool ignoreCase = false )
        {
            try
            {
                result = (TEnum)Enum.Parse( typeof( TEnum ), value, ignoreCase );
            }
            catch
            {
                result = default( TEnum );
                return false;
            }

            return true;
        }
    }
}
