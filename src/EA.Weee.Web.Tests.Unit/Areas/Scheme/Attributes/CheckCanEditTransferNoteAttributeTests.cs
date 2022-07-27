namespace EA.Weee.Web.Tests.Unit.Areas.Scheme.Attributes
{
    using System;
    using System.Collections.Generic;
    using System.Web.Mvc;
    using System.Web.Routing;
    using AutoFixture;
    using Core.AatfEvidence;
    using Core.Scheme;
    using Core.Shared;
    using EA.Weee.Api.Client;
    using EA.Weee.Tests.Core;
    using EA.Weee.Web.Areas.Scheme.Attributes;
    using EA.Weee.Web.Services.Caching;
    using FakeItEasy;
    using FluentAssertions;
    using Weee.Requests.AatfEvidence;
    using Weee.Tests.Core.DataHelpers;
    using Xunit;

    public class CheckCanEditTransferNoteAttributeTests : SimpleUnitTestBase
    {
        private readonly CheckCanEditTransferNoteAttribute attribute;
        private readonly ActionExecutingContext context;
        private readonly IWeeeClient client;
        private readonly IWeeeCache cache;
        private readonly Guid pcsId;
        private readonly Guid evidenceNoteId;

        public CheckCanEditTransferNoteAttributeTests()
        {
            client = A.Fake<IWeeeClient>();
            cache = A.Fake<IWeeeCache>();
            attribute = new CheckCanEditTransferNoteAttribute { Cache = cache, Client = () => client };
            context = A.Fake<ActionExecutingContext>();
            
            pcsId = TestFixture.Create<Guid>();
            evidenceNoteId = TestFixture.Create<Guid>();

            var routeData = new RouteData();
            routeData.Values.Add("pcsId", pcsId);
            routeData.Values.Add("evidenceNoteId", evidenceNoteId);

            A.CallTo(() => context.RouteData).Returns(routeData);
        }

        [Fact]
        public void CheckCanEditTransferNoteAttribute_ShouldBeDerivedFromCheckTransferNoteAttributeBaseAttribute()
        {
            typeof(CheckCanEditTransferNoteAttribute).Should().BeDerivedFrom<CheckSchemeNoteAttributeBase>();
        }

        [Fact]
        public void OnActionExecuting_GivenNoEvidenceNoteIdParameter_ArgumentExceptionExpected()
        {
            //arrange
            var routeData = new RouteData();
            routeData.Values.Add("pcsId", pcsId);

            A.CallTo(() => context.RouteData).Returns(routeData);

            //act
            var exception = Record.Exception(() => attribute.OnActionExecuting(context));

            //assert
            exception.Should().BeOfType<AggregateException>().Which.InnerExceptions.Should()
                .ContainSingle(e => e.GetType() == typeof(ArgumentException) && e.Message.Contains("No evidence note id specified"));
        }

        [Fact]
        public void OnActionExecuting_GivenInvalidEvidenceNoteIdParameter_ArgumentExceptionExpected()
        {
            //arrange
            var routeData = new RouteData();
            routeData.Values.Add("pcsId", pcsId);
            routeData.Values.Add("evidenceNoteId", "2");

            A.CallTo(() => context.RouteData).Returns(routeData);

            //act
            var exception = Record.Exception(() => attribute.OnActionExecuting(context));

            //assert
            exception.Should().BeOfType<AggregateException>().Which.InnerExceptions.Should()
                .ContainSingle(e => e.GetType() == typeof(ArgumentException) && e.Message.Contains("The specified evidence note id is not valid."));
        }

        [Fact]
        public void OnActionExecuting_GivenValidParameters_SchemeShouldBeRetrievedFromCache()
        {
            //act
            attribute.OnActionExecuting(context);

            //assert
            A.CallTo(() => cache.FetchSchemePublicInfo(pcsId)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public void OnActionExecuting_GivenValidParameters_CurrentShouldBeRetrievedFromCache()
        {
            //act
            attribute.OnActionExecuting(context);

            //assert
            A.CallTo(() => cache.FetchCurrentDate()).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public void OnActionExecuting_GivenValidParameters_EvidenceNoteShouldBeRetrieved()
        {
            //act
            attribute.OnActionExecuting(context);

            //assert
            A.CallTo(() => client.SendAsync(A<string>._,
                    A<GetTransferEvidenceNoteForSchemeRequest>.That.Matches(g => g.EvidenceNoteId == evidenceNoteId))).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public void OnActionExecuting_GivenValidParametersAndSchemeIsWithdrawn_ShouldRedirectToManageEvidenceNotes()
        {
            //arrange
            var schemeInfo = TestFixture.Build<SchemePublicInfo>()
                .With(s => s.Status, SchemeStatus.Withdrawn).Create();

            A.CallTo(() => cache.FetchSchemePublicInfo(A<Guid>._)).Returns(schemeInfo);

            //act
            attribute.OnActionExecuting(context);

            //assert
            AssertRoute();
        }

        [Fact]
        public void OnActionExecuting_GivenValidParametersAndDateIsOutsideOfComplianceYear_ShouldRedirectToManageEvidenceNotes()
        {
            //arrange
            var schemeInfo = TestFixture.Build<SchemePublicInfo>()
                .With(s => s.Status, SchemeStatus.Approved).Create();
            var currentDate = new DateTime(2023, 2, 1);
            var note = TestFixture.Build<TransferEvidenceNoteData>().With(e => e.ComplianceYear, 2022).Create();

            A.CallTo(() => cache.FetchCurrentDate()).Returns(currentDate);
            A.CallTo(() => cache.FetchSchemePublicInfo(A<Guid>._)).Returns(schemeInfo);
            A.CallTo(() => client.SendAsync(A<string>._, A<GetTransferEvidenceNoteForSchemeRequest>._)).Returns(note);

            //act
            attribute.OnActionExecuting(context);

            //assert
            AssertRoute();
        }

        [Theory]
        [ClassData(typeof(SchemeStatusCoreData))]
        public void OnActionExecuting_GivenValidParametersAndSchemeIsValid_ResultShouldBeEmpty(SchemeStatus status)
        {
            if (status == SchemeStatus.Withdrawn)
            {
                return;
            }

            //arrange
            var schemeInfo = TestFixture.Build<SchemePublicInfo>()
                .With(s => s.Status, status).Create();
            var currentDate = new DateTime(2022, 1, 31);
            var note = TestFixture.Build<TransferEvidenceNoteData>().With(e => e.ComplianceYear, 2022).Create();
            A.CallTo(() => client.SendAsync(A<string>._, A<GetTransferEvidenceNoteForSchemeRequest>._)).Returns(note);

            A.CallTo(() => cache.FetchCurrentDate()).Returns(currentDate);
            A.CallTo(() => cache.FetchSchemePublicInfo(A<Guid>._)).Returns(schemeInfo);

            //act
            attribute.OnActionExecuting(context);

            //assert
            context.Result.Should().BeNull();
        }

        private void AssertRoute()
        {
            var result = context.Result as RedirectToRouteResult;
            result.RouteValues["action"].Should().Be("Index");
            result.RouteValues["controller"].Should().Be("ManageEvidenceNotes");
        }
    }
}
