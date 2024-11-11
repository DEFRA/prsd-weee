namespace EA.Weee.Web.Tests.Unit.Areas.Admin.Attributes
{
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Api.Client;
    using EA.Weee.Core.DirectRegistrant;
    using EA.Weee.Core.Helpers;
    using EA.Weee.Core.Organisations;
    using EA.Weee.Core.Shared;
    using EA.Weee.Requests.Admin.DirectRegistrants;
    using EA.Weee.Web.Areas.Admin.Controllers;
    using EA.Weee.Web.Areas.Admin.Filters;
    using EA.Weee.Web.Infrastructure;
    using EA.Weee.Web.Infrastructure.PDF;
    using EA.Weee.Web.Services;
    using EA.Weee.Web.Services.Caching;
    using EA.Weee.Web.Services.SubmissionService;
    using FakeItEasy;
    using FluentAssertions;
    using System;
    using System.Collections.Generic;
    using System.Security.Principal;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.Routing;
    using Xunit;

    public class AdminSmallProducerSubmissionContextAttributeTests
    {
        private readonly IWeeeClient client;
        private readonly HttpContextBase httpContext;
        private readonly IPrincipal principal;
        private readonly ActionExecutingContext actionContext;
        private readonly ProducerSubmissionController controller;
        private readonly RequestContext requestContext;
        private readonly HttpRequestBase request;
        private readonly IMvcTemplateExecutor templateExecutor;
        private readonly IMapper mapper;
        private readonly IPdfDocumentProvider pdfDocumentProvider;

        public AdminSmallProducerSubmissionContextAttributeTests()
        {
            client = A.Fake<IWeeeClient>();
            httpContext = A.Fake<HttpContextBase>();
            principal = A.Fake<IPrincipal>();
            request = A.Fake<HttpRequestBase>();
            templateExecutor = A.Fake<IMvcTemplateExecutor>();
            mapper = A.Fake<IMapper>();
            pdfDocumentProvider = A.Fake<IPdfDocumentProvider>();
            controller = new ProducerSubmissionController(
                A.Fake<IWeeeClient>,
                A.Fake<IWeeeCache>(),
                A.Fake<ISubmissionService>(),
                A.Fake<BreadcrumbService>(),
                A.Fake<IMvcTemplateExecutor>(),
                A.Fake<IMapper>(),
                A.Fake<IPdfDocumentProvider>());

            requestContext = new RequestContext(httpContext, new RouteData());
            controller.ControllerContext = new ControllerContext(requestContext, controller);

            actionContext = new ActionExecutingContext(
                controller.ControllerContext,
                A.Fake<ActionDescriptor>(),
                new RouteValueDictionary());

            A.CallTo(() => httpContext.Request).Returns(request);
            A.CallTo(() => httpContext.User).Returns(principal);
        }

        [Fact]
        public void OnActionExecuting_GivenValidSubmissionData_SetsControllerData()
        {
            // Arrange
            var attribute = new AdminSmallProducerSubmissionContextAttribute { Client = () => client };
            SetupQueryString("REG123");

            var submissionHistory = new Dictionary<int, SmallProducerSubmissionHistoryData>
            {
                {
                    2024,
                    new SmallProducerSubmissionHistoryData
                    {
                        ComplianceYear = 2024,
                        Status = SubmissionStatus.Submitted
                    }
                }
            };

            var submissionData = CreateSubmissionData(Guid.NewGuid(), submissionHistory);
            SetupClientResponse(submissionData);

            // Act
            attribute.OnActionExecuting(actionContext);

            // Assert
            controller.SmallProducerSubmissionData.Should().BeEquivalentTo(submissionData);
            actionContext.Result.Should().BeNull();
        }

        [Fact]
        public void OnActionExecuting_VerifiesClientIsCalledWithCorrectRegistrationNumber()
        {
            // Arrange
            var attribute = new AdminSmallProducerSubmissionContextAttribute { Client = () => client };
            const string registrationNumber = "REG123";
            SetupQueryString(registrationNumber);
            SetupClientResponse(CreateSubmissionData(Guid.NewGuid(), new Dictionary<int, SmallProducerSubmissionHistoryData>()));

            // Act
            attribute.OnActionExecuting(actionContext);

            // Assert
            A.CallTo(() => client.SendAsync(
                A<string>.Ignored,
                A<GetSmallProducerSubmissionByRegistrationNumber>.That.Matches(
                    x => x.RegistrationNumber == registrationNumber)))
                .MustHaveHappenedOnceExactly();
        }

        private static SmallProducerSubmissionData CreateSubmissionData(Guid organisationId, IDictionary<int, SmallProducerSubmissionHistoryData> submissionHistory)
        {
            return new SmallProducerSubmissionData
            {
                OrganisationData = new OrganisationData { Id = organisationId },
                ContactData = new ContactData { FirstName = "Test", LastName = "User" },
                ContactAddressData = new AddressData(),
                SubmissionHistory = submissionHistory,
                DirectRegistrantId = Guid.NewGuid(),
                HasAuthorisedRepresentitive = false,
                AuthorisedRepresentitiveData = null,
                ServiceOfNoticeData = new AddressData(),
                ProducerRegistrationNumber = "TEST1234"
            };
        }

        private void SetupQueryString(string registrationNumber)
        {
            var queryString = new System.Collections.Specialized.NameValueCollection();
            if (!string.IsNullOrWhiteSpace(registrationNumber))
            {
                queryString.Add("RegistrationNumber", registrationNumber);
            }

            A.CallTo(() => request.QueryString).Returns(queryString);
        }

        private void SetupClientResponse(SmallProducerSubmissionData response)
        {
            A.CallTo(() => client.SendAsync(
                    A<string>.Ignored,
                    A<GetSmallProducerSubmissionByRegistrationNumber>.Ignored))
                .Returns(response);
        }
    }
}