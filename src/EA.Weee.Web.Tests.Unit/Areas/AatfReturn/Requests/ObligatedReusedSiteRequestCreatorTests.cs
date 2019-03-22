namespace EA.Weee.Web.Tests.Unit.Areas.AatfReturn.Requests
{
    using System;
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Requests.AatfReturn.Obligated;
    using EA.Weee.Web.Areas.AatfReturn.Requests;
    using EA.Weee.Web.Areas.AatfReturn.ViewModels;
    using FakeItEasy;
    using FluentAssertions;
    using Xunit;

    public class ObligatedReusedSiteRequestCreatorTests
    {
        private readonly IObligatedReusedSiteRequestCreator requestCreator;

        public ObligatedReusedSiteRequestCreatorTests()
        {
            requestCreator = new ObligatedReusedSiteRequestCreator();
        }

        [Fact]
        public void ViewModelToRequest_GivenValidViewModel_RequestShouldNotBeNull()
        {
            var viewModel = A.Fake<ReusedOffSiteCreateSiteViewModel>();

            var request = requestCreator.ViewModelToRequest(viewModel);

            request.Should().NotBeNull();
        }

        [Fact]
        public void ViewModelToRequest_GivenValidViewModel_PropertiesShouldBeMapped()
        {
            var viewModel = new ReusedOffSiteCreateSiteViewModel()
            {
                AatfId = Guid.NewGuid(),
                OrganisationId = Guid.NewGuid(),
                ReturnId = Guid.NewGuid(),
                AddressData = A.Fake<SiteAddressData>(),
            };

            var request = requestCreator.ViewModelToRequest(viewModel) as AddAatfSite;

            request.AatfId.Should().Be(viewModel.AatfId);
            request.OrganisationId.Should().Be(viewModel.OrganisationId);
            request.ReturnId.Should().Be(viewModel.ReturnId);
            request.AddressData.Should().Be(viewModel.AddressData);
        }

        [Fact]
        public void ViewModelToRequest_GivenAddViewModel_RequestTypeShouldBeAdd()
        {
            var model = new ReusedOffSiteCreateSiteViewModel();

            var request = requestCreator.ViewModelToRequest(model);

            request.Should().BeOfType<AddAatfSite>();
        }

        [Fact]
        public void ViewModelToRequest_GivenEditViewModel_RequestTypeShouldBeEdit()
        {
            var model = new ReusedOffSiteCreateSiteViewModel();
            model.AddressData.Id = Guid.NewGuid();

            var request = requestCreator.ViewModelToRequest(model);

            request.Should().BeOfType<EditAatfSite>();
        }

        [Fact]
        public void ViewModelToRequest_GivenEditViewModel_CategoryValuesShouldBeMapped()
        {
            var viewModel = new ReusedOffSiteCreateSiteViewModel()
            {
                AatfId = Guid.NewGuid(),
                OrganisationId = Guid.NewGuid(),
                ReturnId = Guid.NewGuid(),
                AddressData = A.Fake<SiteAddressData>(),
            };

            viewModel.AddressData.Id = Guid.NewGuid();

            var request = requestCreator.ViewModelToRequest(viewModel) as EditAatfSite;

            request.AddressData.Should().Be(viewModel.AddressData);
        }
    }
}
