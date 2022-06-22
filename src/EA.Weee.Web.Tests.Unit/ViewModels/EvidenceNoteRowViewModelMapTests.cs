namespace EA.Weee.Web.Tests.Unit.ViewModels
{
    using System;
    using AutoFixture;
    using Core.AatfEvidence;
    using Core.AatfReturn;
    using Core.Organisations;
    using Core.Scheme;
    using EA.Weee.Core.Shared;
    using FluentAssertions;
    using Web.ViewModels.Shared.Mapping;
    using Xunit;

    public class EvidenceNoteRowViewModelMapTests
    {
        private readonly EvidenceNoteRowViewModelMap map;
        private readonly Fixture fixture;

        public EvidenceNoteRowViewModelMapTests()
        {
            map = new EvidenceNoteRowViewModelMap();

            fixture = new Fixture();
        }

        [Fact]
        public void Map_GivenEvidenceNoteDataWithSubmittedDate_PropertiesShouldBeMapped()
        {
            //arrange
            var schemeName = fixture.Create<string>();
            var aatfName = fixture.Create<string>();

            var reference = fixture.Create<int>();
            var status = fixture.Create<NoteStatus>();
            var wasteType = fixture.Create<WasteType>();
            var id = fixture.Create<Guid>();
            var submittedDate = fixture.Create<DateTime>();
            var rejectedDate = fixture.Create<DateTime>();
            var returnedDate = fixture.Create<DateTime>();
            var rejectedReason = fixture.Create<string>();
            var returnedReason = fixture.Create<string>();

            var evidenceNoteData = new EvidenceNoteData(
                new SchemeData() { SchemeName = schemeName }, new AatfData() { Name = aatfName })
            {
                Reference = reference,
                Status = status,
                WasteType = wasteType,
                Id = id,
                Type = NoteType.Evidence,
                SubmittedDate = submittedDate,
                RejectedDate = rejectedDate,
                ReturnedDate = returnedDate,
                ReturnedReason = returnedReason,
                RejectedReason = rejectedReason
            };

            //act
            var result = map.Map(evidenceNoteData);

            //assert
            result.Recipient.Should().Be(schemeName);
            result.ReferenceId.Should().Be(reference);
            result.Status.Should().Be(status);
            result.TypeOfWaste.Should().Be(wasteType);
            result.Id.Should().Be(id);
            result.Type.Should().Be(NoteType.Evidence);
            result.SubmittedDate.Should().Be(submittedDate);
            result.SubmittedBy.Should().Be(aatfName);
            result.RejectedDate.Should().Be(rejectedDate);
            result.ReturnedDate.Should().Be(returnedDate);
            result.ReturnedReason.Should().Be(returnedReason);
            result.RejectedReason.Should().Be(rejectedReason);
        }

        [Fact]
        public void Map_GivenEvidenceNoteDataWithNoSubmittedDate_PropertiesShouldBeMapped()
        {
            //arrange
            var evidenceNoteData = new EvidenceNoteData() { SchemeData = new SchemeData() };

            //act
            var result = map.Map(evidenceNoteData);

            //assert
            result.SubmittedBy.Should().BeEmpty();
        }

        [Fact]
        public void Map_GivenTransferNoteData_SubmittedByShouldBeSet()
        {
            //arrange
            var organisationName = fixture.Create<string>();
            var evidenceNoteData = new EvidenceNoteData() 
            { 
                SchemeData = new SchemeData(), 
                OrganisationData = new OrganisationData() { OrganisationName = organisationName },
                Type = NoteType.Transfer
            };

            //act
            var result = map.Map(evidenceNoteData);

            //assert
            result.SubmittedBy.Should().Be(organisationName);
        }
    }
}
