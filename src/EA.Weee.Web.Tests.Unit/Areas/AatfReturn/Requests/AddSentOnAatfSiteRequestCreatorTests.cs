namespace EA.Weee.Web.Tests.Unit.Areas.AatfReturn.Requests
{
    using System;
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Web.Areas.AatfReturn.Requests;
    using EA.Weee.Web.Areas.AatfReturn.ViewModels;
    using FakeItEasy;
    using FluentAssertions;
    using Xunit;

    public class AddSentOnAatfSiteRequestCreatorTests
    {
        private readonly IAddSentOnAatfSiteRequestCreator requestCreator;

        public AddSentOnAatfSiteRequestCreatorTests()
        {
            requestCreator = new AddSentOnAatfSiteRequestCreator();
        }

        [Fact]
        public void ViewModelToRequest_GivenValidViewModel_RequestShouldNotBeNull()
        {
            var viewModel = A.Fake<SentOnCreateSiteViewModel>();

            var request = requestCreator.ViewModelToRequest(viewModel);

            request.Should().NotBeNull();
        }

        [Fact]
        public void ViewModelToRequest_GivenValidViewModel_PropertiesShouldBeMapped()
        {
            var viewModel = new SentOnCreateSiteViewModel()
            {
                AatfId = Guid.NewGuid(),
                OrganisationId = Guid.NewGuid(),
                ReturnId = Guid.NewGuid(),
                OperatorAddressData = A.Fake<AatfAddressData>(),
                SiteAddressData = A.Fake<AatfAddressData>()
            };

            var request = requestCreator.ViewModelToRequest(viewModel);

            request.AatfId.Should().Be(viewModel.AatfId);
            request.OrganisationId.Should().Be(viewModel.OrganisationId);
            request.ReturnId.Should().Be(viewModel.ReturnId);
            request.OperatorAddressData.Should().Be(viewModel.OperatorAddressData);
            request.SiteAddressData.Should().Be(viewModel.SiteAddressData);
        }
    }
}
