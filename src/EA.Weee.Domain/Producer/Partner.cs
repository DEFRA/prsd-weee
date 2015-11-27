namespace EA.Weee.Domain.Producer
{
    using Prsd.Core.Domain;
    using System;

    public class Partner : Entity, IEquatable<Partner>, IComparable<Partner>
    {
        public Partner(string name)
        {
            Name = name;
        }

        protected Partner()
        {
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public bool Equals(Partner other)
        {
            if (other == null)
            {
                return false;
            }

            return Name == other.Name;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as Partner);
        }

        public int CompareTo(Partner other)
        {            
            if (other == null)
            {
                return 1;
            }

            return Name.CompareTo(other.Name);
        }

        public string Name { get; private set; }

        public virtual Guid PartnershipId { get; private set; }

        public virtual Partnership Partnership { get; private set; }
    }
}
