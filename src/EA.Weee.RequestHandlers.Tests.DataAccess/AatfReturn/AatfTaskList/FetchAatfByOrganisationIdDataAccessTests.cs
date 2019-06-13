namespace EA.Weee.RequestHandlers.Tests.DataAccess.AatfReturn.AatfTaskList
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Threading.Tasks;
    using Domain.AatfReturn;
    using EA.Weee.DataAccess;
    using EA.Weee.Domain;
    using EA.Weee.RequestHandlers.AatfReturn;
    using EA.Weee.RequestHandlers.AatfReturn.AatfTaskList;
    using EA.Weee.RequestHandlers.AatfReturn.ObligatedReceived;
    using EA.Weee.RequestHandlers.Charges;
    using EA.Weee.Tests.Core.Model;
    using FakeItEasy;
    using FluentAssertions;
    using RequestHandlers.Shared;
    using Weee.Tests.Core;
    using Xunit;
    using CompetentAuthority = Core.Shared.CompetentAuthority;
    using Organisation = Domain.Organisation.Organisation;

    public class FetchAatfByOrganisationIdDataAccessTests
    {
        public FetchAatfByOrganisationIdDataAccessTests()
        {
        }

        [Fact]
        public async Task FetchAatfByOrganisationIdDataAccess_ReturnedListContainsAatf()
        {
            using (var database = new DatabaseWrapper())
            {
                var helper = new ModelHelper(database.Model);
                var domainHelper = new DomainHelper(database.WeeeContext);

                var dataAccess = new FetchAatfByOrganisationIdDataAccess(database.WeeeContext);
                var genericDataAccess = new GenericDataAccess(database.WeeeContext);
                var competentAuthorityDataAccess = new CommonDataAccess(database.WeeeContext);
                var competentAuthority = await competentAuthorityDataAccess.FetchCompetentAuthority(CompetentAuthority.England);
                var country = await database.WeeeContext.Countries.SingleAsync(c => c.Name == "France");
                var organisation = Organisation.CreatePartnership("Dummy");
                var contact = new AatfContact("First Name", "Last Name", "Manager", "1 Address Lane", "Address Ward", "Town", "County", "Postcode", country, "01234 567890", "email@email.com");
                var aatf = CreateAatf(competentAuthority, organisation, database, contact, FacilityType.Aatf);
                var ae = CreateAatf(competentAuthority, organisation, database, contact, FacilityType.Ae);
                
                await genericDataAccess.AddMany<Aatf>(new List<Aatf>() { aatf, ae });

                var aatfList = await dataAccess.FetchAatfByOrganisationId(organisation.Id);

                aatfList.Should().Contain(aatf);
                aatfList.Should().NotContain(ae);
            }
        }

        private Aatf CreateAatf(UKCompetentAuthority competentAuthority, Organisation organisation, DatabaseWrapper database, AatfContact contact, FacilityType facilityType)
        {
            var country = database.WeeeContext.Countries.First();

            return new Aatf("name",
                competentAuthority,
                "12345678",
                AatfStatus.Approved,
                organisation,
                new AatfAddress("name", "one", "two", "bath", "BANES", "BA2 2PL", country),
                A.Fake<AatfSize>(),
                DateTime.Now,
                contact,
                facilityType,
                2019,
                database.WeeeContext.LocalAreas.First(),
                database.WeeeContext.PanAreas.First());
        }
    }
}
