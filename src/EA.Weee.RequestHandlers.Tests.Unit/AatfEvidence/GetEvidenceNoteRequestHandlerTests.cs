﻿namespace EA.Weee.RequestHandlers.Tests.Unit.AatfEvidence
{
    using System;
    using System.Security;
    using System.Threading.Tasks;
    using AutoFixture;
    using Castle.Core.Internal;
    using Core.AatfEvidence;
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
    using RequestHandlers.Aatf;
    using RequestHandlers.AatfEvidence;
    using RequestHandlers.Security;
    using Weee.Requests.AatfEvidence;
    using Weee.Tests.Core;
    using Xunit;

    public class GetEvidenceNoteRequestHandlerTests : SimpleUnitTestBase
    {
        private GetEvidenceNoteRequestHandler handler;
        private readonly IWeeeAuthorization weeeAuthorization;
        private readonly IEvidenceDataAccess evidenceDataAccess;
        private readonly IMapper mapper;
        private readonly ISystemDataDataAccess systemDataAccess;
        private readonly GetEvidenceNoteForAatfRequest request;
        private readonly Note note;
        private readonly Guid evidenceNoteId;
        private readonly Guid organisationId;

        public GetEvidenceNoteRequestHandlerTests()
        {
            weeeAuthorization = A.Fake<IWeeeAuthorization>();
            evidenceDataAccess = A.Fake<IEvidenceDataAccess>();
            systemDataAccess = A.Fake<ISystemDataDataAccess>();
            mapper = A.Fake<IMapper>();

            A.Fake<IAatfDataAccess>();
            A.Fake<IUserContext>();

            A.Fake<Organisation>();
            A.Fake<Aatf>();
            A.Fake<Scheme>();
            note = A.Fake<Note>();
            evidenceNoteId = TestFixture.Create<Guid>();
            organisationId = TestFixture.Create<Guid>();

            A.CallTo(() => note.OrganisationId).Returns(organisationId);

            request = new GetEvidenceNoteForAatfRequest(evidenceNoteId);

            handler = new GetEvidenceNoteRequestHandler(weeeAuthorization,
                evidenceDataAccess,
                mapper,
                systemDataAccess);

            A.CallTo(() => evidenceDataAccess.GetNoteById(evidenceNoteId)).Returns(note);
        }

        [Fact]
        public async Task HandleAsync_GivenNoExternalAccess_ShouldThrowSecurityException()
        {
            //arrange
            var authorization = new AuthorizationBuilder().DenyExternalAreaAccess().Build();

            handler = new GetEvidenceNoteRequestHandler(authorization,
                evidenceDataAccess,
                mapper,
                systemDataAccess);

            //act
            var result = await Record.ExceptionAsync(() => handler.HandleAsync(request));

            //assert
            result.Should().BeOfType<SecurityException>();
        }

        [Fact]
        public async Task HandleAsync_GivenNoOrganisationAccess_ShouldThrowSecurityException()
        {
            //arrange
            var authorization = new AuthorizationBuilder().DenyOrganisationAccess().Build();
           
            handler = new GetEvidenceNoteRequestHandler(authorization,
                evidenceDataAccess,
                mapper,
                systemDataAccess);

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
        public async Task HandleAsync_GivenRequest_SystemDateTimeShouldBeRetrieved()
        {
            //act
            await handler.HandleAsync(request);

            //assert
            A.CallTo(() => systemDataAccess.GetSystemDateTime()).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task HandleAsync_GivenRequest_ShouldCheckOrganisationAccess()
        {
            //act
            await handler.HandleAsync(request);

            //assert
            A.CallTo(() => weeeAuthorization.EnsureOrganisationAccess(organisationId))
                .MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task HandleAsync_GivenRequest_NoteShouldBeMapped()
        {
            //arrange
            var dateTime = TestFixture.Create<DateTime>();
            A.CallTo(() => systemDataAccess.GetSystemDateTime()).Returns(dateTime);

            //act
            await handler.HandleAsync(request);

            //assert
            A.CallTo(() => mapper.Map<EvidenceNoteWithCriteriaMapper, EvidenceNoteData>(A<EvidenceNoteWithCriteriaMapper>.That.Matches(e => e.Note.Equals(note) && 
                e.CategoryFilter.Count == 0 && 
                e.IncludeTonnage == true &&
                e.SystemDateTime == dateTime &&
                e.IncludeHistory == false &&
                e.IncludeTotal == false))).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task HandleAsync_GivenRequestAndNote_MappedNoteShouldBeReturned()
        {
            //arrange
            var evidenceNote = TestFixture.Create<EvidenceNoteData>();

            A.CallTo(() => mapper.Map<EvidenceNoteWithCriteriaMapper, EvidenceNoteData>(A<EvidenceNoteWithCriteriaMapper>._)).Returns(evidenceNote);

            //act
            var result = await handler.HandleAsync(request);

            //assert
            result.Should().Be(evidenceNote);
        }
    }
}
