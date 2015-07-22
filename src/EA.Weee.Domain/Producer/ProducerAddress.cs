namespace EA.Weee.Domain.Producer
{
    using System;
    using Prsd.Core.Domain;

    public class ProducerAddress : Entity
    {
        public ProducerAddress(string primaryName, string secondaryName, string street, string town, string locality,
            string administrativeArea,
            Country country, string postCode)
        {
            Country = country;
            PrimaryName = primaryName;
            SecondaryName = secondaryName;
            Street = street;
            Town = town;
            Locality = locality;
            AdministrativeArea = administrativeArea;
            PostCode = postCode;
        }

        protected ProducerAddress()
        {
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            var addressObj = obj as ProducerAddress;
            if (addressObj == null)
            {
                return false;
            }
            return PrimaryName.Equals(addressObj.PrimaryName)
                   && SecondaryName.Equals(addressObj.SecondaryName)
                   && Street.Equals(addressObj.Street)
                   && Town.Equals(addressObj.Town)
                   && Locality.Equals(addressObj.Locality)
                   && AdministrativeArea.Equals(addressObj.AdministrativeArea)
                   && Country.Name.Equals(addressObj.Country.Name);
        }

        public string PrimaryName { get; private set; }

        public string SecondaryName { get; private set; }

        public string Street { get; private set; }

        public string Town { get; private set; }

        public string Locality { get; private set; }

        public string AdministrativeArea { get; private set; }

        public string PostCode { get; private set; }

        public virtual Guid CountryId { get; private set; }

        public virtual Country Country { get; protected set; }

        public bool IsUkAddress()
        {
            if (Country != null)
            {
                return Country.Name.Contains("UK");
            }
            throw new InvalidOperationException("Country not defined.");
        }
    }
}
