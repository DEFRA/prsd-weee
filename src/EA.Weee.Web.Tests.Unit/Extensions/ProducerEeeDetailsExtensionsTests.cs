namespace EA.Weee.Web.Tests.Unit.Extensions
{
    using System.Collections.Generic;
    using Core.Admin;
    using Core.DataReturns;
    using Core.Shared;
    using Web.Extensions;
    using Xunit;

    public class ProducerEeeDetailsExtensionsTests
    {
        [Theory]
        [InlineData(QuarterSelection.All, WeeeCategory.AutomaticDispensers, ObligationType.B2B)]
        [InlineData(QuarterSelection.All, WeeeCategory.AutomaticDispensers, ObligationType.B2C)]
        [InlineData(QuarterSelection.Q1, WeeeCategory.AutomaticDispensers, ObligationType.B2B)]
        [InlineData(QuarterSelection.Q1, WeeeCategory.AutomaticDispensers, ObligationType.B2C)]
        [InlineData(QuarterSelection.Q2, WeeeCategory.AutomaticDispensers, ObligationType.B2B)]
        [InlineData(QuarterSelection.Q2, WeeeCategory.AutomaticDispensers, ObligationType.B2C)]
        [InlineData(QuarterSelection.Q3, WeeeCategory.AutomaticDispensers, ObligationType.B2B)]
        [InlineData(QuarterSelection.Q3, WeeeCategory.AutomaticDispensers, ObligationType.B2C)]
        [InlineData(QuarterSelection.Q4, WeeeCategory.AutomaticDispensers, ObligationType.B2B)]
        [InlineData(QuarterSelection.Q4, WeeeCategory.AutomaticDispensers, ObligationType.B2C)]
        public void NoEeeInProducerEeeDetailsForOneCategory_TonnageShouldAlwaysBeEmpty(QuarterSelection quarter,
            WeeeCategory weeeCategory, ObligationType obligationType)
        {
            var producerDetails = new ProducerEeeDetails();

            var result = producerDetails.DisplayTonnage(quarter, weeeCategory, obligationType);

            Assert.Empty(result);
        }

        [Fact]
        public void EeeForOneCategoryObligationTypeQuarter_ShouldBePresentWhenSelectingQuarterAndObligationTypeAndCategory()
        {
            const decimal tonnage = 25.3M;

            var eee = new Eee(tonnage, WeeeCategory.DisplayEquipment, ObligationType.B2B);

            var producerDetails = new ProducerEeeDetails
            {
                Q3EEE = new List<Eee> { eee }
            };

            var result = producerDetails.DisplayTonnage(QuarterSelection.Q3, eee.Category, eee.ObligationType);
            Assert.Equal(tonnage.ToString(), result);
        }
    }
}
