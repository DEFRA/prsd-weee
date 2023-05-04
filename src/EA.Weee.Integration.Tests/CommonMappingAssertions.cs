﻿namespace EA.Weee.Integration.Tests
{
    using Core.AatfEvidence;
    using Core.Helpers;
    using Core.Scheme;
    using Domain.Evidence;
    using Domain.Scheme;
    using FluentAssertions;

    public static class CommonMappingAssertions
    {
        public static void ShouldMapScheme(this SchemeData schemeData, Scheme scheme)
        {
            schemeData.SchemeName.Should().Be(scheme.SchemeName);
            schemeData.Address.Id.Should().Be(scheme.AddressId.Value);
            schemeData.IbisCustomerReference.Should().Be(scheme.IbisCustomerReference);
            schemeData.ApprovalName.Should().Be(scheme.ApprovalNumber);
            ((int)schemeData.ObligationType.Value).Should().Be((int)scheme.ObligationType);
            schemeData.CompetentAuthorityId.Should().Be(scheme.CompetentAuthorityId);
            schemeData.Contact.Id.Should().Be(scheme.ContactId.Value);
            if (scheme.Organisation != null)
            {
                schemeData.Name.Should().Be(scheme.Organisation.Name);
            }
            schemeData.OrganisationId.Should().Be(scheme.OrganisationId);
            ((int)schemeData.SchemeStatus).Should().Be((int)scheme.SchemeStatus.Value);
        }

        public static void ShouldMapToNote(this EvidenceNoteData result, Note note)
        {
            result.EndDate.Date.Should().Be(note.EndDate.Date);
            result.StartDate.Date.Should().Be(note.StartDate.Date);
            result.Reference.Should().Be(note.Reference);
            result.Protocol.ToInt().Should().Be(note.Protocol.ToInt());
            result.WasteType.ToInt().Should().Be(note.WasteType.ToInt());
            result.ComplianceYear.ToInt().Should().Be(note.ComplianceYear);
            result.AatfData.Should().NotBeNull();
            result.AatfData.Id.Should().Be(note.Aatf.Id);
            result.RecipientSchemeData.Should().NotBeNull();
            result.RecipientSchemeData.Id.Should().Be(note.Recipient.Scheme.Id);
            result.EvidenceTonnageData.Count.Should().BeGreaterThan(0);
            foreach (var evidenceTonnageData in result.EvidenceTonnageData)
            {
                note.NoteTonnage.Should().Contain(n => n.Received.Equals(evidenceTonnageData.Received) &&
                                                       n.Reused.Equals(evidenceTonnageData.Reused) &&
                                                       ((int)n.CategoryId).Equals(
                                                           evidenceTonnageData.CategoryId.ToInt()));
            }
            result.OrganisationData.Should().NotBeNull();
            result.OrganisationData.Id.Should().Be(note.Organisation.Id);
            ((int)result.Type).Should().Be(note.NoteType.Value);
            result.Id.Should().Be(note.Id);
            result.RecipientOrganisationData.Should().NotBeNull();
            result.RecipientOrganisationData.Id.Should().Be(note.Recipient.Id);
        }

        public static void ShouldMapToCutDownEvidenceNote(this EvidenceNoteData result, Note note)
        {
            result.EndDate.Date.Should().Be(note.EndDate.Date);
            result.StartDate.Date.Should().Be(note.StartDate.Date);
            result.Reference.Should().Be(note.Reference);
            result.Protocol.ToInt().Should().Be(note.Protocol.ToInt());
            result.WasteType.ToInt().Should().Be(note.WasteType.ToInt());
            result.ComplianceYear.ToInt().Should().Be(note.ComplianceYear);
            result.AatfData.Should().NotBeNull();
            result.AatfData.Name.Should().Be(note.Aatf.Name);
            result.RecipientSchemeData.Should().NotBeNull();
            result.RecipientSchemeData.SchemeName.Should().Be(note.Recipient.Scheme.SchemeName);
            result.EvidenceTonnageData.Should().BeNull();
            result.OrganisationData.Should().NotBeNull();
            result.OrganisationData.Name.Should().Be(note.Organisation.Name);
            ((int)result.Type).Should().Be(note.NoteType.Value);
            result.Id.Should().Be(note.Id);
            result.RecipientOrganisationData.Should().NotBeNull();
            result.RecipientOrganisationData.Name.Should().Be(note.Recipient.Name);
        }
    }
}
