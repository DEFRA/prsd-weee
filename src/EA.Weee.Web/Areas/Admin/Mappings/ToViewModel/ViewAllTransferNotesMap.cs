namespace EA.Weee.Web.Areas.Admin.Mappings.ToViewModel
{
    using System.Collections.Generic;
    using EA.Prsd.Core;
    using EA.Prsd.Core.Mapper;
    using EA.Weee.Web.Areas.Admin.ViewModels.ManageEvidenceNotes;
    using EA.Weee.Web.ViewModels.Shared;

    public class ViewAllTransferNotesMap : IMap<ViewAllEvidenceNotesMapModel, ViewAllTransferNotesViewModel>
    {
        protected readonly IMapper Mapper;

        public ViewAllTransferNotesMap(IMapper mapper)
        {
            this.Mapper = mapper;
        }

        public ViewAllTransferNotesViewModel Map(ViewAllEvidenceNotesMapModel source)
        {
            Guard.ArgumentNotNull(() => source, source);

            var viewAllEvidenceModel = new ViewAllTransferNotesViewModel();
            viewAllEvidenceModel.EvidenceNotesDataList = Mapper.Map<List<EvidenceNoteRowViewModel>>(source.Notes);
            viewAllEvidenceModel.ManageEvidenceNoteViewModel = source.ManageEvidenceNoteViewModel;

            return viewAllEvidenceModel;
        }
    }
}
