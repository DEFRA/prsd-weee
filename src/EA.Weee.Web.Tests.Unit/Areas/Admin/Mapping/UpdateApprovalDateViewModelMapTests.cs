namespace EA.Weee.Web.Tests.Unit.Areas.Admin.Mapping
{
    using System;
    using AutoFixture;
    using Core.AatfReturn;
    using Core.Admin;
    using FluentAssertions;
    using Web.Areas.Admin.Mappings.ToViewModel;
    using Web.Areas.Admin.ViewModels.Aatf;
    using Weee.Requests.Admin.Aatf;
    using Xunit;

    public class UpdateApprovalDateViewModelMapTests
    {
        private readonly UpdateApprovalDateViewModelMap map;
        private readonly Fixture fixture;

        public UpdateApprovalDateViewModelMapTests()
        {
            map = new UpdateApprovalDateViewModelMap();
            fixture = new Fixture();
        }

        [Fact]
        public void Map_GivenNullSource_ArgumentNullExceptionExpected()
        {
            var result = Xunit.Record.Exception(() => map.Map(null));

            result.Should().BeOfType<ArgumentNullException>();
        }

        [Fact]
        public void Map_GivenNullAatfData_ArgumentNullExceptionExpected()
        {
            var result = Xunit.Record.Exception(() => map.Map(new UpdateApprovalDateViewModelMapTransfer() { Request = fixture.Create<EditAatfDetails>() }));

            result.Should().BeOfType<ArgumentNullException>();
        }

        [Fact]
        public void Map_GivenNullRequest_ArgumentNullExceptionExpected()
        {
            var result = Xunit.Record.Exception(() => map.Map(new UpdateApprovalDateViewModelMapTransfer() { AatfData = fixture.Create<AatfData>() }));

            result.Should().BeOfType<ArgumentNullException>();
        }

        [Fact]
        public void Map_GivenTransferObject_PropertiesShouldBeMapped()
        {
            var flags = fixture.Create<CanApprovalDateBeChangedFlags>();
            var transfer = fixture.Build<UpdateApprovalDateViewModelMapTransfer>()
                .With(s => s.CanApprovalDateBeChangedFlags, flags).Create();

            var result = map.Map(transfer);

            result.FacilityType.Should().Be(transfer.AatfData.FacilityType);
            result.AatfId.Should().Be(transfer.AatfData.Id);
            result.OrganisationId.Should().Be(transfer.AatfData.Organisation.Id);
            result.UpdateApprovalDateData.Should().Be(transfer.CanApprovalDateBeChangedFlags);
            result.AatfName.Should().Be(transfer.AatfData.Name);
            result.OrganisationName.Should().Be(transfer.AatfData.Organisation.OrganisationName);
            result.Request.Should().Be(transfer.Request);
        }
    }
}
