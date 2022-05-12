namespace EA.Weee.Web.ViewModels.Shared.Mapping
{
    using Core.AatfEvidence;
    using Prsd.Core.Mapper;
    using Shared;

    public class EvidenceNoteRowViewModelMap : IMap<EvidenceNoteData, EvidenceNoteRowViewModel>
    {
        public EvidenceNoteRowViewModel Map(EvidenceNoteData source)
        {
            return new EvidenceNoteRowViewModel
            {
                Recipient = source.SchemeData.SchemeName,
                ReferenceId = source.Reference,
                Status = source.Status,
                TypeOfWaste = source.WasteType,
                Id = source.Id,
                Type = source.Type,
                SubmittedDate = source.SubmittedDate,
                SubmittedBy = source.SubmittedDate.HasValue ? source.AatfData.Name : string.Empty,
                ReturnedDate = source.ReturnedDate,
                Reason = source.ReturnedReason
            };
        }
    }
}
