namespace EA.Weee.Web.Services.EmailRules
{
    using System.Configuration;

    public class RuleElementCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new RuleElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            RuleElement myElement = element as RuleElement;
            if (myElement == null)
            {
                return null;
            }

            return myElement.Action.GetHashCode()
                ^ myElement.Type.GetHashCode()
                ^ myElement.Value.GetHashCode();
        }
    }
}
