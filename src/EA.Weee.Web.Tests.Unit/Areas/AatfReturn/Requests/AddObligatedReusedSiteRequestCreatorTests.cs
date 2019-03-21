﻿namespace EA.Weee.Web.Tests.Unit.Areas.AatfReturn.Requests
{
    using System;
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Web.Areas.AatfReturn.Requests;
    using EA.Weee.Web.Areas.AatfReturn.ViewModels;
    using FakeItEasy;
    using FluentAssertions;
    using Xunit;

    public class AddObligatedReusedSiteRequestCreatorTests
    {
        private readonly IAddObligatedReusedSiteRequestCreator requestCreator;

        public AddObligatedReusedSiteRequestCreatorTests()
        {
            requestCreator = new AddObligatedReusedSiteRequestCreator();
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

            var request = requestCreator.ViewModelToRequest(viewModel);

            request.AatfId.Should().Be(viewModel.AatfId);
            request.OrganisationId.Should().Be(viewModel.OrganisationId);
            request.ReturnId.Should().Be(viewModel.ReturnId);
            request.AddressData.Should().Be(viewModel.AddressData);
        }
    }
}
