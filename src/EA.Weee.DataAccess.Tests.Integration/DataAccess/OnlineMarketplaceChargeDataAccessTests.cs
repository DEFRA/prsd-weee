namespace EA.Weee.DataAccess.Tests.Unit.DataAccess
{
    using EA.Weee.DataAccess.DataAccess;
    using EA.Weee.Domain.Lookup;
    using EA.Weee.Tests.Core;
    using FakeItEasy;
    using FluentAssertions;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Xunit;

    public class OnlineMarketplaceChargeDataAccessTests
    {
        private readonly WeeeContext context;
        private readonly OnlineMarketplaceChargeDataAccess dataAccess;
        private readonly DbContextHelper dbContextHelper;

        public OnlineMarketplaceChargeDataAccessTests()
        {
            context = A.Fake<WeeeContext>();
            dbContextHelper = new DbContextHelper();
            dataAccess = new OnlineMarketplaceChargeDataAccess(context);
        }

        [Fact]
        public async Task GetChargeAmountAsync_GivenNoCharges_ShouldReturnNull()
        {
            // Arrange
            var competentAuthority = CompetentAuthorityType.England;
            var asOfUtc = new DateTime(2025, 6, 15);
            A.CallTo(() => context.OnlineMarketplaceCharges)
                .Returns(dbContextHelper.GetAsyncEnabledDbSet(new List<OnlineMarketplaceCharge>()));

            // Act
            var result = await dataAccess.GetChargeAmountAsync(competentAuthority, asOfUtc);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task GetChargeAmountAsync_GivenSingleCharge_WhenDateIsAfterEffectiveFrom_ShouldReturnCharge()
        {
            // Arrange
            var competentAuthority = CompetentAuthorityType.England;
            var asOfUtc = new DateTime(2025, 6, 15);
            var charge = CreateOnlineMarketplaceCharge(competentAuthority, 13631.00m, new DateTime(2025, 1, 1));

            A.CallTo(() => context.OnlineMarketplaceCharges)
                .Returns(dbContextHelper.GetAsyncEnabledDbSet(new List<OnlineMarketplaceCharge> { charge }));

            // Act
            var result = await dataAccess.GetChargeAmountAsync(competentAuthority, asOfUtc);

            // Assert
            result.Should().NotBeNull();
            result.Should().Be(charge);
            result.Amount.Should().Be(13631.00m);
        }

        [Fact]
        public async Task GetChargeAmountAsync_GivenSingleCharge_WhenDateIsBeforeEffectiveFrom_ShouldReturnNull()
        {
            // Arrange
            var competentAuthority = CompetentAuthorityType.England;
            var asOfUtc = new DateTime(2024, 12, 31);
            var charge = CreateOnlineMarketplaceCharge(competentAuthority, 13631.00m, new DateTime(2025, 1, 1));

            A.CallTo(() => context.OnlineMarketplaceCharges)
                .Returns(dbContextHelper.GetAsyncEnabledDbSet(new List<OnlineMarketplaceCharge> { charge }));

            // Act
            var result = await dataAccess.GetChargeAmountAsync(competentAuthority, asOfUtc);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task GetChargeAmountAsync_GivenSingleCharge_WhenDateIsExactlyOnEffectiveFrom_ShouldReturnCharge()
        {
            // Arrange
            var competentAuthority = CompetentAuthorityType.England;
            var effectiveDate = new DateTime(2025, 1, 1);
            var asOfUtc = effectiveDate;
            var charge = CreateOnlineMarketplaceCharge(competentAuthority, 13631.00m, effectiveDate);

            A.CallTo(() => context.OnlineMarketplaceCharges)
                .Returns(dbContextHelper.GetAsyncEnabledDbSet(new List<OnlineMarketplaceCharge> { charge }));

            // Act
            var result = await dataAccess.GetChargeAmountAsync(competentAuthority, asOfUtc);

            // Assert
            result.Should().NotBeNull();
            result.Amount.Should().Be(13631.00m);
        }

        [Fact]
        public async Task GetChargeAmountAsync_GivenMultipleCharges_WhenDateIsAfterAllEffectiveDates_ShouldReturnMostRecent()
        {
            // Arrange
            var competentAuthority = CompetentAuthorityType.England;
            var asOfUtc = new DateTime(2026, 1, 15);
            var charge2025 = CreateOnlineMarketplaceCharge(competentAuthority, 13631.00m, new DateTime(2025, 1, 1));
            var charge2026 = CreateOnlineMarketplaceCharge(competentAuthority, 14653.00m, new DateTime(2025, 10, 1));

            A.CallTo(() => context.OnlineMarketplaceCharges)
                .Returns(dbContextHelper.GetAsyncEnabledDbSet(new List<OnlineMarketplaceCharge> { charge2025, charge2026 }));

            // Act
            var result = await dataAccess.GetChargeAmountAsync(competentAuthority, asOfUtc);

            // Assert
            result.Should().NotBeNull();
            result.Amount.Should().Be(14653.00m);
        }

        [Fact]
        public async Task GetChargeAmountAsync_GivenMultipleCharges_WhenDateIsBetweenEffectiveDates_ShouldReturnApplicableCharge()
        {
            // Arrange
            var competentAuthority = CompetentAuthorityType.England;
            var asOfUtc = new DateTime(2025, 6, 15); // Between Jan 2025 and Oct 2025
            var charge2025 = CreateOnlineMarketplaceCharge(competentAuthority, 13631.00m, new DateTime(2025, 1, 1));
            var charge2026 = CreateOnlineMarketplaceCharge(competentAuthority, 14653.00m, new DateTime(2025, 10, 1));

            A.CallTo(() => context.OnlineMarketplaceCharges)
                .Returns(dbContextHelper.GetAsyncEnabledDbSet(new List<OnlineMarketplaceCharge> { charge2025, charge2026 }));

            // Act
            var result = await dataAccess.GetChargeAmountAsync(competentAuthority, asOfUtc);

            // Assert
            result.Should().NotBeNull();
            result.Amount.Should().Be(13631.00m); // Should get 2025 rate, not 2026
        }

        [Fact]
        public async Task GetChargeAmountAsync_GivenMultipleCharges_WhenDateIsExactlyOnNewerEffectiveDate_ShouldReturnNewerCharge()
        {
            // Arrange
            var competentAuthority = CompetentAuthorityType.England;
            var asOfUtc = new DateTime(2025, 10, 1); // Exactly on 2026 effective date
            var charge2025 = CreateOnlineMarketplaceCharge(competentAuthority, 13631.00m, new DateTime(2025, 1, 1));
            var charge2026 = CreateOnlineMarketplaceCharge(competentAuthority, 14653.00m, new DateTime(2025, 10, 1));

            A.CallTo(() => context.OnlineMarketplaceCharges)
                .Returns(dbContextHelper.GetAsyncEnabledDbSet(new List<OnlineMarketplaceCharge> { charge2025, charge2026 }));

            // Act
            var result = await dataAccess.GetChargeAmountAsync(competentAuthority, asOfUtc);

            // Assert
            result.Should().NotBeNull();
            result.Amount.Should().Be(14653.00m);
        }

        [Fact]
        public async Task GetChargeAmountAsync_GivenMultipleCharges_WhenDateIsBeforeAllEffectiveDates_ShouldReturnNull()
        {
            // Arrange
            var competentAuthority = CompetentAuthorityType.England;
            var asOfUtc = new DateTime(2024, 6, 15); // Before any charges
            var charge2025 = CreateOnlineMarketplaceCharge(competentAuthority, 13631.00m, new DateTime(2025, 1, 1));
            var charge2026 = CreateOnlineMarketplaceCharge(competentAuthority, 14653.00m, new DateTime(2025, 10, 1));

            A.CallTo(() => context.OnlineMarketplaceCharges)
                .Returns(dbContextHelper.GetAsyncEnabledDbSet(new List<OnlineMarketplaceCharge> { charge2025, charge2026 }));

            // Act
            var result = await dataAccess.GetChargeAmountAsync(competentAuthority, asOfUtc);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task GetChargeAmountAsync_GivenChargesInReverseOrder_ShouldStillReturnMostRecentApplicable()
        {
            // Arrange - charges added to list in reverse chronological order
            var competentAuthority = CompetentAuthorityType.England;
            var asOfUtc = new DateTime(2026, 1, 15);
            var charge2026 = CreateOnlineMarketplaceCharge(competentAuthority, 14653.00m, new DateTime(2025, 10, 1));
            var charge2025 = CreateOnlineMarketplaceCharge(competentAuthority, 13631.00m, new DateTime(2025, 1, 1));

            A.CallTo(() => context.OnlineMarketplaceCharges)
                .Returns(dbContextHelper.GetAsyncEnabledDbSet(new List<OnlineMarketplaceCharge> { charge2026, charge2025 }));

            // Act
            var result = await dataAccess.GetChargeAmountAsync(competentAuthority, asOfUtc);

            // Assert
            result.Should().NotBeNull();
            result.Amount.Should().Be(14653.00m);
        }

        [Theory]
        [InlineData("2025-01-01", 13631.00)]
        [InlineData("2025-06-15", 13631.00)]
        [InlineData("2025-09-30", 13631.00)]
        [InlineData("2025-10-01", 14653.00)]
        [InlineData("2025-12-31", 14653.00)]
        [InlineData("2026-06-15", 14653.00)]
        public async Task GetChargeAmountAsync_GivenVariousDates_ShouldReturnCorrectCharge(string dateString, decimal expectedAmount)
        {
            // Arrange
            var competentAuthority = CompetentAuthorityType.England;
            var asOfUtc = DateTime.Parse(dateString);
            var charge2025 = CreateOnlineMarketplaceCharge(competentAuthority, 13631.00m, new DateTime(2025, 1, 1));
            var charge2026 = CreateOnlineMarketplaceCharge(competentAuthority, 14653.00m, new DateTime(2025, 10, 1));

            A.CallTo(() => context.OnlineMarketplaceCharges)
                .Returns(dbContextHelper.GetAsyncEnabledDbSet(new List<OnlineMarketplaceCharge> { charge2025, charge2026 }));

            // Act
            var result = await dataAccess.GetChargeAmountAsync(competentAuthority, asOfUtc);

            // Assert
            result.Should().NotBeNull();
            result.Amount.Should().Be(expectedAmount);
        }

        [Fact]
        public async Task GetChargeAmountAsync_GivenThreeCharges_ShouldReturnCorrectChargeForMiddlePeriod()
        {
            // Arrange
            var competentAuthority = CompetentAuthorityType.England;
            var asOfUtc = new DateTime(2026, 6, 15); // Between 2026 and 2027 effective dates
            var charge2025 = CreateOnlineMarketplaceCharge(competentAuthority, 13631.00m, new DateTime(2025, 1, 1));
            var charge2026 = CreateOnlineMarketplaceCharge(competentAuthority, 14653.00m, new DateTime(2025, 10, 1));
            var charge2027 = CreateOnlineMarketplaceCharge(competentAuthority, 15500.00m, new DateTime(2026, 10, 1));

            A.CallTo(() => context.OnlineMarketplaceCharges)
                .Returns(dbContextHelper.GetAsyncEnabledDbSet(new List<OnlineMarketplaceCharge> { charge2025, charge2026, charge2027 }));

            // Act
            var result = await dataAccess.GetChargeAmountAsync(competentAuthority, asOfUtc);

            // Assert
            result.Should().NotBeNull();
            result.Amount.Should().Be(14653.00m);
        }

        [Fact]
        public async Task GetChargeAmountAsync_GivenChargesForDifferentCompetentAuthorities_ShouldOnlyReturnMatchingAuthority()
        {
            // Arrange
            var asOfUtc = new DateTime(2025, 6, 15);
            var englandCharge = CreateOnlineMarketplaceCharge(CompetentAuthorityType.England, 13631.00m, new DateTime(2025, 1, 1));
            var nonUkCharge = CreateOnlineMarketplaceCharge(CompetentAuthorityType.NonUK, 13631.00m, new DateTime(2025, 1, 1));

            A.CallTo(() => context.OnlineMarketplaceCharges)
                .Returns(dbContextHelper.GetAsyncEnabledDbSet(new List<OnlineMarketplaceCharge> { englandCharge, nonUkCharge }));

            // Act
            var result = await dataAccess.GetChargeAmountAsync(CompetentAuthorityType.England, asOfUtc);

            // Assert
            result.Should().NotBeNull();
            result.CompetentAuthority.Should().Be(CompetentAuthorityType.England);
        }

        [Fact]
        public async Task GetChargeAmountAsync_GivenNonUKCompetentAuthority_ShouldReturnNonUKCharge()
        {
            // Arrange
            var competentAuthority = CompetentAuthorityType.NonUK;
            var asOfUtc = new DateTime(2025, 6, 15);
            var charge = CreateOnlineMarketplaceCharge(competentAuthority, 13631.00m, new DateTime(2025, 1, 1));

            A.CallTo(() => context.OnlineMarketplaceCharges)
                .Returns(dbContextHelper.GetAsyncEnabledDbSet(new List<OnlineMarketplaceCharge> { charge }));

            // Act
            var result = await dataAccess.GetChargeAmountAsync(competentAuthority, asOfUtc);

            // Assert
            result.Should().NotBeNull();
            result.Amount.Should().Be(13631.00m);
            result.CompetentAuthority.Should().Be(CompetentAuthorityType.NonUK);
        }

        [Theory]
        [InlineData(CompetentAuthorityType.Wales)]
        [InlineData(CompetentAuthorityType.Scotland)]
        [InlineData(CompetentAuthorityType.NorthernIreland)]
        public async Task GetChargeAmountAsync_GivenNonApplicableCompetentAuthority_ShouldReturnNull(CompetentAuthorityType competentAuthority)
        {
            // Arrange - Only England and NonUK have OMP charges
            var asOfUtc = new DateTime(2025, 6, 15);
            var englandCharge = CreateOnlineMarketplaceCharge(CompetentAuthorityType.England, 13631.00m, new DateTime(2025, 1, 1));
            var nonUkCharge = CreateOnlineMarketplaceCharge(CompetentAuthorityType.NonUK, 13631.00m, new DateTime(2025, 1, 1));

            A.CallTo(() => context.OnlineMarketplaceCharges)
                .Returns(dbContextHelper.GetAsyncEnabledDbSet(new List<OnlineMarketplaceCharge> { englandCharge, nonUkCharge }));

            // Act
            var result = await dataAccess.GetChargeAmountAsync(competentAuthority, asOfUtc);

            // Assert
            result.Should().BeNull();
        }

        private static OnlineMarketplaceCharge CreateOnlineMarketplaceCharge(CompetentAuthorityType competentAuthority, decimal amount, DateTime effectiveFrom)
        {
            return new OnlineMarketplaceCharge(Guid.NewGuid(), competentAuthority, amount, effectiveFrom);
        }
    }
}