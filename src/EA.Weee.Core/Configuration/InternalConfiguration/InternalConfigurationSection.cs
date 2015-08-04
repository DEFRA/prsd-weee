namespace EA.Weee.Core.Configuration.InternalConfiguration
{
    using System.Configuration;

    public class InternalConfigurationSection : ConfigurationSection
    {
        [ConfigurationProperty("allowedEmailSuffixes", IsRequired = true, IsDefaultCollection = true)]
        [ConfigurationCollection(typeof(AllowedEmailSuffixElement),
           AddItemName = "add",
           ClearItemsName = "clear",
           RemoveItemName = "remove")]
        public AllowedEmailSuffixesElementCollection AllowedEmailSuffixes
        {
            get
            {
                return (AllowedEmailSuffixesElementCollection)base["allowedEmailSuffixes"];
            }
        }
    }
}
