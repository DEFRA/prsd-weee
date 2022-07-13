namespace EA.Weee.Web.Tests.Unit.Areas.Aatf.Attributes
{
    using System;
    using System.Web.Mvc;
    using Api.Client;
    using EA.Weee.Web.Areas.Scheme.Attributes;
    using FakeItEasy;
    using FluentAssertions;
    using Services;
    using Web.Areas.Aatf.Attributes;
    using Xunit;

    public class ValidateEvidenceEnabledAttributeTests
    {
        private readonly ValidateAatfEvidenceEnabledAttribute aatfAttribute;
        private readonly ValidatePcsEvidenceEnabledAttribute pcsAttribute;
        private readonly ValidatePcsObligationsEnabledAttribute obligationsAttribute;
        private readonly ValidatePBSEvidenceNotesEnabledAttribute pbsAttribute;
        private readonly ActionExecutingContext context;
        private readonly IWeeeClient client;

        public ValidateEvidenceEnabledAttributeTests()
        {
            client = A.Fake<IWeeeClient>();
            aatfAttribute = new ValidateAatfEvidenceEnabledAttribute { ConfigService = A.Fake<ConfigurationService>(), Client = () => client };
            pcsAttribute = new ValidatePcsEvidenceEnabledAttribute { ConfigService = A.Fake<ConfigurationService>(), Client = () => client };
            obligationsAttribute = new ValidatePcsObligationsEnabledAttribute { ConfigService = A.Fake<ConfigurationService>(), Client = () => client };
            pbsAttribute = new ValidatePBSEvidenceNotesEnabledAttribute { ConfigService = A.Fake<ConfigurationService>(), Client = () => client };
            context = A.Fake<ActionExecutingContext>();

            A.CallTo(() => pcsAttribute.ConfigService.CurrentConfiguration.EnablePCSEvidenceNotes).Returns(true);
            A.CallTo(() => aatfAttribute.ConfigService.CurrentConfiguration.EnableAATFEvidenceNotes).Returns(true);
            A.CallTo(() => obligationsAttribute.ConfigService.CurrentConfiguration.EnablePCSObligations).Returns(true);
            A.CallTo(() => pbsAttribute.ConfigService.CurrentConfiguration.EnablePBSEvidenceNotes).Returns(true);
        }

        [Fact]
        public void OnActionExecuting_GivenEnableAatfEvidenceNotesIsFalse_InvalidOperationExceptionExpected()
        {
            A.CallTo(() => aatfAttribute.ConfigService.CurrentConfiguration.EnableAATFEvidenceNotes).Returns(false);

            Action action = () => aatfAttribute.OnActionExecuting(context);

            action.Should().Throw<InvalidOperationException>().WithMessage("AATF evidence notes are not enabled.");
        }

        [Fact]
        public void OnActionExecuting_GivenEnableAatfEvidenceNotesIsTrue_NoInvalidOperationExceptionExpected()
        {
            A.CallTo(() => aatfAttribute.ConfigService.CurrentConfiguration.EnableAATFEvidenceNotes).Returns(true);

            Action action = () => aatfAttribute.OnActionExecuting(context);

            action.Should().NotThrow<InvalidOperationException>();
        }

        [Fact]
        public void OnActionExecuting_GivenEnablePcsEvidenceNotesIsFalse_InvalidOperationExceptionExpected()
        {
            A.CallTo(() => pcsAttribute.ConfigService.CurrentConfiguration.EnablePCSEvidenceNotes).Returns(false);

            Action action = () => pcsAttribute.OnActionExecuting(context);

            action.Should().Throw<InvalidOperationException>().WithMessage("PCS evidence notes are not enabled.");
        }

        [Fact]
        public void OnActionExecuting_GivenEnablePcsEvidenceNotesIsTrue_NoInvalidOperationExceptionExpected()
        {
            A.CallTo(() => pcsAttribute.ConfigService.CurrentConfiguration.EnablePCSEvidenceNotes).Returns(true);

            Action action = () => pcsAttribute.OnActionExecuting(context);

            action.Should().NotThrow<InvalidOperationException>();
        }

        [Fact]
        public void OnActionExecuting_GivenEnablePcsObligationsIsFalse_InvalidOperationExceptionExpected()
        {
            A.CallTo(() => obligationsAttribute.ConfigService.CurrentConfiguration.EnablePCSObligations).Returns(false);

            Action action = () => obligationsAttribute.OnActionExecuting(context);

            action.Should().Throw<InvalidOperationException>().WithMessage("Manage PCS obligations is not enabled.");
        }

        [Fact]
        public void OnActionExecuting_GivenEnablePcsObligationssIsTrue_NoInvalidOperationExceptionExpected()
        {
            A.CallTo(() => obligationsAttribute.ConfigService.CurrentConfiguration.EnablePCSObligations).Returns(true);

            Action action = () => obligationsAttribute.OnActionExecuting(context);

            action.Should().NotThrow<InvalidOperationException>();
        }

        [Fact]
        public void OnActionExecuting_GivenEnablePBSEvidenceNotesIsFalse_InvalidOperationExceptionExpected()
        {
            A.CallTo(() => pbsAttribute.ConfigService.CurrentConfiguration.EnablePBSEvidenceNotes).Returns(false);

            Action action = () => pbsAttribute.OnActionExecuting(context);

            action.Should().Throw<InvalidOperationException>().WithMessage("Manage PBS evidence notes is not enabled.");
        }

        [Fact]
        public void OnActionExecuting_GivenEnablePBSEvidenceNotesIsTrue_NoInvalidOperationExceptionExpected()
        {
            A.CallTo(() => pbsAttribute.ConfigService.CurrentConfiguration.EnablePBSEvidenceNotes).Returns(true);

            Action action = () => pbsAttribute.OnActionExecuting(context);

            action.Should().NotThrow<InvalidOperationException>();
        }
    }
}
