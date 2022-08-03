namespace EA.Weee.RequestHandlers.Tests.Unit.Admin.ManageEvidenceNotes
{
    using AutoFixture;
    using Core.Helpers;
    using DataAccess.DataAccess;
    using Domain.Evidence;
    using EA.Weee.RequestHandlers.Admin;
    using EA.Weee.RequestHandlers.Security;
    using EA.Weee.Requests.Admin;
    using EA.Weee.Tests.Core;
    using FakeItEasy;
    using FluentAssertions;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security;
    using System.Threading.Tasks;
    using Xunit;
    using NoteStatus = Core.AatfEvidence.NoteStatus;

    public class GetComplianceYearsFilterHandlerTests : SimpleUnitTestBase
    {
        private GetComplianceYearsFilterHandler handler;
        private readonly IWeeeAuthorization weeeAuthorization;
        private readonly IEvidenceDataAccess evidenceDataAccess;
        private readonly GetComplianceYearsFilter request;

        public GetComplianceYearsFilterHandlerTests()
        {
            weeeAuthorization = A.Fake<IWeeeAuthorization>();
            evidenceDataAccess = A.Fake<IEvidenceDataAccess>();
            request = new GetComplianceYearsFilter(TestFixture.CreateMany<NoteStatus>().ToList());

            handler = new GetComplianceYearsFilterHandler(weeeAuthorization, evidenceDataAccess);
        }

        [Fact]
        public async Task HandleAsync_GivenNoInternalAccess_ShouldThrowSecurityException()
        {
            //arrange
            var noAccessauthorization = new AuthorizationBuilder().DenyInternalAreaAccess().Build();

            handler = new GetComplianceYearsFilterHandler(noAccessauthorization, evidenceDataAccess);

            //act
            var result = await Record.ExceptionAsync(() => handler.HandleAsync(TestFixture.Create<GetComplianceYearsFilter>()));

            //assert
            result.Should().BeOfType<SecurityException>();
        }

        [Fact]
        public async Task HandleAsync_GivenRequest_ShouldCallEnsureCanAccessInternalArea()
        {
            //act
            await handler.HandleAsync(request);

            //assert
            A.CallTo(() => weeeAuthorization.EnsureCanAccessInternalArea())
                .MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async void HandleAsync_GivenRequest_EvidenceDataAccessShouldBeCalledOnce()
        {
            // act
            await handler.HandleAsync(request);

            var allowedStatuses = request.AllowedStatuses
                .Select(a => a.ToDomainEnumeration<Domain.Evidence.NoteStatus>()).ToList();

            var allowedStatusesIntList = allowedStatuses.Select(v => v.Value).ToList();

            // assert
            A.CallTo(() => evidenceDataAccess.GetComplianceYearsForNotes(A<List<int>>.That.Matches(e =>
                e.SequenceEqual(allowedStatusesIntList)))).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async void HandleAsync_GivenNotesWithComplianceYear_ShouldReturnOrderedListOfComplianceYears()
        {
            // arrange
            var note1 = A.Fake<Note>();
            var complianceYearNote1 = DateTime.Now.Year;

            var note2 = A.Fake<Note>();
            var complianceYearNote2 = DateTime.Now.Year + 1;

            var note3 = A.Fake<Note>();
            var complianceYearNote3 = DateTime.Now.Year - 2;

            var note4 = A.Fake<Note>();
            var complianceYearNote4 = DateTime.Now.Year - 1;

            A.CallTo(() => note1.Reference).Returns(1);
            A.CallTo(() => note1.ComplianceYear).Returns(complianceYearNote1);
            A.CallTo(() => note2.Reference).Returns(2);
            A.CallTo(() => note2.ComplianceYear).Returns(complianceYearNote2);
            A.CallTo(() => note3.Reference).Returns(3);
            A.CallTo(() => note3.ComplianceYear).Returns(complianceYearNote3);
            A.CallTo(() => note4.Reference).Returns(4);
            A.CallTo(() => note4.ComplianceYear).Returns(complianceYearNote4);

            var complianceYearList = new List<int>()
            {
                complianceYearNote2,
                complianceYearNote1,
                complianceYearNote4,
                complianceYearNote3
            };

            A.CallTo(() => evidenceDataAccess.GetComplianceYearsForNotes(A<List<int>>._)).Returns(complianceYearList);

            // act
            var result = await handler.HandleAsync(request);
            var resultToList = result.ToList();

            // assert
            resultToList.Should().NotBeEmpty();
            resultToList[0].Should().Be(note2.ComplianceYear);
            resultToList[1].Should().Be(note1.ComplianceYear);
            resultToList[2].Should().Be(note4.ComplianceYear);
            resultToList[3].Should().Be(note3.ComplianceYear);
        }

        [Fact]
        public async void HandleAsync_GivenNoComplianceYears_ShouldReturnEmptyList()
        {
            // arrange
            var newRequest = new GetComplianceYearsFilter(new List<NoteStatus> { NoteStatus.Approved, NoteStatus.Submitted, NoteStatus.Returned});

            A.CallTo(() => evidenceDataAccess.GetComplianceYearsForNotes(A<List<int>>._)).Returns(new List<int>());

            // act
            var result = await handler.HandleAsync(request);

            // assert
            result.Should().BeEmpty();
            result.Should().NotBeNull();
        }
    }
}
