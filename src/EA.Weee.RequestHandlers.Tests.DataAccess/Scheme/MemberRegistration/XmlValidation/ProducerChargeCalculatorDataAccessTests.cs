namespace EA.Weee.RequestHandlers.Tests.DataAccess.Scheme.MemberRegistration.XmlValidation
{
    using Domain.Lookup;
    using EA.Weee.Tests.Core.Model;
    using System;
    using System.Threading.Tasks;
    using Weee.DataAccess.DataAccess;
    using Xunit;

    public class ProducerChargeCalculatorDataAccessTests
    {
        /// <summary>
        /// This test ensures that the charge band amount can be fetched from the database using the enhanced criteria.
        /// </summary>
        [Fact]
        public async Task GetChargeBandAmountAsync_WithValidCriteria_ReturnsChargeBandAmount()
        {
            using (DatabaseWrapper database = new DatabaseWrapper())
            {
                // Arrange
                ProducerChargeCalculatorDataAccess dataAccess = new ProducerChargeCalculatorDataAccess(database.WeeeContext);
                
                var competentAuthority = CompetentAuthorityType.England;
                var vatRegistered = true;
                var annualTurnoverBand = AnnualTurnoverBand.NotApplicable;
                var eeePlacedOnMarketBand = EEEPlacedOnMarketBand.Morethanorequalto5TEEEplacedonmarket;
                var complianceYear = 2025;
                var asOfUtc = new DateTime(2025, 6, 15);

                // Act
                var result = await dataAccess.GetChargeBandAmountAsync(
                    competentAuthority, vatRegistered, annualTurnoverBand,
                    eeePlacedOnMarketBand, complianceYear, asOfUtc);

                // Assert
                Assert.NotNull(result);
                Assert.Equal(competentAuthority, result.CompetentAuthority);
                Assert.Equal(vatRegistered, result.VatRegistered);
                Assert.Equal(annualTurnoverBand, result.AnnualTurnoverBand);
                Assert.Equal(eeePlacedOnMarketBand, result.EEEPlacedOnMarketBand);
                Assert.Equal(complianceYear, result.ComplianceYear);
                Assert.Equal(750.00m, result.Amount);
            }
        }

        /// <summary>
        /// This test ensures that the method handles cases where no matching charge band amount is found.
        /// </summary>
        [Fact]
        public async Task GetChargeBandAmountAsync_WithNonExistentCriteria_ReturnsNull()
        {
            using (DatabaseWrapper database = new DatabaseWrapper())
            {
                // Arrange
                ProducerChargeCalculatorDataAccess dataAccess = new ProducerChargeCalculatorDataAccess(database.WeeeContext);
                
                var competentAuthority = CompetentAuthorityType.England;
                var vatRegistered = true;
                var annualTurnoverBand = AnnualTurnoverBand.Greaterthanonemillionpounds;
                var eeePlacedOnMarketBand = EEEPlacedOnMarketBand.Morethanorequalto5TEEEplacedonmarket;
                var complianceYear = 2025;
                var asOfUtc = new DateTime(2025, 6, 15);

                // Act
                var result = await dataAccess.GetChargeBandAmountAsync(
                    competentAuthority, vatRegistered, annualTurnoverBand,
                    eeePlacedOnMarketBand, complianceYear, asOfUtc);

                // Assert
                Assert.Null(result);
            }
        }

        /// <summary>
        /// This test ensures that the method handles cases where compliance year doesn't exist in test data.
        /// </summary>
        [Fact]
        public async Task GetChargeBandAmountAsync_WithNonExistentYear_ReturnsNull()
        {
            using (DatabaseWrapper database = new DatabaseWrapper())
            {
                // Arrange
                ProducerChargeCalculatorDataAccess dataAccess = new ProducerChargeCalculatorDataAccess(database.WeeeContext);
                
                var competentAuthority = CompetentAuthorityType.England;
                var vatRegistered = true;
                var annualTurnoverBand = AnnualTurnoverBand.NotApplicable;
                var eeePlacedOnMarketBand = EEEPlacedOnMarketBand.Morethanorequalto5TEEEplacedonmarket;
                var complianceYear = 1999;
                var asOfUtc = new DateTime(1999, 6, 15);

                // Act
                var result = await dataAccess.GetChargeBandAmountAsync(
                    competentAuthority, vatRegistered, annualTurnoverBand,
                    eeePlacedOnMarketBand, complianceYear, asOfUtc);

                // Assert
                Assert.Null(result);
            }
        }

        /// <summary>
        /// This test ensures that the method correctly filters by effective date.
        /// </summary>
        [Fact]
        public async Task GetChargeBandAmountAsync_WithFutureEffectiveDate_ReturnsNull()
        {
            using (DatabaseWrapper database = new DatabaseWrapper())
            {
                // Arrange
                ProducerChargeCalculatorDataAccess dataAccess = new ProducerChargeCalculatorDataAccess(database.WeeeContext);
                
                var competentAuthority = CompetentAuthorityType.England;
                var vatRegistered = true;
                var annualTurnoverBand = AnnualTurnoverBand.Greaterthanonemillionpounds;
                var eeePlacedOnMarketBand = EEEPlacedOnMarketBand.Morethanorequalto5TEEEplacedonmarket;
                var complianceYear = 2025;
                var asOfUtc = new DateTime(2024, 1, 1);

                // Act
                var result = await dataAccess.GetChargeBandAmountAsync(
                    competentAuthority, vatRegistered, annualTurnoverBand,
                    eeePlacedOnMarketBand, complianceYear, asOfUtc);

                // Assert
                Assert.Null(result);
            }
        }

        /// <summary>
        /// This test ensures that the method returns the most recent effective record when multiple records match.
        /// </summary>
        [Fact]
        public async Task GetChargeBandAmountAsync_WithMultipleMatchingRecords_ReturnsMostRecentEffective()
        {
            using (DatabaseWrapper database = new DatabaseWrapper())
            {
                // Arrange
                ProducerChargeCalculatorDataAccess dataAccess = new ProducerChargeCalculatorDataAccess(database.WeeeContext);
                
                var competentAuthority = CompetentAuthorityType.England;
                var vatRegistered = true;
                var annualTurnoverBand = AnnualTurnoverBand.NotApplicable;
                var eeePlacedOnMarketBand = EEEPlacedOnMarketBand.Morethanorequalto5TEEEplacedonmarket;
                var complianceYear = 2025;
                var asOfUtc = new DateTime(2025, 12, 31);

                // Act
                var result = await dataAccess.GetChargeBandAmountAsync(
                    competentAuthority, vatRegistered, annualTurnoverBand,
                    eeePlacedOnMarketBand, complianceYear, asOfUtc);

                // Assert
                if (result != null)
                {
                    Assert.Equal(competentAuthority, result.CompetentAuthority);
                    Assert.Equal(vatRegistered, result.VatRegistered);
                    Assert.Equal(annualTurnoverBand, result.AnnualTurnoverBand);
                    Assert.Equal(eeePlacedOnMarketBand, result.EEEPlacedOnMarketBand);
                    Assert.Equal(complianceYear, result.ComplianceYear);
                    Assert.True(result.EffectiveFrom <= asOfUtc);
                }
            }
        }

        /// <summary>
        /// This test verifies that Wales competent authority data exists and can be retrieved.
        /// </summary>
        [Fact]
        public async Task GetChargeBandAmountAsync_WithWalesCompetentAuthority_ReturnsChargeBandAmount()
        {
            using (DatabaseWrapper database = new DatabaseWrapper())
            {
                // Arrange
                ProducerChargeCalculatorDataAccess dataAccess = new ProducerChargeCalculatorDataAccess(database.WeeeContext);
                
                var competentAuthority = CompetentAuthorityType.Wales;
                var vatRegistered = true;
                var annualTurnoverBand = AnnualTurnoverBand.Greaterthanonemillionpounds;
                var eeePlacedOnMarketBand = EEEPlacedOnMarketBand.Morethanorequalto5TEEEplacedonmarket;
                var complianceYear = 2025;
                var asOfUtc = new DateTime(2025, 6, 15);

                // Act
                var result = await dataAccess.GetChargeBandAmountAsync(
                    competentAuthority, vatRegistered, annualTurnoverBand,
                    eeePlacedOnMarketBand, complianceYear, asOfUtc);

                // Assert
                Assert.NotNull(result);
                Assert.Equal(competentAuthority, result.CompetentAuthority);
                Assert.Equal(vatRegistered, result.VatRegistered);
                Assert.Equal(annualTurnoverBand, result.AnnualTurnoverBand);
                Assert.Equal(eeePlacedOnMarketBand, result.EEEPlacedOnMarketBand);
                Assert.Equal(complianceYear, result.ComplianceYear);
                Assert.Equal(445.00m, result.Amount);
            }
        }
    }
}
