namespace EA.Weee.Web.Tests.Unit.Areas.Admin.Requests
{
    using System.Linq;
    using AutoFixture;
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Web.Areas.Admin.Requests;
    using EA.Weee.Web.Areas.Admin.ViewModels.Aatf;
    using FluentAssertions;
    using Xunit;

    public class EditFacilityDetailsRequestCreatorTests
    {
        private readonly Fixture fixture;
        private readonly IEditFacilityDetailsRequestCreator requestCreator;

        public EditFacilityDetailsRequestCreatorTests()
        {
            fixture = new Fixture();
            requestCreator = new EditFacilityDetailsRequestCreator();
        }

        [Fact]
        public void ViewModelToRequest_GivenValidViewModel_PropertiesShouldBeMapped()
        {
            var viewModel = fixture.Build<AatfEditDetailsViewModel>()
                .With(e => e.StatusValue, AatfStatus.Approved.Value)
                .With(e => e.SizeValue, AatfSize.Large.Value)
                .Create();
            viewModel.CompetentAuthorityId = viewModel.CompetentAuthoritiesList.Last().Id;

            var result = requestCreator.ViewModelToRequest(viewModel);

            result.Should().NotBeNull();
            result.Data.Id.Should().Be(viewModel.Id);
            result.Data.Name.Should().Be(viewModel.Name);
            result.Data.ApprovalNumber.Should().Be(viewModel.ApprovalNumber);
            result.Data.CompetentAuthority.Id.Should().Be(viewModel.CompetentAuthorityId);
            result.Data.AatfStatus.Should().Be(AatfStatus.Approved);
            result.Data.SiteAddress.Should().Be(viewModel.SiteAddressData);
            result.Data.Size.Should().Be(AatfSize.Large);
            result.Data.FacilityType.Should().Be(viewModel.FacilityType);
            result.Data.ApprovalDate.Should().Be(viewModel.ApprovalDate.GetValueOrDefault());
        }
    }
}
