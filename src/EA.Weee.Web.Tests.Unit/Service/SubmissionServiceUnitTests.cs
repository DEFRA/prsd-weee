namespace EA.Weee.Web.Tests.Unit.Service
{
    using AutoFixture;
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Core.DirectRegistrant;
    using EA.Weee.Core.Organisations;
    using EA.Weee.Core.Organisations.Base;
    using EA.Weee.Tests.Core;
    using EA.Weee.Web.Areas.Admin.ViewModels.Home;
    using EA.Weee.Web.Areas.Admin.ViewModels.Scheme.Overview;
    using EA.Weee.Web.Areas.Producer.ViewModels;
    using EA.Weee.Web.Constant;
    using EA.Weee.Web.Services;
    using EA.Weee.Web.Services.Caching;
    using EA.Weee.Web.Services.SubmissionsService;
    using FakeItEasy;
    using FluentAssertions;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Xunit;

    public class SubmissionServiceUnitTests : SimpleUnitTestBase
    {
        private readonly ISubmissionService service;
        private readonly BreadcrumbService breadcrumb;
        private readonly IWeeeCache weeeCache;
        private readonly IMapper mapper;

        public SubmissionServiceUnitTests()
        {
            breadcrumb = A.Fake<BreadcrumbService>();
            weeeCache = A.Fake<IWeeeCache>();
            mapper = A.Fake<IMapper>();

            service = new SubmissionService(breadcrumb, weeeCache, mapper);
        }

        [Theory]
        [InlineData(null)]
        [InlineData(2004)]
        public async Task OrganisationDetails_MapsAndReturns(int? year)
        {
            var data = GetDefaultSmallProducerData();

            service.WithSubmissionData(data);

            var result = await service.OrganisationDetails(year);

            var model = result.Should().BeOfType<OrganisationDetailsTabsViewModel>().Subject;

            model.OrganisationViewModel.Should().NotBeNull();

            var expectedyears = ExpectedYears(data);

            model.Years.Should().BeEquivalentTo(expectedyears);
            model.Year.Should().Be(year);
            model.ActiveOption.Should().Be(OrganisationDetailsDisplayOption.OrganisationDetails);
            model.IsInternal.Should().Be(false);
            model.SmallProducerSubmissionData.Should().Be(data);

            A.CallTo(() => mapper
            .Map<SubmissionsYearDetails, OrganisationViewModel>(
                A<SubmissionsYearDetails>.That.Matches(x => x.Year == year && x.SmallProducerSubmissionData == data)))
                .MustHaveHappenedOnceExactly();
        }

        [Theory]
        [InlineData(null)]
        [InlineData(2004)]
        public async Task ContactDetails_MapsAndReturns(int? year)
        {
            var data = GetDefaultSmallProducerData();

            service.WithSubmissionData(data);

            var result = await service.ContactDetails(year);

            var model = result.Should().BeOfType<OrganisationDetailsTabsViewModel>().Subject;

            model.ContactDetailsViewModel.Should().NotBeNull();

            var expectedyears = ExpectedYears(data);

            model.Years.Should().BeEquivalentTo(expectedyears);
            model.Year.Should().Be(year);
            model.ActiveOption.Should().Be(OrganisationDetailsDisplayOption.ContactDetails);
            model.IsInternal.Should().Be(false);
            model.SmallProducerSubmissionData.Should().Be(data);

            A.CallTo(() => mapper
                            .Map<SubmissionsYearDetails, ContactDetailsViewModel>(
                A<SubmissionsYearDetails>.That.Matches(x => x.Year == year && x.SmallProducerSubmissionData == data)))
                .MustHaveHappenedOnceExactly();
        }

        [Theory]
        [InlineData(null)]
        [InlineData(2004)]
        public async Task ServiceOfNoticeDetails_MapsAndReturns(int? year)
        {
            var data = GetDefaultSmallProducerData();

            service.WithSubmissionData(data);

            var result = await service.ServiceOfNoticeDetails(year);

            var model = result.Should().BeOfType<OrganisationDetailsTabsViewModel>().Subject;

            model.ServiceOfNoticeViewModel.Should().NotBeNull();

            var expectedyears = ExpectedYears(data);

            model.Years.Should().BeEquivalentTo(expectedyears);
            model.Year.Should().Be(year);
            model.ActiveOption.Should().Be(OrganisationDetailsDisplayOption.ServiceOfNoticeDetails);
            model.IsInternal.Should().Be(false);
            model.SmallProducerSubmissionData.Should().Be(data);

            A.CallTo(() => mapper
                            .Map<SubmissionsYearDetails, ServiceOfNoticeViewModel>(
                A<SubmissionsYearDetails>.That.Matches(x => x.Year == year && x.SmallProducerSubmissionData == data)))
                .MustHaveHappenedOnceExactly();
        }

        [Theory]
        [InlineData(null)]
        [InlineData(2004)]
        public async Task RepresentedOrganisationDetails_MapsAndReturns(int? year)
        {
            var data = GetDefaultSmallProducerData();

            service.WithSubmissionData(data);

            var result = await service.RepresentedOrganisationDetails(year);

            var model = result.Should().BeOfType<OrganisationDetailsTabsViewModel>().Subject;

            model.RepresentingCompanyDetailsViewModel.Should().NotBeNull();

            var expectedyears = ExpectedYears(data);

            model.Years.Should().BeEquivalentTo(expectedyears);
            model.Year.Should().Be(year);
            model.ActiveOption.Should().Be(OrganisationDetailsDisplayOption.RepresentedOrganisationDetails);
            model.IsInternal.Should().Be(false);
            model.SmallProducerSubmissionData.Should().Be(data);

            A.CallTo(() => mapper
                           .Map<SubmissionsYearDetails, RepresentingCompanyDetailsViewModel>(
               A<SubmissionsYearDetails>.That.Matches(x => x.Year == year && x.SmallProducerSubmissionData == data)))
               .MustHaveHappenedOnceExactly();
        }

        [Theory]
        [InlineData(null)]
        [InlineData(2004)]
        public async Task TotalEEEDetails_MapsAndReturns(int? year)
        {
            var data = GetDefaultSmallProducerData();

            service.WithSubmissionData(data);

            var result = await service.TotalEEEDetails(year);

            var model = result.Should().BeOfType<OrganisationDetailsTabsViewModel>().Subject;

            model.EditEeeDataViewModel.Should().NotBeNull();

            var expectedyears = ExpectedYears(data);

            model.Years.Should().BeEquivalentTo(expectedyears);
            model.Year.Should().Be(year);
            model.ActiveOption.Should().Be(OrganisationDetailsDisplayOption.TotalEEEDetails);
            model.IsInternal.Should().Be(false);
            model.SmallProducerSubmissionData.Should().Be(data);

            A.CallTo(() => mapper
                .Map<SubmissionsYearDetails, EditEeeDataViewModel>(
                A<SubmissionsYearDetails>.That.Matches(x => x.Year == year && x.SmallProducerSubmissionData == data)))
                .MustHaveHappenedOnceExactly();
        }

        [Theory]
        [InlineData("ContactDetails")]
        [InlineData("OrganisationDetails")]
        [InlineData("RepresentedOrganisationDetails")]
        [InlineData("ServiceOfNoticeDetails")]
        [InlineData("TotalEEEDetails")]
        public async Task SetsInternal(string method)
        {
            var data = GetDefaultSmallProducerData();

            service.WithSubmissionData(data, true);

            var methodInfo = typeof(SubmissionService).GetMethod(method, new[] { typeof(int?) });
            var task = (Task<OrganisationDetailsTabsViewModel>)methodInfo.Invoke(service, new object[] { null });

            var model = await task;

            model.IsInternal.Should().Be(true);
        }

        [Theory]
        [InlineData(null, "ContactDetails")]
        [InlineData(2024, "ContactDetails")]

        [InlineData(null, "OrganisationDetails")]
        [InlineData(2024, "OrganisationDetails")]

        [InlineData(null, "RepresentedOrganisationDetails")]
        [InlineData(2024, "RepresentedOrganisationDetails")]

        [InlineData(null, "ServiceOfNoticeDetails")]
        [InlineData(2024, "ServiceOfNoticeDetails")]

        [InlineData(null, "TotalEEEDetails")]
        [InlineData(2024, "TotalEEEDetails")]
        public async Task SetBreadcrumbs(int? year, string method)
        {
            var data = GetDefaultSmallProducerData();

            service.WithSubmissionData(data);

            var expected = "org name";

            A.CallTo(() => weeeCache
            .FetchOrganisationName(data.OrganisationData.Id))
                .Returns(expected);

            var methodInfo = typeof(SubmissionService).GetMethod(method, new[] { typeof(int?) });
            var task = (Task<OrganisationDetailsTabsViewModel>)methodInfo.Invoke(service, new object[] { year });

            var result = await task;

            if (year.HasValue)
            {
                Assert.Equal(breadcrumb.ExternalActivity, ProducerSubmissionConstant.HistoricProducerRegistrationSubmission);
            }
            else
            {
                Assert.Equal(breadcrumb.ExternalActivity, ProducerSubmissionConstant.ViewOrganisation);
            }

            Assert.Equal(breadcrumb.OrganisationId, data.OrganisationData.Id);
            Assert.Equal(breadcrumb.ExternalOrganisation, expected);
        }

        [Theory]
        [InlineData(null, "ContactDetails")]
        [InlineData(null, "OrganisationDetails")]
        [InlineData(null, "RepresentedOrganisationDetails")]
        [InlineData(null, "ServiceOfNoticeDetails")]
        [InlineData(null, "TotalEEEDetails")]
        public async Task SetBreadcrumbsInternal(int? year, string method)
        {
            var data = GetDefaultSmallProducerData();

            service.WithSubmissionData(data, true);

            var expected = "org name";

            A.CallTo(() => weeeCache
            .FetchOrganisationName(data.OrganisationData.Id))
                .Returns(expected);

            var methodInfo = typeof(SubmissionService).GetMethod(method, new[] { typeof(int?) });
            var task = (Task<OrganisationDetailsTabsViewModel>)methodInfo.Invoke(service, new object[] { year });

            var result = await task;

            Assert.Equal(InternalUserActivity.DirectRegistrantDetails, breadcrumb.InternalActivity);
        }

        [Theory]
        [InlineData(null, SubmissionStatus.Submitted, true)]
        [InlineData(2024, SubmissionStatus.Submitted, true)]
        [InlineData(2026, SubmissionStatus.InComplete, false)]
        [InlineData(2030, SubmissionStatus.Submitted, true)]
        public async Task OrganisationDetails_ReturnsExpectedModel(int? year, SubmissionStatus expectedStatus, bool expectedHasPaid)
        {
            // Arrange
            var data = GetDefaultSmallProducerData();
            service.WithSubmissionData(data);

            // Act
            var result = await service.OrganisationDetails(year);

            // Assert
            var model = result.Should().BeOfType<OrganisationDetailsTabsViewModel>().Subject;

            model.Year.Should().Be(year);
            model.Status.Should().Be(expectedStatus);
            model.HasPaid.Should().Be(expectedHasPaid);
        }

        [Theory]
        [InlineData(null, SubmissionStatus.Submitted, true)]
        [InlineData(2024, SubmissionStatus.Submitted, true)]
        [InlineData(2026, SubmissionStatus.InComplete, false)]
        [InlineData(2030, SubmissionStatus.Submitted, true)]
        public async Task ContactDetails_ReturnsExpectedModel(int? year, SubmissionStatus expectedStatus, bool expectedHasPaid)
        {
            // Arrange
            var data = GetDefaultSmallProducerData();
            service.WithSubmissionData(data);

            // Act
            var result = await service.ContactDetails(year);

            // Assert
            var model = result.Should().BeOfType<OrganisationDetailsTabsViewModel>().Subject;

            model.Year.Should().Be(year);
            model.Status.Should().Be(expectedStatus);
            model.HasPaid.Should().Be(expectedHasPaid);
        }

        [Theory]
        [InlineData(null, SubmissionStatus.Submitted, true)]
        [InlineData(2024, SubmissionStatus.Submitted, true)]
        [InlineData(2026, SubmissionStatus.InComplete, false)]
        [InlineData(2030, SubmissionStatus.Submitted, true)]
        public async Task ServiceOfNoticeDetails_ReturnsExpectedModel(int? year, SubmissionStatus expectedStatus, bool expectedHasPaid)
        {
            // Arrange
            var data = GetDefaultSmallProducerData();
            service.WithSubmissionData(data);

            // Act
            var result = await service.ServiceOfNoticeDetails(year);

            // Assert
            var model = result.Should().BeOfType<OrganisationDetailsTabsViewModel>().Subject;

            model.Year.Should().Be(year);
            model.Status.Should().Be(expectedStatus);
            model.HasPaid.Should().Be(expectedHasPaid);
        }

        [Theory]
        [InlineData(null, SubmissionStatus.Submitted, true)]
        [InlineData(2024, SubmissionStatus.Submitted, true)]
        [InlineData(2026, SubmissionStatus.InComplete, false)]
        [InlineData(2030, SubmissionStatus.Submitted, true)]
        public async Task TotalEEEDetails_ReturnsExpectedModel(int? year, SubmissionStatus expectedStatus, bool expectedHasPaid)
        {
            // Arrange
            var data = GetDefaultSmallProducerData();
            service.WithSubmissionData(data);

            // Act
            var result = await service.TotalEEEDetails(year);

            // Assert
            var model = result.Should().BeOfType<OrganisationDetailsTabsViewModel>().Subject;

            model.EditEeeDataViewModel.Should().NotBeNull();
            model.Year.Should().Be(year);
            model.Status.Should().Be(expectedStatus);
            model.HasPaid.Should().Be(expectedHasPaid);
        }

        [Theory]
        [InlineData(null, SubmissionStatus.Submitted, true)]
        [InlineData(2024, SubmissionStatus.Submitted, true)]
        [InlineData(2026, SubmissionStatus.InComplete, false)]
        [InlineData(2030, SubmissionStatus.Submitted, true)]
        public async Task RepresentedOrganisationDetails_ReturnsExpectedModel(int? year, SubmissionStatus expectedStatus, bool expectedHasPaid)
        {
            // Arrange
            var data = GetDefaultSmallProducerData();

            // Ensure the year exists in submission history if it's provided
            if (year.HasValue && !data.SubmissionHistory.ContainsKey(year.Value))
            {
                data.SubmissionHistory.Add(year.Value, TestFixture.Build<SmallProducerSubmissionHistoryData>()
                    .With(s => s.Status, expectedStatus)
                    .With(s => s.HasPaid, expectedHasPaid)
                    .Create());
            }

            service.WithSubmissionData(data);

            // Act
            var result = await service.RepresentedOrganisationDetails(year);

            // Assert
            var model = result.Should().BeOfType<OrganisationDetailsTabsViewModel>().Subject;

            model.RepresentingCompanyDetailsViewModel.Should().NotBeNull();
            model.Year.Should().Be(year);
            model.Status.Should().Be(expectedStatus);
            model.HasPaid.Should().Be(expectedHasPaid);
        }

        [Theory]
        [InlineData(SubmissionStatus.Submitted)]
        [InlineData(SubmissionStatus.Returned)]
        public async Task YearsDropdownData_IncludesCorrectStatuses(SubmissionStatus status)
        {
            // Arrange
            var data = GetDefaultSmallProducerData();
            data.SubmissionHistory.Add(2025, new SmallProducerSubmissionHistoryData
            {
                Status = status
            });

            service.WithSubmissionData(data);

            // Act
            var result = await service.OrganisationDetails(null);

            // Assert
            result.Years.Should().Contain(2025);
        }

        [Fact]
        public async Task YearsDropdownData_ExcludesIncompleteStatus()
        {
            // Arrange
            var data = GetDefaultSmallProducerData();
            data.SubmissionHistory.Add(2025, new SmallProducerSubmissionHistoryData
            {
                Status = SubmissionStatus.InComplete
            });

            service.WithSubmissionData(data);

            // Act
            var result = await service.OrganisationDetails(null);

            // Assert
            result.Years.Should().NotContain(2025);
        }

        [Theory]
        [InlineData(null)]
        [InlineData(2024)]
        public async Task GetSubmissionTabModel_SetsStatusAndPayment(int? year)
        {
            // Arrange
            var data = GetDefaultSmallProducerData();
            var expectedStatus = SubmissionStatus.Submitted;
            var expectedHasPaid = true;

            if (year.HasValue)
            {
                data.SubmissionHistory[year.Value].Status = expectedStatus;
                data.SubmissionHistory[year.Value].HasPaid = expectedHasPaid;
            }
            else
            {
                data.CurrentSubmission.Status = expectedStatus;
                data.CurrentSubmission.HasPaid = expectedHasPaid;
            }

            service.WithSubmissionData(data);

            // Act
            var result = await service.OrganisationDetails(year);

            // Assert
            result.Status.Should().Be(expectedStatus);
            result.HasPaid.Should().Be(expectedHasPaid);
        }

        private static IEnumerable<int> ExpectedYears(SmallProducerSubmissionData d) => 
             d.SubmissionHistory
              .Where(x => x.Value.Status == SubmissionStatus.Submitted)
              .OrderByDescending(x => x.Key)
              .Select(x => x.Key);

        private Core.DirectRegistrant.SmallProducerSubmissionData GetDefaultSmallProducerData()
        {
            var result = new Core.DirectRegistrant.SmallProducerSubmissionData
            {
                SubmissionHistory = new Dictionary<int, SmallProducerSubmissionHistoryData>()
                {
                    { 2004, TestFixture.Build<SmallProducerSubmissionHistoryData>()
                        .With(s => s.Status, SubmissionStatus.Submitted)
                        .With(s => s.HasPaid, true)
                        .Create() },
                    { 2024, TestFixture.Build<SmallProducerSubmissionHistoryData>()
                        .With(s => s.Status, SubmissionStatus.Submitted)
                        .With(s => s.HasPaid, true)
                        .Create() },
                    { 2026, TestFixture.Build<SmallProducerSubmissionHistoryData>()
                        .With(s => s.Status, SubmissionStatus.InComplete)
                        .With(s => s.HasPaid, false)
                        .Create() },
                    { 2030, TestFixture.Build<SmallProducerSubmissionHistoryData>()
                        .With(s => s.Status, SubmissionStatus.Submitted)
                        .With(s => s.HasPaid, true)
                        .Create() }
                },
                CurrentSubmission = new Core.DirectRegistrant.SmallProducerSubmissionHistoryData
                {
                    ComplianceYear = 2005,
                    OrganisationDetailsComplete = true,
                    ContactDetailsComplete = true,
                    ServiceOfNoticeComplete = true,
                    RepresentingCompanyDetailsComplete = true,
                    EEEDetailsComplete = true,
                    Status = SubmissionStatus.Submitted,
                    HasPaid = true,
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
                AuthorisedRepresentitiveData = TestFixture.Create<AuthorisedRepresentitiveData>(),
                OrganisationData = new OrganisationData
                {
                    Id = Guid.NewGuid(),
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
                }
            };

            return result;
        }
    }
}