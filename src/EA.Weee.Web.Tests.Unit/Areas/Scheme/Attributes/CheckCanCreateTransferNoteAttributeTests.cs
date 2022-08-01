namespace EA.Weee.Web.Tests.Unit.Areas.Scheme.Attributes
{
    using System;
    using System.Collections.Generic;
    using System.Web.Mvc;
    using System.Web.Routing;
    using AutoFixture;
    using Core.Scheme;
    using Core.Shared;
    using EA.Weee.Api.Client;
    using EA.Weee.Tests.Core;
    using EA.Weee.Web.Areas.Scheme.Attributes;
    using EA.Weee.Web.Services.Caching;
    using FakeItEasy;
    using FluentAssertions;
    using Weee.Tests.Core.DataHelpers;
    using Xunit;

    public class CheckCanCreateTransferNoteAttributeTests : SimpleUnitTestBase
    {
        private readonly CheckCanCreateTransferNoteAttribute attribute;
        private readonly ActionExecutingContext context;
        private readonly IWeeeClient client;
        private readonly IWeeeCache cache;
        private readonly Guid pcsId;

        public CheckCanCreateTransferNoteAttributeTests()
        {
            client = A.Fake<IWeeeClient>();
            cache = A.Fake<IWeeeCache>();
            attribute = new CheckCanCreateTransferNoteAttribute { Cache = cache, Client = () => client };
            context = A.Fake<ActionExecutingContext>();
            var complianceYear = 2022;
            pcsId = TestFixture.Create<Guid>();

            var routeData = new RouteData();
            routeData.Values.Add("pcsId", pcsId);

            var actionParameters = new Dictionary<string, object> { { "complianceYear", complianceYear } };
            A.CallTo(() => context.RouteData).Returns(routeData);
            A.CallTo(() => context.ActionParameters).Returns(actionParameters);
        }

        [Fact]
        public void CheckCanCreateTransferNoteAttribute_ShouldBeDerivedFromCheckSchemeNoteAttributeBaseAttribute()
        {
            typeof(CheckCanCreateTransferNoteAttribute).Should().BeDerivedFrom<CheckSchemeNoteAttributeBase>();
        }

        [Fact]
        public void OnActionExecuting_GivenNoComplianceYearParameter_ArgumentExceptionExpected()
        {
            //arrange
            var actionParameters = new Dictionary<string, object>();
            A.CallTo(() => context.ActionParameters).Returns(actionParameters);

            //act
            var exception = Record.Exception(() => attribute.OnActionExecuting(context));

            //assert
            exception.Should().BeOfType<AggregateException>().Which.InnerExceptions.Should()
                .ContainSingle(e => e.GetType() == typeof(ArgumentException) && e.Message.Contains("No compliance year was specified."));
        }

        [Fact]
        public void OnActionExecuting_GivenInvalidComplianceYearParameter_ArgumentExceptionExpected()
        {
            //arrange
            var actionParameters = new Dictionary<string, object>() { { "complianceYear", TestFixture.Create<string>() }};
            A.CallTo(() => context.ActionParameters).Returns(actionParameters);

            //act
            var exception = Record.Exception(() => attribute.OnActionExecuting(context));

            //assert
            exception.Should().BeOfType<AggregateException>().Which.InnerExceptions.Should()
                .ContainSingle(e => e.GetType() == typeof(ArgumentException) && e.Message.Contains("The specified compliance year is not valid."));
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

            A.CallTo(() => cache.FetchCurrentDate()).Returns(currentDate);
            A.CallTo(() => cache.FetchSchemePublicInfo(A<Guid>._)).Returns(schemeInfo);

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
