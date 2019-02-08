namespace EA.Weee.RequestHandlers.Tests.DataAccess.AatfReturn
{
    using System;
    using System.Threading.Tasks;
    using Domain;
    using Domain.AatfReturn;
    using FluentAssertions;
    using RequestHandlers.AatfReturn;
    using RequestHandlers.Organisations;
    using Weee.Tests.Core;
    using Weee.Tests.Core.Model;
    using Xunit;
    using Country = Domain.Country;
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
                var organisationDetailsDataAccess = new OrganisationDetailsDataAccess(database.WeeeContext);
                var country = await organisationDetailsDataAccess.FetchCountryAsync(countryId);

                var aatf = CreateAatf(country);

                var result = await dataAccess.Add<Aatf>(aatf);

                result.Should().NotBeEmpty();
            }
        }

        private Aatf CreateAatf(Country country)
        {
            return new Aatf("name", 
                new UKCompetentAuthority(Guid.NewGuid(), "competantauth", "abbv",
                    country, "email"), 
                "12345678",
                AatfStatus.Approved,
                new Operator(Organisation.CreatePartnership("trading")));
        }
    }
}
