namespace EA.Prsd.Core.Helpers
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Reflection;

    public static class EnumHelper
    {
        public static Dictionary<int, string> GetValues(Type enumType)
        {
            var fields = enumType.GetFields(BindingFlags.Public | BindingFlags.Static);
            var fieldNames = new Dictionary<int, string>();

            foreach (var field in fields)
            {
                // Get the display attributes for the enum.
                var displayAttribute =
                    (DisplayAttribute)field.GetCustomAttributes(typeof(DisplayAttribute)).SingleOrDefault();

                // Set field name to either the enum name or the display name.
                var name = (displayAttribute == null) ? field.Name : displayAttribute.Name;

                fieldNames.Add((int)field.GetValue(null), name);
            }

            return fieldNames;
        }

        public static Dictionary<int, string> GetOrderedValues(Type enumType)
        {
            var values = GetValues(enumType).ToList();

            values.Sort((pair1, pair2) => string.Compare(pair1.Value, pair2.Value, StringComparison.Ordinal));

            return values.ToDictionary(v => v.Key, v => v.Value);
        }

        private static readonly ConcurrentDictionary<Type, EnumHelperMetaData> enumHelperMetaDataMap = new ConcurrentDictionary<Type, EnumHelperMetaData>();

        public static string GetDescription<TEnum>(TEnum value)
        {
            var enumMetaData = enumHelperMetaDataMap.GetOrAdd(typeof(TEnum), GetAllMetaDataForType<TEnum>());

            string desc;
            if (enumMetaData.Descriptions == null || !enumMetaData.Descriptions.TryGetValue(value, out desc))
            {
                return value.ToString();
            }
            else
            {
                return desc;
            }
        }

        public static string GetDisplayName<TEnum>(TEnum value)
        {
            var enumMetaData = enumHelperMetaDataMap.GetOrAdd(typeof(TEnum), GetAllMetaDataForType<TEnum>());

            string name;
            if (enumMetaData.Names == null || !enumMetaData.Names.TryGetValue(value, out name))
            {
                return value.ToString();
            }

            return name;
        }

        public static string GetShortName<TEnum>(TEnum value)
        {
            var enumMetaData = enumHelperMetaDataMap.GetOrAdd(typeof(TEnum), GetAllMetaDataForType<TEnum>());

            string shortName;
            if (enumMetaData.ShortNames == null || !enumMetaData.ShortNames.TryGetValue(value, out shortName))
            {
                return value.ToString();
            }
            else
            {
                return shortName;
            }
        }

        private static EnumHelperMetaData GetAllMetaDataForType<TEnum>()
        {
            var descriptions = GetEnumAttributeDictionary<TEnum>(f =>
            {
                DescriptionAttribute attr = f.GetCustomAttribute<DescriptionAttribute>();

                string description;
                if (attr != null)
                {
                    description = attr.Description;
                }
                else
                {
                    DisplayAttribute displayAttribute = f.GetCustomAttribute<DisplayAttribute>();

                    if (displayAttribute == null || string.IsNullOrWhiteSpace(displayAttribute.Description))
                    {
                        description = GetDefaultFieldInfoName(f);
                    }
                    else
                    {
                        description = displayAttribute.Description;
                    }
                }
                return description;
            });

            var names = GetEnumAttributeDictionary<TEnum>(f =>
                {
                    DisplayAttribute attr = f.GetCustomAttribute<DisplayAttribute>();

                    string displayName;
                    if (attr != null)
                    {
                        displayName = attr.Name;
                    }
                    else
                    {
                        displayName = GetDefaultFieldInfoName(f);
                    }
                    return displayName;
                });

            var shortNames = GetEnumAttributeDictionary<TEnum>(f =>
                {
                    DisplayAttribute attr = f.GetCustomAttribute<DisplayAttribute>();

                    string shortName;
                    if (attr != null)
                    {
                        shortName = attr.ShortName;
                    }
                    else
                    {
                        shortName = GetDefaultFieldInfoName(f);
                    }
                    return shortName;
                });

            return new EnumHelperMetaData
            {
                Descriptions = descriptions,
                Names = names,
                ShortNames = shortNames
            };
        }

        private static Dictionary<object, string> GetEnumAttributeDictionary<TEnum>(Func<FieldInfo, string> tryGetAttribute)
        {
            return typeof(TEnum)
                .GetFields(BindingFlags.Public | BindingFlags.Static)
                .Select(f =>
                    {
                        var attributeResult = tryGetAttribute(f);

                        return new KeyValuePair<object, string>(
                            f.GetValue(null),
                            attributeResult);
                    })
                .ToDictionary(p => p.Key, p => p.Value);
        }

        private static string GetDefaultFieldInfoName(FieldInfo f)
        {
            return f.GetValue(null).ToString();
        }

        private class EnumHelperMetaData
        {
            public IDictionary<object, string> Names { get; set; }
            public IDictionary<object, string> ShortNames { get; set; }
            public IDictionary<object, string> Descriptions { get; set; }
        }

        public static T GetValueFromName<T>(this string name) where T : Enum
        {
            var type = typeof(T);

            foreach (var field in type.GetFields())
            {
                if (Attribute.GetCustomAttribute(field, typeof(DisplayAttribute)) is DisplayAttribute attribute)
                {
                    if (attribute.Name == name)
                    {
                        return (T)field.GetValue(null);
                    }
                }

                if (field.Name == name)
                {
                    return (T)field.GetValue(null);
                }
            }

            throw new ArgumentOutOfRangeException(nameof(name));
        }
    }
}