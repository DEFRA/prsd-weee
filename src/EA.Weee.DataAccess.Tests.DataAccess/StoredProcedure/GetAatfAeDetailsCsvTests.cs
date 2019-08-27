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
    using Xunit;
    using Aatf = Domain.AatfReturn.Aatf;
    using Organisation = Domain.Organisation.Organisation;

    public class GetAatfAeDetailsCsvTests
    {
        [Theory]
        [InlineData(false, false)]
        [InlineData(true, false)]
        [InlineData(false, true)]
        [InlineData(true, true)]
        public async Task Execute_GivenComplianceAndFacilityTypeAndLocalAreaAndPanArea_ReturnsData(bool hasArea, bool hasPan)
        {
            using (DatabaseWrapper db = new DatabaseWrapper())
            {
                db.WeeeContext.Aatfs.RemoveRange(db.WeeeContext.Aatfs);
                await db.WeeeContext.SaveChangesAsync();

                OrganisationHelper orgHelper = new OrganisationHelper();

                Organisation org = orgHelper.GetOrganisationWithDetails("org", "org", "1234", Domain.Organisation.OrganisationType.Partnership, Domain.Organisation.OrganisationStatus.Complete);

                Aatf aatf = ObligatedWeeeIntegrationCommon.CreateAatf(db, org, 2019, hasArea, hasPan);
                List<Aatf> aatfs = new List<Aatf>() { aatf };

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

                List<AatfAeDetailsData> results = await db.StoredProcedures.GetAatfAeDetailsCsvData(2019, 1, aatf.CompetentAuthority.Id, areaId, panId);
                
                VerifyResult(results, aatfs);              
            }
        }

        [Fact]
        public async Task Execute_GivenComplianceAndFacilityTypeOfAe_ReturnsData()
        {
            using (DatabaseWrapper db = new DatabaseWrapper())
            {
                db.WeeeContext.Aatfs.RemoveRange(db.WeeeContext.Aatfs);
                await db.WeeeContext.SaveChangesAsync();

                OrganisationHelper orgHelper = new OrganisationHelper();

                Organisation org = orgHelper.GetOrganisationWithDetails("org", "org", "1234", Domain.Organisation.OrganisationType.Partnership, Domain.Organisation.OrganisationStatus.Complete);

                Aatf ae = ObligatedWeeeIntegrationCommon.CreateAe(db, org);
                List<Aatf> aatfs = new List<Aatf>() { ae };

                db.WeeeContext.Aatfs.Add(ae);
                await db.WeeeContext.SaveChangesAsync();

                List<AatfAeDetailsData> results = await db.StoredProcedures.GetAatfAeDetailsCsvData(2019, 2, ae.CompetentAuthority.Id, null, null);

                VerifyResult(results, aatfs);
            }
        }

        private void VerifyResult(List<AatfAeDetailsData> results, List<Aatf> aatfs)
        {
            Assert.Equal(aatfs.Count(), results.Count());

            for (int i = 0; i < results.Count; i++)
            {
                results.ElementAt(i).ComplianceYear.Should().Be(aatfs[i].ComplianceYear);
                results.ElementAt(i).AppropriateAuthorityAbbr.Should().Be(aatfs[i].CompetentAuthority.Abbreviation);
                results.ElementAt(i).EaArea.Should().Be(aatfs[i].LocalArea?.Name);
                results.ElementAt(i).PanAreaTeam.Should().Be(aatfs[i].PanArea?.Name);
                results.ElementAt(i).Name.Should().Be(aatfs[i].Name);
                results.ElementAt(i).Address.Should().Be(GetSiteAddressString(aatfs[i].SiteAddress));
                results.ElementAt(i).PostCode.Should().Be(aatfs[i].SiteAddress.Postcode);
                results.ElementAt(i).ApprovalNumber.Should().Be(aatfs[i].ApprovalNumber);
                results.ElementAt(i).ApprovalDate.Should().Be(aatfs[i].ApprovalDate.GetValueOrDefault());
                results.ElementAt(i).Size.Should().Be(aatfs[i].Size.DisplayName);
                results.ElementAt(i).Status.Should().Be(aatfs[i].AatfStatus.DisplayName);
                results.ElementAt(i).ContactName.Should().Be(aatfs[i].Contact.FirstName + " " + aatfs[i].Contact.LastName);
                results.ElementAt(i).ContactPosition.Should().Be(aatfs[i].Contact.Position);
                results.ElementAt(i).ContactAddress.Should().Be(GetContactAddressString(aatfs[i].Contact));
                results.ElementAt(i).ContactEmail.Should().Be(aatfs[i].Contact.Email);
                results.ElementAt(i).ContactPhone.Should().Be(aatfs[i].Contact.Telephone);
                results.ElementAt(i).OrganisationName.Should().Be(aatfs[i].Organisation.OrganisationName);
                results.ElementAt(i).OrganisationAddress.Should().Be(GetOrganisationAddressString(aatfs[i].Organisation.BusinessAddress));
                results.ElementAt(i).OrganisationPostcode.Should().Be(aatfs[i].Organisation.BusinessAddress.Postcode);
            }
        }

        private string GetSiteAddressString(AatfAddress address)
        {
            string addressString = address.Address1 + ", ";

            if (address.Address2 != null)
            {
                addressString += address.Address2 + ", ";
            }

            addressString += address.TownOrCity + ", ";

            if (address.CountyOrRegion != null)
            {
                addressString += address.CountyOrRegion;
            }

            return addressString;
        }

        private string GetOrganisationAddressString(Domain.Organisation.Address address)
        {
            string addressString = address.Address1 + ", ";

            if (address.Address2 != null)
            {
                addressString += address.Address2 + ", ";
            }

            addressString += address.TownOrCity + ", ";

            if (address.CountyOrRegion != null)
            {
                addressString += address.CountyOrRegion;
            }

            return addressString;
        }

        private string GetContactAddressString(AatfContact contact)
        {
            string addressString = contact.Address1 + ", ";

            if (contact.Address2 != null)
            {
                addressString += contact.Address2 + ", ";
            }

            addressString += contact.TownOrCity + ", ";

            if (contact.CountyOrRegion != null)
            {
                addressString += contact.CountyOrRegion;
            }

            return addressString;
        }
    }
}
