namespace EA.Weee.Core.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class TestInternalUserEmailDomainElementCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new TestInternalUserEmailDomainElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            TestInternalUserEmailDomainElement myElement = element as TestInternalUserEmailDomainElement;
            if (myElement == null)
            {
                return null;
            }

            return myElement.Value.GetHashCode();
        }

        public TestInternalUserEmailDomainElement this[int index]
        {
            get { return (TestInternalUserEmailDomainElement)BaseGet(index); }
            set
            {
                if (BaseGet(index) != null)
                {
                    BaseRemoveAt(index);
                }
                BaseAdd(index, value);
            }
        }

        public void Add(TestInternalUserEmailDomainElement element)
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
