namespace EA.Weee.Web.Tests.Unit.Areas.Aatf.Mapping.ToViewModel
{
    using System;
    using AutoFixture;
    using Core.AatfEvidence;
    using FluentAssertions;
    using Web.Areas.Aatf.Mappings.ToViewModel;
    using Xunit;

    public class EvidenceSummaryMapTransferTests
    {
        private readonly Fixture fixture;

        public EvidenceSummaryMapTransferTests()
        {
            fixture = new Fixture();
        }

        [Fact]
        public void EvidenceSummaryMapTransfer_GivenEmptyOrganisationId_ArgumentExceptionExpected()
        {
            //act
            var exception = Record.Exception(() => new EvidenceSummaryMapTransfer(Guid.Empty, fixture.Create<Guid>(), fixture.Create<AatfEvidenceSummaryData>()));

            //assert
            exception.Should().BeOfType<ArgumentException>();
        }

        [Fact]
        public void EvidenceSummaryMapTransfer_GivenEmptyAatfId_ArgumentExceptionExpected()
        {
            //act
            var exception = Record.Exception(() => new EvidenceSummaryMapTransfer(fixture.Create<Guid>(), Guid.Empty, fixture.Create<AatfEvidenceSummaryData>()));

            //assert
            exception.Should().BeOfType<ArgumentException>();
        }

        [Fact]
        public void EvidenceSummaryMapTransfer_GivenNullSummaryData_ArgumentNullExceptionExpected()
        {
            //act
            var exception = Record.Exception(() => new EvidenceSummaryMapTransfer(fixture.Create<Guid>(), fixture.Create<Guid>(), null));

            //assert
            exception.Should().BeOfType<ArgumentNullException>();
        }
    }
}
