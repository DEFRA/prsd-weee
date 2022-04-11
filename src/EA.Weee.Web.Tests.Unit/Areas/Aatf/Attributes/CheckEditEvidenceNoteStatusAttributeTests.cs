namespace EA.Weee.Web.Tests.Unit.Areas.Aatf.Attributes
{
    using System;
    using System.Web.Mvc;
    using System.Web.Routing;
    using Api.Client;
    using AutoFixture;
    using Core.AatfEvidence;
    using Core.Tests.Unit.Helpers;
    using FakeItEasy;
    using FluentAssertions;
    using Web.Areas.Aatf.Attributes;
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
        public void OnActionExecuting_GivenNoEvidenceNoteId_ArgumentExceptionExpected()
        {
            Action action = () => attribute.OnActionExecuting(context);

            A.CallTo(() => context.RouteData).Returns(new RouteData());

            action.Should().Throw<ArgumentException>().WithMessage("No evidence note ID was specified.");
        }

        [Fact]
        public void OnActionExecuting_GivenEvidenceNoteIdIsNotGuid_ArgumentExceptionExpected()
        {
            Action action = () => attribute.OnActionExecuting(context);

            var routeData = new RouteData();
            routeData.Values.Add("evidenceNoteId", 1);

            A.CallTo(() => context.RouteData).Returns(routeData);

            action.Should().Throw<ArgumentException>().WithMessage("The specified evidence note ID is not valid.");
        }

        [Theory]
        [ClassData(typeof(NoteStatusCoreData))]
        public void OnActionExecuting_GivenEvidenceNoteIdIsNotDraft_InvalidOperationExceptionExpected(NoteStatus noteStatus)
        {
            if (noteStatus != NoteStatus.Draft)
            {
                var note = fixture.Create<EvidenceNoteData>();
                note.Status = noteStatus;

                A.CallTo(() => client.SendAsync(A<string>._,
                    A<GetEvidenceNoteRequest>.That.Matches(r => r.EvidenceNoteId.Equals((Guid)context.RouteData.Values["evidenceNoteId"])))).Returns(note);

                var result = Record.Exception(() => attribute.OnActionExecuting(context));

                result.Should().BeOfType<AggregateException>();
                result.InnerException.Should().BeOfType<InvalidOperationException>()                    
                    .Which.Message.Should()
                    .Be($"Evidence note {note.Id} is incorrect state to be edited");
            }
        }

        [Fact]
        public void OnActionExecuting_GivenEvidenceNoteIdIsDraft_NoExceptionExpected()
        {
            var note = fixture.Create<EvidenceNoteData>();
            note.Status = NoteStatus.Draft;

            A.CallTo(() => client.SendAsync(A<string>._,
                A<GetEvidenceNoteRequest>.That.Matches(r => r.EvidenceNoteId.Equals((Guid)context.RouteData.Values["evidenceNoteId"])))).Returns(note);

            var result = Record.Exception(() => attribute.OnActionExecuting(context));
            result.Should().BeNull();
        }
    }
}
