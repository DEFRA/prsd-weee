namespace EA.Weee.Core.Configuration.InternalConfiguration
{
    using System.Configuration;

    public class AllowedEmailSuffixesElementCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new AllowedEmailSuffixElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            var myElement = element as AllowedEmailSuffixElement;
            if (myElement == null)
            {
                return null;
            }

            return myElement.Value.GetHashCode();
        }

        public AllowedEmailSuffixElement this[int index]
        {
            get { return (AllowedEmailSuffixElement)BaseGet(index); }
            set
            {
                if (BaseGet(index) != null)
                {
                    BaseRemoveAt(index);
                }
                BaseAdd(index, value);
            }
        }

        public void Add(AllowedEmailSuffixElement element)
        {
            BaseAdd(element);
        }

        public void Clear()
        {
            BaseClear();
        }

        public void RemoveAt(int index)
        {
            BaseRemoveAt(index);
        }

        public void Remove(string name)
        {
            BaseRemove(name);
        }
    }
}
