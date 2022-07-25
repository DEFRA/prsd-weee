namespace EA.Weee.Web.Tests.Unit.ViewModels
{
    using System;
    using AutoFixture;
    using Core.AatfEvidence;
    using Core.AatfReturn;
    using Core.Organisations;
    using Core.Scheme;
    using FluentAssertions;
    using Web.ViewModels.Shared.Mapping;
    using Weee.Tests.Core;
    using Xunit;

    public class EvidenceNoteRowViewModelMapTests : SimpleUnitTestBase
    {
        private readonly EvidenceNoteRowViewModelMap map;

        public EvidenceNoteRowViewModelMapTests()
        {
            map = new EvidenceNoteRowViewModelMap();
        }

        [Fact]
        public void Map_GivenEvidenceNoteData_PropertiesShouldBeMapped()
        {
            //arrange
            var schemeName = TestFixture.Create<string>();
            var aatfName = TestFixture.Create<string>();

            var reference = TestFixture.Create<int>();
            var status = TestFixture.Create<NoteStatus>();
            var wasteType = TestFixture.Create<WasteType>();
            var id = TestFixture.Create<Guid>();
            var submittedDate = TestFixture.Create<DateTime>();
            var rejectedDate = TestFixture.Create<DateTime>();
            var returnedDate = TestFixture.Create<DateTime>();
            var rejectedReason = TestFixture.Create<string>();
            var returnedReason = TestFixture.Create<string>();

            var evidenceNoteData = new EvidenceNoteData(
                new SchemeData() { SchemeName = schemeName }, 
                new AatfData() { Name = aatfName })
                {
                RecipientOrganisationData = new OrganisationData(),
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
        public void Map_GivenEvidenceNoteDataWithOrganisationAsPbs_PropertiesShouldBeMapped()
        {
            //arrange
            var organisationName = TestFixture.Create<string>();
            var aatfName = TestFixture.Create<string>();
            var reference = TestFixture.Create<int>();
            var status = TestFixture.Create<NoteStatus>();
            var wasteType = TestFixture.Create<WasteType>();
            var id = TestFixture.Create<Guid>();
            var submittedDate = TestFixture.Create<DateTime>();
            var rejectedDate = TestFixture.Create<DateTime>();
            var returnedDate = TestFixture.Create<DateTime>();
            var rejectedReason = TestFixture.Create<string>();
            var returnedReason = TestFixture.Create<string>();

            var evidenceNoteData = new EvidenceNoteData(
                new SchemeData() { SchemeName = TestFixture.Create<string>() },
                new AatfData() { Name = aatfName })
            {
                RecipientOrganisationData = new OrganisationData()
                {
                    IsBalancingScheme = true,
                    OrganisationName = organisationName
                },
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
            result.Recipient.Should().Be(organisationName);
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
            var evidenceNoteData = new EvidenceNoteData() { RecipientOrganisationData = new OrganisationData(), RecipientSchemeData = new SchemeData() };

            //act
            var result = map.Map(evidenceNoteData);

            //assert
            result.SubmittedBy.Should().BeEmpty();
        }

        [Fact]
        public void Map_GivenTransferNoteData_SubmittedByShouldBeSet_ToSchemeName()
        {
            //arrange
            var schemeName = TestFixture.Create<string>();
            var evidenceNoteData = new EvidenceNoteData() 
            { 
                OrganisationSchemaData = new SchemeData() { SchemeName = schemeName},
                RecipientOrganisationData = new OrganisationData(),
                RecipientSchemeData = new SchemeData(),
                Type = NoteType.Transfer
            };

            //act
            var result = map.Map(evidenceNoteData);

            //assert
            result.SubmittedBy.Should().Be(schemeName);
        }

        [Fact]
        public void Map_GivenTransferNoteData_AndNoOrganisationSchemeData_SubmittedByShouldBeSet_ToOrganisationName()
        {
            //arrange
            var organisationName = fixture.Create<string>();
            var evidenceNoteData = new EvidenceNoteData()
            {
                OrganisationData = new OrganisationData() { OrganisationName = organisationName },
                RecipientSchemeData = new SchemeData(),
                Type = NoteType.Transfer
            };

            //act
            var result = map.Map(evidenceNoteData);

            //assert
            result.SubmittedBy.Should().Be(organisationName);
        }
    }
}
