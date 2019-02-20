namespace EA.Weee.RequestHandlers.Tests.DataAccess.AatfReturn
{
    using System;
    using System.Threading.Tasks;
    using Charges;
    using Domain;
    using Domain.AatfReturn;
    using FluentAssertions;
    using RequestHandlers.AatfReturn;
    using Weee.Tests.Core;
    using Weee.Tests.Core.Model;
    using Xunit;
    using CompetentAuthority = Core.Shared.CompetentAuthority;
    using Operator = Domain.AatfReturn.Operator;
    using Organisation = Domain.Organisation.Organisation;

    public class GenericDataAccessTests
    {
        [Fact]
        public async Task Add_EntityShouldBeAdded()
        {
            using (var database = new DatabaseWrapper())
            {
                var helper = new ModelHelper(database.Model);
                var domainHelper = new DomainHelper(database.WeeeContext);

                var dataAccess = new GenericDataAccess(database.WeeeContext);
                var countryId = new Guid("B5EBE1D1-8349-43CD-9E87-0081EA0A8463");
                var competantAuthorityDataAccess = new CommonDataAccess(database.WeeeContext);
                var competantAuthority = await competantAuthorityDataAccess.FetchCompetentAuthority(CompetentAuthority.England);

                var aatf = CreateAatf(competantAuthority);

                var result = await dataAccess.Add<Aatf>(aatf);

                result.Should().NotBeEmpty();
            }
        }

        private Aatf CreateAatf(UKCompetentAuthority competentAuthority)
        {
            return new Aatf("name",
                competentAuthority,
                "12345678",
                AatfStatus.Approved,
                new Operator(Organisation.CreatePartnership("trading")));
        }
    }
}
