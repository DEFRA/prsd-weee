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

        public override bool Equals(object obj)
        {
            var brandObj = obj as BrandName;
            if (brandObj == null)
            {
                return false;
            }
            return Name.Equals(brandObj.Name);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public string Name { get; private set; }

        public virtual Guid ProducerId { get; private set; }

        public virtual Producer Producer { get; private set; }
    }
}
