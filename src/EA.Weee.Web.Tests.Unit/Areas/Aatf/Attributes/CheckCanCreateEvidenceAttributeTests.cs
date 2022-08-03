namespace EA.Weee.Web.Tests.Unit.Areas.Aatf.Attributes
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;
    using Api.Client;
    using AutoFixture;
    using Constant;
    using Core.AatfReturn;
    using FakeItEasy;
    using FluentAssertions;
    using Services;
    using Web.Areas.Aatf.Attributes;
    using Web.Areas.Aatf.Helpers;
    using Weee.Requests.AatfReturn;
    using Weee.Tests.Core;
    using Xunit;

    public class CheckCanCreateEvidenceAttributeTests : SimpleUnitTestBase
    {
        private readonly CheckCanCreateEvidenceNoteAttribute attribute;
        private readonly ActionExecutingContext context;
        private readonly IWeeeClient client;
        private readonly IAatfEvidenceHelper aatfEvidenceHelper;

        private readonly Guid organisationId;
        private readonly Guid aatfId;
        private readonly int complianceYear;

        public CheckCanCreateEvidenceAttributeTests()
        {
            client = A.Fake<IWeeeClient>();
            aatfEvidenceHelper = A.Fake<IAatfEvidenceHelper>();

            attribute = new CheckCanCreateEvidenceNoteAttribute { Client = () => client, AatfEvidenceHelper = aatfEvidenceHelper };
            context = A.Fake<ActionExecutingContext>();

            organisationId = TestFixture.Create<Guid>();
            aatfId = TestFixture.Create<Guid>();
            complianceYear = TestFixture.Create<int>();

            var actionParameters = new Dictionary<string, object> { { "organisationId", organisationId }, { "aatfId", aatfId }, { "complianceYear", complianceYear } };
            A.CallTo(() => context.ActionParameters).Returns(actionParameters);
        }

        [Fact]
        public void OnActionExecuting_GivenNoAatfId_ArgumentExceptionExpected()
        {
            //arrange
            var actionParameters = new Dictionary<string, object> { { "organisationId", organisationId }, { "complianceYear", complianceYear } };
            A.CallTo(() => context.ActionParameters).Returns(actionParameters);

            //act
            Action action = () => attribute.OnActionExecuting(context);

            //assert
            action.Should().Throw<ArgumentException>().WithMessage("No aatf ID was specified.");
        }

        [Fact]
        public void OnActionExecuting_GivenNoOrganisationId_ArgumentExceptionExpected()
        {
            //arrange
            var actionParameters = new Dictionary<string, object> { { "aatfId", aatfId }, { "complianceYear", complianceYear } };
            A.CallTo(() => context.ActionParameters).Returns(actionParameters);

            //act
            Action action = () => attribute.OnActionExecuting(context);

            //assert
            action.Should().Throw<ArgumentException>().WithMessage("No organisation ID was specified.");
        }

        [Fact]
        public void OnActionExecuting_GivenOrganisationIdIsNotGuid_ArgumentExceptionExpected()
        {
            //arrange
            var actionParameters = new Dictionary<string, object> { { "organisationId", 1 }, { "aatfId", aatfId } };
            A.CallTo(() => context.ActionParameters).Returns(actionParameters);

            //act
            Action action = () => attribute.OnActionExecuting(context);

            //assert
            action.Should().Throw<ArgumentException>().WithMessage("The specified organisation ID is not valid.");
        }

        [Fact]
        public void OnActionExecuting_GivenAatfIdIsNotGuid_ArgumentExceptionExpected()
        {
            //arrange
            var actionParameters = new Dictionary<string, object> { { "organisationId", organisationId }, { "aatfId", 1 } };
            A.CallTo(() => context.ActionParameters).Returns(actionParameters);
            
            //act
            Action action = () => attribute.OnActionExecuting(context);

            //assert
            action.Should().Throw<ArgumentException>().WithMessage("The specified aatf ID is not valid.");
        }

        [Fact]
        public void OnActionExecuting_GivenNoComplianceYear_ArgumentExceptionExpected()
        {
            //arrange
            var actionParameters = new Dictionary<string, object> { { "aatfId", aatfId }, { "organisationId", organisationId } };
            A.CallTo(() => context.ActionParameters).Returns(actionParameters);

            //act
            Action action = () => attribute.OnActionExecuting(context);

            //assert
            action.Should().Throw<ArgumentException>().WithMessage("No compliance year was specified.");
        }

        [Fact]
        public void OnActionExecuting_GivenComplianceYearIsNotValid_ArgumentExceptionExpected()
        {
            //arrange
            var actionParameters = new Dictionary<string, object> { { "aatfId", aatfId }, { "organisationId", organisationId }, { "complianceYear", "AA" } };
            A.CallTo(() => context.ActionParameters).Returns(actionParameters);

            //act
            Action action = () => attribute.OnActionExecuting(context);

            //assert
            action.Should().Throw<ArgumentException>().WithMessage("The specified compliance year is not valid.");
        }

        [Fact]
        public void OnActionExecuting_GivenAatfCannotCreateEditNotes_ExceptionShouldBeThrown()
        {
            //arrange
            var aatfs = TestFixture.CreateMany<AatfData>().ToList();

            A.CallTo(() => client.SendAsync(A<string>._,
                    A<GetAatfByOrganisation>.That.Matches(r => r.OrganisationId == organisationId))).Returns(aatfs);

            A.CallTo(() => aatfEvidenceHelper.AatfCanEditCreateNotes(aatfs, aatfId, complianceYear)).Returns(false);

            //act
            var result = Record.Exception(() => attribute.OnActionExecuting(context));

            //assert
            result.Should().BeOfType<AggregateException>();
            result.InnerException.Should().BeOfType<InvalidOperationException>()
                .Which.Message.Should()
                .Be($"Evidence for organisation ID {organisationId} with aatf ID {aatfId} cannot be created");
        }

        [Fact]
        public void OnActionExecuting_GivenAatfCanCreateEditNotes_NoExceptionShouldBeThrown()
        {
            //arrange
            var aatfs = TestFixture.CreateMany<AatfData>().ToList();

            A.CallTo(() => client.SendAsync(A<string>._,
                A<GetAatfByOrganisation>.That.Matches(r => r.OrganisationId == organisationId))).Returns(aatfs);

            A.CallTo(() => aatfEvidenceHelper.AatfCanEditCreateNotes(aatfs, aatfId, complianceYear)).Returns(true);

            //act
            var result = Record.Exception(() => attribute.OnActionExecuting(context));

            //assert
            result.Should().BeNull();
        }
    }
}
