namespace EA.Weee.Web.Areas.Admin.Mappings.ToViewModel
{
    using EA.Prsd.Core;
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Web.Areas.Admin.ViewModels.ManageEvidenceNotes;
    using EA.Weee.Web.ViewModels.Shared.Mapping;

    public class ViewAllTransferNotesMap : ListOfNotesViewModelBase<ViewAllTransferNotesViewModel>, IMap<ViewAllEvidenceNotesMapTransfer, ViewAllTransferNotesViewModel>
    {
        public ViewAllTransferNotesMap(IMapper mapper) : base(mapper)
        {
        }

        public ViewAllTransferNotesViewModel Map(ViewAllEvidenceNotesMapTransfer source)
        {
            Guard.ArgumentNotNull(() => source, source);

            var model = MapBase(source.NoteData, source.CurrentDate, source.ManageEvidenceNoteViewModel, source.GetCurrentComplianceList);

            return model;
        }
    }
}
