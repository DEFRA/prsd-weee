namespace EA.Weee.RequestHandlers.Tests.Unit.Scheme.MemberRegistration
{
    using EA.Weee.DataAccess;
    using EA.Weee.Domain;
    using EA.Weee.Domain.Lookup;
    using EA.Weee.RequestHandlers.Scheme.MemberRegistration;
    using FakeItEasy;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Weee.Tests.Core;
    using Xunit;

    public class AnnualChargeDataAccessTests
    {
        private readonly WeeeContext context;
        private readonly AnnualChargeDataAccess dataAccess;
        private readonly DbContextHelper helper = new DbContextHelper();

        public AnnualChargeDataAccessTests()
        {
            context = A.Fake<WeeeContext>();
            dataAccess = new AnnualChargeDataAccess(context);
        }

        [Fact]
        public async Task GetAnnualChargeForComplianceYear_ReturnsCorrectCharge_For2025()
        {
            // Arrange
            var competentAuthorityId = Guid.NewGuid();
            var annualCharges = new List<AnnualChargeByYear>
            {
                new AnnualChargeByYear(competentAuthorityId, 2025, 12500.00m, new DateTime(2025, 1, 1))
            };

            var dbSet = helper.GetAsyncEnabledDbSet(annualCharges);
            A.CallTo(() => context.AnnualChargesByYear).Returns(dbSet);

            // Act
            var result = await dataAccess.GetAnnualChargeForComplianceYear(competentAuthorityId, 2025);

            // Assert
            Assert.Equal(12500.00m, result);
        }

        [Fact]
        public async Task GetAnnualChargeForComplianceYear_ReturnsCorrectCharge_For2026()
        {
            // Arrange
            var competentAuthorityId = Guid.NewGuid();
            var annualCharges = new List<AnnualChargeByYear>
            {
                new AnnualChargeByYear(competentAuthorityId, 2026, 13438.00m, new DateTime(2026, 1, 1))
            };

            var dbSet = helper.GetAsyncEnabledDbSet(annualCharges);
            A.CallTo(() => context.AnnualChargesByYear).Returns(dbSet);

            // Act
            var result = await dataAccess.GetAnnualChargeForComplianceYear(competentAuthorityId, 2026);

            // Assert
            Assert.Equal(13438.00m, result);
        }

        [Fact]
        public async Task GetAnnualChargeForComplianceYear_ReturnsUpliftedCharge_For2027()
        {
            // Arrange - 3.8% inflation uplift from £13,438 to £13,948.13 effective from 1st April 2026
            var competentAuthorityId = Guid.NewGuid();
            var annualCharges = new List<AnnualChargeByYear>
            {
                new AnnualChargeByYear(competentAuthorityId, 2027, 13948.13m, new DateTime(2026, 4, 1))
            };

            var dbSet = helper.GetAsyncEnabledDbSet(annualCharges);
            A.CallTo(() => context.AnnualChargesByYear).Returns(dbSet);

            // Act
            var result = await dataAccess.GetAnnualChargeForComplianceYear(competentAuthorityId, 2027);

            // Assert
            Assert.Equal(13948.13m, result);
        }

        [Fact]
        public async Task GetAnnualChargeForComplianceYear_MultipleRecords_ReturnsLatestEffectiveFrom()
        {
            // Arrange
            var competentAuthorityId = Guid.NewGuid();
            var annualCharges = new List<AnnualChargeByYear>
            {
                new AnnualChargeByYear(competentAuthorityId, 2026, 13438.00m, new DateTime(2026, 1, 1)),
                new AnnualChargeByYear(competentAuthorityId, 2026, 14000.00m, new DateTime(2026, 6, 1)) // Mid-year adjustment
            };

            var dbSet = helper.GetAsyncEnabledDbSet(annualCharges);
            A.CallTo(() => context.AnnualChargesByYear).Returns(dbSet);

            // Act
            var result = await dataAccess.GetAnnualChargeForComplianceYear(competentAuthorityId, 2026);

            // Assert
            Assert.Equal(14000.00m, result); // Should return the latest one
        }

        [Fact]
        public async Task GetAnnualChargeForComplianceYear_NoRecordFound_ReturnsNull()
        {
            // Arrange
            var competentAuthorityId = Guid.NewGuid();
            var annualCharges = new List<AnnualChargeByYear>
            {
                new AnnualChargeByYear(competentAuthorityId, 2025, 12500.00m, new DateTime(2025, 1, 1))
            };

            var dbSet = helper.GetAsyncEnabledDbSet(annualCharges);
            A.CallTo(() => context.AnnualChargesByYear).Returns(dbSet);

            // Act - Request year that doesn't exist
            var result = await dataAccess.GetAnnualChargeForComplianceYear(competentAuthorityId, 2027);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetAnnualChargeForComplianceYear_WrongCompetentAuthorityId_ReturnsNull()
        {
            // Arrange
            var competentAuthorityId = Guid.NewGuid();
            var wrongId = Guid.NewGuid();
            var annualCharges = new List<AnnualChargeByYear>
            {
                new AnnualChargeByYear(wrongId, 2026, 13438.00m, new DateTime(2026, 1, 1))
            };

            var dbSet = helper.GetAsyncEnabledDbSet(annualCharges);
            A.CallTo(() => context.AnnualChargesByYear).Returns(dbSet);

            // Act
            var result = await dataAccess.GetAnnualChargeForComplianceYear(competentAuthorityId, 2026);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetAnnualChargeForComplianceYear_LegacyFallback_ReturnsNullWhenNoCompetentAuthorityFound()
        {
            // Arrange
            var competentAuthorityId = Guid.NewGuid();
            var emptyAnnualCharges = new List<AnnualChargeByYear>();
            var emptyCompetentAuthorities = new List<UKCompetentAuthority>();

            var dbSet = helper.GetAsyncEnabledDbSet(emptyAnnualCharges);
            A.CallTo(() => context.AnnualChargesByYear).Returns(dbSet);

            var cadbSet = helper.GetAsyncEnabledDbSet(emptyCompetentAuthorities);
            A.CallTo(() => context.UKCompetentAuthorities).Returns(cadbSet);

            // Act
            var result = await dataAccess.GetAnnualChargeForComplianceYear(competentAuthorityId, 2025);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetAnnualChargeForComplianceYear_ReturnsZero_ForNonEAAuthorities()
        {
            // Arrange
            var competentAuthorityId = Guid.NewGuid();
            var annualCharges = new List<AnnualChargeByYear>
            {
                new AnnualChargeByYear(competentAuthorityId, 2026, 0.00m, new DateTime(2026, 1, 1))
            };

            var dbSet = helper.GetAsyncEnabledDbSet(annualCharges);
            A.CallTo(() => context.AnnualChargesByYear).Returns(dbSet);

            // Act
            var result = await dataAccess.GetAnnualChargeForComplianceYear(competentAuthorityId, 2026);

            // Assert
            Assert.Equal(0.00m, result);
        }

        [Fact]
        public async Task GetAnnualChargeForComplianceYear_AllComplianceYears_ReturnsCorrectValues()
        {
            // Arrange
            var competentAuthorityId = Guid.NewGuid();
            var annualCharges = new List<AnnualChargeByYear>
            {
                new AnnualChargeByYear(competentAuthorityId, 2019, 12500.00m, new DateTime(2019, 1, 1)),
                new AnnualChargeByYear(competentAuthorityId, 2020, 12500.00m, new DateTime(2020, 1, 1)),
                new AnnualChargeByYear(competentAuthorityId, 2021, 12500.00m, new DateTime(2021, 1, 1)),
                new AnnualChargeByYear(competentAuthorityId, 2022, 12500.00m, new DateTime(2022, 1, 1)),
                new AnnualChargeByYear(competentAuthorityId, 2023, 12500.00m, new DateTime(2023, 1, 1)),
                new AnnualChargeByYear(competentAuthorityId, 2024, 12500.00m, new DateTime(2024, 1, 1)),
                new AnnualChargeByYear(competentAuthorityId, 2025, 12500.00m, new DateTime(2025, 1, 1)),
                new AnnualChargeByYear(competentAuthorityId, 2026, 13438.00m, new DateTime(2026, 1, 1)),
                new AnnualChargeByYear(competentAuthorityId, 2027, 13948.13m, new DateTime(2026, 4, 1))
            };

            var dbSet = helper.GetAsyncEnabledDbSet(annualCharges);
            A.CallTo(() => context.AnnualChargesByYear).Returns(dbSet);

            // Act & Assert
            for (int year = 2019; year <= 2025; year++)
            {
                var result = await dataAccess.GetAnnualChargeForComplianceYear(competentAuthorityId, year);
                Assert.Equal(12500.00m, result);
            }

            var result2026 = await dataAccess.GetAnnualChargeForComplianceYear(competentAuthorityId, 2026);
            Assert.Equal(13438.00m, result2026);

            var result2027 = await dataAccess.GetAnnualChargeForComplianceYear(competentAuthorityId, 2027);
            Assert.Equal(13948.13m, result2027);
        }
    }
}