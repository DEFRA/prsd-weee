namespace EA.Weee.RequestHandlers.Tests.Unit.AatfEvidence
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security;
    using System.Threading.Tasks;
    using AutoFixture;
    using Domain.AatfReturn;
    using Domain.Evidence;
    using Domain.Organisation;
    using Domain.Scheme;
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Core.AatfEvidence;
    using EA.Weee.RequestHandlers.Mappings;
    using FakeItEasy;
    using FluentAssertions;
    using RequestHandlers.AatfEvidence;
    using RequestHandlers.AatfReturn.Internal;
    using RequestHandlers.Security;
    using Weee.Requests.AatfEvidence;
    using Weee.Tests.Core;
    using Xunit;

    public class GetAatfNotesRequestHandlerTests
    {
        private GetAatfNotesRequestHandler handler;
        private readonly Fixture fixture;
        private readonly IWeeeAuthorization weeeAuthorization;
        private readonly IAatfDataAccess aatfDataAccess;
        private readonly IMapper mapper;
        private readonly GetAatfNotesRequest request;
        private readonly Organisation organisation;
        private readonly Aatf aatf;
        private readonly Scheme scheme;
        private readonly Note note;

        public GetAatfNotesRequestHandlerTests()
        {
            fixture = new Fixture();
            weeeAuthorization = A.Fake<IWeeeAuthorization>();
            aatfDataAccess = A.Fake<IAatfDataAccess>();
            mapper = A.Fake<IMapper>();

            organisation = A.Fake<Organisation>();
            aatf = A.Fake<Aatf>();
            scheme = A.Fake<Scheme>();
            note = A.Fake<Note>();
    
            A.CallTo(() => note.Reference).Returns(1);
            A.CallTo(() => scheme.Id).Returns(fixture.Create<Guid>());
            A.CallTo(() => organisation.Id).Returns(fixture.Create<Guid>());
            A.CallTo(() => aatf.Id).Returns(fixture.Create<Guid>());
            A.CallTo(() => aatf.Organisation).Returns(organisation);

            request = new GetAatfNotesRequest(organisation.Id,
                aatf.Id);

            handler = new GetAatfNotesRequestHandler(weeeAuthorization,
                aatfDataAccess,
                mapper);

            A.CallTo(() => aatfDataAccess.GetDetails(aatf.Id)).Returns(aatf);
        }

        [Fact]
        public async Task HandleAsync_GivenNoExternalAccess_ShouldThrowSecurityException()
        {
            //arrange
            var authorization = new AuthorizationBuilder().DenyExternalAreaAccess().Build();

            handler = new GetAatfNotesRequestHandler(authorization, aatfDataAccess, mapper);

            //act
            var result = await Record.ExceptionAsync(() => handler.HandleAsync(GetAatfNotesRequest()));

            //assert
            result.Should().BeOfType<SecurityException>();
        }

        [Fact]
        public async Task HandleAsync_GivenNoOrganisationAccess_ShouldThrowSecurityException()
        {
            //arrange
            var authorization = new AuthorizationBuilder().DenyOrganisationAccess().Build();
            handler = new GetAatfNotesRequestHandler(authorization, aatfDataAccess, mapper);

            //act
            var result = await Record.ExceptionAsync(() => handler.HandleAsync(GetAatfNotesRequest()));

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
        public async void HandleAsync_GivenRequest_AatfDataAccessShouldBeCalledOnce()
        {
            // act
            await handler.HandleAsync(request);

            // assert
            A.CallTo(() => aatfDataAccess.GetAllNotes(A<Guid>._, A<Guid>._, A<List<int>>._)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async void HandleAsync_GivenNotesData_ReturnedNotesDataShouldBeMapped()
        {
            // arrange
            var noteList = fixture.CreateMany<Note>().ToList();

            A.CallTo(() => aatfDataAccess.GetAllNotes(A<Guid>._, A<Guid>._, A<List<int>>._)).Returns(noteList);

            // act
            await handler.HandleAsync(request);

            // assert
            A.CallTo(() => mapper.Map<ListOfEvidenceNoteDataMap>(A<ListOfNotesMap>.That.Matches(a => a.ListOfNotes.Equals(noteList)))).MustHaveHappenedOnceExactly();
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

            A.CallTo(() => aatfDataAccess.GetAllNotes(A<Guid>._, A<Guid>._, A<List<int>>._)).Returns(noteList);

            A.CallTo(() => mapper.Map<ListOfEvidenceNoteDataMap>(A<ListOfNotesMap>._)).Returns(listOfEvidenceNotes);

            // act
            var result = await handler.HandleAsync(GetAatfNotesRequest());

            // assert
            result.Should().BeOfType<List<EvidenceNoteData>>();
            result.Count().Should().Be(evidenceNoteDatas.Count);
        }

        private GetAatfNotesRequest GetAatfNotesRequest()
        {
            return new GetAatfNotesRequest(organisation.Id, aatf.Id);
        }
    }
}
