namespace EA.Weee.RequestHandlers.Tests.Unit.Admin.ManageEvidenceNotes
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security;
    using System.Threading.Tasks;
    using AutoFixture;
    using Core.Helpers;
    using DataAccess.DataAccess;
    using Domain.Evidence;
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Core.Shared;
    using EA.Weee.RequestHandlers.Admin;
    using EA.Weee.RequestHandlers.Security;
    using EA.Weee.Requests.Admin;
    using EA.Weee.Tests.Core;
    using FakeItEasy;
    using FluentAssertions;
    using Xunit;
    using AatfDomain = Domain.AatfReturn.Aatf;

    public class GetAllAatfsForComplianceYearHandlerTests : SimpleUnitTestBase
    {
        private GetAllAatfsForComplianceYearHandler handler;
        private readonly IWeeeAuthorization weeeAuthorization;
        private readonly IEvidenceDataAccess evidenceNoteDataAccess;
        private readonly IMapper mapper;
        private readonly GetAllAatfsForComplianceYearRequest request;

        public GetAllAatfsForComplianceYearHandlerTests()
        {
            weeeAuthorization = A.Fake<IWeeeAuthorization>();
            evidenceNoteDataAccess = A.Fake<IEvidenceDataAccess>();
            mapper = A.Fake<IMapper>();

            request = new GetAllAatfsForComplianceYearRequest(TestFixture.Create<int>());

            handler = new GetAllAatfsForComplianceYearHandler(weeeAuthorization, evidenceNoteDataAccess, mapper);
        }

        [Fact]
        public async Task HandleAsync_GivenNoInternalAccess_ShouldThrowSecurityException()
        {
            //arrange
            var authorization = new AuthorizationBuilder().DenyInternalAreaAccess().Build();

            handler = new GetAllAatfsForComplianceYearHandler(authorization, evidenceNoteDataAccess, mapper);

            //act
            var result = await Record.ExceptionAsync(() => handler.HandleAsync(request));

            //assert
            result.Should().BeOfType<SecurityException>();
        }

        [Fact]
        public async Task HandleAsync_GivenRequestWithComplianceYear_ShouldCheckInternalAccess()
        {
            //act
            await handler.HandleAsync(request);

            //assert
            A.CallTo(() => weeeAuthorization.EnsureCanAccessInternalArea()).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async void HandleAsync_GivenRequest_EvidenceDataAccessShouldBeCalledOnce()
        {
            // act
            await handler.HandleAsync(request);

            // assert
            A.CallTo(() => evidenceNoteDataAccess.GetAatfForAllNotesForComplianceYear(request.ComplianceYear, 
                A<List<NoteStatus>>.That.IsSameSequenceAs(request.AllowedStatuses.Select(e => e.ToDomainEnumeration<NoteStatus>()).ToList()))).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async void HandleAsync_GivenNullListOfAatfs_ArgumentExceptionShouldBeThrown()
        {
            // arrange
            A.CallTo(() => evidenceNoteDataAccess.GetAatfForAllNotesForComplianceYear(request.ComplianceYear, A<List<NoteStatus>>._)).Returns<List<AatfDomain>>(null);

            // act
            var result = await Record.ExceptionAsync(() => handler.HandleAsync(request));

            // assert
            result.Should().BeOfType<ArgumentException>();
        }

        [Fact]
        public async void HandleAsync_GivenRequest_MapperShouldBeCalled()
        {
            // arrange
            var aatf1 = TestFixture.Create<AatfDomain>();
            var aatf2 = TestFixture.Create<AatfDomain>();
            var aatf3 = TestFixture.Create<AatfDomain>();

            var aatfs = new List<AatfDomain> { aatf1, aatf2, aatf3 };

            A.CallTo(() => evidenceNoteDataAccess.GetAatfForAllNotesForComplianceYear(request.ComplianceYear, A<List<NoteStatus>>._)).Returns(aatfs);

            // act
            await handler.HandleAsync(request);

            // assert
            A.CallTo(() => mapper.Map<List<AatfDomain>, List<EntityIdDisplayNameData>>(A<List<AatfDomain>>
                .That.IsSameSequenceAs(aatfs))).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async void HandleAsync_GivenRequest_ReturnedResultShouldBeOrdered()
        {
            // arrange
            var aatf1 = TestFixture.Create<AatfDomain>();
            var aatf2 = TestFixture.Create<AatfDomain>();
            var aatf3 = TestFixture.Create<AatfDomain>();

            var aatfs = new List<AatfDomain> { aatf1, aatf2, aatf3 };

            var listOrganisationSchemeData = new List<EntityIdDisplayNameData>();

            foreach (var aatf in aatfs)
            {
                listOrganisationSchemeData.Add(new EntityIdDisplayNameData { Id = aatf.Id, DisplayName = aatf.Name });
            }

            listOrganisationSchemeData.OrderBy(d => d.DisplayName);

            A.CallTo(() => evidenceNoteDataAccess.GetAatfForAllNotesForComplianceYear(request.ComplianceYear,
                    A<List<NoteStatus>>._)).Returns(aatfs);

              A.CallTo(() => mapper.Map<List<AatfDomain>, List<EntityIdDisplayNameData>>(A<List<AatfDomain>>._))
                .Returns(listOrganisationSchemeData);

            // act
            var result = await handler.HandleAsync(request);

            // assert
            result.Count.Should().Be(3);
            result.Should().BeEquivalentTo(listOrganisationSchemeData);
        }
    }
}