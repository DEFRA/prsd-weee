namespace EA.Weee.RequestHandlers.Tests.Unit.Admin.Reports.GetUKWeeeCsv
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Threading.Tasks;
    using AutoFixture;
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.DataAccess;
    using EA.Weee.Domain.AatfReturn;
    using EA.Weee.Domain.DataReturns;
    using EA.Weee.Domain.Lookup;
    using EA.Weee.RequestHandlers.Admin.Reports.GetUkWeeeAtAatfsCsv;
    using EA.Weee.Tests.Core;
    using FakeItEasy;
    using Xunit;

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
            Assert.Equal(1, results.Count());

            var result = results.First();
            Assert.Equal(year, result.Quarter.Year);
            Assert.Equal(1, result.ObligatedWeeeReceivedData.Count());
            AssertWeeeObligatedData(result.ObligatedWeeeReceivedData.First(), categoryReceived, receivedB2bQ1, receivedB2cQ1);
            Assert.Equal(1, result.ObligatedWeeeReusedData.Count());
            AssertWeeeObligatedData(result.ObligatedWeeeReusedData.First(), categoryReused, reusedB2bQ1, reusedB2cQ1);
            Assert.Equal(1, result.ObligatedWeeeSentOnData.Count());
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
            Assert.Equal(1, result.ObligatedWeeeReceivedData.Count());
            AssertWeeeObligatedData(result.ObligatedWeeeReceivedData.First(), categoryReceived, receivedB2bQ1, receivedB2cQ1);
            Assert.Equal(1, result.ObligatedWeeeReusedData.Count());
            AssertWeeeObligatedData(result.ObligatedWeeeReusedData.First(), categoryReused, reusedB2bQ1, reusedB2cQ1);
            Assert.Equal(1, result.ObligatedWeeeSentOnData.Count());
            AssertWeeeObligatedData(result.ObligatedWeeeSentOnData.First(), categorySentOn, sentOnB2bQ1, sentOnB2cQ1);

            result = results.Skip(1).First();
            Assert.Equal(1, result.ObligatedWeeeReceivedData.Count());
            AssertWeeeObligatedData(result.ObligatedWeeeReceivedData.First(), categoryReceived, receivedB2bQ2, receivedB2cQ2);
            Assert.Equal(1, result.ObligatedWeeeReusedData.Count());
            AssertWeeeObligatedData(result.ObligatedWeeeReusedData.First(), categoryReused, reusedB2bQ2, reusedB2cQ2);
            Assert.Equal(1, result.ObligatedWeeeSentOnData.Count());
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
            Assert.Equal(1, result.ObligatedWeeeReceivedData.Count());
            AssertWeeeObligatedData(result.ObligatedWeeeReceivedData.First(), categoryReceived, receivedB2b1, receivedB2c1);
            Assert.Equal(1, result.ObligatedWeeeReusedData.Count());
            AssertWeeeObligatedData(result.ObligatedWeeeReusedData.First(), categoryReused, reusedB2b1, reusedB2c1);
            Assert.Equal(1, result.ObligatedWeeeSentOnData.Count());
            AssertWeeeObligatedData(result.ObligatedWeeeSentOnData.First(), categorySentOn, sentOnB2b1, sentOnB2c1);

            result = results.Skip(1).First();
            Assert.Equal(1, result.ObligatedWeeeReceivedData.Count());
            AssertWeeeObligatedData(result.ObligatedWeeeReceivedData.First(), categoryReceived, receivedB2b2, receivedB2c2);
            Assert.Equal(1, result.ObligatedWeeeReusedData.Count());
            AssertWeeeObligatedData(result.ObligatedWeeeReusedData.First(), categoryReused, reusedB2b2, reusedB2c2);
            Assert.Equal(1, result.ObligatedWeeeSentOnData.Count());
            AssertWeeeObligatedData(result.ObligatedWeeeSentOnData.First(), categorySentOn, sentOnB2b2, sentOnB2c2);
        }

        [Fact]
        public async Task FetchPartialAatfReturnsForComplianceYearAsync_DoesNotIncludePreviousVersions()
        {
            // Arrange
            var year = 1900 + fixture.Create<int>();
            var previousId = fixture.Create<Guid>();
            var id = fixture.Create<Guid>();

            var returns = new List<Return>
            {
                CreateReturn(previousId, year, QuarterType.Q1),
                CreateReturn(id, year, QuarterType.Q1, previousId)
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
            Assert.Equal(1, results.Count());

            var result = results.First();
            Assert.Equal(year, result.Quarter.Year);
            Assert.Equal(1, result.ObligatedWeeeReceivedData.Count());
            AssertWeeeObligatedData(result.ObligatedWeeeReceivedData.First(), categoryReceived, receivedB2bQ1, receivedB2cQ1);
            Assert.Equal(1, result.ObligatedWeeeReusedData.Count());
            AssertWeeeObligatedData(result.ObligatedWeeeReusedData.First(), categoryReused, reusedB2bQ1, reusedB2cQ1);
            Assert.Equal(1, result.ObligatedWeeeSentOnData.Count());
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

        private Return CreateReturn(Guid id, int year, QuarterType quarter, Guid? previousId = null)
        {
            var @return = A.Fake<Return>();
            A.CallTo(() => @return.Id).Returns(id);
            A.CallTo(() => @return.Quarter).Returns(new Quarter(year, quarter));
            A.CallTo(() => @return.ParentId).Returns(null);
            A.CallTo(() => @return.SubmittedDate).Returns(fixture.Create<DateTime>());
            A.CallTo(() => @return.ParentId).Returns(previousId);

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
