namespace EA.Weee.Web.Services
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.Configuration;
    using System.Globalization;
    using System.Linq;
    using System.Web.Configuration;

    // https://github.com/NuGet/NuGetGallery
    public class ConfigurationService
    {
        private const string SettingPrefix = "Weee.";

        private IAppConfiguration currentConfiguration;

        public virtual IAppConfiguration CurrentConfiguration
        {
            get { return currentConfiguration ?? ResolveSettings(); }
            set { currentConfiguration = value; }
        }

        private AppConfiguration ResolveSettings()
        {
            return ResolveConfigObject(new AppConfiguration(), SettingPrefix);
        }

        private T ResolveConfigObject<T>(T configurationInstance, string prefix)
        {
            // Iterate over the properties
            foreach (var property in GetConfigProperties(configurationInstance))
            {
                string baseName = String.IsNullOrEmpty(property.DisplayName) ? property.Name : property.DisplayName;
                string settingName = prefix + baseName;

                string value = ReadSetting(settingName);

                if (String.IsNullOrEmpty(value))
                {
                    var defaultValue = property.Attributes.OfType<DefaultValueAttribute>().FirstOrDefault();
                    if (defaultValue != null && defaultValue.Value != null)
                    {
                        if (defaultValue.Value.GetType() == property.PropertyType)
                        {
                            property.SetValue(configurationInstance, defaultValue.Value);
                            continue;
                        }
                    }
                }

                if (!String.IsNullOrEmpty(value))
                {
                    if (property.PropertyType.IsAssignableFrom(typeof(string)))
                    {
                        property.SetValue(configurationInstance, value);
                    }
                    else if (property.Converter != null && property.Converter.CanConvertFrom(typeof(string)))
                    {
                        // Convert the value
                        property.SetValue(configurationInstance, property.Converter.ConvertFromString(value));
                    }
                }
                else if (property.Attributes.OfType<RequiredAttribute>().Any())
                {
                    throw new ConfigurationErrorsException(String.Format(CultureInfo.InvariantCulture, "Missing required configuration setting: '{0}'", settingName));
                }
            }
            return configurationInstance;
        }

        protected static IEnumerable<PropertyDescriptor> GetConfigProperties<T>(T instance)
        {
            return TypeDescriptor.GetProperties(instance).Cast<PropertyDescriptor>().Where(p => !p.IsReadOnly);
        }

        public virtual string ReadSetting(string settingName)
        {
            string value;

            var connectionString = GetConnectionString(settingName);

            if (connectionString != null)
            {
                value = connectionString.ConnectionString;
            }
            else
            {
                value = GetAppSetting(settingName);
            }

            return value;
        }

        protected virtual string GetAppSetting(string settingName)
        {
            return WebConfigurationManager.AppSettings[settingName];
        }

        protected virtual ConnectionStringSettings GetConnectionString(string settingName)
        {
            return WebConfigurationManager.ConnectionStrings[settingName];
        }
    }
}