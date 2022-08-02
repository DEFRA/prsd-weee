namespace EA.Weee.Web.Tests.Unit.Areas.Aatf.Attributes
{
    using AutoFixture;
    using Core.Scheme;
    using EA.Weee.Web.Areas.Scheme.Attributes;
    using FakeItEasy;
    using FluentAssertions;
    using Services;
    using Services.Caching;
    using System;
    using System.Web.Mvc;
    using Web.Areas.Aatf.Attributes;
    using Weee.Tests.Core;
    using Xunit;

    public class ValidateEvidenceEnabledAttributeTests : SimpleUnitTestBase
    {
        private readonly ValidateAatfEvidenceEnabledAttribute aatfAttribute;
        private readonly ValidateSchemeEvidenceEnabledAttribute schemeAttribute;
        private readonly ValidatePcsObligationsEnabledAttribute obligationsAttribute;
        private readonly ActionExecutingContext context;
        private readonly IWeeeCache cache;

        public ValidateEvidenceEnabledAttributeTests()
        {
            cache = A.Fake<IWeeeCache>();
            aatfAttribute = new ValidateAatfEvidenceEnabledAttribute { ConfigService = A.Fake<ConfigurationService>() };
            schemeAttribute = new ValidateSchemeEvidenceEnabledAttribute { ConfigService = A.Fake<ConfigurationService>(), Cache = cache };
            obligationsAttribute = new ValidatePcsObligationsEnabledAttribute { ConfigService = A.Fake<ConfigurationService>() };

            context = A.Fake<ActionExecutingContext>();

            A.CallTo(() => schemeAttribute.ConfigService.CurrentConfiguration.EnablePCSEvidenceNotes).Returns(true);
            A.CallTo(() => aatfAttribute.ConfigService.CurrentConfiguration.EnableAATFEvidenceNotes).Returns(true);
            A.CallTo(() => obligationsAttribute.ConfigService.CurrentConfiguration.EnablePCSObligations).Returns(true);
            A.CallTo(() => schemeAttribute.ConfigService.CurrentConfiguration.EnablePBSEvidenceNotes).Returns(true);
        }

        [Fact]
        public void OnActionExecuting_GivenEnableAatfEvidenceNotesIsFalse_InvalidOperationExceptionExpected()
        {
            //arrange
            A.CallTo(() => aatfAttribute.ConfigService.CurrentConfiguration.EnableAATFEvidenceNotes).Returns(false);

            //act
            Action action = () => aatfAttribute.OnActionExecuting(context);

            //assert
            action.Should().Throw<InvalidOperationException>().WithMessage("AATF evidence notes are not enabled.");
        }

        [Fact]
        public void OnActionExecuting_GivenEnableAatfEvidenceNotesIsTrue_NoInvalidOperationExceptionExpected()
        {
            //arrange
            A.CallTo(() => aatfAttribute.ConfigService.CurrentConfiguration.EnableAATFEvidenceNotes).Returns(true);

            //act
            Action action = () => aatfAttribute.OnActionExecuting(context);

            //assert
            action.Should().NotThrow<InvalidOperationException>();
        }

        [Fact]
        public void OnActionExecuting_GivenNoPcsRouteParameter_ArgumentExceptionExpected()
        {
            //act
            Action action = () => schemeAttribute.OnActionExecuting(context);

            //assert
            action.Should().Throw<ArgumentException>().WithMessage("No pcsId specified");
        }

        [Fact]
        public void OnActionExecuting_GivenPcsRouteParameterIsNotGuid_ArgumentExceptionExpected()
        {
            //arrange
            context.RouteData.Values.Add("pcsId", 1);
            var schemeInfo = TestFixture.Build<SchemePublicInfo>().With(s => s.IsBalancingScheme, false).Create();

            A.CallTo(() => schemeAttribute.ConfigService.CurrentConfiguration.EnablePCSEvidenceNotes).Returns(false);

            //act
            Action action = () => schemeAttribute.OnActionExecuting(context);

            //assert
            action.Should().Throw<ArgumentException>().WithMessage("The specified pcsId is not valid");
        }

        [Fact]
        public void OnActionExecuting_GivenEnablePcsEvidenceNotesIsFalseAndSchemeIsNotBalancingScheme_InvalidOperationExceptionExpected()
        {
            //arrange
            var organisationId = TestFixture.Create<Guid>();
            context.RouteData.Values.Add("pcsId", organisationId);
            var schemeInfo = TestFixture.Build<SchemePublicInfo>().With(s => s.IsBalancingScheme, false).Create();

            A.CallTo(() => cache.FetchSchemePublicInfo(organisationId)).Returns(schemeInfo);
            A.CallTo(() => schemeAttribute.ConfigService.CurrentConfiguration.EnablePCSEvidenceNotes).Returns(false);

            //act
            Action action = () => schemeAttribute.OnActionExecuting(context);

            //assert
            action.Should().Throw<InvalidOperationException>().WithMessage("PCS evidence notes are not enabled.");
        }

        [Fact]
        public void OnActionExecuting_GivenEnablePcsEvidenceNotesIsFalseAndSchemeIsBalancingScheme_NoInvalidOperationExceptionExpected()
        {
            //arrange
            var organisationId = TestFixture.Create<Guid>();
            context.RouteData.Values.Add("pcsId", organisationId);
            var schemeInfo = TestFixture.Build<SchemePublicInfo>().With(s => s.IsBalancingScheme, true).Create();

            A.CallTo(() => cache.FetchSchemePublicInfo(organisationId)).Returns(schemeInfo);
            A.CallTo(() => schemeAttribute.ConfigService.CurrentConfiguration.EnablePCSEvidenceNotes).Returns(false);

            //act
            Action action = () => schemeAttribute.OnActionExecuting(context);

            //assert
            action.Should().NotThrow<InvalidOperationException>();
        }

        [Fact]
        public void OnActionExecuting_GivenEnablePcsEvidenceNotesIsTrueAndSchemeIsNotBalancingScheme_NoInvalidOperationExceptionExpected()
        {
            //arrange
            var organisationId = TestFixture.Create<Guid>();
            context.RouteData.Values.Add("pcsId", organisationId);
            var schemeInfo = TestFixture.Build<SchemePublicInfo>().With(s => s.IsBalancingScheme, false).Create();

            A.CallTo(() => cache.FetchSchemePublicInfo(organisationId)).Returns(schemeInfo);
            A.CallTo(() => schemeAttribute.ConfigService.CurrentConfiguration.EnablePCSEvidenceNotes).Returns(true);

            //act
            Action action = () => schemeAttribute.OnActionExecuting(context);

            //assert
            action.Should().NotThrow<InvalidOperationException>();
        }

        [Fact]
        public void OnActionExecuting_GivenEnablePbsEvidenceNotesIsFalseAndSchemeIsNotBalancingScheme_NoInvalidOperationExceptionExpected()
        {
            //arrange
            var organisationId = TestFixture.Create<Guid>();
            context.RouteData.Values.Add("pcsId", organisationId);
            var schemeInfo = TestFixture.Build<SchemePublicInfo>().With(s => s.IsBalancingScheme, false).Create();

            A.CallTo(() => cache.FetchSchemePublicInfo(organisationId)).Returns(schemeInfo);
            A.CallTo(() => schemeAttribute.ConfigService.CurrentConfiguration.EnablePBSEvidenceNotes).Returns(false);

            //act
            Action action = () => schemeAttribute.OnActionExecuting(context);

            //assert
            action.Should().NotThrow<InvalidOperationException>();
        }

        [Fact]
        public void OnActionExecuting_GivenEnablePbsEvidenceNotesIsFalseAndSchemeIsBalancingScheme_InvalidOperationExceptionExpected()
        {
            //arrange
            var organisationId = TestFixture.Create<Guid>();
            context.RouteData.Values.Add("pcsId", organisationId);
            var schemeInfo = TestFixture.Build<SchemePublicInfo>().With(s => s.IsBalancingScheme, true).Create();

            A.CallTo(() => cache.FetchSchemePublicInfo(organisationId)).Returns(schemeInfo);
            A.CallTo(() => schemeAttribute.ConfigService.CurrentConfiguration.EnablePBSEvidenceNotes).Returns(false);

            //act
            Action action = () => schemeAttribute.OnActionExecuting(context);

            //assert
            action.Should().Throw<InvalidOperationException>().WithMessage("PBS evidence notes are not enabled.");
        }

        [Fact]
        public void OnActionExecuting_GivenEnablePbsEvidenceNotesIsTrueAndSchemeIsNotBalancingScheme_NoInvalidOperationExceptionExpected()
        {
            //arrange
            var organisationId = TestFixture.Create<Guid>();
            context.RouteData.Values.Add("pcsId", organisationId);
            var schemeInfo = TestFixture.Build<SchemePublicInfo>().With(s => s.IsBalancingScheme, true).Create();

            A.CallTo(() => cache.FetchSchemePublicInfo(organisationId)).Returns(schemeInfo);
            A.CallTo(() => schemeAttribute.ConfigService.CurrentConfiguration.EnablePBSEvidenceNotes).Returns(true);

            //act
            Action action = () => schemeAttribute.OnActionExecuting(context);

            //assert
            action.Should().NotThrow<InvalidOperationException>();
        }

        [Fact]
        public void OnActionExecuting_GivenEnablePcsObligationsIsFalse_InvalidOperationExceptionExpected()
        {
            //arrange
            A.CallTo(() => obligationsAttribute.ConfigService.CurrentConfiguration.EnablePCSObligations).Returns(false);

            //act
            Action action = () => obligationsAttribute.OnActionExecuting(context);

            //assert
            action.Should().Throw<InvalidOperationException>().WithMessage("Manage PCS obligations is not enabled.");
        }

        [Fact]
        public void OnActionExecuting_GivenEnablePcsObligationsIsTrue_NoInvalidOperationExceptionExpected()
        {
            //arrange
            A.CallTo(() => obligationsAttribute.ConfigService.CurrentConfiguration.EnablePCSObligations).Returns(true);

            //act
            Action action = () => obligationsAttribute.OnActionExecuting(context);

            //assert
            action.Should().NotThrow<InvalidOperationException>();
        }
    }
}
