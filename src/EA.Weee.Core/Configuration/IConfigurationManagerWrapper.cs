namespace EA.Weee.Core.Configuration
{
    using System.Collections.Specialized;
    using EmailRules;

    public interface IConfigurationManagerWrapper
    {
        NameValueCollection AppSettings { get; }

        RulesSection InternalEmailRules { get; }
    }
}
