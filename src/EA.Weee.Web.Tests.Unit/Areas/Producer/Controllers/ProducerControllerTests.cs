namespace EA.Weee.Web.Tests.Unit.Areas.Producer.Controllers
{
    using AutoFixture;
    using Core.Organisations;
    using EA.Prsd.Core;
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Core;
    using EA.Weee.Core.DirectRegistrant;
    using EA.Weee.Core.Helpers;
    using EA.Weee.Core.Organisations.Base;
    using EA.Weee.Core.Shared;
    using EA.Weee.Tests.Core;
    using EA.Weee.Web.Areas.Producer.Controllers;
    using EA.Weee.Web.Areas.Producer.Filters;
    using EA.Weee.Web.Areas.Producer.Mappings.ToViewModel;
    using EA.Weee.Web.Areas.Producer.ViewModels;
    using EA.Weee.Web.Constant;
    using EA.Weee.Web.Controllers.Base;
    using EA.Weee.Web.Infrastructure;
    using EA.Weee.Web.Infrastructure.PDF;
    using EA.Weee.Web.Services.Caching;
    using EA.Weee.Web.Services.SubmissionService;
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
        private readonly IMvcTemplateExecutor templateExecutor;
        private readonly IPdfDocumentProvider pdfDocumentProvider;
        private readonly ISubmissionService submissionService;

        public ProducerControllerTests()
        {
            breadcrumb = A.Fake<BreadcrumbService>();
            weeeCache = A.Fake<IWeeeCache>();
            mapper = A.Fake<IMapper>();
            templateExecutor = A.Fake<IMvcTemplateExecutor>();
            pdfDocumentProvider = A.Fake<IPdfDocumentProvider>();

            submissionService = A.Fake<ISubmissionService>();

            controller = new ProducerController(
               breadcrumb, 
               weeeCache, 
               mapper,
               templateExecutor,
               pdfDocumentProvider,
               submissionService);
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
                SubmissionHistory = new Dictionary<int, SmallProducerSubmissionHistoryData>()
                {
                    { 2024, TestFixture.Build<SmallProducerSubmissionHistoryData>().With(s => s.Status, SubmissionStatus.Submitted).Create() },
                },
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
        public void TaskList_Get_ShouldHaveSmallProducerSubmissionSubmittedAttribute()
        {
            // Arrange
            var methodInfo = typeof(ProducerController).GetMethod("TaskList");

            // Act & Assert
            methodInfo.Should().BeDecoratedWith<SmallProducerSubmissionSubmittedAttribute>();
        }

        [Fact]
        public void AlreadySubmittedAndPaid_Get_ReturnView()
        {
            // Arrange
            var id = Guid.NewGuid();
            const int complianceYear = 2024;

            controller.SmallProducerSubmissionData = new Core.DirectRegistrant.SmallProducerSubmissionData
            {
                OrganisationData = new OrganisationData
                {
                    Id = id
                },
                CurrentSubmission = new SmallProducerSubmissionHistoryData()
                {
                    ComplianceYear = complianceYear
                }
            };

            // Act
            var result = controller.AlreadySubmittedAndPaid() as ViewResult;

            // Assert
            result.Should().NotBeNull();
            result.Model.Should().BeOfType<AlreadySubmittedAndPaidViewModel>();

            var model = result.Model as AlreadySubmittedAndPaidViewModel;
            model.OrganisationId.Should().Be(id);
            model.ComplianceYear.Should().Be(complianceYear);

            result.ViewName.Should().BeEmpty();
        }

        [Fact]
        public void AlreadySubmittedAndPaid_Get_ShouldHaveSmallProducerSubmissionContextAttribute()
        {
            // Arrange
            var methodInfo = typeof(ProducerController).GetMethod("AlreadySubmittedAndPaid");

            // Act & Assert
            methodInfo.Should().BeDecoratedWith<SmallProducerSubmissionContextAttribute>();
            methodInfo.Should().BeDecoratedWith<HttpGetAttribute>();
        }

        [Fact]
        public void OrganisationHasNoSubmissions_Get_ReturnView()
        {
            // Arrange
            var id = Guid.NewGuid();
            const int complianceYear = 2024;

            controller.SmallProducerSubmissionData = new Core.DirectRegistrant.SmallProducerSubmissionData
            {
                OrganisationData = new OrganisationData
                {
                    Id = id
                },
                CurrentSubmission = new SmallProducerSubmissionHistoryData()
                {
                    ComplianceYear = complianceYear
                }
            };

            // Act
            var result = controller.OrganisationHasNoSubmissions() as ViewResult;

            // Assert
            result.Should().NotBeNull();
            result.Model.Should().BeOfType<AlreadySubmittedAndPaidViewModel>();

            var model = result.Model as AlreadySubmittedAndPaidViewModel;
            model.OrganisationId.Should().Be(id);
            model.ComplianceYear.Should().Be(complianceYear);

            result.ViewName.Should().BeEmpty();
        }

        [Fact]
        public void OrganisationHasNoSubmissions_Get_ShouldHaveSmallProducerSubmissionContextAttribute()
        {
            // Arrange
            var methodInfo = typeof(ProducerController).GetMethod("OrganisationHasNoSubmissions");

            // Act & Assert
            methodInfo.Should().BeDecoratedWith<SmallProducerSubmissionContextAttribute>();
            methodInfo.Should().BeDecoratedWith<HttpGetAttribute>();
        }

        [Fact]
        public void OrganisationHasNoSubmissions_Get_ShouldSetBreadcrumb()
        {
            // Arrange
            var id = Guid.NewGuid();
            const int complianceYear = 2024;

            controller.SmallProducerSubmissionData = new Core.DirectRegistrant.SmallProducerSubmissionData
            {
                OrganisationData = new OrganisationData
                {
                    Id = id
                },
                CurrentSubmission = new SmallProducerSubmissionHistoryData()
                {
                    ComplianceYear = complianceYear
                }
            };

            // Act
            var result = controller.OrganisationHasNoSubmissions() as ViewResult;

            Assert.Equal(breadcrumb.ExternalActivity, ProducerSubmissionConstant.HistoricProducerRegistrationSubmission);
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
        public void CheckAnswers_Get_ShouldHaveSmallProducerSubmissionSubmittedAttribute()
        {
            // Arrange
            var methodInfo = typeof(ProducerController).GetMethod("CheckAnswers");

            // Act & Assert
            methodInfo.Should().BeDecoratedWith<SmallProducerSubmissionSubmittedAttribute>();
        }

        [Fact]
        public async Task OrganisationDetails_ReturnOrganisationDetailsView()
        {
            SetupDefaultControllerData();

            var view = (await controller.OrganisationDetails()) as ViewResult;

            view.ViewName.Should().Be("Producer/ViewOrganisation/OrganisationDetails");
        }

        [Fact]
        public async Task ContactDetails_ReturnOrganisationDetailsView()
        {
            SetupDefaultControllerData();

            var view = (await controller.ContactDetails()) as ViewResult;

            view.ViewName.Should().Be("Producer/ViewOrganisation/ContactDetails");
        }

        [Fact]
        public void ContactDetails_Get_ShouldHaveSmallProducerSubmissionContextAttribute()
        {
            // Arrange
            var methodInfo = typeof(ProducerController).GetMethod("ContactDetails", new[] { typeof(int?) });

            // Act & Assert
            methodInfo.Should().BeDecoratedWith<SmallProducerSubmissionContextAttribute>();
            methodInfo.Should().BeDecoratedWith<HttpGetAttribute>();
        }

        [Fact]
        public async Task ServiceOfNoticeDetails_ReturnOrganisationDetailsView()
        {
            SetupDefaultControllerData();

            var view = (await controller.ServiceOfNoticeDetails()) as ViewResult;

            view.ViewName.Should().Be("Producer/ViewOrganisation/ServiceOfNoticeDetails");
        }

        [Theory]
        [InlineData(null)]
        [InlineData(2004)]
        public async Task OrganisationDetails_ReturnViewModelAndCallsService(int? year)
        {
            SetupDefaultControllerData();

            var expcted = new OrganisationDetailsTabsViewModel();
            expcted.OrganisationViewModel = new OrganisationViewModel();

            A.CallTo(() => this.submissionService.OrganisationDetails(year)).Returns(expcted);

            var result = (await controller.OrganisationDetails(year)) as ViewResult;

            var viewResult = result.Should().BeOfType<ViewResult>().Subject;

            var model = viewResult.Model as OrganisationDetailsTabsViewModel;

            model.Should().NotBeNull();
            model.OrganisationViewModel.Should().NotBeNull();
            A.CallTo(() => this.submissionService.OrganisationDetails(year)).MustHaveHappenedOnceExactly();
            A.CallTo(() => this.submissionService.WithSubmissionData(controller.SmallProducerSubmissionData, false)).MustHaveHappenedOnceExactly();
        }

        [Theory]
        [InlineData(null)]
        [InlineData(2004)]
        public async Task ContactDetails_ReturnViewModelAndCallsService(int? year)
        {
            SetupDefaultControllerData();

            var expcted = new OrganisationDetailsTabsViewModel();
            expcted.ContactDetailsViewModel = new ContactDetailsViewModel();

            A.CallTo(() => this.submissionService.ContactDetails(year)).Returns(expcted);

            var result = (await controller.ContactDetails(year)) as ViewResult;

            var viewResult = result.Should().BeOfType<ViewResult>().Subject;

            var model = viewResult.Model as OrganisationDetailsTabsViewModel;

            model.Should().NotBeNull();
            model.ContactDetailsViewModel.Should().NotBeNull();
            A.CallTo(() => this.submissionService.ContactDetails(year)).MustHaveHappenedOnceExactly();
            A.CallTo(() => this.submissionService.WithSubmissionData(controller.SmallProducerSubmissionData, false)).MustHaveHappenedOnceExactly();
        }

        [Theory]
        [InlineData(null)]
        [InlineData(2004)]
        public async Task ServiceOfNotice_ReturnViewModelAndCallsService(int? year)
        {
            SetupDefaultControllerData();

            var expcted = new OrganisationDetailsTabsViewModel();
            expcted.ServiceOfNoticeViewModel = new ServiceOfNoticeViewModel();

            A.CallTo(() => this.submissionService.ServiceOfNoticeDetails(year)).Returns(expcted);

            var result = (await controller.ServiceOfNoticeDetails(year)) as ViewResult;

            var viewResult = result.Should().BeOfType<ViewResult>().Subject;

            var model = viewResult.Model as OrganisationDetailsTabsViewModel;

            model.Should().NotBeNull();
            model.ServiceOfNoticeViewModel.Should().NotBeNull();
            A.CallTo(() => this.submissionService.ServiceOfNoticeDetails(year)).MustHaveHappenedOnceExactly();
            A.CallTo(() => this.submissionService.WithSubmissionData(controller.SmallProducerSubmissionData, false)).MustHaveHappenedOnceExactly();
        }

        [Theory]
        [InlineData(null)]
        [InlineData(2004)]
        public async Task RepresentedOrganisationDetails_ReturnViewModelAndCallsService(int? year)
        {
            SetupDefaultControllerData();

            var expcted = new OrganisationDetailsTabsViewModel();
            expcted.RepresentingCompanyDetailsViewModel = new RepresentingCompanyDetailsViewModel();

            A.CallTo(() => this.submissionService.RepresentedOrganisationDetails(year)).Returns(expcted);

            var result = (await controller.RepresentedOrganisationDetails(year)) as ViewResult;

            var viewResult = result.Should().BeOfType<ViewResult>().Subject;

            var model = viewResult.Model as OrganisationDetailsTabsViewModel;

            model.Should().NotBeNull();
            model.RepresentingCompanyDetailsViewModel.Should().NotBeNull();

            A.CallTo(() => this.submissionService.RepresentedOrganisationDetails(year)).MustHaveHappenedOnceExactly();
            A.CallTo(() => this.submissionService.WithSubmissionData(controller.SmallProducerSubmissionData, false)).MustHaveHappenedOnceExactly();
        }

        [Theory]
        [InlineData(null)]
        [InlineData(2004)]
        public async Task TotalEEEDetails_ReturnViewModelAndCallsService(int? year)
        {
            SetupDefaultControllerData();

            var expcted = new OrganisationDetailsTabsViewModel();
            expcted.EditEeeDataViewModel = new EditEeeDataViewModel();

            A.CallTo(() => this.submissionService.TotalEEEDetails(year)).Returns(expcted);

            var result = (await controller.TotalEEEDetails(year)) as ViewResult;

            var viewResult = result.Should().BeOfType<ViewResult>().Subject;

            var model = viewResult.Model as OrganisationDetailsTabsViewModel;

            model.Should().NotBeNull();
            model.EditEeeDataViewModel.Should().NotBeNull();

            A.CallTo(() => this.submissionService.TotalEEEDetails(year)).MustHaveHappenedOnceExactly();
            A.CallTo(() => this.submissionService.WithSubmissionData(controller.SmallProducerSubmissionData, false)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public void ServiceOfNoticeDetails_Get_ShouldHaveSmallProducerSubmissionContextAttribute()
        {
            // Arrange
            var methodInfo = typeof(ProducerController).GetMethod("ServiceOfNoticeDetails", new[] { typeof(int?) });

            // Act & Assert
            methodInfo.Should().BeDecoratedWith<SmallProducerSubmissionContextAttribute>();
            methodInfo.Should().BeDecoratedWith<HttpGetAttribute>();
        }

        [Fact]
        public async Task RepresentedOrganisationDetails_ReturnOrganisationDetailsView()
        {
            SetupDefaultControllerData();

            var view = (await controller.RepresentedOrganisationDetails()) as ViewResult;

            view.ViewName.Should().Be("Producer/ViewOrganisation/RepresentedOrganisationDetails");
        }

        [Fact]
        public void RepresentedOrganisationDetails_Get_ShouldHaveSmallProducerSubmissionContextAttribute()
        {
            // Arrange
            var methodInfo = typeof(ProducerController).GetMethod("RepresentedOrganisationDetails", new[] { typeof(int?) });

            // Act & Assert
            methodInfo.Should().BeDecoratedWith<SmallProducerSubmissionContextAttribute>();
            methodInfo.Should().BeDecoratedWith<HttpGetAttribute>();
        }

        [Fact]
        public async Task TotalEEEDetails_ReturnOrganisationDetailsView()
        {
            SetupDefaultControllerData();

            var view = (await controller.TotalEEEDetails()) as ViewResult;

            view.ViewName.Should().Be("Producer/ViewOrganisation/TotalEEEDetails");
        }

        [Fact]
        public async Task TotalEEEDetails_IfNoSubmittedSubmissions_RedirectToOrganisationHasNoSubmissions()
        {
            SetupDefaultControllerData();
            SetupInCompleteSubmission();

            var result = (await controller.TotalEEEDetails(2000)) as RedirectToRouteResult;

            result.RouteValues["action"].Should().Be("OrganisationHasNoSubmissions");
        }

        [Fact]
        public async Task RepresentedOrganisationDetails_IfNoSubmittedSubmissions_RedirectToOrganisationHasNoSubmissions()
        {
            SetupDefaultControllerData();
            SetupInCompleteSubmission();

            var result = (await controller.RepresentedOrganisationDetails(2000)) as RedirectToRouteResult;

            result.RouteValues["action"].Should().Be("OrganisationHasNoSubmissions");
        }

        [Fact]
        public async Task ServiceOfNoticeDetails_IfNoSubmittedSubmissions_RedirectToOrganisationHasNoSubmissions()
        {
            SetupDefaultControllerData();
            SetupInCompleteSubmission();

            var result = (await controller.ServiceOfNoticeDetails(2000)) as RedirectToRouteResult;

            result.RouteValues["action"].Should().Be("OrganisationHasNoSubmissions");
        }

        [Fact]
        public async Task ContactDetails_IfNoSubmittedSubmissions_RedirectToOrganisationHasNoSubmissions()
        {
            SetupDefaultControllerData();
            SetupInCompleteSubmission();

            var result = (await controller.ContactDetails(2000)) as RedirectToRouteResult;

            result.RouteValues["action"].Should().Be("OrganisationHasNoSubmissions");
        }

        [Fact]
        public async Task OrganisationDetails_IfNoSubmittedSubmissions_RedirectToOrganisationHasNoSubmissions()
        {
            SetupDefaultControllerData();
            SetupInCompleteSubmission();

            var result = (await controller.OrganisationDetails(2000)) as RedirectToRouteResult;

            result.RouteValues["action"].Should().Be("OrganisationHasNoSubmissions");
        }

        [Fact]
        public void TotalEEEDetails_Get_ShouldHaveSmallProducerSubmissionContextAttribute()
        {
            // Arrange
            var methodInfo = typeof(ProducerController).GetMethod("TotalEEEDetails", new[] { typeof(int?) });

            // Act & Assert
            methodInfo.Should().BeDecoratedWith<SmallProducerSubmissionContextAttribute>();
            methodInfo.Should().BeDecoratedWith<HttpGetAttribute>();
        }

        [Fact]
        public void DownloadSubmission_Get_GivenPdf_FileShouldBeReturned()
        {
            //arrange
            var date = new DateTime(2022, 09, 2, 13, 22, 0);
            SystemTime.Freeze(date);
            var pdf = TestFixture.Create<byte[]>();

            var submissionData = TestFixture.Create<SmallProducerSubmissionMapperData>();
            controller.SmallProducerSubmissionData = submissionData.SmallProducerSubmissionData;

            var viewModel = TestFixture.Create<CheckAnswersViewModel>();
            A.CallTo(() => mapper.Map<SmallProducerSubmissionMapperData, CheckAnswersViewModel>
                (A<SmallProducerSubmissionMapperData>.That.Matches(sd => sd.SmallProducerSubmissionData.Equals(submissionData.SmallProducerSubmissionData)))).Returns(viewModel);

            A.CallTo(() => pdfDocumentProvider.GeneratePdfFromHtml(A<string>._, null)).Returns(pdf);
            A.CallTo(() => mapper.Map<SmallProducerSubmissionMapperData, CheckAnswersViewModel>
                (A<SmallProducerSubmissionMapperData>.That.Matches(sd => sd.SmallProducerSubmissionData.Equals(submissionData.SmallProducerSubmissionData) &&
                    sd.RedirectToCheckAnswers.Equals(submissionData.RedirectToCheckAnswers)))).Returns(viewModel);

            //act
            var result = controller.DownloadSubmission() as FileContentResult;

            //assert
            result.FileContents.Should().BeSameAs(pdf);
            result.FileDownloadName.Should().Be("producer_submission_020922_1422.pdf");
            result.ContentType.Should().Be("application/pdf");
            SystemTime.Unfreeze();
        }

        [Fact]
        public void DownloadSubmission_Get_WithComplianceYear_GivenPdf_FileShouldBeReturned()
        {
            // Arrange
            var date = new DateTime(2023, 10, 15, 10, 30, 0);
            SystemTime.Freeze(date);
            var pdf = TestFixture.Create<byte[]>();
            var complianceYear = 2024;

            var submissionData = TestFixture.Create<SmallProducerSubmissionMapperData>();
            controller.SmallProducerSubmissionData = submissionData.SmallProducerSubmissionData;

            var source = new SmallProducerSubmissionMapperData()
            {
                SmallProducerSubmissionData = submissionData.SmallProducerSubmissionData,
                Year = complianceYear
            };

            var viewModel = TestFixture.Create<CheckAnswersViewModel>();
            A.CallTo(() => mapper.Map<SmallProducerSubmissionMapperData, CheckAnswersViewModel>
                (A<SmallProducerSubmissionMapperData>.That.Matches(sd => sd.SmallProducerSubmissionData.Equals(submissionData.SmallProducerSubmissionData) && sd.Year == complianceYear)))
                .Returns(viewModel);

            A.CallTo(() => pdfDocumentProvider.GeneratePdfFromHtml(A<string>._, null)).Returns(pdf);

            // Act
            var result = controller.DownloadSubmission(complianceYear) as FileContentResult;

            // Assert
            result.Should().NotBeNull();
            result.FileContents.Should().BeSameAs(pdf);
            result.FileDownloadName.Should().Be("producer_submission_151023_1130.pdf");
            result.ContentType.Should().Be("application/pdf");

            SystemTime.Unfreeze();
        }

        [Fact]
        public void DownloadSubmission_Get_ShouldHaveSmallProducerSubmissionContextAttribute()
        {
            // Arrange
            var methodInfo = typeof(ProducerController).GetMethod("DownloadSubmission");

            // Act & Assert
            methodInfo.Should().BeDecoratedWith<SmallProducerSubmissionContextAttribute>();
            methodInfo.Should().BeDecoratedWith<HttpGetAttribute>();
        }

        [Fact]
        public void ProducerController_ShouldHaveAuthorizeRouteClaimsAttribute()
        {
            // Arrange
            var typeInfo = typeof(ProducerController);

            // Act
            var attribute = (AuthorizeRouteClaimsAttribute)Attribute.GetCustomAttribute(typeInfo, typeof(AuthorizeRouteClaimsAttribute));

            // Assert
            attribute.Should().NotBeNull();

            var routeIdParamField = typeof(AuthorizeRouteClaimsAttribute)
                .GetField("routeIdParam", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var claimsField = typeof(AuthorizeRouteClaimsAttribute)
                .GetField("claims", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

            var routeIdParam = (string)routeIdParamField.GetValue(attribute);
            var claims = (string[])claimsField.GetValue(attribute);

            routeIdParam.Should().Be("directRegistrantId");
            claims.Should().Contain(WeeeClaimTypes.DirectRegistrantAccess);
        }

        [Fact]
        public void ProducerController_ShouldHaveOutputCacheAttribute()
        {
            // Arrange
            var typeInfo = typeof(ProducerController);

            // Act & Assert
            typeInfo.Should().BeDecoratedWith<OutputCacheAttribute>(attr =>
                attr.NoStore == true &&
                attr.Duration == 0 &&
                attr.VaryByParam == "None");
        }

        private void SetupInCompleteSubmission()
        {
            controller.SmallProducerSubmissionData.SubmissionHistory = new Dictionary<int, SmallProducerSubmissionHistoryData>();
            controller.SmallProducerSubmissionData.SubmissionHistory.Add(2000, new SmallProducerSubmissionHistoryData()
            {
                Status = SubmissionStatus.InComplete
            });
        }
    }
}