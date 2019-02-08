namespace EA.Weee.RequestHandlers.Tests.DataAccess.AatfReturn
{
    using System;
    using System.Threading.Tasks;
    using Domain;
    using Domain.AatfReturn;
    using FluentAssertions;
    using RequestHandlers.AatfReturn;
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

                var aatf = CreateAatf();

                var dataAccess = new GenericDataAccess(database.WeeeContext);

                var result = await dataAccess.Add<Aatf>(aatf);

                result.Should().NotBeEmpty();
            }
        }

        private Aatf CreateAatf()
        {
            return new Aatf("name", 
                new UKCompetentAuthority(Guid.NewGuid(), "competantauth", "abbv", 
                    new Country(Guid.NewGuid(), "country"), "email"), 
                "12345678",
                AatfStatus.Approved,
                new Operator(Organisation.CreatePartnership("trading")));
        }
    }
}
