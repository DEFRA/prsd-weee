namespace EA.Weee.Web.Tests.Unit.Areas.Admin.Attributes
{
    using EA.Weee.Api.Client;
    using EA.Weee.Core.Scheme;
    using EA.Weee.Requests.Scheme;
    using EA.Weee.Web.Areas.Admin.Controllers.Attributes;
    using FakeItEasy;
    using FluentAssertions;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using System.Web.Routing;
    using Xunit;

    public class ValidateOrganisationHasNoSchemeFilterAttributeTests
    {
        private readonly ValidateOrganisationHasNoSchemeFilterAttribute attribute;
        private readonly ActionExecutingContext context;
        private readonly IWeeeClient client;

        public ValidateOrganisationHasNoSchemeFilterAttributeTests()
        {
            client = A.Fake<IWeeeClient>();
            attribute = new ValidateOrganisationHasNoSchemeFilterAttribute { Client = () => client };
            context = A.Fake<ActionExecutingContext>();

            IDictionary<string, object> actionParameters = new Dictionary<string, object>();
            actionParameters.Add("organisationId", Guid.NewGuid());
            A.CallTo(() => context.ActionParameters).Returns(actionParameters);
        }

        [Fact]
        public async Task OnActionExecuting_GivenOrganisationHasAScheme_ShouldBeRedirectedToManageSchemes()
        {
            SchemeData schemeData = new SchemeData();

            Guid organisationId = (Guid)context.ActionParameters["organisationId"];

            A.CallTo(() => client.SendAsync(A<string>._,
                A<GetSchemeByOrganisationId>.That.Matches(r => r.OrganisationId.Equals(organisationId)))).Returns(schemeData);

            await attribute.OnAuthorizationAsync(context, organisationId);

            RedirectResult result = context.Result as RedirectResult;

            result.Url.Should().Be("~/admin/scheme/manage-schemes");
        }

        [Fact]
        public async Task OnActionExecuting_GivenOrganisationHasNoScheme_ContextResultShouldBeNull()
        {
            SchemeData schemeData = null;

            Guid organisationId = (Guid)context.ActionParameters["organisationId"];

            A.CallTo(() => client.SendAsync(A<string>._,
                A<GetSchemeByOrganisationId>.That.Matches(r => r.OrganisationId.Equals(organisationId)))).Returns(schemeData);

            await attribute.OnAuthorizationAsync(context, organisationId);

            context.Result.Should().BeNull();
        }

        [Fact]
        public void ValidateOrganisationHasNoSchemeFilterAttribute_ShouldInheritFromValidateOrganisationSchemeBaseActionFilterAttribute()
        {
            typeof(ValidateOrganisationHasNoSchemeFilterAttribute).BaseType.Name.Should().Be(typeof(ValidateOrganisationSchemeBaseActionFilterAttribute).Name);
        }
    }
}
