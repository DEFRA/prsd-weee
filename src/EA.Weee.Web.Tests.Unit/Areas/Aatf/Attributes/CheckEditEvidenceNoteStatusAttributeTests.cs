﻿namespace EA.Weee.Web.Tests.Unit.Areas.Aatf.Attributes
{
    using System;
    using System.Collections.Generic;
    using System.Web.Mvc;
    using System.Web.Routing;
    using Api.Client;
    using AutoFixture;
    using Core.AatfEvidence;
    using Core.Tests.Unit.Helpers;
    using FakeItEasy;
    using FluentAssertions;
    using Web.Areas.Aatf.Attributes;
    using Web.Areas.Aatf.ViewModels;
    using Web.ViewModels.Shared;
    using Weee.Requests.AatfEvidence;
    using Xunit;

    public class CheckEditEvidenceNoteStatusAttributeTests
    {
        private readonly CheckEditEvidenceNoteStatusAttribute attribute;
        private readonly ActionExecutingContext context;
        private readonly IWeeeClient client;
        private readonly Fixture fixture;

        public CheckEditEvidenceNoteStatusAttributeTests()
        {
            client = A.Fake<IWeeeClient>();
            attribute = new CheckEditEvidenceNoteStatusAttribute { Client = () => client };
            context = A.Fake<ActionExecutingContext>();
            fixture = new Fixture();

            var routeData = new RouteData();
            routeData.Values.Add("evidenceNoteId", Guid.NewGuid());
            A.CallTo(() => context.RouteData).Returns(routeData);
        }

        [Fact]
        public void OnActionExecuting_GivenViewModel_AndNotEvidenceNoteViewModel_ArgumentExceptionExpected()
        {
            //arrange
            var actionParameters = new Dictionary<string, object> { { "viewModel", new object() } };
            A.CallTo(() => context.ActionParameters).Returns(actionParameters);

            //act
            Action action = () => attribute.OnActionExecuting(context);

            //assert
            action.Should().Throw<ArgumentException>().WithMessage("Edit evidence note view model incorrect type.");
        }

        [Theory]
        [ClassData(typeof(NoteStatusCoreData))]
        public void OnActionExecuting_GivenViewModel_AndEvidenceNoteIsNotDraft_InvalidOperationExceptionExpected(NoteStatus noteStatus)
        {
            if (noteStatus != NoteStatus.Draft && noteStatus != NoteStatus.Returned)
            {
                //arrange
                var viewModel = new EvidenceNoteViewModel()
                {
                    Id = Guid.NewGuid()
                };

                var actionParameters = new Dictionary<string, object> { { "viewModel", viewModel } };
                A.CallTo(() => context.ActionParameters).Returns(actionParameters);

                var note = fixture.Create<EvidenceNoteData>();
                note.Status = noteStatus;

                A.CallTo(() => client.SendAsync(A<string>._,
                    A<GetEvidenceNoteForAatfRequest>.That.Matches(r => r.EvidenceNoteId.Equals(viewModel.Id)))).Returns(note);

                //act
                var result = Record.Exception(() => attribute.OnActionExecuting(context));

                //assert
                result.Should().BeOfType<AggregateException>();
                result.InnerException.Should().BeOfType<InvalidOperationException>()
                    .Which.Message.Should()
                    .Be($"Evidence note {note.Id} is incorrect state to be edited");
            }
        }

        [Fact]
        public void OnActionExecuting_GivenNoEvidenceNoteId_ArgumentExceptionExpected()
        {
            //arrange
            Action action = () => attribute.OnActionExecuting(context);

            //act
            A.CallTo(() => context.RouteData).Returns(new RouteData());

            //assert
            action.Should().Throw<ArgumentException>().WithMessage("No evidence note ID was specified.");
        }

        [Fact]
        public void OnActionExecuting_GivenEvidenceNoteIdIsNotGuid_ArgumentExceptionExpected()
        {
            //arrange
            var routeData = new RouteData();
            routeData.Values.Add("evidenceNoteId", 1);

            A.CallTo(() => context.RouteData).Returns(routeData);
            
            //act
            Action action = () => attribute.OnActionExecuting(context);
            
            //assert
            action.Should().Throw<ArgumentException>().WithMessage("The specified evidence note ID is not valid.");
        }

        [Theory]
        [ClassData(typeof(NoteStatusCoreData))]
        public void OnActionExecuting_GivenEvidenceNoteIdIsNotDraft_InvalidOperationExceptionExpected(NoteStatus noteStatus)
        {
            if (noteStatus != NoteStatus.Draft && noteStatus != NoteStatus.Returned)
            {
                //arrange
                var note = fixture.Create<EvidenceNoteData>();
                note.Status = noteStatus;

                A.CallTo(() => client.SendAsync(A<string>._,
                    A<GetEvidenceNoteForAatfRequest>.That.Matches(r => r.EvidenceNoteId.Equals((Guid)context.RouteData.Values["evidenceNoteId"])))).Returns(note);

                //act
                var result = Record.Exception(() => attribute.OnActionExecuting(context));

                //assert
                result.Should().BeOfType<AggregateException>();
                result.InnerException.Should().BeOfType<InvalidOperationException>()                    
                    .Which.Message.Should()
                    .Be($"Evidence note {note.Id} is incorrect state to be edited");
            }
        }

        [Fact]
        public void OnActionExecuting_GivenEvidenceNoteIdIsDraft_NoExceptionExpected()
        {
            //arrange
            var note = fixture.Create<EvidenceNoteData>();
            note.Status = NoteStatus.Draft;

            //act
            A.CallTo(() => client.SendAsync(A<string>._,
                A<GetEvidenceNoteForAatfRequest>.That.Matches(r => r.EvidenceNoteId.Equals((Guid)context.RouteData.Values["evidenceNoteId"])))).Returns(note);

            //assert
            var result = Record.Exception(() => attribute.OnActionExecuting(context));
            result.Should().BeNull();
        }
    }
}
