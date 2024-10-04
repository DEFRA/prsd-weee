namespace EA.Weee.Web.Tests.Unit.Areas.Producer.Controllers
{
    using AutoFixture;
    using Core.Organisations;
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Core;
    using EA.Weee.Core.DirectRegistrant;
    using EA.Weee.Core.Organisations.Base;
    using EA.Weee.Core.Shared;
    using EA.Weee.Requests.Shared;
    using EA.Weee.Tests.Core;
    using EA.Weee.Web.Areas.Producer.Controllers;
    using EA.Weee.Web.Areas.Producer.Filters;
    using EA.Weee.Web.Areas.Producer.Mappings.ToViewModel;
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
        private readonly IMapper mapper;

        public ProducerControllerTests()
        {
            breadcrumb = A.Fake<BreadcrumbService>();
            weeeCache = A.Fake<IWeeeCache>();
            mapper = A.Fake<IMapper>();

            controller = new ProducerController(
               breadcrumb, 
               weeeCache, 
               mapper);
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

        public static IEnumerable<object[]> TaskCompletionCombinations()
        {
            var allCombinations = new List<object[]>();
            for (var i = 0; i < 64; i++)
            {
                allCombinations.Add(new object[]
                {
                    (i & 1) != 0,  // OrganisationDetailsComplete
                    (i & 2) != 0,  // ContactDetailsComplete
                    (i & 4) != 0,  // ServiceOfNoticeComplete
                    (i & 8) != 0,  // RepresentingCompanyDetailsComplete
                    (i & 16) != 0, // EEEDetailsComplete
                    (i & 32) != 0  // HasAuthorizedRepresentative
                });
            }
            return allCombinations;
        }

        [Theory]
        [MemberData(nameof(TaskCompletionCombinations))]
        public async Task TaskList_ReturnsCorrectViewModelForAllCombinations(
            bool organisationDetailsComplete,
            bool contactDetailsComplete,
            bool serviceOfNoticeComplete,
            bool representingCompanyDetailsComplete,
            bool eeeDetailsComplete,
            bool hasAuthorizedRepresentative)
        {
            // Arrange
            SetupControllerData(
                organisationDetailsComplete,
                contactDetailsComplete,
                serviceOfNoticeComplete,
                representingCompanyDetailsComplete,
                eeeDetailsComplete,
                hasAuthorizedRepresentative);

            var expectedModel = GetExpectedModel(
                organisationDetailsComplete,
                contactDetailsComplete,
                serviceOfNoticeComplete,
                representingCompanyDetailsComplete,
                eeeDetailsComplete,
                hasAuthorizedRepresentative);

            // Act
            var result = await controller.TaskList();

            // Assert
            var viewResult = result.Should().BeOfType<ViewResult>().Subject;
            var model = viewResult.Model.Should().BeOfType<TaskListViewModel>().Subject;

            model.OrganisationId.Should().Be(expectedModel.OrganisationId);
            model.ProducerTaskModels.Should().HaveSameCount(expectedModel.ProducerTaskModels);

            for (int i = 0; i < model.ProducerTaskModels.Count; i++)
            {
                var actualTask = model.ProducerTaskModels[i];
                var expectedTask = expectedModel.ProducerTaskModels[i];

                actualTask.TaskLinkName.Should().Be(expectedTask.TaskLinkName);
                actualTask.Complete.Should().Be(expectedTask.Complete);
                actualTask.Action.Should().Be(expectedTask.Action);
            }
        }

        private void SetupControllerData(
            bool organisationDetailsComplete,
            bool contactDetailsComplete,
            bool serviceOfNoticeComplete,
            bool representingCompanyDetailsComplete,
            bool eeeDetailsComplete,
            bool hasAuthorizedRepresentative)
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
                    OrganisationDetailsComplete = organisationDetailsComplete,
                    ContactDetailsComplete = contactDetailsComplete,
                    ServiceOfNoticeComplete = serviceOfNoticeComplete,
                    RepresentingCompanyDetailsComplete = representingCompanyDetailsComplete,
                    EEEDetailsComplete = eeeDetailsComplete
                },
                HasAuthorisedRepresentitive = hasAuthorizedRepresentative
            };
        }

        private void SetupDefaultControllerData()
        {
            controller.SmallProducerSubmissionData = new Core.DirectRegistrant.SmallProducerSubmissionData
            {
                OrganisationData = new OrganisationData
                {
                    Id = organisationId,
                    CompanyRegistrationNumber = Guid.NewGuid().ToString(),
                    Name = Guid.NewGuid().ToString(),
                    OrganisationType = OrganisationType.Partnership,
                    TradingName = Guid.NewGuid().ToString(),
                    BusinessAddress = new Core.Shared.AddressData
                    {
                        Address1 = Guid.NewGuid().ToString(),
                        Address2 = Guid.NewGuid().ToString(),
                        TownOrCity = Guid.NewGuid().ToString(),
                        CountryName = Guid.NewGuid().ToString(),
                        WebAddress = Guid.NewGuid().ToString(),
                        Telephone = "4567894563",
                        Postcode = Guid.NewGuid().ToString()
                    }
                },
                CurrentSubmission = new Core.DirectRegistrant.SmallProducerSubmissionHistoryData
                {
                    ComplianceYear = 2005,
                    OrganisationDetailsComplete = true,
                    ContactDetailsComplete = true,
                    ServiceOfNoticeComplete = true,
                    RepresentingCompanyDetailsComplete = true,
                    EEEDetailsComplete = true,
                    ServiceOfNoticeData = new Core.Shared.AddressData
                    {
                        Address1 = Guid.NewGuid().ToString(),
                        Address2 = Guid.NewGuid().ToString(),
                        TownOrCity = Guid.NewGuid().ToString(),
                        CountryName = Guid.NewGuid().ToString(),
                        WebAddress = Guid.NewGuid().ToString(),
                        Telephone = "4567894563",
                        Postcode = Guid.NewGuid().ToString()
                    },
                    AuthorisedRepresentitiveData = TestFixture.Create<AuthorisedRepresentitiveData>()
                },
                HasAuthorisedRepresentitive = true,
                AuthorisedRepresentitiveData = TestFixture.Create<AuthorisedRepresentitiveData>()
            };
        }

        private TaskListViewModel GetExpectedModel(
            bool organisationDetailsComplete,
            bool contactDetailsComplete,
            bool serviceOfNoticeComplete,
            bool representingCompanyDetailsComplete,
            bool eeeDetailsComplete,
            bool hasAuthorizedRepresentative)
        {
            var taskModels = new List<ProducerTaskModel>
            {
                new ProducerTaskModel
                {
                    TaskLinkName = "Organisation details",
                    Complete = organisationDetailsComplete,
                    Action = nameof(ProducerSubmissionController.EditOrganisationDetails)
                },
                new ProducerTaskModel
                {
                    TaskLinkName = "Contact details",
                    Complete = contactDetailsComplete,
                    Action = nameof(ProducerSubmissionController.EditContactDetails)
                },
                new ProducerTaskModel
                {
                    TaskLinkName = "Service of notice",
                    Complete = serviceOfNoticeComplete,
                    Action = nameof(ProducerSubmissionController.ServiceOfNotice)
                }
            };

            if (hasAuthorizedRepresentative)
            {
                taskModels.Add(new ProducerTaskModel
                {
                    TaskLinkName = "Represented organisation details",
                    Complete = representingCompanyDetailsComplete,
                    Action = nameof(ProducerSubmissionController.EditRepresentedOrganisationDetails)
                });
            }

            taskModels.Add(new ProducerTaskModel
            {
                TaskLinkName = "EEE details",
                Complete = eeeDetailsComplete,
                Action = nameof(ProducerSubmissionController.EditEeeeData)
            });

            return new TaskListViewModel
            {
                OrganisationId = organisationId,
                ProducerTaskModels = taskModels
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
        public async Task CheckAnswers_Get_ShouldReturnViewWithMappedModel()
        {
            // Arrange
            var submissionData = TestFixture.Create<SmallProducerSubmissionMapperData>();
            controller.SmallProducerSubmissionData = submissionData.SmallProducerSubmissionData;

            var viewModel = TestFixture.Create<CheckAnswersViewModel>();
            A.CallTo(() => mapper.Map<SmallProducerSubmissionMapperData, CheckAnswersViewModel>
                (A<SmallProducerSubmissionMapperData>.That.Matches(sd => sd.SmallProducerSubmissionData.Equals(submissionData.SmallProducerSubmissionData)))).Returns(viewModel);

            // Act
            var result = await controller.CheckAnswers() as ViewResult;

            // Assert
            result.Should().NotBeNull();
            result.ViewName.Should().BeEmpty();
            result.Model.Should().Be(viewModel);
        }

        [Fact]
        public async Task CheckAnswers_Get_ShouldSetBreadCrumb()
        {
            // Arrange
            var submissionData = TestFixture.Create<SmallProducerSubmissionMapperData>();
            var organisationName = TestFixture.Create<string>();

            controller.SmallProducerSubmissionData = submissionData.SmallProducerSubmissionData;

            A.CallTo(() => weeeCache.FetchOrganisationName(submissionData.SmallProducerSubmissionData.OrganisationData.Id)).Returns(organisationName);

            var viewModel = TestFixture.Create<CheckAnswersViewModel>();
            A.CallTo(() => mapper.Map<SmallProducerSubmissionMapperData, CheckAnswersViewModel>
                (A<SmallProducerSubmissionMapperData>.That.Matches(sd => sd.SmallProducerSubmissionData.Equals(submissionData.SmallProducerSubmissionData) &&
                    sd.RedirectToCheckAnswers.Equals(submissionData.RedirectToCheckAnswers)))).Returns(viewModel);

            // Act
            await controller.CheckAnswers();

            // Assert
            breadcrumb.OrganisationId.Should().Be(submissionData.SmallProducerSubmissionData.OrganisationData.Id);
            breadcrumb.ExternalOrganisation.Should().Be(organisationName);
            breadcrumb.ExternalActivity.Should()
                .Be(ProducerSubmissionConstant.NewContinueProducerRegistrationSubmission);
        }

        [Fact]
        public void CheckAnswers_Get_ShouldHaveSmallProducerSubmissionContextAttribute()
        {
            // Arrange
            var methodInfo = typeof(ProducerController).GetMethod("CheckAnswers");

            // Act & Assert
            methodInfo.Should().BeDecoratedWith<SmallProducerSubmissionContextAttribute>();
            methodInfo.Should().BeDecoratedWith<HttpGetAttribute>();
        }

        [Fact]
        public async Task OrganisationDetails_ReturnOrganisationDetailsView()
        {
            SetupDefaultControllerData();

            var view = (await controller.OrganisationDetails()) as ViewResult;

            view.ViewName.Should().Be("ViewOrganisation/OrganisationDetails");
        }

        [Fact]
        public async Task OrganisationDetails_ReturnViewModelAndMapsData()
        {
            SetupDefaultControllerData();

            var organisationData = controller.SmallProducerSubmissionData.OrganisationData;

            var result = (await controller.OrganisationDetails()) as ViewResult;

            var viewResult = result.Should().BeOfType<ViewResult>().Subject;

            var model = viewResult.Model as OrganisationDetailsTabsViewModel;

            model.Should().NotBeNull();
            model.OrganisationViewModel.Should().NotBeNull();

            A.CallTo(() => mapper
                            .Map<SmallProducerSubmissionData, OrganisationViewModel>(controller.SmallProducerSubmissionData))
                            .MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task OrganisationDetails_SetViewBreadcrumb()
        {
            SetupDefaultControllerData();

            var expected = "org name";

            A.CallTo(() => weeeCache
            .FetchOrganisationName(controller.SmallProducerSubmissionData.OrganisationData.Id))
                .Returns(expected);

            var result = (await controller.OrganisationDetails()) as ViewResult;

            Assert.Equal(breadcrumb.ExternalActivity, ProducerSubmissionConstant.ViewOrganisation);
            Assert.Equal(breadcrumb.OrganisationId, controller.SmallProducerSubmissionData.OrganisationData.Id);
            Assert.Equal(breadcrumb.ExternalOrganisation, expected);
        }

        [Fact]
        public void ContactDetails_Get_ShouldHaveSmallProducerSubmissionContextAttribute()
        {
            // Arrange
            var methodInfo = typeof(ProducerController).GetMethod("ContactDetails", new Type[0]);

            // Act & Assert
            methodInfo.Should().BeDecoratedWith<SmallProducerSubmissionContextAttribute>();
            methodInfo.Should().BeDecoratedWith<HttpGetAttribute>();
        }

        [Fact]
        public void ServiceOfNoticeDetails_Get_ShouldHaveSmallProducerSubmissionContextAttribute()
        {
            // Arrange
            var methodInfo = typeof(ProducerController).GetMethod("ServiceOfNoticeDetails", new Type[0]);

            // Act & Assert
            methodInfo.Should().BeDecoratedWith<SmallProducerSubmissionContextAttribute>();
            methodInfo.Should().BeDecoratedWith<HttpGetAttribute>();
        }

        [Fact]
        public async Task ServiceOfNoticeDetails_Get_ReturnViewModelAndMapsData()
        {
            // Arrange
            SetupDefaultControllerData();

            var expectedAddress = TestFixture.Create<ServiceOfNoticeAddressData>();

            A.CallTo(() => mapper
                    .Map<AddressData, ServiceOfNoticeAddressData>(A<AddressData>._))
                    .Returns(expectedAddress);

            var expectedViewModel = TestFixture.Create<ServiceOfNoticeViewModel>();
            expectedViewModel.Address = expectedAddress;

            A.CallTo(() => mapper
                    .Map<SmallProducerSubmissionMapperData, ServiceOfNoticeViewModel>(A<SmallProducerSubmissionMapperData>.That.Matches(sd => sd.SmallProducerSubmissionData.Equals(controller.SmallProducerSubmissionData))))
                    .Returns(expectedViewModel);

            // Act
            var result = (await controller.ServiceOfNoticeDetails()) as ViewResult;

            // Assert
            var viewResult = result.Should().BeOfType<ViewResult>().Subject;

            var model = viewResult.Model as OrganisationDetailsTabsViewModel;

            model.Should().NotBeNull();
            model.ServiceOfNoticeViewModel.Should().NotBeNull();
            model.ServiceOfNoticeViewModel.Should().Be(expectedViewModel);
            model.ServiceOfNoticeViewModel.Address.Should().Be(expectedAddress);

            A.CallTo(() => mapper
                            .Map<SmallProducerSubmissionMapperData, ServiceOfNoticeViewModel>(A<SmallProducerSubmissionMapperData>.That.Matches(sd => sd.SmallProducerSubmissionData.Equals(controller.SmallProducerSubmissionData))))
                            .MustHaveHappenedOnceExactly();
        }

        [Fact]
        public void RepresentedOrganisationDetails_Get_ShouldHaveSmallProducerSubmissionContextAttribute()
        {
            // Arrange
            var methodInfo = typeof(ProducerController).GetMethod("RepresentedOrganisationDetails", new Type[0]);

            // Act & Assert
            methodInfo.Should().BeDecoratedWith<SmallProducerSubmissionContextAttribute>();
            methodInfo.Should().BeDecoratedWith<HttpGetAttribute>();
        }

        [Fact]
        public async Task RepresentedOrganisationDetails_Get_ReturnViewModelAndMapsData()
        {
            // Arrange
            SetupDefaultControllerData();

            var expectedAddress = TestFixture.Create<RepresentingCompanyAddressData>();

            var expectedViewModel = TestFixture.Create<RepresentingCompanyDetailsViewModel>();
            expectedViewModel.Address = expectedAddress;

            A.CallTo(() => mapper
                    .Map<SmallProducerSubmissionMapperData, RepresentingCompanyDetailsViewModel>(A<SmallProducerSubmissionMapperData>.That.Matches(sd => sd.SmallProducerSubmissionData.Equals(controller.SmallProducerSubmissionData))))
                    .Returns(expectedViewModel);

            // Act
            var result = (await controller.RepresentedOrganisationDetails()) as ViewResult;

            // Assert
            var viewResult = result.Should().BeOfType<ViewResult>().Subject;

            var model = viewResult.Model as OrganisationDetailsTabsViewModel;

            model.Should().NotBeNull();
            model.RepresentingCompanyDetailsViewModel.Should().Be(expectedViewModel);
            model.RepresentingCompanyDetailsViewModel.Address.Should().Be(expectedAddress);

            A.CallTo(() => mapper
                            .Map<SmallProducerSubmissionMapperData, RepresentingCompanyDetailsViewModel>(A<SmallProducerSubmissionMapperData>.That.Matches(sd => sd.SmallProducerSubmissionData.Equals(controller.SmallProducerSubmissionData))))
                            .MustHaveHappenedOnceExactly();
        }
    }
}