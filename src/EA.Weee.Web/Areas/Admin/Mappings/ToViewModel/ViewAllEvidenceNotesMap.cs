namespace EA.Weee.Web.Areas.Admin.Mappings.ToViewModel
{
    using EA.Prsd.Core;
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Web.Areas.Admin.ViewModels.ManageEvidenceNotes;

    public class ViewAllEvidenceNotesMap : ViewAllNotesMapBase<ViewAllEvidenceNotesViewModel>, IMap<ViewAllEvidenceNotesMapTransfer, ViewAllEvidenceNotesViewModel>
    {
        public ViewAllEvidenceNotesMap(IMapper mapper) : base(mapper)
        {
        }

        public ViewAllEvidenceNotesViewModel Map(ViewAllEvidenceNotesMapTransfer source)
        {
            Guard.ArgumentNotNull(() => source, source);

            var model = CreateModel(source);

            return model;
        }
    }
}
