namespace EA.Weee.Core.Validation
{
    public static class DecimalPlaceHelper
    {
        public static int DecimalPlaces(this decimal value)
        {
            if (value == 0)
            {
                return 0;
            }
            else
            {
                var bits = decimal.GetBits(value);
                var exponent = bits[3] >> 16;
                var result = exponent;
                long lowDecimal = bits[0] | (bits[1] >> 8);

                while ((lowDecimal % 10) == 0)
                {
                    result--;
                    lowDecimal /= 10;
                }

                return result;
            }
        }
    }
}
