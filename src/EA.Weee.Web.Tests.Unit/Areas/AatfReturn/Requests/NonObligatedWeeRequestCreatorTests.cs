namespace EA.Weee.Web.Tests.Unit.Areas.AatfReturn.Requests
{
    using EA.Prsd.Core.Domain;
    using EA.Prsd.Core.Mapper;
    using EA.Prsd.Core.Web.OAuth;
    using EA.Weee.Api.Client;
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Web.Areas.AatfReturn.Requests;
    using EA.Weee.Web.Services;
    using FakeItEasy;
    using FluentAssertions;
    using Microsoft.Owin.Security;
    using System;
    using System.Security;
    using System.Security.Principal;
    using System.Threading.Tasks;
    using Web.Areas.AatfReturn.Controllers;
    using Web.Areas.AatfReturn.ViewModels;
    using Web.Controllers.Base;
    using Xunit;

    public class NonObligatedWeeRequestCreatorTests
    {
        private readonly INonObligatedWeeRequestCreator requestCreator;

        public NonObligatedWeeRequestCreatorTests()
        {
            requestCreator = new NonObligatedWeeRequestCreator();
        }

        [Fact]
        public async void AddNonObligatedRequest_GivenValidViewModel_CorrectlyMapToARequest()
        {
            var categoryValues = new CategoryValues();

            for (var i = 0; i < categoryValues.Count; i++)
            {
                categoryValues[i].Tonnage = i;
            }

            var viewModel = new NonObligatedValuesViewModel(categoryValues);

            var request = requestCreator.ViewModelToRequest(viewModel);
            Assert.NotNull(request);

            for (var i = 0; i < categoryValues.Count; i++)
            {
                Assert.Equal(request.CategoryValues[i].Tonnage, viewModel.CategoryValues[i].Tonnage);
                Assert.Equal(request.CategoryValues[i].CategoryId, viewModel.CategoryValues[i].CategoryId);
            }
        }
    }
}
