namespace EA.Weee.Domain.Tests.Unit.AatfReturn
{
    using EA.Weee.Domain.AatfReturn;
    using FakeItEasy;
    using FluentAssertions;
    using System;
    using System.Collections.Generic;
    using Xunit;

    public class WeeeSentOnAmountTests
    {
        [Fact]
        public void WeeeSentOnAmount_WeeeSentOnNotDefined_ThrowsArgumentNullException()
        {
            Action constructor = () =>
            {
                var @return = new WeeeSentOnAmount(null, 2, 2, 3, A.Dummy<Guid>());
            };

            constructor.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void WeeeSentOnAmount_GivenValidParameters_WeeeSentOnAmountPropertiesShouldBeSet()
        {
            var weeeSentOn = A.Fake<WeeeSentOn>();
            const int categoryId = 1;
            var household = 1.000m;
            var nonHousehold = 2.000m;

            var weeeReceivedAmount = new WeeeSentOnAmount(weeeSentOn, categoryId, household, nonHousehold, A.Dummy<Guid>());

            weeeReceivedAmount.WeeeSentOn.Should().Be(weeeSentOn);
            weeeReceivedAmount.CategoryId.Should().Be(categoryId);
            weeeReceivedAmount.HouseholdTonnage.Should().Be(household);
            weeeReceivedAmount.NonHouseholdTonnage.Should().Be(nonHousehold);
        }

        public static IEnumerable<object[]> GetData()
        {
            var allData = new List<object[]>
            {
                new object[] { null, 1.00m },
                new object[] { 1.00m, null },
            };

            return allData;
        }

        [Theory]
        [MemberData(nameof(GetData))]
        public void UpdateTonnages_GivenTonnages_TonnageValuesShouldBeUpdated(decimal value1, decimal value2)
        {
            var amount = new WeeeSentOnAmount(A.Fake<WeeeSentOn>(), A.Dummy<int>(), 4.00m, 5.00m, A.Dummy<Guid>());

            amount.UpdateTonnages(value1, value2);

            amount.HouseholdTonnage.Should().Be(value1);
            amount.NonHouseholdTonnage.Should().Be(value2);
        }

        [Fact]
        public void UpdateTonnages_GivenTonnagesZero_TonnageValuesShouldBeUpdated()
        {
            var amount = new WeeeSentOnAmount(A.Fake<WeeeSentOn>(), A.Dummy<int>(), 4.00m, 5.00m, A.Dummy<Guid>());

            amount.UpdateTonnages(0m, 0m);

            amount.HouseholdTonnage.Should().Be(0m);
            amount.NonHouseholdTonnage.Should().Be(0m);
        }
    }
}
