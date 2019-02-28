namespace EA.Weee.RequestHandlers.Tests.DataAccess.AatfReturn.ObligatedReused
{
    using System;
    using System.Collections.Generic;
    using Domain.AatfReturn;
    using EA.Weee.DataAccess;
    using EA.Weee.RequestHandlers.AatfReturn.ObligatedReused;
    using FakeItEasy;
    using FluentAssertions;
    using Weee.Tests.Core;
    using Xunit;

    public class AddAatfSiteDataAccessTests
    {
        private readonly WeeeContext context;
        private readonly AddAatfSiteDataAccess dataAccess;
        private readonly DbContextHelper dbContextHelper;
        private readonly Guid aatfId;

        public AddAatfSiteDataAccessTests()
        {
            context = A.Fake<WeeeContext>();
            dataAccess = new AddAatfSiteDataAccess(context);
            dbContextHelper = new DbContextHelper();
            aatfId = A.Dummy<Guid>();
        }

        [Fact]
        public void Submit_GivenReusedSiteData_ValuesShouldBeAddedToContext()
        {
            var aatfAddress = new AatfAddress(
                "Name",
                "Address",
                A.Dummy<string>(),
                "TownOrCity",
                A.Dummy<string>(),
                A.Dummy<string>(),
                A.Dummy<Guid>());
            var weeeReused = new WeeeReused(aatfId, A.Dummy<Guid>());
            var weeeReusedSite = new WeeeReusedSite(weeeReused, aatfAddress);

            var weeeReusedSiteDbSet = dbContextHelper.GetAsyncEnabledDbSet(new List<WeeeReusedSite>());

            A.CallTo(() => context.WeeeReusedSite).Returns(weeeReusedSiteDbSet);

            dataAccess.Submit(weeeReusedSite);

            context.WeeeReusedSite.Should().AllBeEquivalentTo(weeeReusedSite);
        }

        [Fact]
        public void Submit_GivenReusedSiteData_SaveChangesAsyncShouldBeCalled()
        {
            dataAccess.Submit(new WeeeReusedSite());

            A.CallTo(() => context.SaveChangesAsync()).MustHaveHappened(Repeated.Exactly.Once);
        }
    }
}
