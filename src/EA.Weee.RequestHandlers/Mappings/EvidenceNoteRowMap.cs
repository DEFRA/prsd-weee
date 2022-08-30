namespace EA.Weee.RequestHandlers.Mappings
{
    using System.Linq;
    using Core.AatfReturn;
    using Core.Helpers;
    using Core.Organisations;
    using Core.Scheme;
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Core.AatfEvidence;
    using NoteStatus = Core.AatfEvidence.NoteStatus;
    using NoteType = Core.AatfEvidence.NoteType;

    public class EvidenceNoteRowMap : IMap<EvidenceNoteRowMapperObject, EvidenceNoteData>
    {
        public EvidenceNoteData Map(EvidenceNoteRowMapperObject source)
        {
            var data = new EvidenceNoteData
            {
                Id = source.Note.Id,
                ComplianceYear = source.Note.ComplianceYear,
                Type = source.Note.NoteType.ToCoreEnumeration<NoteType>(),
                StartDate = source.Note.StartDate,
                EndDate = source.Note.EndDate,
                SubmittedDate = source.Note.NoteStatusHistory
                    .Where(n => n.ToStatus.Equals(Domain.Evidence.NoteStatus.Submitted))
                    .OrderByDescending(n => n.ChangedDate).FirstOrDefault()
                    ?.ChangedDate,
                Reference = source.Note.Reference,
                RecipientOrganisationData = new OrganisationData()
                {
                    IsBalancingScheme = source.Note.Recipient.ProducerBalancingScheme != null,
                    Name = source.Note.Recipient.Name,
                    TradingName = source.Note.Recipient.TradingName,
                    OrganisationName = source.Note.Recipient.OrganisationName,
                },
                Status = (NoteStatus)source.Note.Status.Value,
                Protocol = source.Note.Protocol.HasValue ? (Protocol?)source.Note.Protocol.Value : null,
                WasteType = source.Note.WasteType.HasValue
                    ? (WasteType?)source.Note.WasteType.Value
                    : null,
                OrganisationData = new OrganisationData()
                {
                    Name = source.Note.Organisation.Name,
                    TradingName = source.Note.Organisation.TradingName,
                    OrganisationName = source.Note.Organisation.OrganisationName
                }
            };

            if (source.Note.Aatf != null)
            {
                data.AatfData = new AatfData()
                {
                    Name = source.Note.Aatf.Name
                };
            }
            if (source.Note.Organisation.Scheme != null)
            {
                data.OrganisationSchemaData = new SchemeData()
                {
                    SchemeName = source.Note.Organisation.Scheme.SchemeName
                };
            }

            if (source.Note.Recipient.Scheme != null)
            {
                data.RecipientSchemeData = new SchemeData()
                {
                    SchemeName = source.Note.Recipient.Scheme.SchemeName
                };
            }
            
            return data;
        }
    }
}
