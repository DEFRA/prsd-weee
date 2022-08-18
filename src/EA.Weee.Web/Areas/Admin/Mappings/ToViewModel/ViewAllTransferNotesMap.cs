namespace EA.Weee.Web.Areas.Admin.Mappings.ToViewModel
{
    using EA.Prsd.Core;
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Web.Areas.Admin.ViewModels.ManageEvidenceNotes;
    using EA.Weee.Web.ViewModels.Shared.Mapping;
    using Services;

    public class ViewAllTransferNotesMap : ListOfNotesViewModelBase<ViewAllTransferNotesViewModel>, IMap<ViewEvidenceNotesMapTransfer, ViewAllTransferNotesViewModel>
    {
        public ViewAllTransferNotesMap(IMapper mapper) : base(mapper)
        {
        }

        public ViewAllTransferNotesViewModel Map(ViewEvidenceNotesMapTransfer source)
        {
            Guard.ArgumentNotNull(() => source, source);

            var model = MapBase(source.NoteData, source.CurrentDate, source.ManageEvidenceNoteViewModel, source.PageNumber, source.PageSize, source.ComplianceYearList);

            return model;
        }
    }
}
