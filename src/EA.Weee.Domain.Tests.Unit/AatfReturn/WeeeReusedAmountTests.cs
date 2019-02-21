namespace EA.Weee.Domain.Tests.Unit.AatfReturn
{
    using System;
    using Domain.AatfReturn;
    using FakeItEasy;
    using FluentAssertions;
    using Xunit;

    public class WeeeReusedAmountTests
    {
        [Fact]
        public void WeeeReusedAmount_WeeeReusedNotDefined_ThrowsArgumentNullException()
        {
            Action constructor = () =>
            {
                var @return = new WeeeReusedAmount(null, 2, 2, 3);
            };

            constructor.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void WeeeReceivedAmount_GivenValidParameters_WeeeReceivedAmountPropertiesShouldBeSet()
        {
            var weeeReused = A.Fake<WeeeReused>();
            const int categoryId = 1;
            decimal household = 1.000m;
            decimal nonHousehold = 2.000m;

            var weeeReceivedAmount = new WeeeReusedAmount(weeeReused, categoryId, household, nonHousehold);

            weeeReceivedAmount.WeeeReused.Should().Be(weeeReused);
            weeeReceivedAmount.CategoryId.Should().Be(categoryId);
            weeeReceivedAmount.HouseholdTonnage.Should().Be(household);
            weeeReceivedAmount.NonHouseholdTonnage.Should().Be(nonHousehold);
        }
    }
}