namespace EA.Weee.Web.Areas.Admin.Mappings.ToViewModel
{
    using EA.Prsd.Core;
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Web.Areas.Admin.ViewModels.ManageEvidenceNotes;
    using EA.Weee.Web.ViewModels.Shared.Mapping;
    using Services;

    public class ViewAllEvidenceNotesMap : ListOfNotesViewModelBase<ViewAllEvidenceNotesViewModel>, IMap<ViewEvidenceNotesMapTransfer, ViewAllEvidenceNotesViewModel>
    {
        public ViewAllEvidenceNotesMap(IMapper mapper, ConfigurationService configurationService) : base(mapper, configurationService)
        {
        }

        public ViewAllEvidenceNotesViewModel Map(ViewEvidenceNotesMapTransfer source)
        {
            Guard.ArgumentNotNull(() => source, source);

            var model = MapBase(source.NoteData, source.CurrentDate, source.ManageEvidenceNoteViewModel, source.PageNumber, source.ComplianceYearList);

            return model;
        }
    }
}
