namespace EA.Weee.RequestHandlers.Tests.DataAccess.AatfReturn.AatfTaskList
{
    using System;
    using System.Collections.Generic;
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
        private readonly AddObligatedReceivedDataAccess dataAccess;
        private readonly WeeeContext context;
        private readonly DbContextHelper dbContextHelper;
        private readonly Organisation organisation;
        private readonly Operator @operator;

        public FetchAatfByOrganisationIdDataAccessTests()
        {
            context = A.Fake<WeeeContext>();
            dbContextHelper = new DbContextHelper();
            dataAccess = new AddObligatedReceivedDataAccess(context);
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
                var countryId = new Guid("B5EBE1D1-8349-43CD-9E87-0081EA0A8463");
                var competantAuthorityDataAccess = new CommonDataAccess(database.WeeeContext);
                var competantAuthority = await competantAuthorityDataAccess.FetchCompetentAuthority(CompetentAuthority.England);

                var aatf = CreateAatf(competantAuthority, @operator);

                await genericDataAccess.Add<Aatf>(aatf);

                List<Aatf> aatfList = await dataAccess.FetchAatfByOrganisationId(organisation.Id);

                aatfList.Should().Contain(aatf);
            }
        }

        private Aatf CreateAatf(UKCompetentAuthority competentAuthority, Operator @operator)
        {
            return new Aatf("name",
                competentAuthority,
                "12345678",
                AatfStatus.Approved,
                @operator);
        }
    }
}
