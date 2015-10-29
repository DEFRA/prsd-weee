namespace EA.Weee.Core.Configuration
{
    using System.Configuration;

    public class TestUserEmailDomainElementCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new TestUserEmailDomainElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            TestUserEmailDomainElement myElement = element as TestUserEmailDomainElement;
            if (myElement == null)
            {
                return null;
            }

            return myElement.Value.GetHashCode();
        }

        public TestUserEmailDomainElement this[int index]
        {
            get { return (TestUserEmailDomainElement)BaseGet(index); }
            set
            {
                if (BaseGet(index) != null)
                {
                    BaseRemoveAt(index);
                }
                BaseAdd(index, value);
            }
        }

        public void Add(TestUserEmailDomainElement element)
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
