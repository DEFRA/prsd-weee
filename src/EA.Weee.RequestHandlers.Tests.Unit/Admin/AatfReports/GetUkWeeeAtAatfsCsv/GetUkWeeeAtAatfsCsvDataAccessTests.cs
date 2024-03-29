﻿namespace EA.Weee.RequestHandlers.Tests.Unit.Admin.AatfReports.GetUkWeeeAtAatfsCsv
{
    using AutoFixture;
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.DataAccess;
    using EA.Weee.Domain.AatfReturn;
    using EA.Weee.Domain.DataReturns;
    using EA.Weee.Domain.Lookup;
    using EA.Weee.Domain.Organisation;
    using EA.Weee.RequestHandlers.Admin.AatfReports.GetUkWeeeAtAatfsCsv;
    using EA.Weee.Tests.Core;
    using FakeItEasy;
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Threading.Tasks;
    using Xunit;
    using DomainFacilityType = EA.Weee.Domain.AatfReturn.FacilityType;

    public class GetUkWeeeAtAatfsCsvDataAccessTests
    {
        private readonly DbContextHelper helper = new DbContextHelper();
        private readonly Fixture fixture;
        private readonly WeeeContext context;
        private readonly GetUkWeeeAtAatfsCsvDataAccess dataAccess;

        public GetUkWeeeAtAatfsCsvDataAccessTests()
        {
            fixture = new Fixture();
            context = A.Fake<WeeeContext>();
            dataAccess = new GetUkWeeeAtAatfsCsvDataAccess(context);
        }

        [Fact]
        public async Task FetchPartialAatfReturnsForComplianceYearAsync_ReturnsReceivedReusedAndSentOn()
        {
            // Arrange
            var year = 1900 + fixture.Create<int>();
            var idQ1 = fixture.Create<Guid>();

            var returns = new List<Return>
            {
                CreateReturn(idQ1, year, QuarterType.Q1)
            };
            var returnsDbSet = helper.GetAsyncEnabledDbSet(returns);
            A.CallTo(() => context.Returns).Returns(returnsDbSet);

            var received = new List<WeeeReceived>
            {
                CreateWeeeReceived(idQ1, out var categoryReceived, out var receivedB2bQ1, out var receivedB2cQ1)
            };
            var receivedDbSet = helper.GetAsyncEnabledDbSet(received);
            A.CallTo(() => context.WeeeReceived).Returns(receivedDbSet);

            var reused = new List<WeeeReused>
            {
                CreateWeeeReused(idQ1, out var categoryReused, out var reusedB2bQ1, out var reusedB2cQ1)
            };
            var reusedDbSet = helper.GetAsyncEnabledDbSet(reused);
            A.CallTo(() => context.WeeeReused).Returns(reusedDbSet);

            var sentOn = new List<WeeeSentOn>
            {
                CreateWeeeSentOn(idQ1, out var categorySentOn, out var sentOnB2bQ1, out var sentOnB2cQ1)
            };
            var sentOnDbSet = helper.GetAsyncEnabledDbSet(sentOn);
            A.CallTo(() => context.WeeeSentOn).Returns(sentOnDbSet);

            // Act
            var results = await dataAccess.FetchPartialAatfReturnsForComplianceYearAsync(year);

            // Assert
            Assert.Single(results);

            var result = results.First();
            Assert.Equal(year, result.Quarter.Year);
            Assert.Single(result.ObligatedWeeeReceivedData);
            AssertWeeeObligatedData(result.ObligatedWeeeReceivedData.First(), categoryReceived, receivedB2bQ1, receivedB2cQ1);
            Assert.Single(result.ObligatedWeeeReusedData);
            AssertWeeeObligatedData(result.ObligatedWeeeReusedData.First(), categoryReused, reusedB2bQ1, reusedB2cQ1);
            Assert.Single(result.ObligatedWeeeSentOnData);
            AssertWeeeObligatedData(result.ObligatedWeeeSentOnData.First(), categorySentOn, sentOnB2bQ1, sentOnB2cQ1);
        }

        [Fact]
        public async Task FetchPartialAatfReturnsForComplianceYearAsync_ReturnsReceivedReusedAndSentOnFiltersOutAeData()
        {
            // Arrange
            var year = 1900 + fixture.Create<int>();
            var idQ1 = fixture.Create<Guid>();

            var returns = new List<Return>
            {
                CreateReturn(idQ1, year, QuarterType.Q1),
                CreateReturn(idQ1, year, QuarterType.Q1, facilityType: DomainFacilityType.Ae)
            };
            var returnsDbSet = helper.GetAsyncEnabledDbSet(returns);
            A.CallTo(() => context.Returns).Returns(returnsDbSet);

            var received = new List<WeeeReceived>
            {
                CreateWeeeReceived(idQ1, out var categoryReceived, out var receivedB2bQ1, out var receivedB2cQ1)
            };
            var receivedDbSet = helper.GetAsyncEnabledDbSet(received);
            A.CallTo(() => context.WeeeReceived).Returns(receivedDbSet);

            var reused = new List<WeeeReused>
            {
                CreateWeeeReused(idQ1, out var categoryReused, out var reusedB2bQ1, out var reusedB2cQ1)
            };
            var reusedDbSet = helper.GetAsyncEnabledDbSet(reused);
            A.CallTo(() => context.WeeeReused).Returns(reusedDbSet);

            var sentOn = new List<WeeeSentOn>
            {
                CreateWeeeSentOn(idQ1, out var categorySentOn, out var sentOnB2bQ1, out var sentOnB2cQ1)
            };
            var sentOnDbSet = helper.GetAsyncEnabledDbSet(sentOn);
            A.CallTo(() => context.WeeeSentOn).Returns(sentOnDbSet);

            // Act
            var results = await dataAccess.FetchPartialAatfReturnsForComplianceYearAsync(year);

            // Assert
            Assert.Single(results);

            var result = results.First();
            Assert.Equal(year, result.Quarter.Year);
            Assert.Single(result.ObligatedWeeeReceivedData);
            AssertWeeeObligatedData(result.ObligatedWeeeReceivedData.First(), categoryReceived, receivedB2bQ1, receivedB2cQ1);
            Assert.Single(result.ObligatedWeeeReusedData);
            AssertWeeeObligatedData(result.ObligatedWeeeReusedData.First(), categoryReused, reusedB2bQ1, reusedB2cQ1);
            Assert.Single(result.ObligatedWeeeSentOnData);
            AssertWeeeObligatedData(result.ObligatedWeeeSentOnData.First(), categorySentOn, sentOnB2bQ1, sentOnB2cQ1);
        }

        [Fact]
        public async Task FetchPartialAatfReturnsForComplianceYearAsync_MultipleReturnsForMultipleQuarters_ReturnsMultiplePartialReturns()
        {
            // Arrange
            var year = 1900 + fixture.Create<int>();
            var idQ1 = fixture.Create<Guid>();
            var idQ2 = fixture.Create<Guid>();

            var returns = new List<Return>
            {
                CreateReturn(idQ1, year, QuarterType.Q1),
                CreateReturn(idQ2, year, QuarterType.Q2)
            };
            var returnsDbSet = helper.GetAsyncEnabledDbSet(returns);
            A.CallTo(() => context.Returns).Returns(returnsDbSet);

            var received = new List<WeeeReceived>
            {
                CreateWeeeReceived(idQ1, out var categoryReceived, out var receivedB2bQ1, out var receivedB2cQ1),
                CreateWeeeReceived(idQ2, categoryReceived, out var receivedB2bQ2, out var receivedB2cQ2)
            };
            var receivedDbSet = helper.GetAsyncEnabledDbSet(received);
            A.CallTo(() => context.WeeeReceived).Returns(receivedDbSet);

            var reused = new List<WeeeReused>
            {
                CreateWeeeReused(idQ1, out var categoryReused, out var reusedB2bQ1, out var reusedB2cQ1),
                CreateWeeeReused(idQ2, categoryReused, out var reusedB2bQ2, out var reusedB2cQ2)
            };
            var reusedDbSet = helper.GetAsyncEnabledDbSet(reused);
            A.CallTo(() => context.WeeeReused).Returns(reusedDbSet);

            var sentOn = new List<WeeeSentOn>
            {
                CreateWeeeSentOn(idQ1, out var categorySentOn, out var sentOnB2bQ1, out var sentOnB2cQ1),
                CreateWeeeSentOn(idQ2, categorySentOn, out var sentOnB2bQ2, out var sentOnB2cQ2)
            };
            var sentOnDbSet = helper.GetAsyncEnabledDbSet(sentOn);
            A.CallTo(() => context.WeeeSentOn).Returns(sentOnDbSet);

            // Act
            var results = await dataAccess.FetchPartialAatfReturnsForComplianceYearAsync(year);

            // Assert
            Assert.Equal(2, results.Count());

            var result = results.First();
            Assert.Equal(year, result.Quarter.Year);
            Assert.Single(result.ObligatedWeeeReceivedData);
            AssertWeeeObligatedData(result.ObligatedWeeeReceivedData.First(), categoryReceived, receivedB2bQ1, receivedB2cQ1);
            Assert.Single(result.ObligatedWeeeReusedData);
            AssertWeeeObligatedData(result.ObligatedWeeeReusedData.First(), categoryReused, reusedB2bQ1, reusedB2cQ1);
            Assert.Single(result.ObligatedWeeeSentOnData);
            AssertWeeeObligatedData(result.ObligatedWeeeSentOnData.First(), categorySentOn, sentOnB2bQ1, sentOnB2cQ1);

            result = results.Skip(1).First();
            Assert.Single(result.ObligatedWeeeReceivedData);
            AssertWeeeObligatedData(result.ObligatedWeeeReceivedData.First(), categoryReceived, receivedB2bQ2, receivedB2cQ2);
            Assert.Single(result.ObligatedWeeeReusedData);
            AssertWeeeObligatedData(result.ObligatedWeeeReusedData.First(), categoryReused, reusedB2bQ2, reusedB2cQ2);
            Assert.Single(result.ObligatedWeeeSentOnData);
            AssertWeeeObligatedData(result.ObligatedWeeeSentOnData.First(), categorySentOn, sentOnB2bQ2, sentOnB2cQ2);
        }

        [Fact]
        public async Task FetchPartialAatfReturnsForComplianceYearAsync_MultipleReturnsForSameQuarter_ReturnsMultiplePartialReturns()
        {
            // Arrange
            var year = 1900 + fixture.Create<int>();
            var id1 = fixture.Create<Guid>();
            var id2 = fixture.Create<Guid>();

            var returns = new List<Return>
            {
                CreateReturn(id1, year, QuarterType.Q1),
                CreateReturn(id2, year, QuarterType.Q1)
            };
            var returnsDbSet = helper.GetAsyncEnabledDbSet(returns);
            A.CallTo(() => context.Returns).Returns(returnsDbSet);

            var received = new List<WeeeReceived>
            {
                CreateWeeeReceived(id1, out var categoryReceived, out var receivedB2b1, out var receivedB2c1),
                CreateWeeeReceived(id2, categoryReceived, out var receivedB2b2, out var receivedB2c2)
            };
            var receivedDbSet = helper.GetAsyncEnabledDbSet(received);
            A.CallTo(() => context.WeeeReceived).Returns(receivedDbSet);

            var reused = new List<WeeeReused>
            {
                CreateWeeeReused(id1, out var categoryReused, out var reusedB2b1, out var reusedB2c1),
                CreateWeeeReused(id2, categoryReused, out var reusedB2b2, out var reusedB2c2)
            };
            var reusedDbSet = helper.GetAsyncEnabledDbSet(reused);
            A.CallTo(() => context.WeeeReused).Returns(reusedDbSet);

            var sentOn = new List<WeeeSentOn>
            {
                CreateWeeeSentOn(id1, out var categorySentOn, out var sentOnB2b1, out var sentOnB2c1),
                CreateWeeeSentOn(id2, categorySentOn, out var sentOnB2b2, out var sentOnB2c2)
            };
            var sentOnDbSet = helper.GetAsyncEnabledDbSet(sentOn);
            A.CallTo(() => context.WeeeSentOn).Returns(sentOnDbSet);

            // Act
            var results = await dataAccess.FetchPartialAatfReturnsForComplianceYearAsync(year);

            // Assert
            Assert.Equal(2, results.Count());

            var result = results.First();
            Assert.Equal(year, result.Quarter.Year);
            Assert.Single(result.ObligatedWeeeReceivedData);
            AssertWeeeObligatedData(result.ObligatedWeeeReceivedData.First(), categoryReceived, receivedB2b1, receivedB2c1);
            Assert.Single(result.ObligatedWeeeReusedData);
            AssertWeeeObligatedData(result.ObligatedWeeeReusedData.First(), categoryReused, reusedB2b1, reusedB2c1);
            Assert.Single(result.ObligatedWeeeSentOnData);
            AssertWeeeObligatedData(result.ObligatedWeeeSentOnData.First(), categorySentOn, sentOnB2b1, sentOnB2c1);

            result = results.Skip(1).First();
            Assert.Single(result.ObligatedWeeeReceivedData);
            AssertWeeeObligatedData(result.ObligatedWeeeReceivedData.First(), categoryReceived, receivedB2b2, receivedB2c2);
            Assert.Single(result.ObligatedWeeeReusedData);
            AssertWeeeObligatedData(result.ObligatedWeeeReusedData.First(), categoryReused, reusedB2b2, reusedB2c2);
            Assert.Single(result.ObligatedWeeeSentOnData);
            AssertWeeeObligatedData(result.ObligatedWeeeSentOnData.First(), categorySentOn, sentOnB2b2, sentOnB2c2);
        }

        [Theory]
        [InlineData(QuarterType.Q1)]
        [InlineData(QuarterType.Q2)]
        [InlineData(QuarterType.Q3)]
        [InlineData(QuarterType.Q4)]
        public async Task FetchPartialAatfReturnsForComplianceYearAsync_DoesNotIncludePreviousVersions(QuarterType quarter)
        {
            // Arrange
            var year = 1900 + fixture.Create<int>();
            var previousId = fixture.Create<Guid>();
            var id = fixture.Create<Guid>();
            var organisationId = fixture.Create<Guid>();
            var organisation = A.Fake<Organisation>();
            A.CallTo(() => organisation.Id).Returns(organisationId);

            var returns = new List<Return>
            {
                CreateReturn(previousId, year, quarter, organisation, new DateTime(year, 01, 01)),
                CreateReturn(id, year, quarter, organisation, new DateTime(year, 01, 02))
            };
            var returnsDbSet = helper.GetAsyncEnabledDbSet(returns);
            A.CallTo(() => context.Returns).Returns(returnsDbSet);

            var received = new List<WeeeReceived>
            {
                CreateWeeeReceived(previousId, out var categoryReceivedPrevious, out var receivedB2bQ1Previous, out var receivedB2cQ1Previous),
                CreateWeeeReceived(id, out var categoryReceived, out var receivedB2bQ1, out var receivedB2cQ1)
            };
            var receivedDbSet = helper.GetAsyncEnabledDbSet(received);
            A.CallTo(() => context.WeeeReceived).Returns(receivedDbSet);

            var reused = new List<WeeeReused>
            {
                CreateWeeeReused(previousId, out var categoryReusedPrevious, out var reusedB2bQ1Previous, out var reusedB2cQ1Previous),
                CreateWeeeReused(id, out var categoryReused, out var reusedB2bQ1, out var reusedB2cQ1)
            };
            var reusedDbSet = helper.GetAsyncEnabledDbSet(reused);
            A.CallTo(() => context.WeeeReused).Returns(reusedDbSet);

            var sentOn = new List<WeeeSentOn>
            {
                CreateWeeeSentOn(previousId, out var categorySentOnPrevious, out var sentOnB2bQ1Previous, out var sentOnB2cQ1Previous),
                CreateWeeeSentOn(id, out var categorySentOn, out var sentOnB2bQ1, out var sentOnB2cQ1)
            };
            var sentOnDbSet = helper.GetAsyncEnabledDbSet(sentOn);
            A.CallTo(() => context.WeeeSentOn).Returns(sentOnDbSet);

            // Act
            var results = await dataAccess.FetchPartialAatfReturnsForComplianceYearAsync(year);

            // Assert
            Assert.Single(results);

            var result = results.First();
            Assert.Equal(year, result.Quarter.Year);
            Assert.Single(result.ObligatedWeeeReceivedData);
            AssertWeeeObligatedData(result.ObligatedWeeeReceivedData.First(), categoryReceived, receivedB2bQ1, receivedB2cQ1);
            Assert.Single(result.ObligatedWeeeReusedData);
            AssertWeeeObligatedData(result.ObligatedWeeeReusedData.First(), categoryReused, reusedB2bQ1, reusedB2cQ1);
            Assert.Single(result.ObligatedWeeeSentOnData);
            AssertWeeeObligatedData(result.ObligatedWeeeSentOnData.First(), categorySentOn, sentOnB2bQ1, sentOnB2cQ1);
        }

        [Fact]
        public async Task FetchPartialAatfReturnsForComplianceYearAsync_NoReturns()
        {
            // Arrange
            var year = 1900 + fixture.Create<int>();

            var returns = new List<Return>();
            var returnsDbSet = helper.GetAsyncEnabledDbSet(returns);
            A.CallTo(() => context.Returns).Returns(returnsDbSet);

            // Act
            var results = await dataAccess.FetchPartialAatfReturnsForComplianceYearAsync(year);

            // Assert
            Assert.False(results.Any());
        }

         private Return CreateReturn(Guid id, int year, QuarterType quarter, Organisation organisation = null, DateTime? submittedDate = null, DomainFacilityType facilityType = null)
        {
            var @return = A.Fake<Return>();
            A.CallTo(() => @return.Id).Returns(id);
            A.CallTo(() => @return.Quarter).Returns(new Quarter(year, quarter));
            A.CallTo(() => @return.SubmittedDate).Returns(submittedDate ?? fixture.Create<DateTime>());

            if (organisation == null)
            {
                organisation = A.Fake<Organisation>();
                A.CallTo(() => organisation.Id).Returns(fixture.Create<Guid>());
            }

            A.CallTo(() => @return.Organisation).Returns(organisation);

            if (facilityType == null)
            {
                facilityType = DomainFacilityType.Aatf;
            }
            A.CallTo(() => @return.FacilityType).Returns(facilityType);

            return @return;
        }

        private WeeeReceived CreateWeeeReceived(Guid id, out int category, out decimal? b2b, out decimal? b2c)
        {
            category = (int)fixture.Create<WeeeCategory>();
            return CreateWeeeReceived(id, category, out b2b, out b2c);
        }

        private WeeeReceived CreateWeeeReceived(Guid id, int category, out decimal? b2b, out decimal? b2c)
        {
            b2b = fixture.Create<decimal>();
            b2c = fixture.Create<decimal>();
            var amount = new WeeeReceivedAmount(new WeeeReceived(), category, b2c, b2b);
            var amounts = new List<WeeeReceivedAmount> { amount };
            return new WeeeReceived(new Guid(), new Guid(), id) { WeeeReceivedAmounts = amounts };
        }

        private WeeeReused CreateWeeeReused(Guid id, out int category, out decimal? b2b, out decimal? b2c)
        {
            category = (int)fixture.Create<WeeeCategory>();
            return CreateWeeeReused(id, category, out b2b, out b2c);
        }

        private WeeeReused CreateWeeeReused(Guid id, int category, out decimal? b2b, out decimal? b2c)
        {
            b2b = fixture.Create<decimal>();
            b2c = fixture.Create<decimal>();
            var amount = new WeeeReusedAmount(new WeeeReused(), category, b2c, b2b);
            var amounts = new List<WeeeReusedAmount> { amount };
            return new WeeeReused(new Guid(), id) { WeeeReusedAmounts = amounts };
        }

        private WeeeSentOn CreateWeeeSentOn(Guid id, out int category, out decimal? b2b, out decimal? b2c)
        {
            category = (int)fixture.Create<WeeeCategory>();
            return CreateWeeeSentOn(id, category, out b2b, out b2c);
        }

        private WeeeSentOn CreateWeeeSentOn(Guid id, int category, out decimal? b2b, out decimal? b2c)
        {
            b2b = fixture.Create<decimal>();
            b2c = fixture.Create<decimal>();
            var amount = new WeeeSentOnAmount(new WeeeSentOn(), category, b2c, b2b);
            var amounts = new List<WeeeSentOnAmount> { amount };
            return new WeeeSentOn(new Guid(), new Guid(), id) { WeeeSentOnAmounts = amounts };
        }

        private void AssertWeeeObligatedData(WeeeObligatedData data, int expectedCategory, decimal? expectedB2b, decimal? expectedB2c)
        {
            Assert.Equal(expectedCategory, data.CategoryId);
            Assert.Equal(expectedB2b, data.B2B);
            Assert.Equal(expectedB2c, data.B2C);
        }
    }
}
