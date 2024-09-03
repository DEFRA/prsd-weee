namespace EA.Weee.RequestHandlers.Tests.Unit.AatfEvidence.Requests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using AutoFixture;
    using Core.AatfEvidence;
    using FluentAssertions;
    using Weee.Requests.AatfEvidence;
    using Xunit;

    public class GetEvidenceNotesByOrganisationRequestTests
    {
        private readonly Fixture fixture;
        private readonly int complianceYear;

        public GetEvidenceNotesByOrganisationRequestTests()
        {
            fixture = new Fixture();
            complianceYear = fixture.Create<int>();
        }

        [Fact]
        public void GetEvidenceNotesByOrganisationRequest_ShouldBeDerivedFromEvidenceNoteFilterBaseRequestAsBase()
        {
            typeof(GetEvidenceNotesByOrganisationRequest).Should().BeDerivedFrom<EvidenceNoteFilterBaseRequest>();
        }

        [Fact]
        public void GetEvidenceNotesByOrganisationRequest_GivenOrganisationIdIsDefaultValue_ArgumentExceptionExpected()
        {
            //act
            var exception = Record.Exception(() => new GetEvidenceNotesByOrganisationRequest(Guid.Empty, fixture.CreateMany<NoteStatus>().ToList(), complianceYear, 
                                                                                             new List<NoteType>() { NoteType.Evidence }, false, 1, 25, null, null, null, null, 
                                                                                             new List<WasteType>() { WasteType.Household }, null));

            //assert
            exception.Should().BeOfType<ArgumentException>();
        }

        [Fact]
        public void GetEvidenceNotesByOrganisationRequest_GivenNoteStatusListIsNull_ArgumentNullExceptionExpected()
        {
            //act
            var exception = Record.Exception(() => new GetEvidenceNotesByOrganisationRequest(fixture.Create<Guid>(), null, complianceYear, new List<NoteType>() { NoteType.Evidence }, 
                                                                                             false, 1, 25, null, null, null, null, new List<WasteType>() { WasteType.Household }, null));

            //assert
            exception.Should().BeOfType<ArgumentNullException>();
        }

        [Fact]
        public void GetEvidenceNotesByOrganisationRequest_GivenNoteStatusListEmpty_ArgumentExceptionExpected()
        {
            //act
            var exception = Record.Exception(() => new GetEvidenceNotesByOrganisationRequest(fixture.Create<Guid>(), new List<NoteStatus>(), complianceYear, 
                                                                                             new List<NoteType>() { NoteType.Evidence }, false, 1, 25, null, null, null, null, 
                                                                                             new List<WasteType>() { WasteType.Household }, null));

            //assert
            exception.Should().BeOfType<ArgumentException>();
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void GetEvidenceNotesByOrganisationRequest_GivenNComplianceYearIsNotGreaterThanZero_ArgumentOutOfRangeExceptionExpected(int currentYear)
        {
            //act
            var exception = Record.Exception(() => new GetEvidenceNotesByOrganisationRequest(fixture.Create<Guid>(), fixture.CreateMany<NoteStatus>().ToList(), currentYear, 
                                                                                             new List<NoteType>() { NoteType.Evidence }, false, 1, 25, null, null, null, null, 
                                                                                             new List<WasteType>() { WasteType.Household }, null));

            //assert
            exception.Should().BeOfType<ArgumentOutOfRangeException>();
        }

        [Fact]
        public void GetEvidenceNotesByOrganisationRequest_GivenZeroPageSize_ShouldThrowArgumentOutOfRangeException()
        {
            //act
            var exception = Record.Exception(() => new GetEvidenceNotesByOrganisationRequest(fixture.Create<Guid>(), fixture.CreateMany<NoteStatus>().ToList(), 2022, 
                                                                                             new List<NoteType>() { NoteType.Evidence }, false, 1, 0, null, null, null, null, 
                                                                                             new List<WasteType>() { WasteType.Household }, null));

            //assert
            exception.Should().BeOfType<ArgumentOutOfRangeException>();
        }

        [Fact]
        public void GetEvidenceNotesByOrganisationRequest_GivenZeroPageNumber_ShouldThrowArgumentOutOfRangeException()
        {
            //act
            var exception = Record.Exception(() => new GetEvidenceNotesByOrganisationRequest(fixture.Create<Guid>(), fixture.CreateMany<NoteStatus>().ToList(), 2022, 
                                                                                             new List<NoteType>() { NoteType.Evidence }, false, 0, 25, null, null, null, null, 
                                                                                             new List<WasteType>() { WasteType.Household }, null));

            //assert
            exception.Should().BeOfType<ArgumentOutOfRangeException>();
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void GetEvidenceNotesByOrganisationRequest_GivenValues_PropertiesShouldBeSet(bool transferredOut)
        {
            //arrange
            var organisationId = fixture.Create<Guid>();
            var statusList = fixture.CreateMany<NoteStatus>().ToList();
            var searchRef = fixture.Create<string>();

            //act
            var result = new GetEvidenceNotesByOrganisationRequest(organisationId, statusList, complianceYear, new List<NoteType>() { NoteType.Evidence }, transferredOut, 1, 25, 
                                                                    searchRef, null, null, null, new List<WasteType>() { WasteType.Household }, null);

            //assert
            result.OrganisationId.Should().Be(organisationId);
            result.AllowedStatuses.Should().BeEquivalentTo(statusList);
            result.ComplianceYear.Should().Be(complianceYear);
            result.TransferredOut.Should().Be(transferredOut);
            result.SearchRef.Should().Be(searchRef);
        }
    }
}
