namespace EA.Weee.RequestHandlers.Tests.DataAccess.AatfReturn.ObligatedSentOn
{
    using EA.Weee.DataAccess;
    using EA.Weee.Domain.AatfReturn;
    using EA.Weee.RequestHandlers.AatfReturn.ObligatedSentOn;
    using EA.Weee.Tests.Core;
    using FakeItEasy;
    using FluentAssertions;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Xunit;

    public class AddObligatedSentOnDataAccessTests
    {
        private readonly ObligatedSentOnDataAccess dataAccess;
        private readonly WeeeContext context;
        private readonly DbContextHelper dbContextHelper;
        private readonly Guid aatfId;
        private readonly Guid returnId;
        private readonly Guid weeeSentOnId;
        private readonly Guid siteAddressId;

        public AddObligatedSentOnDataAccessTests()
        {
            context = A.Fake<WeeeContext>();
            dbContextHelper = new DbContextHelper();
            dataAccess = new ObligatedSentOnDataAccess(context);
            aatfId = Guid.NewGuid();
            returnId = Guid.NewGuid();
            weeeSentOnId = Guid.NewGuid();
            siteAddressId = Guid.NewGuid();
        }

        [Fact]
        public void Submit_GivenObligatedWeeeValues_ValuesShouldBeAddedToContext()
        {
            var weeeSentOn = new WeeeSentOn(siteAddressId, aatfId, returnId);

            decimal houseHoldTonnage = 1.000m;
            decimal nonHouseHoldTonnage = 2.000m;

            var obligatedSentOnWeee = new List<WeeeSentOnAmount> { new WeeeSentOnAmount(weeeSentOn, 1, houseHoldTonnage, nonHouseHoldTonnage, weeeSentOnId), new WeeeSentOnAmount(weeeSentOn, 2, houseHoldTonnage, nonHouseHoldTonnage, weeeSentOnId) };

            var obligatedSentOnWeeeDbSet = dbContextHelper.GetAsyncEnabledDbSet(new List<WeeeSentOnAmount>());

            A.CallTo(() => context.WeeeSentOnAmount).Returns(obligatedSentOnWeeeDbSet);

            dataAccess.Submit(obligatedSentOnWeee);

            context.WeeeSentOnAmount.Should().AllBeEquivalentTo(obligatedSentOnWeee);
        }

        [Fact]
        public void Submit_GivenObligatedWeeeValues_SaveChangesAsyncShouldBeCalled()
        {
            dataAccess.Submit(new List<WeeeSentOnAmount>());

            A.CallTo(() => context.SaveChangesAsync()).MustHaveHappened(Repeated.Exactly.Once);
        }
    }
}
