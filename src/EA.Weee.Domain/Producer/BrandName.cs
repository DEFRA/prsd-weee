namespace EA.Weee.Domain.Producer
{
    using System;
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

        public virtual Guid ProducerId { get; private set; }

        public virtual Producer Producer { get; private set; }
    }
}
