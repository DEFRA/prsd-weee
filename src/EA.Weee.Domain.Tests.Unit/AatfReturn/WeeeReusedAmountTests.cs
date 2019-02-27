namespace EA.Weee.Domain.Tests.Unit.AatfReturn
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
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
            var household = 1.000m;
            var nonHousehold = 2.000m;

            var weeeReceivedAmount = new WeeeReusedAmount(weeeReused, categoryId, household, nonHousehold);

            weeeReceivedAmount.WeeeReused.Should().Be(weeeReused);
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
            var amount = new WeeeReceivedAmount(A.Fake<WeeeReceived>(), A.Dummy<int>(), 4.00m, 5.00m);

            amount.UpdateTonnages(value1, value2);

            amount.HouseholdTonnage.Should().Be(value1);
            amount.NonHouseholdTonnage.Should().Be(value2);
        }

        [Fact]
        public void UpdateTonnages_GivenTonnagesZero_TonnageValuesShouldBeUpdated()
        {
            var amount = new WeeeReceivedAmount(A.Fake<WeeeReceived>(), A.Dummy<int>(), 4.00m, 5.00m);

            amount.UpdateTonnages(0m, 0m);

            amount.HouseholdTonnage.Should().Be(0m);
            amount.NonHouseholdTonnage.Should().Be(0m);
        }
    }
}