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
    using Domain.Evidence;
    using Domain.Scheme;
    using FakeItEasy;
    using FluentAssertions;
    using Mappings;
    using Prsd.Core.Mapper;
    using RequestHandlers.AatfEvidence;
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
        private readonly ISystemDataDataAccess systemDataDataAccess;
        private readonly GetEvidenceNotesForTransferRequest request;
        private readonly Scheme scheme;
        private readonly Guid schemeId;

        public GetEvidenceNotesForTransferRequestHandlerTests()
        {
            fixture = new Fixture();
            weeeAuthorization = A.Fake<IWeeeAuthorization>();
            evidenceDataAccess = A.Fake<IEvidenceDataAccess>();
            schemeDataAccess = A.Fake<ISchemeDataAccess>();
            systemDataDataAccess = A.Fake<ISystemDataDataAccess>();
            mapper = A.Fake<IMapper>();
            fixture.Create<Guid>();
            schemeId = fixture.Create<Guid>();
            scheme = A.Fake<Scheme>();
            var organisationId = fixture.Create<Guid>();

            A.CallTo(() => scheme.Id).Returns(schemeId);
            A.CallTo(() => schemeDataAccess.GetSchemeOrDefaultByOrganisationId(organisationId)).Returns(scheme);

            request = new GetEvidenceNotesForTransferRequest(organisationId, fixture.CreateMany<int>().ToList());

            handler = new GetEvidenceNotesForTransferRequestHandler(weeeAuthorization, evidenceDataAccess, mapper, schemeDataAccess, systemDataDataAccess);
        }

        [Fact]
        public async Task HandleAsync_GivenNoExternalAccess_ShouldThrowSecurityException()
        {
            //arrange
            var authorization = new AuthorizationBuilder().DenyExternalAreaAccess().Build();

            handler = new GetEvidenceNotesForTransferRequestHandler(authorization, evidenceDataAccess, mapper, schemeDataAccess, systemDataDataAccess);

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
           
            handler = new GetEvidenceNotesForTransferRequestHandler(authorization, evidenceDataAccess, mapper, schemeDataAccess, systemDataDataAccess);

            //act
            var result = await Record.ExceptionAsync(() => handler.HandleAsync(request));

            //assert
            result.Should().BeOfType<SecurityException>();
        }

        [Fact]
        public async Task HandleAsync_GivenRequest_ShouldCheckSchemeAccess()
        {
            //arrange
            A.CallTo(() => schemeDataAccess.GetSchemeOrDefaultByOrganisationId(request.OrganisationId)).Returns(scheme);

            //act
            await handler.HandleAsync(request);

            //assert
            A.CallTo(() => weeeAuthorization.EnsureSchemeAccess(schemeId))
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
        public async void HandleAsync_GivenRequest_SystemTimeShouldBeRetrieved()
        {
            // act
            await handler.HandleAsync(request);

            // assert
            A.CallTo(() => systemDataDataAccess.GetSystemDateTime()).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async void HandleAsync_GivenRequest_DataAccessGetNotesToTransferShouldBeCalled()
        {
            //arrange
            var date = fixture.Create<DateTime>();
            A.CallTo(() => systemDataDataAccess.GetSystemDateTime()).Returns(date);

            //act
            await handler.HandleAsync(request);

            //assert
            A.CallTo(() =>
                    evidenceDataAccess.GetNotesToTransfer(schemeId, 
                        A<List<int>>.That.IsSameSequenceAs(request.Categories.Select(w => (int)w).ToList()), A<List<Guid>>._, date.Year))
                .MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async void HandleAsync_GivenRequestWithEvidenceNotes_DataAccessGetNotesToTransferShouldBeCalled()
        {
            //arrange
            var request = new GetEvidenceNotesForTransferRequest(fixture.Create<Guid>(), fixture.CreateMany<int>().ToList(),
                fixture.CreateMany<Guid>().ToList());
            
            A.CallTo(() => scheme.Id).Returns(schemeId);
            A.CallTo(() => schemeDataAccess.GetSchemeOrDefaultByOrganisationId(A<Guid>._)).Returns(scheme);
            var date = fixture.Create<DateTime>();
            A.CallTo(() => systemDataDataAccess.GetSystemDateTime()).Returns(date);

            //act
            await handler.HandleAsync(request);

            //assert
            A.CallTo(() =>
                    evidenceDataAccess.GetNotesToTransfer(schemeId, A<List<int>>.That.IsSameSequenceAs(request.Categories.Select(w => (int)w).ToList()), A<List<Guid>>.That.IsSameSequenceAs(request.EvidenceNotes), date.Year))
                .MustHaveHappenedOnceExactly();
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

            A.CallTo(() =>
                    evidenceDataAccess.GetNotesToTransfer(A<Guid>._, A<List<int>>._, A<List<Guid>>._, A<int>._)).Returns(noteList);

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
        public async void HandleAsync_GivenCategoryFilterAndNotesData_ReturnedNotesDataShouldBeMapped()
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

            A.CallTo(() =>
                evidenceDataAccess.GetNotesToTransfer(A<Guid>._, A<List<int>>._, A<List<Guid>>._, A<int>._)).Returns(noteList);

            // act
            await handler.HandleAsync(request);

            // assert
            A.CallTo(() => mapper.Map<ListOfEvidenceNoteDataMap>(A<ListOfNotesMap>.That.Matches(a =>
                a.ListOfNotes.ElementAt(0).Reference.Equals(6) &&
                a.ListOfNotes.ElementAt(1).Reference.Equals(2) &&
                a.ListOfNotes.ElementAt(2).Reference.Equals(4) &&
                a.ListOfNotes.Count.Equals(3) &&
                a.CategoryFilter.SequenceEqual(request.Categories) &&
                a.IncludeTonnage == true))).MustHaveHappenedOnceExactly();
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

            A.CallTo(() => evidenceDataAccess.GetNotesToTransfer(A<Guid>._, A<List<int>>._, A<List<Guid>>._, A<int>._)).Returns(noteList);
            A.CallTo(() => mapper.Map<ListOfEvidenceNoteDataMap>(A<ListOfNotesMap>._)).Returns(listOfEvidenceNotes);

            // act
            var result = await handler.HandleAsync(request);

            // assert
            result.Should().BeEquivalentTo(noteData);
        }
    }
}
