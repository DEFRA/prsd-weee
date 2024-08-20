namespace EA.Weee.Xml.Tests.Unit.MemberRegistration
{
    using DataAccess.DataAccess;
    using Domain.Lookup;
    using FakeItEasy;
    using System;
    using Xml.MemberRegistration;
    using Xunit;
    using System.Threading.Tasks;

    public class FetchProducerChargeTests
    {
        private readonly IProducerChargeCalculatorDataAccess producerChargeCalculatorDataAccess;
        private readonly FetchProducerCharge fetchProducerCharge;

        public FetchProducerChargeTests()
        {
            producerChargeCalculatorDataAccess = A.Fake<IProducerChargeCalculatorDataAccess>();

            fetchProducerCharge = new FetchProducerCharge(producerChargeCalculatorDataAccess);
        }

        [Theory]
        [InlineData(ChargeBand.A)]
        [InlineData(ChargeBand.B)]
        [InlineData(ChargeBand.C)]
        [InlineData(ChargeBand.D)]
        [InlineData(ChargeBand.E)]
        [InlineData(ChargeBand.A2)]
        [InlineData(ChargeBand.C2)]
        [InlineData(ChargeBand.D2)]
        [InlineData(ChargeBand.D3)]
        public async Task GetCharge_GivenChargeBand_ChargeBandAmountShouldBeRetrieved(ChargeBand band)
        {
            await fetchProducerCharge.GetCharge(band);

            A.CallTo(() => producerChargeCalculatorDataAccess.FetchCurrentChargeBandAmount(band)).MustHaveHappened(1, Times.Exactly);
        }

        [Theory]
        [InlineData(ChargeBand.A)]
        [InlineData(ChargeBand.B)]
        [InlineData(ChargeBand.C)]
        [InlineData(ChargeBand.D)]
        [InlineData(ChargeBand.E)]
        [InlineData(ChargeBand.A2)]
        [InlineData(ChargeBand.C2)]
        [InlineData(ChargeBand.D2)]
        [InlineData(ChargeBand.D3)]
        public async Task GetCharge_GivenChargeBandAmount_ProducerChargeShouldBeReturned(ChargeBand band)
        {
            var chargeBandAmount = new ChargeBandAmount(Guid.NewGuid(), band, 1);

            A.CallTo(() => producerChargeCalculatorDataAccess.FetchCurrentChargeBandAmount(band)).Returns(chargeBandAmount);

            var result = await fetchProducerCharge.GetCharge(band);

            Assert.Equal(result.Amount, chargeBandAmount.Amount);
            Assert.Equal(result.ChargeBandAmount.ChargeBand, chargeBandAmount.ChargeBand);
        }
    }
}
