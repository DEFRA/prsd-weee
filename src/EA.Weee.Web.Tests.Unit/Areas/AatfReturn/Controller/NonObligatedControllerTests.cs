namespace EA.Weee.Web.Tests.Unit.Areas.AatfReturn.Controller
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;
    using Api.Client;
    using Constant;
    using Core.AatfReturn;
    using EA.Weee.Requests.AatfReturn;
    using EA.Weee.Web.Areas.AatfReturn.ViewModels.Validation;
    using FakeItEasy;
    using FluentAssertions;
    using FluentValidation.Results;
    using Services;
    using Services.Caching;
    using Web.Areas.AatfReturn.Controllers;
    using Web.Areas.AatfReturn.Requests;
    using Web.Areas.AatfReturn.ViewModels;
    using Web.Controllers.Base;
    using Weee.Requests.AatfReturn.NonObligated;
    using Xunit;

    public class NonObligatedControllerTests
    {
        private readonly IWeeeClient weeeClient;
        private readonly INonObligatedWeeRequestCreator requestCreator;
        private readonly NonObligatedController controller;
        private readonly BreadcrumbService breadcrumb;
        private readonly INonObligatedValuesViewModelValidatorWrapper validator;

        public NonObligatedControllerTests()
        {
            weeeClient = A.Fake<IWeeeClient>();
            requestCreator = A.Fake<INonObligatedWeeRequestCreator>();
            breadcrumb = A.Fake<BreadcrumbService>();
            validator = A.Fake<INonObligatedValuesViewModelValidatorWrapper>();
            controller = new NonObligatedController(A.Fake<IWeeeCache>(), breadcrumb, () => weeeClient, requestCreator, validator);
        }

        [Fact]
        public void CheckNonObligatedControllerInheritsExternalSiteController()
        {
            typeof(NonObligatedController).BaseType.Name.Should().Be(typeof(ExternalSiteController).Name);
        }

        [Fact]
        public async void IndexPost_GivenValidViewModel_ApiSendShouldBeCalled()
        {
            var model = new NonObligatedValuesViewModel();
            var request = new AddNonObligated();

            A.CallTo(() => requestCreator.ViewModelToRequest(model)).Returns(request);

            await controller.Index(model);

            A.CallTo(() => weeeClient.SendAsync(A<string>._, request)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public async void IndexPost_GivenInvalidViewModel_ApiShouldNotBeCalled()
        {
            controller.ModelState.AddModelError("error", "error");

            await controller.Index(A.Dummy<NonObligatedValuesViewModel>());

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<AddNonObligated>._)).MustNotHaveHappened();
        }

        [Fact]
        public async void IndexGet_GivenValidViewModel_BreadcrumbShouldBeSet()
        {
            var organisationId = Guid.NewGuid();

            await controller.Index(organisationId, A.Dummy<Guid>(), A.Dummy<bool>());

            Assert.Equal(breadcrumb.ExternalActivity, BreadCrumbConstant.AatfReturn);
        }

        [Fact]
        public async void IndexPost_GivenNonObligatedValuesAreSubmitted_PageRedirectShouldBeCorrect()
        {
            var model = new NonObligatedValuesViewModel() { Dcf = false };

            var result = await controller.Index(model) as RedirectToRouteResult;

            result.RouteValues["action"].Should().Be("Index");
            result.RouteValues["controller"].Should().Be("AatfTaskList");
            result.RouteValues["area"].Should().Be("AatfReturn");
        }

        [Fact]
        public async void IndexPost_GivenNonObligatedDcfValuesAreSubmitted_PageRedirectShouldBeCorrect()
        {
            var model = new NonObligatedValuesViewModel() { Dcf = true };

            var result = await controller.Index(model) as RedirectToRouteResult;

            result.RouteValues["action"].Should().Be("Index");
            result.RouteValues["controller"].Should().Be("AatfTaskList");
            result.RouteValues["area"].Should().Be("AatfReturn");
        }

        [Fact]
        public async void IndexGet_GivenAction_DefaultViewShouldBeReturned()
        {
            var result = await controller.Index(A.Dummy<Guid>(), A.Dummy<Guid>(), A.Dummy<bool>()) as ViewResult;

            result.ViewName.Should().BeEmpty();
        }

        [Fact]
        public async void IndexGet_GivenActionAndParameters_NonObligatedViewModelShouldBeReturned()
        {
            var model = new NonObligatedValuesViewModel(new NonObligatedCategoryValues()) { Dcf = true, OrganisationId = Guid.NewGuid(), ReturnId = Guid.NewGuid() };

            var result = await controller.Index(model.OrganisationId, model.ReturnId, model.Dcf) as ViewResult;

            result.Model.Should().BeEquivalentTo(model);
        }

        [Fact]
        public async void IndexPost_GivenValidViewModel_ValidatorShouldReturnAValidResult()
        {
            var result = new ValidationResult();

            A.CallTo(() => validator.Validate(A<NonObligatedValuesViewModel>._, A<ReturnData>._)).Returns(result);

            await controller.Index(A.Fake<NonObligatedValuesViewModel>());

            controller.ModelState.IsValid.Should().BeTrue();
        }

        [Fact]
        public async void IndexPost_GivenInvalidViewModel_ValidatorShouldReturnAnInvalidResult()
        {
            var result = new ValidationResult();
            result.Errors.Add(new ValidationFailure("property", "error"));

            A.CallTo(() => validator.Validate(A<NonObligatedValuesViewModel>._, A<ReturnData>._)).Returns(result);

            await controller.Index(A.Fake<NonObligatedValuesViewModel>());

            controller.ModelState.IsValid.Should().BeFalse();
            controller.ModelState.Should().ContainKey("property");
            controller.ModelState.Count(c => c.Value.Errors.Any(d => d.ErrorMessage == "error")).Should().Be(1);
        }

        [Fact]
        public async void IndexPost_GivenValidViewModel_ValidatorShouldBeCalled()
        {
            var model = new NonObligatedValuesViewModel();
            var returnData = new ReturnData();

            A.CallTo(() => weeeClient.SendAsync(A<string>._, A<GetReturn>._)).Returns(returnData);

            await controller.Index(model);

            A.CallTo(() => validator.Validate(model, returnData)).MustHaveHappened(Repeated.Exactly.Once);
        }
    }
}
