namespace EA.Weee.Web.Areas.Admin.Mappings.ToViewModel
{
    using System.Collections.Generic;
    using System.Linq;
    using EA.Prsd.Core.Mapper;
    using Web.ViewModels.Shared;

    public abstract class ViewAllNotesMapBase<T> where T : IManageEvidenceViewModel, new()
    {
        protected readonly IMapper Mapper;

        protected ViewAllNotesMapBase(IMapper mapper)
        {
            this.Mapper = mapper;
        }

        protected T CreateModel(ViewAllEvidenceNotesMapTransfer source)
        {
            var model = new T
            {
                EvidenceNotesDataList = Mapper.Map<List<EvidenceNoteRowViewModel>>(source.NoteData.Results.ToList()),
                ManageEvidenceNoteViewModel = source.ManageEvidenceNoteViewModel
            };

            return model;
        }
    }
}