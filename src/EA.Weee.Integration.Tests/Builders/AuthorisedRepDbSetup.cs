namespace EA.Weee.Integration.Tests.Builders
{
    using Base;
    using EA.Prsd.Core;
    using EA.Weee.Domain.Producer;
    using System.Linq;

    public class AuthorisedRepDbSetup : DbTestDataBuilder<AuthorisedRepresentative, AuthorisedRepDbSetup>
    {
        protected override AuthorisedRepresentative Instantiate()
        {
            var country = DbContext.Countries.First(c => c.Name.Equals("UK - England"));

            var producerAddress = new ProducerAddress(
                "address 1",
                string.Empty,
                "address 2",
                "town or city",
                "locality",
                "administrative area",
                country,
                "GU21");

            var producerContact = new ProducerContact(string.Empty,
                string.Empty,
                string.Empty,
                "01483",
                string.Empty,
                string.Empty,
                "test@email.com",
                producerAddress);

            instance = new AuthorisedRepresentative(
                "company name",
                "trading name",
                producerContact);

            return instance;
        }
    }
}
