namespace EA.Weee.RequestHandlers.Tests.DataAccess.AatfReturn.ObligatedSentOn
{
    using EA.Weee.DataAccess;
    using EA.Weee.Domain;
    using EA.Weee.Domain.AatfReturn;
    using EA.Weee.RequestHandlers.AatfReturn.ObligatedSentOn;
    using EA.Weee.Tests.Core;
    using FakeItEasy;
    using FluentAssertions;
    using System.Collections.Generic;
    using Xunit;

    public class AddSentOnAatfSiteDataAccessTests
    {
        private readonly WeeeContext context;
        private readonly AddSentOnAatfSiteDataAccess dataAccess;
        private readonly DbContextHelper dbContextHelper;

        public AddSentOnAatfSiteDataAccessTests()
        {
            context = A.Fake<WeeeContext>();
            dataAccess = new AddSentOnAatfSiteDataAccess(context);
            dbContextHelper = new DbContextHelper();
        }
        
        [Fact]
        public void Submit_GivenSentOnData_ValuesShouldBeAddedToContext()
        {
            var aatf = A.Fake<Aatf>();
            var @return = A.Fake<Return>();

            var aatfAddress = new AatfAddress(
                "Name",
                "Address",
                A.Dummy<string>(),
                "TownOrCity",
                A.Dummy<string>(),
                A.Dummy<string>(),
                A.Dummy<Country>());

            var weeeSentOn = new WeeeSentOn(aatfAddress, aatf, @return);

            var weeeSentDbSet = dbContextHelper.GetAsyncEnabledDbSet(new List<WeeeSentOn>());

            A.CallTo(() => context.WeeeSentOn).Returns(weeeSentDbSet);

            dataAccess.Submit(weeeSentOn);

            context.WeeeSentOn.Should().AllBeEquivalentTo(weeeSentOn);
        }

        [Fact]
        public void Submit_GivenSentOnData_SaveChangesAsyncShouldBeCalled()
        {
            var site = new WeeeSentOn();

            dataAccess.Submit(site);

            A.CallTo(() => context.WeeeSentOn.Add(site)).MustHaveHappened(Repeated.Exactly.Once)
                .Then(A.CallTo(() => context.SaveChangesAsync()).MustHaveHappened(Repeated.Exactly.Once));
        }
    }
}
