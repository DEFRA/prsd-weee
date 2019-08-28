namespace EA.Weee.DataAccess.Tests.DataAccess.StoredProcedure
{
    using EA.Weee.DataAccess.StoredProcedure;
    using EA.Weee.Domain.AatfReturn;
    using EA.Weee.Tests.Core;
    using EA.Weee.Tests.Core.Model;
    using FluentAssertions;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using AutoFixture;
    using Domain;
    using Xunit;
    using Aatf = Domain.AatfReturn.Aatf;
    using Address = Domain.Organisation.Address;
    using Organisation = Domain.Organisation.Organisation;

    public class GetAatfAeDetailsCsvTests
    {
        private readonly Fixture fixture;

        public GetAatfAeDetailsCsvTests()
        {
            fixture = new Fixture();
        }

        [Theory]
        [InlineData(false, false)]
        [InlineData(true, false)]
        [InlineData(false, true)]
        [InlineData(true, true)]
        public async Task Execute_GivenComplianceAndFacilityTypeAndLocalAreaAndPanArea_ReturnsData(bool hasArea, bool hasPan)
        {
            using (var db = new DatabaseWrapper())
            {
                var org = OrgWithAddress(db);
                var aatf = ObligatedWeeeIntegrationCommon.CreateAatf(db, org, 2019, hasArea, hasPan, fixture.Create<string>());
                var aatfs = new List<Aatf>() { aatf };

                db.WeeeContext.Organisations.Add(org);
                db.WeeeContext.Aatfs.Add(aatf);
                await db.WeeeContext.SaveChangesAsync();

                Guid? areaId = null;
                if (hasArea)
                {
                    areaId = aatf.LocalAreaId;
                }

                Guid? panId = null;
                if (hasPan)
                {
                    panId = aatf.PanAreaId;
                }

                var results = await db.StoredProcedures.GetAatfAeDetailsCsvData(2019, 1, aatf.CompetentAuthority.Id, areaId, panId);
                
                VerifyResult(results, aatfs);              
            }
        }

        [Fact]
        public async Task Execute_GivenComplianceAndFacilityTypeOfAe_ReturnsData()
        {
            using (var db = new DatabaseWrapper())
            {
                var org = OrgWithAddress(db);
                var ae = ObligatedWeeeIntegrationCommon.CreateAe(db, org);
                var aatfs = new List<Aatf>() { ae };

                db.WeeeContext.Aatfs.Add(ae);
                await db.WeeeContext.SaveChangesAsync();

                var results = await db.StoredProcedures.GetAatfAeDetailsCsvData(2019, 2, ae.CompetentAuthority.Id, null, null);

                VerifyResult(results, aatfs);
            }
        }

        private void VerifyResult(List<AatfAeDetailsData> results, List<Aatf> aatfs)
        {
            foreach (var verifyAatf in aatfs)
            {
                results.Count(x => x.Name == verifyAatf.Name).Should().Be(1);
                var aatf = results.First(x => x.Name == verifyAatf.Name);
                aatf.ComplianceYear.Should().Be(verifyAatf.ComplianceYear);
                aatf.AppropriateAuthorityAbbr.Should().Be(verifyAatf.CompetentAuthority.Abbreviation);
                aatf.EaArea.Should().Be(verifyAatf.LocalArea?.Name);
                aatf.PanAreaTeam.Should().Be(verifyAatf.PanArea?.Name);
                aatf.Name.Should().Be(verifyAatf.Name);
                aatf.Address.Should().Be(GetSiteAddressString(verifyAatf.SiteAddress));
                aatf.PostCode.Should().Be(verifyAatf.SiteAddress.Postcode);
                aatf.ApprovalNumber.Should().Be(verifyAatf.ApprovalNumber);
                aatf.ApprovalDate.Should().Be(verifyAatf.ApprovalDate.GetValueOrDefault());
                aatf.Size.Should().Be(verifyAatf.Size.DisplayName);
                aatf.Status.Should().Be(verifyAatf.AatfStatus.DisplayName);
                aatf.ContactName.Should().Be(verifyAatf.Contact.FirstName + " " + verifyAatf.Contact.LastName);
                aatf.ContactPosition.Should().Be(verifyAatf.Contact.Position);
                aatf.ContactAddress.Should().Be(GetContactAddressString(verifyAatf.Contact));
                aatf.ContactPostcode.Should().Be(verifyAatf.Contact.Postcode);
                aatf.ContactEmail.Should().Be(verifyAatf.Contact.Email);
                aatf.ContactPhone.Should().Be(verifyAatf.Contact.Telephone);
                aatf.OrganisationName.Should().Be(verifyAatf.Organisation.OrganisationName);
                aatf.OrganisationAddress.Should().Be(GetOrganisationAddressString(verifyAatf.Organisation.BusinessAddress));
                aatf.OrganisationPostcode.Should().Be(verifyAatf.Organisation.BusinessAddress.Postcode);
            }
        }

        private static Organisation OrgWithAddress(DatabaseWrapper db)
        {
            var org = Organisation.CreatePartnership("trading");
            var address = new Address("1", "street", "Woking", "Hampshire", "GU21 5EE", db.WeeeContext.Countries.First(), "12345678", "test@co.uk");
            org.AddOrUpdateAddress(AddressType.RegisteredOrPPBAddress, address);
            return org;
        }

        private string GetSiteAddressString(AatfAddress address)
        {
            var addressString = address.Address1 + ", ";

            if (address.Address2 != null)
            {
                addressString += address.Address2 + ", ";
            }

            addressString += address.TownOrCity + ", ";

            if (address.CountyOrRegion != null)
            {
                addressString += address.CountyOrRegion + ", ";
            }

            addressString += address.Country.Name;

            return addressString;
        }

        private string GetOrganisationAddressString(Domain.Organisation.Address address)
        {
            var addressString = address.Address1 + ", ";

            if (address.Address2 != null)
            {
                addressString += address.Address2 + ", ";
            }

            addressString += address.TownOrCity + ", ";

            if (address.CountyOrRegion != null)
            {
                addressString += address.CountyOrRegion + ", ";
            }

            addressString += address.Country.Name;

            return addressString;
        }

        private string GetContactAddressString(AatfContact contact)
        {
            var addressString = contact.Address1 + ", ";

            if (contact.Address2 != null)
            {
                addressString += contact.Address2 + ", ";
            }

            addressString += contact.TownOrCity + ", ";

            if (contact.CountyOrRegion != null)
            {
                addressString += contact.CountyOrRegion + ", ";
            }

            addressString += contact.Country.Name;

            return addressString;
        }
    }
}
