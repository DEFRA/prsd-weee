namespace EA.Weee.Core.Helpers
{
    using System;
    using System.Globalization;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Web.Mvc;
    using Prsd.Core.Domain;

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

        public static byte[] ToByteArray(this long value)
        {
            var result = new byte[8];

            result[0] = (byte)(value >> 56);
            result[1] = (byte)(value >> 48);
            result[2] = (byte)(value >> 40);
            result[3] = (byte)(value >> 32);
            result[4] = (byte)(value >> 24);
            result[5] = (byte)(value >> 16);
            result[6] = (byte)(value >> 8);
            result[7] = (byte)(value);

            return result;
        }

        public static TCoreEnumeration ToCoreEnumeration<TCoreEnumeration>(
            this Enumeration domainEnumeration)
        {
            if (typeof(TCoreEnumeration).IsSubclassOf(typeof(Enum)))
            {
                var coreEnumValues = Enum.GetValues(typeof(TCoreEnumeration)).Cast<TCoreEnumeration>();

                return coreEnumValues
                    .SingleOrDefault(v => Convert.ToInt32(v) == domainEnumeration.Value);
            }

            throw new InvalidOperationException(string.Format("The type '{0}' is not an enum",
                typeof(TCoreEnumeration).Name));
        }

        public static TDomainEnumeration ToDomainEnumeration<TDomainEnumeration>(
            this object coreEnumeration) where TDomainEnumeration : Enumeration
        {
            if (coreEnumeration is Enum)
            {
                var enumValue = Convert.ToInt32(coreEnumeration);

                return Enumeration.GetAll<TDomainEnumeration>()
                    .SingleOrDefault(v => v.Value == enumValue);
            }

            throw new InvalidOperationException(string.Format("The type '{0}' is not an enum",
                coreEnumeration.GetType().Name));
        }
    }
}
