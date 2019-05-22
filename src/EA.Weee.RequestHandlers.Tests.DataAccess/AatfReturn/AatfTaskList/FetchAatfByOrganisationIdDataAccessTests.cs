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
    using Weee.Tests.Core;
    using Xunit;
    using CompetentAuthority = Core.Shared.CompetentAuthority;
    using Operator = Domain.AatfReturn.Operator;
    using Organisation = Domain.Organisation.Organisation;

    public class FetchAatfByOrganisationIdDataAccessTests
    {
        private readonly ObligatedReceivedDataAccess dataAccess;
        private readonly WeeeContext context;
        private readonly DbContextHelper dbContextHelper;
        private readonly Organisation organisation;
        private readonly Operator @operator;

        public FetchAatfByOrganisationIdDataAccessTests()
        {
            context = A.Fake<WeeeContext>();
            dbContextHelper = new DbContextHelper();
            dataAccess = new ObligatedReceivedDataAccess(context);
            organisation = Organisation.CreatePartnership("Dummy");
            @operator = new Operator(organisation);
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
                var contact = new AatfContact("First Name", "Last Name", "Manager", "1 Address Lane", "Address Ward", "Town", "County", "Postcode", country, "01234 567890", "email@email.com");
                var aatf = CreateAatf(competentAuthority, @operator, database, contact, FacilityType.Aatf);
                var ae = CreateAatf(competentAuthority, @operator, database, contact, FacilityType.Ae);
                
                await genericDataAccess.AddMany<Aatf>(new List<Aatf>() { aatf, ae });

                var aatfList = await dataAccess.FetchAatfByOrganisationId(organisation.Id);

                aatfList.Should().Contain(aatf);
                aatfList.Should().NotContain(ae);
            }
        }

        private Aatf CreateAatf(UKCompetentAuthority competentAuthority, Operator @operator, DatabaseWrapper database, AatfContact contact, FacilityType facilityType)
        {
            var country = database.WeeeContext.Countries.First();

            return new Aatf("name",
                competentAuthority,
                "12345678",
                AatfStatus.Approved,
                @operator,
                new AatfAddress("name", "one", "two", "bath", "BANES", "BA2 2PL", country),
                A.Fake<AatfSize>(),
                DateTime.Now,
                contact,
                facilityType);
        }
    }
}
