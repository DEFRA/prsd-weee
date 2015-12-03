namespace EA.Weee.Domain.Producer
{
    using System;
    using Prsd.Core.Domain;

    public class BrandName : Entity, IEquatable<BrandName>, IComparable<BrandName>
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

        public int CompareTo(BrandName other)
        {
            if (other == null)
            {
                return 1;
            }

            return Name.CompareTo(other.Name);
        }

        public string Name { get; private set; }

        public virtual Guid ProducerSubmissionId { get; private set; }

        public virtual ProducerSubmission ProducerSubmission { get; private set; }
    }
}
