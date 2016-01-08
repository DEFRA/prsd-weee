namespace EA.Weee.Core.Configuration
{
    using System.Collections.Specialized;

    public interface IConfigurationManagerWrapper
    {
        NameValueCollection AppSettings { get; }

        string GetKeyValue(string key);

        bool HasKey(string key);

        bool IsLiveEnvironment { get; }

        ITestUserEmailDomains TestInternalUserEmailDomains { get; }
    }
}
