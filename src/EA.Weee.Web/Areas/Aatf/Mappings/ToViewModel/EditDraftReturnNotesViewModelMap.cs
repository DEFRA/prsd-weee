namespace EA.Weee.Web.Areas.Aatf.Mappings.ToViewModel
{
    using System.Collections.Generic;
    using System.Linq;
    using EA.Prsd.Core;
    using Prsd.Core.Mapper;
    using ViewModels;

    public class EditDraftReturnNotesViewModelMap : IMap<EditDraftReturnNotesViewModelTransfer, EditDraftReturnedNotesViewModel>
    {
        private readonly IMapper mapper;

        public EditDraftReturnNotesViewModelMap(IMapper mapper)
        {
            this.mapper = mapper;
        }

        public EditDraftReturnedNotesViewModel Map(EditDraftReturnNotesViewModelTransfer source)
        {
            Guard.ArgumentNotNull(() => source, source);

            var model = new EditDraftReturnedNotesViewModel
            {
                ListOfNotes = mapper.Map<List<EvidenceNoteRowViewModel>>(source.Notes)
            };

            return model;
        }
    }
}