﻿namespace EA.Weee.Web.Tests.Unit.Areas.Producer.Controllers
{
    using Core.Organisations;
    using EA.Weee.Core;
    using EA.Weee.Tests.Core;
    using EA.Weee.Web.Areas.Producer.Controllers;
    using EA.Weee.Web.Areas.Producer.Filters;
    using EA.Weee.Web.Areas.Producer.ViewModels;
    using EA.Weee.Web.Constant;
    using EA.Weee.Web.Controllers.Base;
    using EA.Weee.Web.Services.Caching;
    using FakeItEasy;
    using FluentAssertions;
    using Services;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using Xunit;

    public class ProducerControllerTests : SimpleUnitTestBase
    {
        private readonly ProducerController controller;
        private readonly IWeeeCache weeeCache;
        private readonly BreadcrumbService breadcrumb;
        private readonly Guid organisationId = Guid.NewGuid();

        public ProducerControllerTests()
        {
            breadcrumb = A.Fake<BreadcrumbService>();
            weeeCache = A.Fake<IWeeeCache>();

            controller = new ProducerController(
               breadcrumb, weeeCache);
        }

        [Fact]
        public void Controller_Should_Have_ExternalSiteController_As_Base()
        {
            typeof(ProducerController).BaseType.Name.Should().Be(nameof(ExternalSiteController));
        }

        [Fact]
        public void Controller_Should_Have_AuthorizeRouteClaims_Attribute()
        {
            // Arrange
            var controllerType = typeof(ProducerController);

            // Act
            var attribute = controllerType.GetCustomAttributes(typeof(AuthorizeRouteClaimsAttribute), true)
                .FirstOrDefault() as AuthorizeRouteClaimsAttribute;

            // Assert
            attribute.Should().NotBeNull();

            var attributeType = typeof(AuthorizeRouteClaimsAttribute);
            var flags = BindingFlags.NonPublic | BindingFlags.Instance;

            var routeIdParamField = attributeType.GetField("routeIdParam", flags);
            var routeIdParam = routeIdParamField.GetValue(attribute) as string;

            var claimsField = attributeType.GetField("claims", flags);
            var claims = claimsField.GetValue(attribute) as string[];

            routeIdParam.Should().Be("directRegistrantId");
            claims.Should().ContainSingle().Which.Should().Be(WeeeClaimTypes.DirectRegistrantAccess);

            var usage = attribute.GetType().GetCustomAttribute<AttributeUsageAttribute>();
            usage.Should().NotBeNull();
            usage.ValidOn.Should().Be(AttributeTargets.Class | AttributeTargets.Method);
            usage.AllowMultiple.Should().BeTrue();
        }

        [Fact]
        public async Task TaskList_WithAuthorisedRepresentative_ReturnsCorrectViewModel()
        {
            // Arrange
            SetupControllerData(hasAuthorisedRepresentative: true);
            var expectedModel = GetExpectedModel(includeRepresentingCompanyDetails: true);

            // Act
            var result = await controller.TaskList();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<TaskListViewModel>(viewResult.Model);
            model.Should().BeEquivalentTo(expectedModel);
        }

        [Fact]
        public async Task TaskList_WithoutAuthorisedRepresentative_ReturnsCorrectViewModel()
        {
            // Arrange
            SetupControllerData(hasAuthorisedRepresentative: false);
            var expectedModel = GetExpectedModel(includeRepresentingCompanyDetails: false);

            // Act
            var result = await controller.TaskList();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<TaskListViewModel>(viewResult.Model);
            model.Should().BeEquivalentTo(expectedModel);
        }

        private void SetupControllerData(bool hasAuthorisedRepresentative)
        {
            controller.SmallProducerSubmissionData = new Core.DirectRegistrant.SmallProducerSubmissionData
            {
                OrganisationData = new OrganisationData
                {
                    Id = organisationId
                },
                CurrentSubmission = new Core.DirectRegistrant.SmallProducerSubmissionHistoryData
                {
                    ComplianceYear = 2005,
                    ContactDetailsComplete = true,
                    EEEDetailsComplete = true,
                    OrganisationDetailsComplete = true,
                    RepresentingCompanyDetailsComplete = false,
                    ServiceOfNoticeComplete = true
                },
                HasAuthorisedRepresentitive = hasAuthorisedRepresentative
            };
        }

        [Fact]
        public async Task TaskList_GivenValidViewModel_BreadcrumbShouldBeSet()
        {
            controller.SmallProducerSubmissionData = new Core.DirectRegistrant.SmallProducerSubmissionData
            {
                OrganisationData = new OrganisationData
                {
                    Id = Guid.Empty
                },
                CurrentSubmission = new Core.DirectRegistrant.SmallProducerSubmissionHistoryData
                {
                    ComplianceYear = 2005,
                    ContactDetailsComplete = true,
                    EEEDetailsComplete = true,
                    OrganisationDetailsComplete = true,
                    RepresentingCompanyDetailsComplete = false,
                    ServiceOfNoticeComplete = true
                }
            };

            await controller.TaskList();

            Assert.Equal(breadcrumb.ExternalActivity, ProducerSubmissionConstant.NewContinueProducerRegistrationSubmission);
        }

        [Fact]
        public void TaskList_ReturnView()
        {
            var view = controller.CheckAnswers() as ViewResult;

            view.ViewName.Should().Be("CheckAnswers");
        }

        private TaskListViewModel GetExpectedModel(bool includeRepresentingCompanyDetails)
        {
            var taskModels = new List<ProducerTaskModel>
            {
                new ProducerTaskModel
                {
                    TaskLinkName = "Organisation details",
                    Complete = true,
                    Action = nameof(ProducerSubmissionController.EditOrganisationDetails)
                },
                new ProducerTaskModel
                {
                    TaskLinkName = "Contact details",
                    Complete = true
                },
                new ProducerTaskModel
                {
                    TaskLinkName = "Service of notice",
                    Complete = true
                }
            };

            if (includeRepresentingCompanyDetails)
            {
                taskModels.Add(new ProducerTaskModel
                {
                    TaskLinkName = "Representing company details",
                    Complete = false
                });
            }

            taskModels.Add(new ProducerTaskModel
            {
                TaskLinkName = "EEE details",
                Complete = true,
                Action = nameof(ProducerSubmissionController.EditEeeeData)
            });

            return new TaskListViewModel
            {
                OrganisationId = organisationId,
                ProducerTaskModels = taskModels
            };
        }
    }
}
