namespace EA.Weee.Domain.AatfReturn
{
    using System;
    using EA.Prsd.Core.Domain;

    public class AatfContact : Entity
    {
        public AatfContact()
        {
        }

        public virtual string FirstName { get; set; }

        public virtual string LastName { get; set; }

        public virtual string Position { get; set; }

        public virtual string Address1 { get; private set; }

        public virtual string Address2 { get; private set; }

        public virtual string TownOrCity { get; private set; }

        public virtual string CountyOrRegion { get; private set; }

        public virtual string Postcode { get; private set; }

        public virtual Guid CountryId { get; private set; }

        public virtual Country Country { get; private set; }

        public virtual string Telephone { get; set; }

        public virtual string Email { get; set; }

        public virtual Guid AddressId { get; set; }

        public virtual AatfAddress Address { get; set; }
    }
}