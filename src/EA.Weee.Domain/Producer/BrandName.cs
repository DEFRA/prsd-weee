namespace EA.Weee.Domain.Producer
{
    using System;
    using Prsd.Core.Domain;

    public class BrandName : Entity, IEquatable<BrandName>
    {
        public BrandName(string name)
        {
            Name = name;
        }

        protected BrandName()
        {
        }

        public virtual bool Equals(BrandName other)
        {
            if (other == null)
            {
                return false;
            }

            return Name == other.Name;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as BrandName);
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
