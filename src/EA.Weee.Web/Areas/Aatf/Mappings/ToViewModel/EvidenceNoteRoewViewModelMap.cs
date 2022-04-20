namespace EA.Weee.Web.Areas.Aatf.Mappings.ToViewModel
{
    using Core.AatfEvidence;
    using EA.Weee.Web.Areas.Aatf.ViewModels;
    using Prsd.Core.Mapper;
    using Web.ViewModels.Shared;

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
                SubmittedBy = source.SubmittedDate.HasValue ? source.AatfData.Name : string.Empty
            };
        }
    }
}
