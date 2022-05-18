namespace EA.Weee.RequestHandlers.Tests.Unit.AatfEvidence
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security;
    using System.Threading.Tasks;
    using AutoFixture;
    using Core.Tests.Unit.Helpers;
    using DataAccess.DataAccess;
    using Domain.Evidence;
    using Domain.Lookup;
    using Domain.Organisation;
    using Domain.Scheme;
    using FakeItEasy;
    using FluentAssertions;
    using RequestHandlers.AatfEvidence;
    using RequestHandlers.Security;
    using Weee.Requests.Aatf;
    using Weee.Requests.AatfEvidence;
    using Weee.Tests.Core;
    using Xunit;
    using Protocol = Core.AatfEvidence.Protocol;
    using WasteType = Core.AatfEvidence.WasteType;

    public class EditEvidenceNoteRequestHandlerTests
    {
        private EditEvidenceNoteRequestHandler handler;
        private readonly Fixture fixture;
        private readonly IWeeeAuthorization weeeAuthorization;
        private readonly IEvidenceDataAccess evidenceDataAccess;
        private readonly ISchemeDataAccess schemeDataAccess;
        private readonly EditEvidenceNoteRequest request;
        private readonly Organisation organisation;
        private readonly Scheme scheme;
        private readonly Note note;

        public EditEvidenceNoteRequestHandlerTests()
        {
            fixture = new Fixture();
            weeeAuthorization = A.Fake<IWeeeAuthorization>();
            evidenceDataAccess = A.Fake<IEvidenceDataAccess>();
            schemeDataAccess = A.Fake<ISchemeDataAccess>();

            organisation = A.Fake<Organisation>();
            scheme = A.Fake<Scheme>();
            note = A.Fake<Note>();
            fixture.Create<Guid>();
            var organisationId = fixture.Create<Guid>();

            A.CallTo(() => scheme.Id).Returns(fixture.Create<Guid>());
            A.CallTo(() => organisation.Id).Returns(organisationId);
            A.CallTo(() => note.Organisation).Returns(organisation);
            A.CallTo(() => note.OrganisationId).Returns(organisationId);
            A.CallTo(() => note.Id).Returns(fixture.Create<Guid>());
            A.CallTo(() => note.Status).Returns(NoteStatus.Draft);

            request = Request();

            handler = new EditEvidenceNoteRequestHandler(weeeAuthorization, evidenceDataAccess, schemeDataAccess);

            A.CallTo(() => evidenceDataAccess.GetNoteById(request.Id)).Returns(note);
        }

        [Fact]
        public async Task HandleAsync_GivenNoExternalAccess_ShouldThrowSecurityException()
        {
            //arrange
            var authorization = new AuthorizationBuilder().DenyExternalAreaAccess().Build();

            handler = new EditEvidenceNoteRequestHandler(authorization, evidenceDataAccess, schemeDataAccess);

            //act
            var result = await Record.ExceptionAsync(() => handler.HandleAsync(Request()));

            //assert
            result.Should().BeOfType<SecurityException>();
        }

        [Fact]
        public async Task HandleAsync_GivenNoOrganisationAccess_ShouldThrowSecurityException()
        {
            //arrange
            var authorization = new AuthorizationBuilder().DenyOrganisationAccess().Build();

            handler = new EditEvidenceNoteRequestHandler(authorization, evidenceDataAccess, schemeDataAccess);

            //act
            var result = await Record.ExceptionAsync(() => handler.HandleAsync(Request()));

            //assert
            result.Should().BeOfType<SecurityException>();
        }

        [Theory]
        [ClassData(typeof(NoteStatusData))]
        public async Task HandleAsync_GivenNoteIsNotInDraftAndReturned_ShouldInvalidOperationException(NoteStatus status)
        {
            //arrange
            var allowedStatus = new List<int>() { NoteStatus.Draft.Value, NoteStatus.Returned.Value };

            if (!allowedStatus.Contains(status.Value))
            {
                A.CallTo(() => note.Status).Returns(status);
                A.CallTo(() => note.RecipientId).Returns(scheme.Id);
                A.CallTo(() => evidenceDataAccess.GetNoteById(A<Guid>._)).Returns(note);

                //act
                var result = await Record.ExceptionAsync(() => handler.HandleAsync(Request()));

                //assert
                result.Should().BeOfType<InvalidOperationException>().Which.Message.Should().Be($"Evidence note {note.Id} is incorrect state to be edited");
            }
        }

        [Fact]
        public async Task HandleAsync_EnsureTheSchemeNotChanged_GivenNoteStatusIsReturnedAndSchemeChanged_ShouldThrowInvalidOperationException()
        {
            //arrange 
            A.CallTo(() => note.Status).Returns(NoteStatus.Returned);
            A.CallTo(() => note.RecipientId).Returns(fixture.Create<Guid>());
            A.CallTo(() => evidenceDataAccess.GetNoteById(A<Guid>._)).Returns(note);

            //act
            var result = await Record.ExceptionAsync(() => handler.HandleAsync(Request()));

            //assert
            result.Should().BeOfType<InvalidOperationException>().Which.Message.Should().Be($"Evidence note {note.Id} has incorrect Recipient Id to be saved");
        }

        [Fact]
        public async Task HandleAsync_GivenRequestAndNoSchemeFound_ShowThrowArgumentNullExceptionExpected()
        {
            //arrange
            A.CallTo(() => schemeDataAccess.GetSchemeOrDefault(A<Guid>._)).Returns((Scheme)null);

            //act
            var result = await Record.ExceptionAsync(() => handler.HandleAsync(Request()));

            //assert
            result.Should().BeOfType<ArgumentNullException>();
        }

        [Fact]
        public async Task HandleAsync_GivenRequest_ShouldCheckOrganisationAccess()
        {
            //act
            await handler.HandleAsync(request);

            //assert
            A.CallTo(() => weeeAuthorization.EnsureOrganisationAccess(note.OrganisationId))
                .MustHaveHappenedOnceExactly();
        }

        [Theory]
        [ClassData(typeof(ProtocolData))]
        public async Task HandleAsync_GivenRequest_DataAccessShouldBeCalled(Domain.Evidence.Protocol protocol)
        {
            //arrange
            A.CallTo(() => evidenceDataAccess.GetNoteById(A<Guid>._)).Returns(note);
            A.CallTo(() => schemeDataAccess.GetSchemeOrDefault(A<Guid>._)).Returns(scheme);

            var request = Request();
            request.Protocol = (Protocol?)protocol;

            var tonnageValues = request.TonnageValues.Select(t => new NoteTonnage(
                (WeeeCategory)t.CategoryId,
                t.FirstTonnage,
                t.SecondTonnage)).ToList();

            //act
            await handler.HandleAsync(request);

            //assert
            A.CallTo(() => evidenceDataAccess.Update(note, scheme, request.StartDate, request.EndDate, A<Domain.Evidence.WasteType>._, protocol, A<IList<NoteTonnage>>._, A<NoteStatus>._)).MustHaveHappenedOnceExactly();

            AssertTonnages(tonnageValues);
        }

        [Theory]
        [ClassData(typeof(WasteTypeData))]
        public async Task HandleAsync_GivenRequest_DataAccessShouldBeCalled(Domain.Evidence.WasteType waste)
        {
            //arrange
            A.CallTo(() => evidenceDataAccess.GetNoteById(A<Guid>._)).Returns(note);
            A.CallTo(() => schemeDataAccess.GetSchemeOrDefault(A<Guid>._)).Returns(scheme);

            var request = Request();
            request.WasteType = (WasteType?)waste;

            var tonnageValues = request.TonnageValues.Select(t => new NoteTonnage(
                (WeeeCategory)t.CategoryId,
                t.FirstTonnage,
                t.SecondTonnage)).ToList();

            //act
            await handler.HandleAsync(request);

            //assert
            A.CallTo(() => evidenceDataAccess.Update(note, scheme, request.StartDate, request.EndDate, waste, A<Domain.Evidence.Protocol>._, A<IList<NoteTonnage>>._, A<NoteStatus>._)).MustHaveHappenedOnceExactly();

            AssertTonnages(tonnageValues);
        }

        [Theory]
        [ClassData(typeof(NoteStatusData))]
        public async Task HandleAsync_GivenRequest_DataAccessShouldBeCalled(Domain.Evidence.NoteStatus status)
        {
            //arrange
            A.CallTo(() => evidenceDataAccess.GetNoteById(A<Guid>._)).Returns(note);
            A.CallTo(() => schemeDataAccess.GetSchemeOrDefault(A<Guid>._)).Returns(scheme);

            var request = Request();
            request.Status = (Core.AatfEvidence.NoteStatus)status.Value;

            var tonnageValues = request.TonnageValues.Select(t => new NoteTonnage(
                (WeeeCategory)t.CategoryId,
                t.FirstTonnage,
                t.SecondTonnage)).ToList();

            //act
            await handler.HandleAsync(request);

            //assert
            A.CallTo(() => evidenceDataAccess.Update(note, scheme, request.StartDate, request.EndDate, A<Domain.Evidence.WasteType>._, A<Domain.Evidence.Protocol>._, A<IList<NoteTonnage>>._, status)).MustHaveHappenedOnceExactly();

            AssertTonnages(tonnageValues);
        }

        [Fact]
        public async Task HandleAsync_GivenRequest_IdShouldBeReturned()
        {
            //act
            A.CallTo(() => evidenceDataAccess.GetNoteById(A<Guid>._)).Returns(note);
            A.CallTo(() => schemeDataAccess.GetSchemeOrDefault(A<Guid>._)).Returns(scheme);

            //arrange
            var result = await handler.HandleAsync(request);

            //assert
            result.Should().Be(note.Id);
        }

        [Fact]
        public async Task HandleAsync_GivenRequestAndReturnedStatus_IdShouldBeReturned()
        {
            //act
            A.CallTo(() => evidenceDataAccess.GetNoteById(A<Guid>._)).Returns(note);
            A.CallTo(() => note.Status).Returns(NoteStatus.Returned);
            A.CallTo(() => note.RecipientId).Returns(scheme.Id);
            A.CallTo(() => schemeDataAccess.GetSchemeOrDefault(A<Guid>._)).Returns(scheme);

            //arrange
            var result = await handler.HandleAsync(request);

            //assert
            result.Should().Be(note.Id);
        }

        private void AssertTonnages(List<NoteTonnage> tonnageValues)
        {
            foreach (var tonnageValue in tonnageValues)
            {
                A.CallTo(() => evidenceDataAccess.Update(A<Note>._,
                        A<Scheme>._, A<DateTime>._, A<DateTime>._, A<Domain.Evidence.WasteType>._, A<Domain.Evidence.Protocol>._,
                        A<IList<NoteTonnage>>.That.Matches(t =>
                            t.Count(n => n.CategoryId.Equals(tonnageValue.CategoryId) &&
                                         n.Received.Equals(tonnageValue.Received) &&
                                         n.Reused.Equals(tonnageValue.Reused)).Equals(1)), A<NoteStatus>._))
                    .MustHaveHappenedOnceExactly();
            }
        }

        private EditEvidenceNoteRequest Request()
        {
            return new EditEvidenceNoteRequest(organisation.Id,
                fixture.Create<Guid>(),
                scheme.Id,
                DateTime.Now,
                DateTime.Now.AddDays(1),
                fixture.Create<WasteType>(),
                fixture.Create<Protocol>(),
                fixture.CreateMany<TonnageValues>().ToList(),
                fixture.Create<Core.AatfEvidence.NoteStatus>(),
                fixture.Create<Guid>());
        }
    }
}
