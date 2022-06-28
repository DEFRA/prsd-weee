namespace EA.Weee.Web.Areas.Admin.Mappings.ToViewModel
{
    using EA.Prsd.Core;
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Core.Admin;
    using EA.Weee.Web.ViewModels.Shared;

    public class AdminEvidenceNoteDataMap : IMap<AdminEvidenceNoteData, EvidenceNoteRowViewModel>
    {
        public EvidenceNoteRowViewModel Map(AdminEvidenceNoteData source)
        {
            Guard.ArgumentNotNull(() => source, source);

            return new EvidenceNoteRowViewModel
            {
                Recipient = source.SchemeData.SchemeName,
                ReferenceId = source.ReferenceId,
                Status = source.Status,
                Id = source.Id,
                Type = source.Type,
                SubmittedDate = source.SubmittedDate,
                SubmittedBy = source.SubmittedDate.HasValue ? source.AatfData.Name : string.Empty,
            };
        }
    }
}
