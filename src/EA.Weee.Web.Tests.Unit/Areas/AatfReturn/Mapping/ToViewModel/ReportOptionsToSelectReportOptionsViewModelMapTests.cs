namespace EA.Weee.Web.Tests.Unit.Areas.AatfReturn.Mapping.ToViewModel
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Core.DataReturns;
    using EA.Weee.Web.Areas.AatfReturn.Mappings.ToViewModel;
    using FakeItEasy;
    using FluentAssertions;
    using Xunit;

    public class ReportOptionsToSelectReportOptionsViewModelMapTests
    {
        private readonly ReportOptionsToSelectReportOptionsViewModelMap map;

        public ReportOptionsToSelectReportOptionsViewModelMapTests()
        {
            map = new ReportOptionsToSelectReportOptionsViewModelMap();
        }

        [Fact]
        public void Map_GivenNullSource_ArgumentNullExceptionExpected()
        {
            Action act = () => map.Map(null);

            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void Map_GivenNullReportOnQuestionsList_ArgumentNullExceptionExpected()
        {
            Action act = () => map.Map(new ReportOptionsToSelectReportOptionsViewModelMapTransfer() { OrganisationId = Guid.NewGuid(), ReturnId = Guid.NewGuid(), ReturnData = A.Fake<ReturnData>() });

            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void Map_GivenNullReturnData_ArgumentNullExceptionExpected()
        {
            Action act = () => map.Map(new ReportOptionsToSelectReportOptionsViewModelMapTransfer() { OrganisationId = Guid.NewGuid(), ReturnId = Guid.NewGuid(), ReportOnQuestions = A.Fake<List<ReportOnQuestion>>() });

            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void Map_GivenMappingObjects_MappedObjectShouldContainShouldContainQuestions()
        {
            var questions = new List<ReportOnQuestion>();

            var @return = A.Fake<ReturnData>();
            @return.Quarter = new Quarter(2019, QuarterType.Q1);
            @return.QuarterWindow = A.Fake<QuarterWindow>();

            for (var i = 0; i < 5; i++)
            {
                questions.Add(new ReportOnQuestion(i + 1, A.Dummy<string>(), A.Dummy<string>(), null));
            }

            var result = map.Map(new ReportOptionsToSelectReportOptionsViewModelMapTransfer() { OrganisationId = Guid.NewGuid(), ReturnId = Guid.NewGuid(), ReportOnQuestions = questions, ReturnData = @return });

            result.ReportOnQuestions.Count.Should().Be(5);
            result.ReportOnQuestions.ElementAt(0).Id.Should().Be(1);
            result.ReportOnQuestions.ElementAt(1).Id.Should().Be(2);
            result.ReportOnQuestions.ElementAt(2).Id.Should().Be(3);
            result.ReportOnQuestions.ElementAt(3).Id.Should().Be(4);
            result.ReportOnQuestions.ElementAt(4).Id.Should().Be(5);
        }
    }
}
