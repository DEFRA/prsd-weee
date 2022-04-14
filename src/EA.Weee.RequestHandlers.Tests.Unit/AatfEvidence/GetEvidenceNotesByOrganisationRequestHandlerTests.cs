namespace EA.Weee.RequestHandlers.Tests.Unit.AatfEvidence
{
    using AutoFixture;
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Core.AatfEvidence;
    using EA.Weee.DataAccess.DataAccess;
    using EA.Weee.Domain.Evidence;
    using EA.Weee.RequestHandlers.AatfEvidence;
    using EA.Weee.RequestHandlers.AatfReturn.Internal;
    using EA.Weee.RequestHandlers.Mappings;
    using EA.Weee.RequestHandlers.Security;
    using EA.Weee.Requests.AatfEvidence;
    using EA.Weee.Tests.Core;
    using FakeItEasy;
    using FluentAssertions;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security;
    using System.Threading.Tasks;
    using Xunit;

    public class GetEvidenceNotesByOrganisationRequestHandlerTests
    {
        private GetEvidenceNotesByOrganisationRequestHandler handler;
        private readonly Fixture fixture;
        private readonly IWeeeAuthorization weeeAuthorization;
        private readonly IAatfDataAccess aatfDataAccess;
        private readonly ISchemeDataAccess schemeDataAccess;
        private readonly IMapper mapper;
        private readonly Guid organisationId;
        private readonly GetEvidenceNotesByOrganisationRequest request;

        public GetEvidenceNotesByOrganisationRequestHandlerTests()
        {
            fixture = new Fixture();
            weeeAuthorization = A.Fake<IWeeeAuthorization>();
            aatfDataAccess = A.Fake<IAatfDataAccess>();
            schemeDataAccess = A.Fake<ISchemeDataAccess>();
            mapper = A.Fake<IMapper>();

            organisationId = Guid.NewGuid();

            request = new GetEvidenceNotesByOrganisationRequest(organisationId);

            handler = new GetEvidenceNotesByOrganisationRequestHandler(weeeAuthorization,
                aatfDataAccess,
                mapper,
                schemeDataAccess);
        }

        [Fact]
        public async Task HandleAsync_GivenNoExternalAccess_ShouldThrowSecurityException()
        {
            //arrange
            var authorization = new AuthorizationBuilder().DenyExternalAreaAccess().Build();
            handler = new GetEvidenceNotesByOrganisationRequestHandler(authorization, aatfDataAccess, mapper, schemeDataAccess);

            //act
            var result = await Record.ExceptionAsync(() => handler.HandleAsync(GetEvidenceNotesByOrganisationRequest()));

            //assert
            result.Should().BeOfType<SecurityException>();
        }

        [Fact]
        public async Task HandleAsync_GivenNoOrganisationAccess_ShouldThrowSecurityException()
        {
            //arrange
            var authorization = new AuthorizationBuilder().DenyOrganisationAccess().Build();
            handler = new GetEvidenceNotesByOrganisationRequestHandler(authorization, aatfDataAccess, mapper, schemeDataAccess);

            //act
            var result = await Record.ExceptionAsync(() => handler.HandleAsync(GetEvidenceNotesByOrganisationRequest()));

            //assert
            result.Should().BeOfType<SecurityException>();
        }

        [Fact]
        public async Task HandleAsync_GivenRequest_ShouldCheckOrganisationAccess()
        {
            //act
            await handler.HandleAsync(request);

            //assert
            A.CallTo(() => weeeAuthorization.EnsureOrganisationAccess(request.OrganisationId))
                .MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task HandleAsync_GivenRequest_ShouldExternalAccess()
        {
            //act
            await handler.HandleAsync(request);

            //assert
            A.CallTo(() => weeeAuthorization.EnsureCanAccessExternalArea())
                .MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async void HandleAsync_GivenRequest_AatfDataAccessShouldBeCalledOnce()
        {
            // act
            await handler.HandleAsync(request);

            // assert
            A.CallTo(() => aatfDataAccess.GetAllSubmittedNotesByScheme(A<Guid>._, A<List<int>>._)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async void HandleAsync_GivenRequest_SchemeDataAccessShouldBeCalledOnce()
        {
            // act
            await handler.HandleAsync(request);

            // assert
            A.CallTo(() => schemeDataAccess.GetSchemeOrDefaultByOrganisationId(A<Guid>._)).MustHaveHappenedOnceExactly();
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

            A.CallTo(() => aatfDataAccess.GetAllSubmittedNotesByScheme(A<Guid>._, A<List<int>>._)).Returns(noteList);

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

            var evidenceNoteDatas = new List<EvidenceNoteData>()
            {
                A.Fake<EvidenceNoteData>(),
                A.Fake<EvidenceNoteData>()
            };

            var listOfEvidenceNotes = new ListOfEvidenceNoteDataMap() { ListOfEvidenceNoteData = evidenceNoteDatas };

            A.CallTo(() => aatfDataAccess.GetAllSubmittedNotesByScheme(A<Guid>._, A<List<int>>._)).Returns(noteList);

            A.CallTo(() => mapper.Map<ListOfEvidenceNoteDataMap>(A<ListOfNotesMap>._)).Returns(listOfEvidenceNotes);

            // act
            var result = await handler.HandleAsync(request);

            // assert
            result.Should().BeOfType<List<EvidenceNoteData>>();
            result.Count().Should().Be(evidenceNoteDatas.Count);
        }

        private GetEvidenceNotesByOrganisationRequest GetEvidenceNotesByOrganisationRequest()
        {
            return new GetEvidenceNotesByOrganisationRequest(organisationId);
        }
    }
}
