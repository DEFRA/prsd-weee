﻿namespace EA.Weee.Web.Tests.Unit.Areas.AatfReturn.Requests
{
    using EA.Weee.Core.Scheme;
    using EA.Weee.Web.Areas.AatfReturn.Requests;
    using EA.Weee.Web.Areas.AatfReturn.ViewModels;
    using FakeItEasy;
    using FluentAssertions;
    using System;
    using System.Collections.Generic;
    using System.Linq;
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

            var viewModel = new SelectYourPcsViewModel(schemeList, selectedSchemeList);

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

            var viewModel = new SelectYourPcsViewModel(schemeList, selectedSchemeList);
            viewModel.ReturnId = returnId;

            var requests = requestCreator.ViewModelToRequest(viewModel);

            requests.ReturnId.Should().Be(returnId);
            requests.SchemeIds.Count.Should().Be(1);
            requests.SchemeIds.First().Should().Be(selectedSchemeId);
        }
    }
}
