namespace EA.Weee.Web.Tests.Unit.Areas.Admin.Mapping
{
    using System;
    using System.Collections.Generic;
    using AutoFixture;
    using Core.Admin.AatfReports;
    using FakeItEasy;
    using FluentAssertions;
    using Web.Areas.Admin.Mappings.ToViewModel;
    using Web.ViewModels.Returns.Mappings.ToViewModel;
    using Xunit;

    public class AatfSubmissionHistoryDataToViewModelMapTests
    {
        private readonly AatfSubmissionHistoryDataToViewModelMap mapper;
        private readonly ITonnageUtilities tonnageUtilities;
        private readonly Fixture fixture;

        public AatfSubmissionHistoryDataToViewModelMapTests()
        {
            fixture = new Fixture();
            tonnageUtilities = A.Fake<ITonnageUtilities>();

            mapper = new AatfSubmissionHistoryDataToViewModelMap(tonnageUtilities);
        }

        [Fact]
        public void AatfSubmissionHistoryDataToViewModelMap_GivenNullSource_ArgumentNullExceptionExpected()
        {
            var assert = Xunit.Record.Exception(() => mapper.Map(null));

            assert.Should().BeOfType<ArgumentNullException>();
        }

        [Fact]
        public void Map_GivenSource_DefaultPropertiesShouldBeMapped()
        {
            var source = fixture.Create<AatfSubmissionHistoryData>();

            var result = mapper.Map(source);

            result.ReturnId = source.ReturnId;
            result.ComplianceYear = source.ComplianceYear;
            result.Quarter = source.Quarter;
            result.SubmittedDate = source.SubmittedDate.ToString("dd/MM/yyyy HH:mm:ss");
            result.SubmittedBy = source.SubmittedBy;
        }

        [Fact]
        public void Map_GivenWeeeReceivedHouseHoldTotal_TotalHouseHoldShouldBeCalculated()
        {
            var source = fixture.Create<AatfSubmissionHistoryData>();

            mapper.Map(source);

            A.CallTo(() => tonnageUtilities.SumTotals(A<List<decimal?>>.That.IsSameSequenceAs(new List<decimal?>() {source.WeeeReceivedHouseHold})))
                .MustHaveHappenedOnceExactly();
        }

        [Fact]
        public void Map_GivenWeeeReceivedHouseHoldTotal_TotalHouseHoldShouldBeReturned()
        {
            var source = fixture.Create<AatfSubmissionHistoryData>();

            A.CallTo(() => tonnageUtilities.SumTotals(A<List<decimal?>>.That.IsSameSequenceAs(new List<decimal?>() { source.WeeeReceivedHouseHold }))).Returns("10");

            var result = mapper.Map(source);

            result.ObligatedHouseHoldTotal.Should().Be("10");
        }

        [Fact]
        public void Map_GivenWeeeReceivedNonHouseHoldTotal_TotalNonHouseHoldShouldBeCalculated()
        {
            var source = fixture.Create<AatfSubmissionHistoryData>();

            mapper.Map(source);

            A.CallTo(() => tonnageUtilities.SumTotals(A<List<decimal?>>.That.IsSameSequenceAs(new List<decimal?>() { source.WeeeReceivedNonHouseHold })))
                .MustHaveHappenedOnceExactly();
        }

        [Fact]
        public void Map_GivenWeeeReceivedNonHouseHoldTotal_TotalNonHouseHoldShouldBeReturned()
        {
            var source = fixture.Create<AatfSubmissionHistoryData>();

            A.CallTo(() => tonnageUtilities.SumTotals(A<List<decimal?>>.That.IsSameSequenceAs(new List<decimal?>() { source.WeeeReceivedNonHouseHold }))).Returns("10");

            var result = mapper.Map(source);

            result.ObligatedNonHouseHoldTotal.Should().Be("10");
        }
    }
}
