namespace EA.Weee.Domain.Producer
{
    using System;
    using Prsd.Core.Domain;

    public class Partner : Entity
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

        public override bool Equals(object obj)
        {
            var partnerObj = obj as Partner;
            if (partnerObj == null)
            {
                return false;
            }
            return Name.Equals(partnerObj.Name);
        }

        public string Name { get; private set; }

        public virtual Guid PartnershipId { get; private set; }

        public virtual Partnership Partnership { get; private set; }
    }
}
