namespace EA.Weee.Core.Configuration
{
    using System.Collections.Specialized;
    using InternalConfiguration;

    public interface IConfigurationManagerWrapper
    {
        NameValueCollection AppSettings { get; }

        InternalConfigurationSection InternalConfiguration { get; }
    }
}
