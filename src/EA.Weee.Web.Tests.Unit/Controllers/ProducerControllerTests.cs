namespace EA.Weee.Web.Tests.Unit.Controllers
{
    using Core.Organisations;
    using EA.Weee.Tests.Core;
    using EA.Weee.Web.Areas.Producer.Controllers;
    using EA.Weee.Web.Areas.Producer.ViewModels;
    using EA.Weee.Web.Constant;
    using EA.Weee.Web.Services.Caching;
    using FakeItEasy;
    using FluentAssertions;
    using Services;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using Xunit;

    public class ProducerControllerTests : SimpleUnitTestBase
    {
        private ProducerController controller;
        private readonly IWeeeCache weeeCache;
        private readonly BreadcrumbService breadcrumb;

        public ProducerControllerTests()
        {
            breadcrumb = A.Fake<BreadcrumbService>();
            weeeCache = A.Fake<IWeeeCache>();

            controller = new ProducerController(
               breadcrumb, weeeCache);
        }

        [Fact]
        public async Task TaskList_ReturnsTaskListViewView()
        {
            // Arrange
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
                    OrganizationDetailsComplete = true,
                    RepresentingCompanyDetailsComplete = false,
                    ServiceOfNoticeComplete = true
                }
            };

            var expectedModel = new TaskListViewModel()
            {
                ProducerTaskModels = new List<ProducerTaskModel>
                {
                    new ProducerTaskModel
                    {
                        TaskLinkName = "Organisation details",
                        Complete = true
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
                    },
                    new ProducerTaskModel
                    {
                        TaskLinkName = "Representing company details",
                        Complete = false
                    },
                    new ProducerTaskModel
                    {
                        TaskLinkName = "EEE details",
                        Complete = true
                    }
                }
            };

            // Act
            ActionResult result = await controller.TaskList();

            // Assert
            var model = ((ViewResult)result).Model;

            Assert.NotNull(model);
            Assert.IsType<TaskListViewModel>(model);

            model.Should().BeEquivalentTo(expectedModel);
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
                    OrganizationDetailsComplete = true,
                    RepresentingCompanyDetailsComplete = false,
                    ServiceOfNoticeComplete = true
                }
            };

            await controller.TaskList();

            Assert.Equal(breadcrumb.ExternalActivity, BreadCrumbConstant.AatfReturn);
        }

        [Fact]
        public void TaskList_ReturnView()
        {
            var view = controller.CheckAnswers() as ViewResult;

            view.ViewName.Should().Be("CheckAnswers");
        }
    }
}
