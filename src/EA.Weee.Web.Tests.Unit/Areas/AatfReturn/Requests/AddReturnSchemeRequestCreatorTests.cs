namespace EA.Weee.Web.Tests.Unit.Areas.AatfReturn.Requests
{
    using System;
    using System.Collections.Generic;
    using EA.Weee.Core.Scheme;
    using EA.Weee.Web.Areas.AatfReturn.Requests;
    using EA.Weee.Web.Areas.AatfReturn.ViewModels;
    using FakeItEasy;
    using FluentAssertions;
    using Xunit;

    public class AddReturnSchemeRequestCreatorTests
    {
        private readonly IAddReturnSchemeRequestCreator requestCreator;

        public AddReturnSchemeRequestCreatorTests()
        {
            requestCreator = new AddReturnSchemeRequestCreator();
        }

        [Fact]
        public void ViewModelToRequested_GivenValidViewModel_RequestShouldNotBeNull()
        {
            var schemeList = A.Fake<List<SchemeData>>();
            var selectedSchemeList = new List<Guid>();
            selectedSchemeList.Add(Guid.NewGuid());

            var viewModel = new SelectYourPCSViewModel(schemeList, selectedSchemeList);

            var request = requestCreator.ViewModelToRequest(viewModel);

            request.Should().NotBeNull();
        }

        [Fact]
        public void ViewModelToRequested_GivenValidViewModel_AddReturnSchemeRequestShouldBeMapped()
        {
            var schemeList = A.Fake<List<SchemeData>>();
            var selectedSchemeList = new List<Guid>();
            var selectedSchemeId = Guid.NewGuid();
            var returnId = Guid.NewGuid();
            selectedSchemeList.Add(selectedSchemeId);

            var viewModel = new SelectYourPCSViewModel(schemeList, selectedSchemeList);
            viewModel.ReturnId = returnId;

            var requests = requestCreator.ViewModelToRequest(viewModel);

            foreach (var request in requests)
            {
                request.SchemeId.Should().Be(selectedSchemeId);
                request.ReturnId.Should().Be(returnId);
            }
        }
    }
}
