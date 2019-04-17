namespace EA.Weee.RequestHandlers.Tests.DataAccess.Scheme.MemberRegistration.XmlValidation
{
    using Domain.Lookup;
    using EA.Weee.RequestHandlers.Scheme.MemberRegistration;
    using EA.Weee.Tests.Core.Model;
    using Weee.DataAccess.DataAccess;
    using Xunit;

    public class ProducerChargeCalculatorDataAccessTests
    {
        /// <summary>
        /// This test ensures that the charge band amount can be fetched from the database by type.
        /// </summary>
        [Fact]
        public async void FetchCurrentChargeBandAmount_WithChargeBandTypeA_ReturnsChargeBandAmountWithTypeA()
        {
            using (DatabaseWrapper database = new DatabaseWrapper())
            {
                // Arrange
                ProducerChargeCalculatorDataAccess dataAccess = new ProducerChargeCalculatorDataAccess(database.WeeeContext);

                // Act
                var result = await dataAccess.FetchCurrentChargeBandAmount(ChargeBand.A);

                // Assert
                Assert.NotNull(result);
                Assert.Equal(ChargeBand.A, result.ChargeBand);
            }
        }
    }
}
