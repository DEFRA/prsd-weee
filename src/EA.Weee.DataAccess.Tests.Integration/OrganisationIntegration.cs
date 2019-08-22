﻿namespace EA.Weee.DataAccess.Tests.Integration
{
    using Domain;
    using Domain.Organisation;
    using System;
    using System.Data.Entity;
    using System.Linq;
    using System.Threading.Tasks;
    using Weee.Tests.Core.Model;
    using Xunit;
    using Address = Domain.Organisation.Address;
    using Contact = Domain.Organisation.Contact;
    using Organisation = Domain.Organisation.Organisation;

    public class OrganisationIntegration
    {
        [Fact]
        public async Task CanCreateRegisteredCompany()
        {
            using (DatabaseWrapper database = new DatabaseWrapper())
            {
                var context = database.WeeeContext;
                var contact = MakeContact();

                string name = "Test Name" + Guid.NewGuid();
                string tradingName = "Test Trading Name" + Guid.NewGuid();
                string crn = "ABC12345";
                var status = OrganisationStatus.Incomplete;
                var type = OrganisationType.RegisteredCompany;

                var businessAddress = await MakeUKAddress(context, "B");
                var notificationAddress = await MakeInternationalAddress(context, "N");

                var organisation = Organisation.CreateRegisteredCompany(name, crn, tradingName);
                organisation.AddOrUpdateAddress(AddressType.RegisteredOrPPBAddress, businessAddress);
                organisation.AddOrUpdateAddress(AddressType.ServiceOfNoticeAddress, notificationAddress);

                context.Organisations.Add(organisation);
                await context.SaveChangesAsync();

                var thisTestOrganisationArray = context.Organisations.Where(o => o.Name == name).ToArray();

                Assert.NotNull(thisTestOrganisationArray);
                Assert.NotEmpty(thisTestOrganisationArray);

                var thisTestOrganisation = thisTestOrganisationArray.FirstOrDefault();

                VerifyOrganisation(name, null, crn, status, type, thisTestOrganisation);
                if (thisTestOrganisation != null)
                {
                    VerifyAddress(businessAddress, thisTestOrganisation.BusinessAddress);
                }
            }
        }

        [Fact]
        public async Task CanCreatePartnership()
        {
            using (DatabaseWrapper database = new DatabaseWrapper())
            {
                var context = database.WeeeContext;
                var contact = MakeContact();

                string tradingName = "Test Trading Name" + Guid.NewGuid();
                var status = OrganisationStatus.Incomplete;
                var type = OrganisationType.Partnership;

                var businessAddress = await MakeUKAddress(context, "B");
                var notificationAddress = await MakeInternationalAddress(context, "N");

                var organisation = Organisation.CreatePartnership(tradingName);

                organisation.AddOrUpdateAddress(AddressType.RegisteredOrPPBAddress, businessAddress);
                organisation.AddOrUpdateAddress(AddressType.ServiceOfNoticeAddress, notificationAddress);

                context.Organisations.Add(organisation);

                await context.SaveChangesAsync();

                var thisTestOrganisationArray =
                    context.Organisations.Where(o => o.TradingName == tradingName).ToArray();

                Assert.NotNull(thisTestOrganisationArray);
                Assert.NotEmpty(thisTestOrganisationArray);

                var thisTestOrganisation = thisTestOrganisationArray.FirstOrDefault();

                VerifyOrganisation(null, tradingName, null, status, type, thisTestOrganisation);
                VerifyAddress(businessAddress, thisTestOrganisation.BusinessAddress);
            }
        }

        [Fact]
        public async Task CanCreateSoleTrader()
        {
            using (DatabaseWrapper database = new DatabaseWrapper())
            {
                var context = database.WeeeContext;
                var contact = MakeContact();

                string companyName = "Test Trading Name" + Guid.NewGuid();
                var status = OrganisationStatus.Incomplete;
                var type = OrganisationType.SoleTraderOrIndividual;

                var businessAddress = await MakeUKAddress(context, "B");
                var notificationAddress = await MakeInternationalAddress(context, "N");

                var organisation = Organisation.CreateSoleTrader(companyName);

                organisation.AddOrUpdateAddress(AddressType.RegisteredOrPPBAddress, businessAddress);
                organisation.AddOrUpdateAddress(AddressType.ServiceOfNoticeAddress, notificationAddress);

                context.Organisations.Add(organisation);

                await context.SaveChangesAsync();

                var thisTestOrganisationArray =
                    context.Organisations.Where(o => o.Name == companyName).ToArray();

                Assert.NotNull(thisTestOrganisationArray);
                Assert.NotEmpty(thisTestOrganisationArray);

                var thisTestOrganisation = thisTestOrganisationArray.FirstOrDefault();

                VerifyOrganisation(companyName, null, null, status, type, thisTestOrganisation);
                VerifyAddress(businessAddress, thisTestOrganisation.BusinessAddress);
            }
        }

        private void VerifyOrganisation(string expectedName, string expectedTradingName, string expectedCrn, OrganisationStatus expectedStatus, OrganisationType expectedType, Organisation fromDatabase)
        {
            Assert.NotNull(fromDatabase);

            if (expectedType == OrganisationType.RegisteredCompany)
            {
                Assert.Equal(expectedName, fromDatabase.Name);
            }
            else
            {
                Assert.Equal(expectedTradingName, fromDatabase.TradingName);
            }

            Assert.Equal(expectedCrn, fromDatabase.CompanyRegistrationNumber);
            Assert.Equal(expectedStatus, fromDatabase.OrganisationStatus);
            Assert.Equal(expectedType, fromDatabase.OrganisationType);

            var thisTestOrganisationAddress = fromDatabase.BusinessAddress;
            Assert.NotNull(thisTestOrganisationAddress);
        }

        private void VerifyAddress(Address expected, Address fromDatabase)
        {
            Assert.NotNull(fromDatabase);

            // this wants to be in a compare method on the class, doesn't it? will have to exclude Id/RowVersion though
            Assert.Equal(expected.Address1, fromDatabase.Address1);
            Assert.Equal(expected.Address2, fromDatabase.Address2);
            Assert.Equal(expected.TownOrCity, fromDatabase.TownOrCity);
            Assert.Equal(expected.CountyOrRegion, fromDatabase.CountyOrRegion);
            Assert.Equal(expected.Postcode, fromDatabase.Postcode);
            Assert.Equal(expected.Country.Id, fromDatabase.Country.Id);
            Assert.Equal(expected.Telephone, fromDatabase.Telephone);
            Assert.Equal(expected.Email, fromDatabase.Email);
        }

        private async Task<Address> MakeInternationalAddress(WeeeContext context, string identifier)
        {
            var country = await context.Countries.SingleAsync(c => c.Name == "France");
            return new Address(
                "Line 1 " + identifier,
                "Line 2 " + identifier,
                "Town " + identifier,
                "Region" + identifier,
                "Postcode " + identifier,
                 country,
                "Phone" + identifier,
                "Email" + identifier);
        }

        private async Task<Address> MakeUKAddress(WeeeContext context, string identifier)
        {
            var country = await context.Countries.SingleAsync(c => c.Name == "UK - England");
            return new Address(
                "Line 1 " + identifier,
                "Line 2 " + identifier,
                "Town " + identifier,
                "Region" + identifier,
                "Postcode " + identifier,
                country,
                "Phone" + identifier,
                "Email" + identifier);
        }

        private Contact MakeContact()
        {
            return new Contact("Test firstname", "Test lastname", "Test position");
        }
    }
}