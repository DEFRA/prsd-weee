namespace EA.Weee.Web.Tests.Unit.Areas.AatfReturn.Mapping.ToViewModel
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using AutoFixture;

    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Core.DataReturns;
    using EA.Weee.Web.Areas.AatfReturn.Mappings.ToViewModel;

    using FakeItEasy;

    using FluentAssertions;

    using Web.Areas.AatfReturn.ViewModels;

    using Weee.Tests.Core;

    using Xunit;

    public class ReportOptionsToSelectReportOptionsViewModelMapTests
    {
        private readonly ReportOptionsToSelectReportOptionsViewModelMap map;
        private const string DcfNoValue = "No";
        private readonly Fixture fixture;

        public ReportOptionsToSelectReportOptionsViewModelMapTests()
        {
            this.fixture = new Fixture();

            this.map = new ReportOptionsToSelectReportOptionsViewModelMap();
        }

        [Fact]
        public void Map_GivenNullSource_ArgumentNullExceptionExpected()
        {
            Action act = () => this.map.Map(null);

            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void Map_GivenNullReportOnQuestionsList_ArgumentNullExceptionExpected()
        {
            Action act = () => this.map.Map(new ReportOptionsToSelectReportOptionsViewModelMapTransfer() { OrganisationId = Guid.NewGuid(), ReturnId = Guid.NewGuid(), ReturnData = A.Fake<ReturnData>() });

            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void Map_GivenNullReturnData_ArgumentNullExceptionExpected()
        {
            Action act = () => this.map.Map(new ReportOptionsToSelectReportOptionsViewModelMapTransfer() { OrganisationId = Guid.NewGuid(), ReturnId = Guid.NewGuid(), ReportOnQuestions = A.Fake<List<ReportOnQuestion>>() });

            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void Map_GivenMappingObjects_MappedObjectShouldContainShouldContainQuestions()
        {
            var questions = new List<ReportOnQuestion>();

            var @return = A.Fake<ReturnData>();
            @return.Quarter = new Quarter(2019, QuarterType.Q1);
            @return.QuarterWindow = QuarterWindowTestHelper.GetDefaultQuarterWindow();

            for (var i = 0; i < 5; i++)
            {
                questions.Add(this.fixture.Build<ReportOnQuestion>().With(r => r.Id, i + 1).With(r => r.Selected, false).Create());
            }

            var result = this.map.Map(new ReportOptionsToSelectReportOptionsViewModelMapTransfer() { OrganisationId = Guid.NewGuid(), ReturnId = Guid.NewGuid(), ReportOnQuestions = questions, ReturnData = @return });

            this.AssertValues(result, false);
        }

        [Fact]
        public void Map_GivenExistingReturnReportOns_MappedObjectShouldUpdatedReportOnQuestions()
        {
            var questions = new List<ReportOnQuestion>();
            var returnReportOns = new List<ReturnReportOn>();

            var @return = A.Fake<ReturnData>();
            @return.Quarter = new Quarter(2019, QuarterType.Q1);
            @return.QuarterWindow = QuarterWindowTestHelper.GetDefaultQuarterWindow();

            for (var i = 0; i < 5; i++)
            {
                questions.Add(this.fixture.Build<ReportOnQuestion>().With(r => r.Id, i + 1).Create());
                returnReportOns.Add(new ReturnReportOn(i + 1, @return.Id));
            }

            @return.ReturnReportOns = returnReportOns;

            var result = this.map.Map(new ReportOptionsToSelectReportOptionsViewModelMapTransfer() { OrganisationId = Guid.NewGuid(), ReturnId = Guid.NewGuid(), ReportOnQuestions = questions, ReturnData = @return });

            this.AssertValues(result, true);
        }

        [Fact]
        public void Map_GivenReturnData_ReturnPropertiesShouldBeMapped()
        {
            var returnData = this.fixture.Build<ReturnData>()
                    .With(r => r.Quarter, new Quarter(2019, QuarterType.Q1))
                    .With(r => r.QuarterWindow, QuarterWindowTestHelper.GetDefaultQuarterWindow())
                    .With(r => r.ReturnReportOns, ((List<ReturnReportOn>)null))
                    .Create();

            var organisationId = this.fixture.Create<Guid>();
            var returnId = this.fixture.Create<Guid>();
            var reportQuestions = this.fixture.CreateMany<ReportOnQuestion>().ToList();

            var result = this.map.Map(new ReportOptionsToSelectReportOptionsViewModelMapTransfer() { OrganisationId = organisationId, ReturnId = returnId, ReturnData = returnData, ReportOnQuestions = reportQuestions });

            result.OrganisationId.Should().Be(organisationId);
            result.ReturnId.Should().Be(returnId);
            result.ReportOnQuestions.Should().BeSameAs(reportQuestions);
            result.ReturnData.Should().Be(returnData);
            result.Year.Should().Be(returnData.Quarter.Year.ToString());
            result.QuarterWindowEndDate.Should().Be(returnData.QuarterWindow.QuarterEnd);
            result.QuarterWindowStartDate.Should().Be(returnData.QuarterWindow.QuarterStart);
            result.Quarter.Should().Be(returnData.Quarter.Q.ToString());
        }

        [Fact]
        public void Map_GivenExistingNonObligatedAlreadySelected_MappedObjectShouldSetDcfSelectedValueToNo()
        {
            var questions = new List<ReportOnQuestion>();
            var returnReportOns = new List<ReturnReportOn>();

            var @return = A.Fake<ReturnData>();
            @return.Quarter = new Quarter(2019, QuarterType.Q1);
            @return.QuarterWindow = QuarterWindowTestHelper.GetDefaultQuarterWindow();

            for (var i = 0; i < 4; i++)
            {
                questions.Add(this.fixture.Build<ReportOnQuestion>().With(r => r.Id, i + 1).Create());
                returnReportOns.Add(new ReturnReportOn(i + 1, @return.Id));
            }

            @return.ReturnReportOns = returnReportOns;

            var result = this.map.Map(new ReportOptionsToSelectReportOptionsViewModelMapTransfer() { OrganisationId = Guid.NewGuid(), ReturnId = Guid.NewGuid(), ReportOnQuestions = questions, ReturnData = @return });

            result.ReportOnQuestions.Count.Should().Be(4);
            result.ReportOnQuestions.ElementAt(0).Id.Should().Be(1);
            result.ReportOnQuestions.ElementAt(0).Selected.Should().Be(true);
            result.ReportOnQuestions.ElementAt(1).Id.Should().Be(2);
            result.ReportOnQuestions.ElementAt(1).Selected.Should().Be(true);
            result.ReportOnQuestions.ElementAt(2).Id.Should().Be(3);
            result.ReportOnQuestions.ElementAt(2).Selected.Should().Be(true);
            result.ReportOnQuestions.ElementAt(3).Id.Should().Be(4);
            result.ReportOnQuestions.ElementAt(3).Selected.Should().Be(true);
            result.DcfSelectedValue = DcfNoValue;
        }

        private void AssertValues(SelectReportOptionsViewModel result, bool selected)
        {
            result.ReportOnQuestions.Count.Should().Be(5);
            result.ReportOnQuestions.ElementAt(0).Id.Should().Be(1);
            result.ReportOnQuestions.ElementAt(0).Selected.Should().Be(selected);
            result.ReportOnQuestions.ElementAt(1).Id.Should().Be(2);
            result.ReportOnQuestions.ElementAt(1).Selected.Should().Be(selected);
            result.ReportOnQuestions.ElementAt(2).Id.Should().Be(3);
            result.ReportOnQuestions.ElementAt(2).Selected.Should().Be(selected);
            result.ReportOnQuestions.ElementAt(3).Id.Should().Be(4);
            result.ReportOnQuestions.ElementAt(3).Selected.Should().Be(selected);
            result.ReportOnQuestions.ElementAt(4).Id.Should().Be(5);
            result.ReportOnQuestions.ElementAt(4).Selected.Should().Be(selected);
        }
    }
}
