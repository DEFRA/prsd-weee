﻿namespace EA.Weee.Core.Helpers
{
    using System;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.Globalization;
    using System.Linq;
    using System.Reflection;
    using Prsd.Core.Domain;

    public static partial class Extensions
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

        public static T MakeEmptyStringsNull<T>(this T obj)
        {
            var type = obj.GetType();

            foreach (PropertyDescriptor property in TypeDescriptor.GetProperties(type))
            {
                if (property.PropertyType == typeof(string)
                    && !property.IsReadOnly
                    && (string)property.GetValue(obj) == string.Empty)
                {
                    property.SetValue(obj, null);
                }
                else if (property.PropertyType.IsCustom() 
                    && type.IsClass
                    && property.GetValue(obj) != null)
                {
                    property.SetValue(obj, MakeEmptyStringsNull(property.GetValue(obj)));
                }
            }

            return obj;
        }

        public static bool IsCustom(this Type type)
        {
            if (type.Namespace != null && type.Namespace.StartsWith("EA.Weee"))
            {
                return true;
            }

            return false;
        }

        public static TAttribute GetAttribute<TAttribute>(this Enum enumValue)
                where TAttribute : Attribute
        {
            return enumValue.GetType()
                            .GetMember(enumValue.ToString())
                            .First()
                            .GetCustomAttribute<TAttribute>();
        }

        public static string ToDisplayString<T>(this T value)
        {
            var fieldInfo = typeof(T).GetField(value.ToString());

            var descriptionAttributes = fieldInfo.GetCustomAttributes(
                typeof(DisplayAttribute), false) as DisplayAttribute[];

            if (descriptionAttributes == null)
            {
                return string.Empty;
            }

            return descriptionAttributes.Length > 0
                ? descriptionAttributes[0].Name
                : value.ToString();
        }

        public static T GetPropertyValue<T>(this object obj, string propertyName)
        {
            var property = obj.GetType().GetProperty(propertyName);

            if (property == null)
            {
                throw new ArgumentException($"A property with the name '{propertyName}' was not found", nameof(propertyName));
            }

            if (property.PropertyType != typeof(T))
            {
                throw new InvalidCastException($"The property '{propertyName}' is of type '{property.PropertyType}' not the specified '{typeof(T)}'");
            }

            return (T)property.GetValue(obj, null);
        }

        public static string ToReadableDateTime(this DateTime date)
        {
            var monthYear = date.ToString(" MMMM yyyy");
            var day = date.ToString("d ").Trim();

            switch (date.Day)
            {
                case 1:
                case 21:
                case 31:
                    day += "st";
                    break;
                case 2:
                case 22:
                    day += "nd";
                    break;
                case 3:
                case 23:
                    day += "rd";
                    break;
                default:
                    day += "th";
                    break;
            }
            return day + " " + monthYear;
        }
    }
}
