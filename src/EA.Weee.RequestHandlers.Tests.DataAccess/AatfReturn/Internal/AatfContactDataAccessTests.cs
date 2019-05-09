namespace EA.Weee.RequestHandlers.Tests.DataAccess.AatfReturn.Internal
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.DataAccess;
    using EA.Weee.Domain;
    using EA.Weee.Domain.AatfReturn;
    using EA.Weee.RequestHandlers.AatfReturn.Internal;
    using EA.Weee.Tests.Core;
    using FakeItEasy;
    using FluentAssertions;
    using Xunit;

    public class AatfContactDataAccessTests
    {
        private readonly WeeeContext context;
        private readonly AatfContactDataAccess dataAccess;
        private readonly DbContextHelper dbContextHelper;

        public AatfContactDataAccessTests()
        {
            context = A.Fake<WeeeContext>();
            dbContextHelper = new DbContextHelper();

            dataAccess = new AatfContactDataAccess(context);
        }

        [Fact]
        public async Task GetContact_GivenAatfId_ContactShouldBeReturned()
        {
            var aatfId = Guid.NewGuid();

            var aatfContact = A.Fake<AatfContact>();
            var aatf = A.Fake<Aatf>();

            A.CallTo(() => context.AatfContacts).Returns(dbContextHelper.GetAsyncEnabledDbSet(new List<AatfContact>() { aatfContact }));
            A.CallTo(() => aatf.Id).Returns(aatfId);
            A.CallTo(() => aatf.Contact).Returns(aatfContact);
            A.CallTo(() => context.Aatfs).Returns(dbContextHelper.GetAsyncEnabledDbSet(new List<Aatf>() { aatf }));

            var result = await dataAccess.GetContact(aatfId);

            result.Should().BeEquivalentTo(aatfContact);
        }

        [Fact]
        public void Update_GivenNewAddressData_SaveChangesAsyncShouldBeCalled()
        {
            var oldSite = A.Fake<AatfContact>();
            var newSite = new AatfContactData();
            var country = A.Fake<Country>();

            dataAccess.Update(oldSite, newSite, country);

            A.CallTo(() => oldSite.UpdateDetails(
                newSite.FirstName,
                newSite.LastName,
                newSite.Position,
                newSite.Address1,
                newSite.Address2,
                newSite.TownOrCity,
                newSite.CountyOrRegion,
                newSite.Postcode,
                country,
                newSite.Telephone,
                newSite.Email)).MustHaveHappened(Repeated.Exactly.Once)
            .Then(A.CallTo(() => context.SaveChangesAsync()).MustHaveHappened(Repeated.Exactly.Once));
        }
    }
}
