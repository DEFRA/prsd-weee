namespace EA.Weee.Web.Tests.Unit.Areas.AatfEvidence.Attributes
{
    using Api.Client;
    using EA.Weee.Web.Areas.AatfEvidence.Attributes;
    using FakeItEasy;
    using FluentAssertions;
    using Services;
    using System;
    using System.Web.Mvc;
    using Xunit;

    public class ValidateReturnBaseActionFilterAttributeTests
    {
        private readonly ValidateReturnActionFilterAttribute attribute;
        private readonly ActionExecutingContext context;
        private readonly IWeeeClient client;

        public ValidateReturnBaseActionFilterAttributeTests()
        {
            client = A.Fake<IWeeeClient>();
            attribute = new ValidateReturnActionFilterAttribute { ConfigService = A.Fake<ConfigurationService>(), Client = () => client };
            context = A.Fake<ActionExecutingContext>();

            A.CallTo(() => attribute.ConfigService.CurrentConfiguration.EnableAATFEvidenceNotes).Returns(true);
        }

        [Fact]
        public void OnActionExecuting_GivenEnableAatfEvidenceNotesIsFalse_InvalidOperationExceptionExpected()
        {
            A.CallTo(() => attribute.ConfigService.CurrentConfiguration.EnableAATFEvidenceNotes).Returns(false);

            Action action = () => attribute.OnActionExecuting(context);

            action.Should().Throw<InvalidOperationException>().WithMessage("AATF evidence notes are not enabled.");
        }

        [Fact]
        public void OnActionExecuting_GivenEnableAatfEvidenceIsTrue_NoInvalidOperationExceptionExpected()
        {
            A.CallTo(() => attribute.ConfigService.CurrentConfiguration.EnableAATFEvidenceNotes).Returns(true);

            Action action = () => attribute.OnActionExecuting(context);

            action.Should().NotThrow<InvalidOperationException>();
        }
    }
}
