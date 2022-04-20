namespace EA.Weee.Web.Areas.Aatf.Mappings.ToViewModel
{
    using EA.Prsd.Core;
    using Prsd.Core.Mapper;
    using ViewModels;

    public class EditDraftReturnNotesViewModelMap : ListOfNotesViewModelBase<EditDraftReturnedNotesViewModel>, IMap<EditDraftReturnNotesViewModelTransfer, EditDraftReturnedNotesViewModel>
    {
        public EditDraftReturnNotesViewModelMap(IMapper mapper) : base(mapper)
        {
        }

        public EditDraftReturnedNotesViewModel Map(EditDraftReturnNotesViewModelTransfer source)
        {
            Guard.ArgumentNotNull(() => source, source);

            return Map(source.Notes);
        }
    }
}