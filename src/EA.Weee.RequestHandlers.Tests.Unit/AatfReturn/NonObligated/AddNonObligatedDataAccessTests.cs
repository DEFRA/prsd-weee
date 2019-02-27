namespace EA.Weee.RequestHandlers.Tests.Unit.AatfReturn.NonObligated
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using DataAccess;
    using Domain.AatfReturn;
    using EA.Prsd.Core.Domain;
    using EA.Weee.RequestHandlers.AatfReturn;
    using FakeItEasy;
    using FluentAssertions;
    using RequestHandlers.AatfReturn.NonObligated;
    using Weee.Tests.Core;
    using Xunit;
    using DatabaseWrapper = EA.Weee.Tests.Core.Model.DatabaseWrapper;

    public class AddNonObligatedDataAccessTests
    {
        private readonly AddNonObligatedDataAccess dataAccess;
        private readonly WeeeContext context;
        private readonly DbContextHelper dbContextHelper;

        public AddNonObligatedDataAccessTests()
        {
            context = A.Fake<WeeeContext>();
            dbContextHelper = new DbContextHelper();
            dataAccess = new AddNonObligatedDataAccess(context);
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
