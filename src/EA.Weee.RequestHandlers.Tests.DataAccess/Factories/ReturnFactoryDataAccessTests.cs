namespace EA.Weee.RequestHandlers.Tests.DataAccess.Factories
{
    using System;
    using System.Data.Entity;
    using System.Linq;
    using System.Threading.Tasks;
    using AutoFixture;
    using Charges;
    using Domain.AatfReturn;
    using FluentAssertions;
    using RequestHandlers.Factories;
    using Weee.Tests.Core;
    using Weee.Tests.Core.Model;
    using Xunit;
    using CompetentAuthority = Core.Shared.CompetentAuthority;
    using Organisation = Domain.Organisation.Organisation;

    public class ReturnFactoryDataAccessTests
    {
        private readonly Fixture fixture;

        public ReturnFactoryDataAccessTests()
        {
            fixture = new Fixture();
        }

        [Fact]
        public async Task FetchAatfByOrganisationIdFacilityTypeAndComplianceYear_ReturnedListContainsExpectedAatfs()
        {
            using (var database = new DatabaseWrapper())
            {
                var helper = new ModelHelper(database.Model);
                var domainHelper = new DomainHelper(database.WeeeContext);

                var organisation = Organisation.CreatePartnership("Dummy");
                
                await CreateAatf(database, organisation, FacilityType.Aatf, 2019);
                await CreateAatf(database, organisation, FacilityType.Aatf, 2020);
                await CreateAatf(database, organisation, FacilityType.Ae, 2019);

                var organisation2 = Organisation.CreatePartnership("Dummy");

                await CreateAatf(database, organisation2, FacilityType.Aatf, 2019);

                var dataAccess = new ReturnFactoryDataAccess(database.WeeeContext);

                var aatfs = await dataAccess.FetchAatfsByOrganisationFacilityTypeListAndYear(organisation.Id, 2019, EA.Weee.Core.AatfReturn.FacilityType.Aatf);

                aatfs.Count.Should().Be(1);
                aatfs.Count(a => a.FacilityType != FacilityType.Aatf).Should().Be(0);
                aatfs.Count(a => a.ComplianceYear != 2019).Should().Be(0);
            }
        }

        private async Task<Aatf> CreateAatf(DatabaseWrapper database, Organisation organisation, FacilityType facilityType, short year)
        {
            var competentAuthorityDataAccess = new CommonDataAccess(database.WeeeContext);
            var country = await database.WeeeContext.Countries.SingleAsync(c => c.Name == "France");
            var competentAuthority = await competentAuthorityDataAccess.FetchCompetentAuthority(CompetentAuthority.England);

            var aatf = new Aatf("name",
                competentAuthority,
                "12345678",
                AatfStatus.Approved,
                organisation,
                new AatfAddress("name", "one", "two", "bath", "BANES", "BA2 2PL", country),
                AatfSize.Large, 
                DateTime.Now,
                new AatfContact("first", "last", "position", "adress1", "address2", "town", "county", "postcode", country, "telephone", "email"), 
                facilityType,
                year);

            database.WeeeContext.Aatfs.Add(aatf);

            await database.WeeeContext.SaveChangesAsync();

            return aatf;
        }
    }
}
