namespace EA.Prsd.Core.Extensions
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public static class EnumExtensions
    {
        public static TEnum GetValueFromDisplayName<TEnum>(this string displayName)
        {
            var type = typeof(TEnum);
            if (!type.IsEnum)
            {
                throw new InvalidOperationException();
            }
            foreach (var field in type.GetFields())
            {
                var attribute = Attribute.GetCustomAttribute(field,
                    typeof(DisplayAttribute)) as DisplayAttribute;
                if (attribute != null)
                {
                    if (attribute.Name == displayName)
                    {
                        return (TEnum)field.GetValue(null);
                    }
                }
                else
                {
                    if (field.Name == displayName)
                    {
                        return (TEnum)field.GetValue(null);
                    }
                }
            }
            throw new ArgumentException("Not found.", "displayName");
        }
    }
}