namespace EA.Weee.Domain.Tests.Unit.AatfReturn
{
    using System;
    using Domain.AatfReturn;
    using FakeItEasy;
    using FluentAssertions;
    using Xunit;

    public class WeeeReceivedAmountTests
    {
        [Fact]
        public void WeeeReceivedAmount_WeeeReceivedNotDefined_ThrowsArgumentNullException()
        {
            Action constructor = () =>
            {
                var @return = new WeeeReceivedAmount(null, 2, 2, 3);
            };

            constructor.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void WeeeReceivedAmount_GivenValidParameters_WeeeReceivedAmountPropertiesShouldBeSet()
        {
            var weeeReceived = A.Fake<WeeeReceived>();
            const int categoryId = 1;
            decimal household = 1.000m;
            decimal nonHousehold = 2.000m;

            var weeeReceivedAmount = new WeeeReceivedAmount(weeeReceived, categoryId, household, nonHousehold);

            weeeReceivedAmount.WeeeReceived.Should().Be(weeeReceived);
            weeeReceivedAmount.CategoryId.Should().Be(categoryId);
            weeeReceivedAmount.HouseholdTonnage.Should().Be(household);
            weeeReceivedAmount.NonHouseholdTonnage.Should().Be(nonHousehold);
        }
    }
}