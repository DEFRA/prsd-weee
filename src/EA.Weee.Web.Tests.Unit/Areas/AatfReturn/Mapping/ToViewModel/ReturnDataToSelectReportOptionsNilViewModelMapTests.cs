namespace EA.Weee.Web.Tests.Unit.Areas.AatfReturn.Mapping.ToViewModel
{
    using System;
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Core.DataReturns;
    using EA.Weee.Web.Areas.AatfReturn.Mappings.ToViewModel;
    using FakeItEasy;
    using FluentAssertions;
    using Weee.Tests.Core;
    using Xunit;

    public class ReturnDataToSelectReportOptionsNilViewModelMapTests
    {
        private readonly ReturnDataToSelectReportOptionsNilViewModelMap map;

        public ReturnDataToSelectReportOptionsNilViewModelMapTests()
        {
            map = new ReturnDataToSelectReportOptionsNilViewModelMap();
        }

        [Fact]
        public void Map_GivenNullSource_ArgumentNullExceptionExpected()
        {
            Action act = () => map.Map(null);

            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void Map_GivenNullReturnData_ArgumentNullExceptionExpected()
        {
            Action act = () => map.Map(new ReturnDataToSelectReportOptionsNilViewModelMapTransfer() { OrganisationId = Guid.NewGuid(), ReturnId = Guid.NewGuid() });

            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void Map_GivenMappingObjects_ObjectShouldBeMapped()
        {
            var @return = A.Fake<ReturnData>();
            @return.Quarter = new Quarter(2019, QuarterType.Q1);
            @return.QuarterWindow = QuarterWindowTestHelper.GetDefaultQuarterWindow();
            var transfer = new ReturnDataToSelectReportOptionsNilViewModelMapTransfer() {OrganisationId = Guid.NewGuid(), ReturnId = Guid.NewGuid(), ReturnData = @return};

            var result = map.Map(transfer);

            result.Year.Should().Be("2019");
            result.Period.Should().Be("Q1 Jan - Mar");
            result.ReturnId.Should().Be(transfer.ReturnId);
            result.OrganisationId.Should().Be(transfer.OrganisationId);
        }
    }
}
