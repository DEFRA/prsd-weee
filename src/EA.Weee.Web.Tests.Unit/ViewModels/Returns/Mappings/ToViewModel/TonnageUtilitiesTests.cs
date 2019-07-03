namespace EA.Weee.Web.Tests.Unit.ViewModels.Returns.Mapping.ToViewModel
{
    using System;
    using System.Collections.Generic;
    using Core.Shared;
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Web.ViewModels.Returns.Mappings.ToViewModel;
    using FakeItEasy;
    using FluentAssertions;
    using Xunit;

    public class TonnageUtilitiesTests
    {
        private readonly ITonnageUtilities tonnageUtilities;
        private readonly string nullTonnageDisplay;

        public TonnageUtilitiesTests()
        {
            tonnageUtilities = new TonnageUtilities();
            nullTonnageDisplay = "-";
        }

        [Fact]
        public void InitialiseTotalDecimal_OnNullInput_ReturnsZeroDecimal()
        {
            var result = tonnageUtilities.InitialiseTotalDecimal(null);

            result.Should().Be(0.000m);
        }

        [Fact]
        public void InitialiseTotalDecimal_OnInput_SameDecimal()
        {
            var result = tonnageUtilities.InitialiseTotalDecimal(1.234m);

            result.Should().Be(1.234m);
        }

        [Fact]
        public void CheckIfTonnageIsNull_OnNullInput_ReturnsNullTonnageDisplay()
        {
            var result = tonnageUtilities.CheckIfTonnageIsNull(null);

            result.Should().Be(nullTonnageDisplay);
        }

        [Fact]
        public void CheckIfTonnageIsNull_OnInput_ReturnsDecimalInStringFormat()
        {
            var result = tonnageUtilities.CheckIfTonnageIsNull(1.234m);

            result.Should().Be("1.234");
        }

        [Fact]
        public void SumObligatedValues_OnZeroCountInput_ReturnsNullTonnageDisplay()
        {
            var result = tonnageUtilities.SumObligatedValues(new List<WeeeObligatedData>());

            result.B2B.Should().Be(nullTonnageDisplay);
            result.B2C.Should().Be(nullTonnageDisplay);
        }

        [Fact]
        public void SumObligatedValues_OnInput_ReturnsCorrectTotal()
        {
            var obligatedDataList = new List<WeeeObligatedData>()
            {
                new WeeeObligatedData(A.Dummy<Guid>(), A.Dummy<Scheme>(), A.Dummy<AatfData>(), 0, 1.234m, 1.234m),
                new WeeeObligatedData(A.Dummy<Guid>(), A.Dummy<Scheme>(), A.Dummy<AatfData>(), 0, 2.345m, 3.456m),
            };

            var result = tonnageUtilities.SumObligatedValues(obligatedDataList);

            result.B2B.Should().Be("3.579");
            result.B2C.Should().Be("4.690");
        }

        [Fact]
        public void SumTotals_GivenNullValues_EmptyTonnageShouldBeReturned()
        {
            var result = tonnageUtilities.SumTotals(new List<decimal?>() {null, null});

            result.Should().Be("-");
        }

        [Fact]
        public void SumTotals_GivenValues_TonnageShouldBeReturned()
        {
            var result = tonnageUtilities.SumTotals(new List<decimal?>() { 1, 3, 5.001m });

            result.Should().Be("9.001");
        }

        [Fact]
        public void SumTotals_GivenValuesThatContainANullValue_TonnageShouldBeReturned()
        {
            var result = tonnageUtilities.SumTotals(new List<decimal?>() { 1, null, 5.001m });

            result.Should().Be("6.001");
        }
    }
}
