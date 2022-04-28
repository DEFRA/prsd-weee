namespace EA.Weee.RequestHandlers.Tests.Unit.AatfEvidence
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security;
    using System.Threading.Tasks;
    using AutoFixture;
    using Core.AatfEvidence;
    using Core.DataReturns;
    using DataAccess.DataAccess;
    using Domain.AatfReturn;
    using Domain.Evidence;
    using Domain.Organisation;
    using Domain.Scheme;
    using FakeItEasy;
    using FluentAssertions;
    using Mappings;
    using Prsd.Core.Domain;
    using Prsd.Core.Mapper;
    using RequestHandlers.AatfEvidence;
    using RequestHandlers.AatfReturn.Internal;
    using RequestHandlers.Security;
    using Weee.Requests.AatfEvidence;
    using Weee.Tests.Core;
    using Xunit;

    public class GetEvidenceNotesForTransferRequestHandlerTests
    {
        private GetEvidenceNotesForTransferRequestHandler handler;
        private readonly Fixture fixture;
        private readonly IWeeeAuthorization weeeAuthorization;
        private readonly IEvidenceDataAccess evidenceDataAccess;
        private readonly ISchemeDataAccess schemeDataAccess;
        private readonly IMapper mapper;
        private readonly GetEvidenceNotesForTransferRequest request;
        private readonly Note note;
        private readonly Guid evidenceNoteId;
        private readonly Guid recipientId;

        public GetEvidenceNotesForTransferRequestHandlerTests()
        {
            fixture = new Fixture();
            weeeAuthorization = A.Fake<IWeeeAuthorization>();
            evidenceDataAccess = A.Fake<IEvidenceDataAccess>();
            mapper = A.Fake<IMapper>();
            note = A.Fake<Note>();
            fixture.Create<Guid>();
            evidenceNoteId = fixture.Create<Guid>();
            recipientId = fixture.Create<Guid>();

            A.CallTo(() => note.Recipient.Id).Returns(recipientId);

            request = new GetEvidenceNotesForTransferRequest(evidenceNoteId, fixture.CreateMany<WeeeCategory>().ToList());

            handler = new GetEvidenceNotesForTransferRequestHandler(weeeAuthorization, evidenceDataAccess, mapper, schemeDataAccess);

            A.CallTo(() => evidenceDataAccess.GetNoteById(evidenceNoteId)).Returns(note);
        }

        [Fact]
        public async Task HandleAsync_GivenNoExternalAccess_ShouldThrowSecurityException()
        {
            //arrange
            var authorization = new AuthorizationBuilder().DenyExternalAreaAccess().Build();

            handler = new GetEvidenceNotesForTransferRequestHandler(authorization, evidenceDataAccess, mapper, schemeDataAccess);

            //act
            var result = await Record.ExceptionAsync(() => handler.HandleAsync(request));

            //assert
            result.Should().BeOfType<SecurityException>();
        }

        [Fact]
        public async Task HandleAsync_GivenNoSchemeAccess_ShouldThrowSecurityException()
        {
            //arrange
            var authorization = new AuthorizationBuilder().DenySchemeAccess().Build();
           
            handler = new GetEvidenceNotesForTransferRequestHandler(authorization, evidenceDataAccess, mapper, schemeDataAccess);

            //act
            var result = await Record.ExceptionAsync(() => handler.HandleAsync(request));

            //assert
            result.Should().BeOfType<SecurityException>();
        }

        [Fact]
        public async Task HandleAsync_GivenRequest_EvidenceNoteShouldBeRetrieved()
        {
            //act
            await handler.HandleAsync(request);

            //assert
            A.CallTo(() => evidenceDataAccess.GetNoteById(evidenceNoteId)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task HandleAsync_GivenRequestAndEvidenceNoteNotFound_ShowThrowArgumentNullExceptionExpected()
        {
            //arrange
            A.CallTo(() => evidenceDataAccess.GetNoteById(evidenceNoteId)).Returns((Note)null);

            //act
            var result = await Record.ExceptionAsync(() => handler.HandleAsync(request));

            //assert
            result.Should().BeOfType<ArgumentNullException>();
        }

        [Fact]
        public async Task HandleAsync_GivenRequest_ShouldCheckSchemeAccess()
        {
            //act
            await handler.HandleAsync(request);

            //assert
            A.CallTo(() => weeeAuthorization.EnsureSchemeAccess(recipientId))
                .MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async void HandleAsync_GivenRequest_SchemeDataAccessShouldBeCalledOnce()
        {
            // act
            await handler.HandleAsync(request);

            // assert
            A.CallTo(() => schemeDataAccess.GetSchemeOrDefaultByOrganisationId(request.OrganisationId)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async void HandleAsync_GivenNotesData_ReturnedNotesDataShouldBeMapped()
        {
            // arrange
            var note1 = A.Fake<Note>();
            var note2 = A.Fake<Note>();
            var note3 = A.Fake<Note>();

            A.CallTo(() => note1.Reference).Returns(2);
            A.CallTo(() => note1.CreatedDate).Returns(DateTime.Now.AddDays(1));
            A.CallTo(() => note2.Reference).Returns(4);
            A.CallTo(() => note2.CreatedDate).Returns(DateTime.Now);
            A.CallTo(() => note3.Reference).Returns(6);
            A.CallTo(() => note3.CreatedDate).Returns(DateTime.Now.AddDays(2));

            var noteList = new List<Note>()
            {
                note1,
                note2,
                note3
            };

            A.CallTo(() => evidenceDataAccess.GetAllNotes(A<EvidenceNoteFilter>._)).Returns(noteList);

            // act
            await handler.HandleAsync(request);

            // assert
            A.CallTo(() => mapper.Map<ListOfEvidenceNoteDataMap>(A<ListOfNotesMap>.That.Matches(a =>
                a.ListOfNotes.ElementAt(0).Reference.Equals(6) &&
                a.ListOfNotes.ElementAt(1).Reference.Equals(2) &&
                a.ListOfNotes.ElementAt(2).Reference.Equals(4) &&
                a.ListOfNotes.Count.Equals(3)))).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async void HandleAsync_GivenMappedEvidenceNoteData_ListEvidenceNoteDataShouldBeReturn()
        {
            // arrange
            var noteList = fixture.CreateMany<Note>().ToList();

            var noteData = new List<EvidenceNoteData>()
            {
                A.Fake<EvidenceNoteData>(),
                A.Fake<EvidenceNoteData>()
            };

            var listOfEvidenceNotes = new ListOfEvidenceNoteDataMap() { ListOfEvidenceNoteData = noteData };

            A.CallTo(() => evidenceDataAccess.GetAllNotes(A<EvidenceNoteFilter>._)).Returns(noteList);

            A.CallTo(() => mapper.Map<ListOfEvidenceNoteDataMap>(A<ListOfNotesMap>._)).Returns(listOfEvidenceNotes);

            // act
            var result = await handler.HandleAsync(request);

            // assert
            result.Should().BeEquivalentTo(noteData);
        }
    }
}
