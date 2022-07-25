namespace EA.Weee.Web.Areas.Admin.Mappings.ToViewModel
{
    using EA.Prsd.Core;
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Web.Areas.Admin.ViewModels.ManageEvidenceNotes;

    public class ViewAllTransferNotesMap : ViewAllNotesMapBase<ViewAllTransferNotesViewModel>, IMap<ViewAllEvidenceNotesMapTransfer, ViewAllTransferNotesViewModel>
    {
        public ViewAllTransferNotesMap(IMapper mapper) : base(mapper)
        {
        }

        public ViewAllTransferNotesViewModel Map(ViewAllEvidenceNotesMapTransfer source)
        {
            Guard.ArgumentNotNull(() => source, source);

            var model = CreateModel(source);

            return model;
        }
    }
}
