namespace EA.Weee.Domain.Producer
{
    using System;
    using PCS;
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

        public string Name { get; private set; }

        public virtual Guid PartnershipId { get; private set; }
        public virtual Partnership Partnership { get; private set; }
    }
}
