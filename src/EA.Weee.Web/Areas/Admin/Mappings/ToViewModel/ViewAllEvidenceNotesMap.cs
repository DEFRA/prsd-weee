namespace EA.Weee.Web.Areas.Admin.Mappings.ToViewModel
{
    using EA.Prsd.Core;
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Web.Areas.Admin.ViewModels.ManageEvidenceNotes;
    using EA.Weee.Web.ViewModels.Shared.Mapping;

    public class ViewAllEvidenceNotesMap : ListOfNotesViewModelBase<ViewAllEvidenceNotesViewModel>, IMap<ViewAllEvidenceNotesMapTransfer, ViewAllEvidenceNotesViewModel>
    {
        public ViewAllEvidenceNotesMap(IMapper mapper) : base(mapper)
        {
        }

        public ViewAllEvidenceNotesViewModel Map(ViewAllEvidenceNotesMapTransfer source)
        {
            Guard.ArgumentNotNull(() => source, source);

            var model = MapBase(source.NoteData, source.CurrentDate, source.ManageEvidenceNoteViewModel, source.GetCurrentComplianceList);

            return model;
        }
    }
}
