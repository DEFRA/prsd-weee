namespace EA.Weee.Core.Configuration.EmailRules
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

        public RuleElement this[int index]
        {
            get { return (RuleElement)BaseGet(index); }
            set
            {
                if (BaseGet(index) != null)
                {
                    BaseRemoveAt(index);
                }
                BaseAdd(index, value);
            }
        }

        public void Add(RuleElement element)
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
