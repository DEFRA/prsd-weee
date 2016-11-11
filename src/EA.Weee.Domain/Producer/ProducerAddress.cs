namespace EA.Weee.Domain.Producer
{
    using System;
    using System.Linq;
    using Prsd.Core.Domain;

    public class ProducerAddress : Entity, IEquatable<ProducerAddress>
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
            return Equals(obj as ProducerAddress);
        }

        public virtual bool Equals(ProducerAddress other)
        {
            if (other == null)
            {
                return false;
            }

            return PrimaryName == other.PrimaryName
                   && SecondaryName == other.SecondaryName
                   && Street == other.Street
                   && Town == other.Town
                   && Locality == other.Locality
                   && AdministrativeArea == other.AdministrativeArea
                   && PostCode == other.PostCode
                   && object.Equals(Country, other.Country);
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

        /// <summary>
        /// Returns a concatenated string containing the address represented by this instance.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            var addressValues = new[]
            {
                PrimaryName, SecondaryName, Street, Town,
                Locality, AdministrativeArea, PostCode, Country.Name
            };

            return string.Join(", ", addressValues.Where(x => !string.IsNullOrWhiteSpace(x)));
        }

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
