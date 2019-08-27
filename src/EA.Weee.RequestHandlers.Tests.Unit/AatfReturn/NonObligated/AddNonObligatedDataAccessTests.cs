namespace EA.Weee.RequestHandlers.Tests.Unit.AatfReturn.NonObligated
{
    using DataAccess;
    using Domain.AatfReturn;
    using FakeItEasy;
    using FluentAssertions;
    using RequestHandlers.AatfReturn.NonObligated;
    using System.Collections.Generic;
    using Weee.Tests.Core;
    using Xunit;

    public class NonObligatedDataAccessTests
    {
        private readonly NonObligatedDataAccess dataAccess;
        private readonly WeeeContext context;
        private readonly DbContextHelper dbContextHelper;

        public NonObligatedDataAccessTests()
        {
            context = A.Fake<WeeeContext>();
            dbContextHelper = new DbContextHelper();
            dataAccess = new NonObligatedDataAccess(context);
        }

        [Fact]
        public void Submit_GivenNonObligatedWeeValues_ValuesShouldBeAddedToContext()
        {
            var nonObligateWeee = new List<NonObligatedWeee> { new NonObligatedWeee(), new NonObligatedWeee() };

            var nonObligatedWeeeDbSet = dbContextHelper.GetAsyncEnabledDbSet(new List<NonObligatedWeee>());

            A.CallTo(() => context.NonObligatedWeee).Returns(nonObligatedWeeeDbSet);

            dataAccess.Submit(nonObligateWeee);

            context.NonObligatedWeee.Should().AllBeEquivalentTo(nonObligateWeee);
        }

        [Fact]
        public void Submit_GivenNonObligatedWeeValues_SaveChangesAsyncShouldBeCalled()
        {
            dataAccess.Submit(new List<NonObligatedWeee>());

            A.CallTo(() => context.SaveChangesAsync()).MustHaveHappened(Repeated.Exactly.Once);
        }
    }
}
