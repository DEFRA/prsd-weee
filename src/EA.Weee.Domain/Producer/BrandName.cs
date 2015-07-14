namespace EA.Weee.Domain.Producer
{
    using Prsd.Core.Domain;

    public class BrandName : Entity
    {
        public BrandName(string name)
        {
            Name = name;
        }

        protected BrandName()
        {
        }
        public string Name { get; private set; }
    }
}
