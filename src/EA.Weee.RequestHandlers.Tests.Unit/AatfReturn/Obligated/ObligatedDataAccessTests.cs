﻿namespace EA.Weee.RequestHandlers.Tests.Unit.AatfReturn.Obligated
{
    using DataAccess;
    using Domain.AatfReturn;
    using FakeItEasy;
    using System.Collections.Generic;
    using Weee.Tests.Core;
    using Xunit;

    public class ObligatedDataAccessTests
    {
        private readonly WeeeContext context;
        private readonly ObligatedDataAccessTestAccess dataAccess;
        private readonly DbContextHelper dbHelper;

        public ObligatedDataAccessTests()
        {
            dbHelper = new DbContextHelper();
            context = A.Fake<WeeeContext>();
            dataAccess = new ObligatedDataAccessTestAccess(context);
        }

        [Fact]
        public void Submit_GivenAmounts_AddRangeAndSaveChangesShouldBeCalled()
        {
            var list = new List<WeeeReceivedAmount>();
            A.CallTo(() => context.Set<WeeeReceivedAmount>()).Returns(dbHelper.GetAsyncEnabledDbSet(list));

            dataAccess.Submit(list);

            A.CallTo(() => context.Set<WeeeReceivedAmount>().AddRange(A<IEnumerable<WeeeReceivedAmount>>.That.Matches(c => c.Equals(list)))).MustHaveHappened(1, Times.Exactly).Then(
                    A.CallTo(() => context.SaveChangesAsync()).MustHaveHappened(1, Times.Exactly));
        }

        [Fact]
        public void UpdateAmounts_GivenAmountAndNewAmounts_AmountsShouldBeSetAndSaveCalled()
        {
            var weeeReceivedAmount = A.Fake<WeeeReceivedAmount>();
            A.CallTo(() => weeeReceivedAmount.HouseholdTonnage).Returns(1);
            A.CallTo(() => weeeReceivedAmount.NonHouseholdTonnage).Returns(2);

            dataAccess.UpdateAmounts(weeeReceivedAmount, 99, 100);

            A.CallTo(() => weeeReceivedAmount.UpdateTonnages(99, 100)).MustHaveHappened(1, Times.Exactly)
                .Then(A.CallTo(() => context.SaveChangesAsync()).MustHaveHappened(1, Times.Exactly));
        }
    }
}
