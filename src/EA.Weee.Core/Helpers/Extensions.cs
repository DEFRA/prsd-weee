namespace EA.Weee.Core.Helpers
{
    using System;
    using System.Globalization;

    public static class Extensions
    {
        public static string ToStringWithXSignificantFigures(this double doub, int significantFigures)
        {
            if (doub == 0)
            {
                return "0.00";
            }

            double scale = Math.Pow(10, Math.Floor(Math.Log10(Math.Abs(doub))) + 1);
            var value = (scale * Math.Round(doub / scale, significantFigures)).ToString(CultureInfo.InvariantCulture);

            var appendedZeros = string.Empty;
            for (var i = 0; i < significantFigures - value.Replace(".", string.Empty).Length; i++)
            {
                appendedZeros += "0";
            }

            return value + appendedZeros;
        }
    }
}
